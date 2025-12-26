using Account.Application.Resources;
using Account.Domain.DTO.Student;
using Account.Domain.Entity;
using Account.Domain.Entity.LinkedEntites;
using Account.Domain.Enum;
using Account.Domain.Interfaces.Repositories;
using Account.Domain.Interfaces.Services;
using Account.Domain.Result;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IBaseRepository<Student> _studentRepository;
        private readonly IBaseRepository<Teacher> _teacherRepository;
        private readonly IBaseRepository<Discipline> _disciplineRepository;
        private readonly IBaseRepository<Indicator> _indicatorRepository;
        private readonly IBaseRepository<DisciplineScore> _disciplineScoreRepository;
        private readonly IBaseRepository<IndicatorScore> _indicatorScoreRepository;
        private readonly IBaseRepository<Group> _groupRepository;
        private readonly IBaseRepository<Competence> _competenceRepository;
        private readonly IBaseRepository<CompetenceScore> _competenceScoreRepository;
        private readonly ILogger _logger;

        public StudentService(IBaseRepository<Student> studentRepository, IBaseRepository<Teacher> teacherRepository, 
            IBaseRepository<Discipline> disciplineRepository, IBaseRepository<Indicator> indicatorRepository, 
            IBaseRepository<DisciplineScore> disciplineScoreRepository, IBaseRepository<IndicatorScore> indicatorScoreRepository,
            IBaseRepository<Group> groupRepository, IBaseRepository<Competence> competenceRepository, 
            IBaseRepository<CompetenceScore> competenceScoreRepository, ILogger logger)
        {
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _disciplineRepository = disciplineRepository;
            _indicatorRepository = indicatorRepository;
            _disciplineScoreRepository = disciplineScoreRepository;
            _indicatorScoreRepository = indicatorScoreRepository;
            _groupRepository = groupRepository;
            _competenceRepository = competenceRepository;
            _competenceScoreRepository = competenceScoreRepository;
            _logger = logger;
        }

        public async Task<CollectionResult<StudentDisciplinesDto>> GetStudentDisciplinesAsync(long studentId)
        {
            var student = await _studentRepository.GetAll()
                .Include(s => s.Group)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
            {
                _logger.Error("Студент не найден. Id: {StudentId}", studentId);
                throw new ExceptionResult(
                    ErrorCodes.StudentNotFound,
                    ErrorMessage.StudentNotFound
                );
            }

            if (student.Group == null)
            {
                _logger.Information("У студента {StudentId} нет группы", studentId);
                throw new ExceptionResult(
                    ErrorCodes.StudentDoesNotHaveGroup,
                    ErrorMessage.StudentDoesNotHaveGroup
                );
            }

            var groupDisciplines = await _groupRepository.GetAll()
                .Where(g => g.Id == student.Group.Id)
                .SelectMany(g => g.Disciplines)
                .Include(d => d.Teachers)
                .ToListAsync();

            var result = new List<StudentDisciplinesDto>();

            foreach (var discipline in groupDisciplines)
            {
                var disciplineScore = await _disciplineScoreRepository.GetAll()
                    .FirstOrDefaultAsync(ds => ds.DisciplineId == discipline.Id && ds.StudentId == studentId);

                var teacher = discipline.Teachers.FirstOrDefault();

                var dto = new StudentDisciplinesDto
                {
                    Id = discipline.Id,
                    Name = discipline.Name,
                    TeacherName = teacher != null
                        ? $"{teacher.Lastname} {teacher.Firstname} {teacher.Middlename}".Trim()
                        : "Преподаватель не назначен",
                    Score = disciplineScore?.Score ?? 0
                };

                result.Add(dto);
            }

            _logger.Information("Получены дисциплины студента {StudentId}. Количество: {Count}", studentId, result.Count);

            return new CollectionResult<StudentDisciplinesDto>
            {
                Data = result
            };
        }
        public async Task<BaseResult<StudentDisciplineScoresDto>> GetDisciplineScoresAsync(long disciplineId, long studentId)
        {
            var student = await _studentRepository.GetAll()
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
            {
                _logger.Error("Студент не найден. Id: {StudentId}", studentId);
                throw new ExceptionResult(
                    ErrorCodes.StudentNotFound,
                    ErrorMessage.StudentNotFound
                );
            }

            var studentGroup = await _studentRepository.GetAll()
                .Where(s => s.Id == studentId)
                .Select(s => s.Group)
                .FirstOrDefaultAsync();

            if (studentGroup == null)
            {
                _logger.Error("У студента {StudentId} нет группы", studentId);
                throw new ExceptionResult(
                    ErrorCodes.StudentDoesNotHaveGroup,
                    ErrorMessage.StudentDoesNotHaveGroup
                );
            }

            var hasAccess = await _groupRepository.GetAll()
                .Where(g => g.Id == studentGroup.Id)
                .SelectMany(g => g.Disciplines)
                .AnyAsync(d => d.Id == disciplineId);

            if (!hasAccess)
            {
                _logger.Error("Студент {StudentId} не имеет доступа к дисциплине {DisciplineId}", studentId, disciplineId);
                throw new ExceptionResult(
                    ErrorCodes.GroupDoesNotHaveThisDiscipline,
                    ErrorMessage.GroupDoesNotHaveThisDiscipline
                );
            }

            var discipline = await _disciplineRepository.GetAll()
                .Include(d => d.Indicators)
                .FirstOrDefaultAsync(d => d.Id == disciplineId);

            if (discipline == null)
            {
                _logger.Error("Дисциплина не найдена. Id: {DisciplineId}", disciplineId);
                throw new ExceptionResult(
                    ErrorCodes.DisciplineNotFound,
                    ErrorMessage.DisciplineNotFound
                );
            }

            var disciplineScore = await _disciplineScoreRepository.GetAll()
                .FirstOrDefaultAsync(ds => ds.DisciplineId == disciplineId && ds.StudentId == studentId);

            var indicatorIds = discipline.Indicators.Select(i => i.Id).ToList();
            var indicatorScores = await _indicatorScoreRepository.GetAll()
                .Where(score => score.StudentId == studentId && indicatorIds.Contains(score.IndicatorId))
                .ToListAsync();

            var indicatorDtos = new List<DisciplineIndicatorScoreDto>();

            foreach (var indicator in discipline.Indicators)
            {
                var indicatorScore = indicatorScores.FirstOrDefault(score => score.IndicatorId == indicator.Id);

                indicatorDtos.Add(new DisciplineIndicatorScoreDto
                {
                    Id = indicator.Id,
                    Index = indicator.Index,
                    Name = indicator.Name,
                    Score = indicatorScore?.ScoreValue
                });
            }

            var result = new StudentDisciplineScoresDto
            {
                Name = discipline.Name,
                Indicators = indicatorDtos,
                DisciplineScore = disciplineScore?.Score
            };

            _logger.Information("Получены оценки по дисциплине {DisciplineId} для студента {StudentId}",
                disciplineId, studentId);

            return new BaseResult<StudentDisciplineScoresDto>
            {
                Data = result
            };
        }

        public async Task<CollectionResult<StudentCompetencesDto>> GetStudentCompetencesAsync(long studentId)
        {
            var student = await _studentRepository.GetAll()
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
            {
                _logger.Error("Студент не найден. Id: {StudentId}", studentId);
                throw new ExceptionResult(
                    ErrorCodes.StudentNotFound,
                    ErrorMessage.StudentNotFound
                );
            }

            var allCompetences = await _competenceRepository.GetAll()
                .Include(c => c.Indicators)
                .ToListAsync();

            var result = new List<StudentCompetencesDto>();

            foreach (var competence in allCompetences)
            {
                var indicatorIds = competence.Indicators.Select(i => i.Id).ToList();
                var indicatorScores = await _indicatorScoreRepository.GetAll()
                    .Where(score => score.StudentId == studentId && indicatorIds.Contains(score.IndicatorId))
                    .ToListAsync();

                decimal progress = 0;
                if (indicatorIds.Count > 0)
                {
                    var scoredCount = indicatorScores.Count(score => score.ScoreValue != null);
                    progress = Math.Round((decimal)scoredCount / indicatorIds.Count * 100);
                }

                var competenceScore = await _competenceScoreRepository.GetAll()
                    .FirstOrDefaultAsync(cs => cs.CompetenceId == competence.Id && cs.StudentId == studentId);

                if (competenceScore == null)
                {
                    competenceScore = new CompetenceScore
                    {
                        CompetenceId = competence.Id,
                        StudentId = studentId,
                        Score = progress,
                    };
                    await _competenceScoreRepository.CreateAsync(competenceScore);
                    await _competenceScoreRepository.SaveChangesAsync();
                }
                else if (competenceScore.Score != progress)
                {
                    competenceScore.Score = progress;
                    _competenceScoreRepository.Update(competenceScore);
                    await _competenceScoreRepository.SaveChangesAsync();
                }

                result.Add(new StudentCompetencesDto
                {
                    Id = competence.Id,
                    Index = competence.Index,
                    Name = competence.Name,
                    Progress = progress
                });
            }

            _logger.Information("Получены компетенции студента {StudentId}. Количество: {Count}",
                studentId, result.Count);

            return new CollectionResult<StudentCompetencesDto>
            {
                Data = result
            };
        }

    }
}
