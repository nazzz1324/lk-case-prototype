using Account.Application.Resources;
using Account.Domain.DTO.Discipline;
using Account.Domain.DTO.Teacher;
using Account.Domain.Entity;
using Account.Domain.Entity.LinkedEntites;
using Account.Domain.Enum;
using Account.Domain.Interfaces.Repositories;
using Account.Domain.Interfaces.Services;
using Account.Domain.Result;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly IBaseRepository<Student> _studentRepository;
        private readonly IBaseRepository<Teacher> _teacherRepository;
        private readonly IBaseRepository<Discipline> _disciplineRepository;
        private readonly IBaseRepository<Indicator> _indicatorRepository;
        private readonly IBaseRepository<DisciplineScore> _disciplineScoreRepository;
        private readonly IBaseRepository<IndicatorScore> _indicatorScoreRepository;
        private readonly IBaseRepository<StudentIndicatorDisciplineScore> _studentIndicatorDisciplineScoreRepository;
        private readonly ILogger _logger;

        public TeacherService(IBaseRepository<Student> studentRepository, IBaseRepository<Teacher> teacherRepository,
            IBaseRepository<Discipline> disciplineRepository, IBaseRepository<Indicator> indicatorRepository,
            IBaseRepository<DisciplineScore> disciplineScoreRepository,
            IBaseRepository<IndicatorScore> indicatorScoreRepository, ILogger logger, 
            IBaseRepository<StudentIndicatorDisciplineScore> studentIndicatorDisciplineScoreRepository)
        {
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _disciplineRepository = disciplineRepository;
            _indicatorRepository = indicatorRepository;
            _disciplineScoreRepository = disciplineScoreRepository;
            _indicatorScoreRepository = indicatorScoreRepository;
            _logger = logger;
            _studentIndicatorDisciplineScoreRepository = studentIndicatorDisciplineScoreRepository;
        }
        public async Task<CollectionResult<TeacherDisciplineDto>> GetTeacherDisciplinesAsync(long teacherId)
        {
            var teacher = await _teacherRepository.GetAll()
                .Include(t => t.Disciplines)
                    .ThenInclude(d => d.Indicators)
                .FirstOrDefaultAsync(t => t.Id == teacherId);

            if (teacher == null)
            {
                _logger.Error("Преподаватель не найден. Id: {TeacherId}", teacherId);
                throw new ExceptionResult(
                    ErrorCodes.TeacherNotFound,
                    ErrorMessage.TeacherNotFound
                );
            }

            var disciplines = teacher.Disciplines
                .Select(d => new TeacherDisciplineDto
                {
                    Id = d.Id,     
                    Name = d.Name,
                    IndicatorCount = d.Indicators.Count
                })
                .ToArray();

            _logger.Information("Дисциплин преподавателя: {Count}", disciplines.Length);

            return new CollectionResult<TeacherDisciplineDto>
            {
                Data = disciplines
            };
        }

        public async Task<BaseResult<ScoringDataDto>> GetScoringDataAsync(ScoringFilterDto filter)
        {
            var discipline = await _disciplineRepository.GetAll()
                .Include(d => d.Indicators)
                .Include(d => d.Teachers)
                .FirstOrDefaultAsync(d => d.Id == filter.DisciplineId);

            if (discipline == null)
            {
                _logger.Error("Дисциплина не найдена. Id: {DisciplineId}", filter.DisciplineId);
                throw new ExceptionResult(ErrorCodes.DisciplineNotFound, ErrorMessage.DisciplineNotFound);
            }

            if (!discipline.Teachers.Any(t => t.Id == filter.TeacherId))
            {
                _logger.Error("Преподаватель не ведет эту дисциплину. TeacherId: {TeacherId}, DisciplineId: {DisciplineId}",
                    filter.TeacherId, filter.DisciplineId);

                throw new ExceptionResult(ErrorCodes.TeacherNoAccess, ErrorMessage.TeacherNoAccess);
            }

            var students = await _studentRepository.GetAll()
                .Where(s => s.GroupId == filter.GroupId)
                .Select(s => new StudentScoreDto
                {
                    Id = s.Id,
                    FullName = $"{s.Lastname} {s.Firstname} {s.Middlename}".Trim(),
                    Scores = new List<decimal?>()
                })
                .ToListAsync();

            var indicators = discipline.Indicators
                .Take(3)
                .Select(i => new TeacherIndicatorDto
                {
                    Id = i.Id,
                    Index = i.Index,
                    Name = i.Name
                })
                .ToList();

            var studentIds = students.Select(s => s.Id).ToList();
            var indicatorIds = indicators.Select(i => i.Id).ToList();

            // ИЗМЕНЕНИЕ ТОЛЬКО ЭТОГО БЛОКА:
            var allExistingScores = await _studentIndicatorDisciplineScoreRepository.GetAll()
                .Where(s => studentIds.Contains(s.StudentId) &&
                            indicatorIds.Contains(s.IndicatorId) &&
                            s.DisciplineId == filter.DisciplineId) // ← ДОБАВЛЯЕМ фильтр по дисциплине
                .ToListAsync();

            foreach (var student in students)
            {
                student.Scores = indicators
                    .Select(indicator => allExistingScores
                        .FirstOrDefault(s => s.StudentId == student.Id &&
                                             s.IndicatorId == indicator.Id &&
                                             s.DisciplineId == filter.DisciplineId)?.Score) // ← меняем ScoreValue на Score
                    .ToList();
            }

            _logger.Information("Данные для оценивания. Дисциплина: {DisciplineId}, Группа: {GroupId}, Студентов: {Count}",
                filter.DisciplineId, filter.GroupId, students.Count);

            return new BaseResult<ScoringDataDto>
            {
                Data = new ScoringDataDto
                {
                    DisciplineName = discipline.Name,
                    Students = students,
                    Indicators = indicators
                }
            };
        }

        public async Task<BaseResult<bool>> SaveScoresAsync(SaveScoresDto dto)
        {
            var discipline = await _disciplineRepository.GetAll()
                .Include(d => d.Indicators)
                .Include(d => d.Teachers)
                .FirstOrDefaultAsync(d => d.Id == dto.DisciplineId);

            if (discipline == null)
                throw new ExceptionResult(ErrorCodes.DisciplineNotFound, ErrorMessage.DisciplineNotFound);

            if (!discipline.Teachers.Any(t => t.Id == dto.TeacherId))
                throw new ExceptionResult(ErrorCodes.TeacherNoAccess, ErrorMessage.TeacherNoAccess);

            var studentIds = dto.Scores.Select(s => s.StudentId).Distinct().ToList();
            var students = await _studentRepository.GetAll()
                .Where(s => studentIds.Contains(s.Id))
                .ToDictionaryAsync(s => s.Id);

            var indicatorIds = dto.Scores.Select(s => s.IndicatorId).Distinct().ToList();
            var disciplineIndicatorIds = discipline.Indicators.Select(i => i.Id).ToList();

            // 1. Ищем в СТАРОЙ таблице (для обратной совместимости)
            var existingIndicatorScores = await _indicatorScoreRepository.GetAll()
                .Where(s => studentIds.Contains(s.StudentId) &&
                            indicatorIds.Contains(s.IndicatorId) &&
                            s.TeacherId == dto.TeacherId)
                .ToListAsync();

            // 2. Ищем в НОВОЙ таблице
            var existingStudentIndicatorScores = await _studentIndicatorDisciplineScoreRepository.GetAll()
                .Where(s => studentIds.Contains(s.StudentId) &&
                            indicatorIds.Contains(s.IndicatorId) &&
                            s.DisciplineId == dto.DisciplineId)
                .ToListAsync();

            var existingDictOld = existingIndicatorScores.ToDictionary(s => (s.StudentId, s.IndicatorId));
            var existingDictNew = existingStudentIndicatorScores.ToDictionary(s => (s.StudentId, s.DisciplineId, s.IndicatorId));

            var newScoresOld = new List<IndicatorScore>();
            var newScoresNew = new List<StudentIndicatorDisciplineScore>();

            var scoresByStudent = dto.Scores.GroupBy(s => s.StudentId);

            foreach (var studentGroup in scoresByStudent)
            {
                var studentId = studentGroup.Key;

                if (!students.ContainsKey(studentId))
                    throw new ExceptionResult(ErrorCodes.StudentNotFound, ErrorMessage.StudentNotFound);

                foreach (var scoreDto in studentGroup)
                {
                    if (!disciplineIndicatorIds.Contains(scoreDto.IndicatorId))
                        throw new ExceptionResult(ErrorCodes.IndicatorNotFound, ErrorMessage.IndicatorNotFound);

                    if (scoreDto.Score < 0 || scoreDto.Score > 5)
                        throw new ExceptionResult(ErrorCodes.InvalidScore, ErrorMessage.InvalidScore);

                    // ===== СТАРАЯ ТАБЛИЦА (IndicatorScore) =====
                    var keyOld = (scoreDto.StudentId, scoreDto.IndicatorId);
                    if (existingDictOld.TryGetValue(keyOld, out var existingScoreOld))
                    {
                        existingScoreOld.ScoreValue = scoreDto.Score;
                        _indicatorScoreRepository.Update(existingScoreOld);
                    }
                    else
                    {
                        var newScoreOld = new IndicatorScore
                        {
                            StudentId = scoreDto.StudentId,
                            IndicatorId = scoreDto.IndicatorId,
                            TeacherId = dto.TeacherId,
                            ScoreValue = scoreDto.Score,
                        };
                        newScoresOld.Add(newScoreOld);
                        existingDictOld[keyOld] = newScoreOld;
                    }

                    // ===== НОВАЯ ТАБЛИЦА (StudentIndicatorDisciplineScore) =====
                    var keyNew = (StudentId: scoreDto.StudentId, DisciplineId: dto.DisciplineId, IndicatorId: scoreDto.IndicatorId);
                    if (existingDictNew.TryGetValue(keyNew, out var existingScoreNew))
                    {
                        existingScoreNew.Score = scoreDto.Score;
                        existingScoreNew.TeacherId = dto.TeacherId;
                        _studentIndicatorDisciplineScoreRepository.Update(existingScoreNew);
                    }
                    else
                    {
                        var newScoreNew = new StudentIndicatorDisciplineScore
                        {
                            StudentId = scoreDto.StudentId,
                            DisciplineId = dto.DisciplineId,
                            IndicatorId = scoreDto.IndicatorId,
                            TeacherId = dto.TeacherId,
                            Score = scoreDto.Score,
                        };
                        newScoresNew.Add(newScoreNew);
                        existingDictNew[keyNew] = newScoreNew;
                    }
                }

                // Рассчет средней оценки (оставляем как было)
                var studentScores = studentGroup.Select(s => s.Score).ToList();
                var averageScore = Math.Round(studentScores.Average(), 1);

                var disciplineScore = await _disciplineScoreRepository.GetAll()
                    .FirstOrDefaultAsync(ds => ds.StudentId == studentId &&
                                               ds.DisciplineId == dto.DisciplineId);

                if (disciplineScore != null)
                {
                    disciplineScore.Score = averageScore;
                    _disciplineScoreRepository.Update(disciplineScore);
                }
                else
                {
                    var newDisciplineScore = new DisciplineScore
                    {
                        StudentId = studentId,
                        DisciplineId = dto.DisciplineId,
                        Score = averageScore,
                    };
                    await _disciplineScoreRepository.CreateAsync(newDisciplineScore);
                }
            }

            // Сохраняем в ОБЕ таблицы
            foreach (var score in newScoresOld)
            {
                await _indicatorScoreRepository.CreateAsync(score);
            }

            foreach (var score in newScoresNew)
            {
                await _studentIndicatorDisciplineScoreRepository.CreateAsync(score);
            }

            // Сохраняем изменения
            await _indicatorScoreRepository.SaveChangesAsync();
            await _studentIndicatorDisciplineScoreRepository.SaveChangesAsync();
            await _disciplineScoreRepository.SaveChangesAsync();

            _logger.Information("Оценки сохранены. Преподаватель: {TeacherId}, Дисциплина: {DisciplineId}, Студентов: {Count}",
                dto.TeacherId, dto.DisciplineId, scoresByStudent.Count());

            return new BaseResult<bool>
            {
                Data = true,
            };
        }
    }
}
