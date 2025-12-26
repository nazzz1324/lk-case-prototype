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
        private readonly IBaseRepository<StudentIndicatorDisciplineScore> _studentIndicatorDisciplineScoreRepository;
        private readonly IBaseRepository<ProfessionalRole> _proleRepository; 
        private readonly IBaseRepository<CompetenceProle> _competenceProleRepository;
        private readonly ILogger _logger;

        public StudentService(IBaseRepository<Student> studentRepository, IBaseRepository<Teacher> teacherRepository,
            IBaseRepository<Discipline> disciplineRepository, IBaseRepository<Indicator> indicatorRepository,
            IBaseRepository<DisciplineScore> disciplineScoreRepository, IBaseRepository<IndicatorScore> indicatorScoreRepository,
            IBaseRepository<Group> groupRepository, IBaseRepository<Competence> competenceRepository,
            IBaseRepository<CompetenceScore> competenceScoreRepository, ILogger logger,
            IBaseRepository<StudentIndicatorDisciplineScore> studentIndicatorDisciplineScoreRepository, IBaseRepository<ProfessionalRole> proleRepository, IBaseRepository<CompetenceProle> competenceProleRepository)
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
            _studentIndicatorDisciplineScoreRepository = studentIndicatorDisciplineScoreRepository;
            _proleRepository = proleRepository;
            _competenceProleRepository = competenceProleRepository;
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

            // ИЗМЕНЕНИЕ ТОЛЬКО ЭТОЙ СТРОКИ:
            var indicatorScores = await _studentIndicatorDisciplineScoreRepository.GetAll()
                .Where(score => score.StudentId == studentId &&
                               score.DisciplineId == disciplineId && // ← добавляем фильтр по дисциплине
                               indicatorIds.Contains(score.IndicatorId))
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
                    Score = indicatorScore != null ? (int?)indicatorScore.Score : null // ← оставляем int
                });
            }

            var result = new StudentDisciplineScoresDto
            {
                Name = discipline.Name,
                Indicators = indicatorDtos,
                Score = disciplineScore?.Score
            };

            _logger.Information("Получены оценки по дисциплине {DisciplineId} для студента {StudentId}",
                disciplineId, studentId);

            return new BaseResult<StudentDisciplineScoresDto>
            {
                Data = result
            };
        }
        public async Task<BaseResult<StudentCompetenceScoresDto>> GetCompetenceScoresAsync(long competenceId, long studentId)
        {
            // 1. Получаем компетенцию с индикаторами
            var competence = await _competenceRepository.GetAll()
                .Include(c => c.Indicators)
                .FirstOrDefaultAsync(c => c.Id == competenceId);

            if (competence == null)
                throw new ExceptionResult(ErrorCodes.CompetenceNotFound, ErrorMessage.CompetenceNotFound);

            var indicatorIds = competence.Indicators.Select(i => i.Id).ToList();
            var indicatorDtos = new List<CompetenceIndicatorScore>();
            decimal competenceProgress = 0;

            // 2. Если есть индикаторы - считаем прогресс
            if (indicatorIds.Any())
            {
                // Получаем оценки студента по индикаторам компетенции
                var studentScores = await _indicatorScoreRepository.GetAll()
                    .Where(s => s.StudentId == studentId && indicatorIds.Contains(s.IndicatorId))
                    .GroupBy(s => s.IndicatorId)
                    .Select(g => new
                    {
                        IndicatorId = g.Key,
                        AverageScore = g.Average(s => (decimal?)s.ScoreValue) ?? 0
                    })
                    .ToListAsync();

                // Создаем DTO для индикаторов и считаем сумму баллов
                decimal totalScore = 0;

                foreach (var indicator in competence.Indicators)
                {
                    var studentScore = studentScores.FirstOrDefault(s => s.IndicatorId == indicator.Id);
                    var scoreValue = studentScore?.AverageScore ?? 0;

                    indicatorDtos.Add(new CompetenceIndicatorScore
                    {
                        Id = indicator.Id,
                        Index = indicator.Index,
                        Name = indicator.Name,
                        Score = scoreValue > 0 ? (decimal?)scoreValue : null
                    });

                    // Суммируем баллы (0-5 за каждый индикатор)
                    totalScore += scoreValue;
                }

                // Рассчитываем прогресс компетенции
                decimal maxPossibleScore = 5 * indicatorIds.Count;
                competenceProgress = maxPossibleScore > 0
                    ? Math.Round((totalScore / maxPossibleScore) * 100, 0)
                    : 0;
            }

            // 3. Сохраняем/обновляем прогресс в CompetenceScore
            var competenceScore = await _competenceScoreRepository.GetAll()
                .FirstOrDefaultAsync(cs => cs.StudentId == studentId && cs.CompetenceId == competenceId);

            if (competenceScore != null)
            {
                competenceScore.Score = competenceProgress;
                _competenceScoreRepository.Update(competenceScore);
            }
            else
            {
                var newCompetenceScore = new CompetenceScore
                {
                    StudentId = studentId,
                    CompetenceId = competenceId,
                    Score = competenceProgress,
                };
                await _competenceScoreRepository.CreateAsync(newCompetenceScore);
            }

            await _competenceScoreRepository.SaveChangesAsync();

            // 4. Возвращаем результат
            var result = new StudentCompetenceScoresDto
            {
                Name = competence.Name,
                Indicators = indicatorDtos,
                Score = competenceProgress
            };

            _logger.Information("Получены оценки по компетенции {CompetenceId} для студента {StudentId}. Прогресс: {Progress}%",
                competenceId, studentId, competenceProgress);

            return new BaseResult<StudentCompetenceScoresDto>
            {
                Data = result
            };
        }


        public async Task<CollectionResult<StudentCompetencesDto>> GetStudentCompetencesAsync(long studentId)
        {
            // Проверяем существование студента
            var student = await _studentRepository.GetAll()
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
            {
                _logger.Error("Студент не найден. Id: {StudentId}", studentId);
                throw new ExceptionResult(ErrorCodes.StudentNotFound, ErrorMessage.StudentNotFound);
            }

            // Получаем все компетенции
            var competences = await _competenceRepository.GetAll()
                .Include(c => c.Indicators)
                .ToListAsync();

            var result = new List<StudentCompetencesDto>();

            foreach (var competence in competences)
            {
                var indicatorIds = competence.Indicators.Select(i => i.Id).ToList();
                decimal progress = 0;

                if (indicatorIds.Any())
                {
                    // Читаем из IndicatorScore (куда пишет SaveScoresAsync)
                    var studentScores = await _indicatorScoreRepository.GetAll()
                        .Where(s => s.StudentId == studentId &&
                                   indicatorIds.Contains(s.IndicatorId))
                        .GroupBy(s => s.IndicatorId)
                        .Select(g => new
                        {
                            IndicatorId = g.Key,
                            AverageScore = g.Average(s => (decimal?)s.ScoreValue) ?? 0
                        })
                        .ToListAsync();

                    
                    decimal totalScore = 0;
                    foreach (var indicatorId in indicatorIds)
                    {
                        var score = studentScores.FirstOrDefault(s => s.IndicatorId == indicatorId);
                        totalScore += score?.AverageScore ?? 0; 
                    }

                    decimal maxPossibleScore = 5 * indicatorIds.Count;
                    progress = maxPossibleScore > 0
                        ? Math.Round((totalScore / maxPossibleScore) * 100, 0)
                        : 0;
                }

                result.Add(new StudentCompetencesDto
                {
                    Id = competence.Id,
                    Index = competence.Index,
                    Name = competence.Name,
                    Progress = progress
                });
            }

            return new CollectionResult<StudentCompetencesDto>
            {
                Data = result,
            };
        }

        public async Task<BaseResult<StudentProlesDto>> GetStudentProlesAsync(long studentId)
        {
            // 1. Проверяем студента
            var student = await _studentRepository.GetAll()
                .Include(s => s.Group)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
                throw new ExceptionResult(ErrorCodes.StudentNotFound, ErrorMessage.StudentNotFound);

            // 2. Получаем группу студента
            var group = student.Group;
            if (group == null)
                throw new ExceptionResult(ErrorCodes.StudentDoesNotHaveGroup, ErrorMessage.StudentDoesNotHaveGroup);

            // 3. Получаем профроль группы
            var prole = await _proleRepository.GetAll()
                .FirstOrDefaultAsync(p => p.Id == group.ProleId);

            if (prole == null)
                throw new ExceptionResult(ErrorCodes.GroupDoesNotHaveProle, "У группы не назначена профроль");

            // 4. Получаем компетенции профроли через репозиторий CompetenceProle
            var competenceIds = await _competenceProleRepository.GetAll()
                .Where(cp => cp.ProleId == prole.Id)
                .Select(cp => cp.CompetenceId)
                .ToListAsync();

            // 5. Получаем прогресс студента по компетенциям
            var competenceScores = await _competenceScoreRepository.GetAll()
                .Where(cs => cs.StudentId == studentId && competenceIds.Contains(cs.CompetenceId))
                .ToListAsync();

            // 6. Рассчитываем статистику
            decimal? averageScore = null;
            int completedCount = 0;
            List<long> competencies = new List<long>();

            if (competenceIds.Any())
            {
                competencies = competenceIds;

                if (competenceScores.Any())
                {
                    averageScore = Math.Round(competenceScores.Average(cs => (decimal?)cs.Score) ?? 0, 1);
                    completedCount = competenceScores.Count(cs => cs.Score >= 60);
                }
                else
                {
                    averageScore = 0;
                    completedCount = 0;
                }
            }

            // 7. Формируем результат
            var result = new StudentProlesDto
            {
                Id = prole.Id,
                Name = prole.Name,
                Index = prole.Index,
                Score = averageScore,
                Competencies = competencies,
                CompletedCompetenciesCount = completedCount
            };

            _logger.Information("Получена профроль студента {StudentId}. Компетенций: {Count}, Прогресс: {Progress}%",
                studentId, competencies.Count, averageScore);

            return new BaseResult<StudentProlesDto>
            {
                Data = result
            };
        }
    }
}
