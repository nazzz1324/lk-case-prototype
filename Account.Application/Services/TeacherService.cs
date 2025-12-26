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
        private readonly ILogger _logger;

        public TeacherService(IBaseRepository<Student> studentRepository, IBaseRepository<Teacher> teacherRepository,
            IBaseRepository<Discipline> disciplineRepository, IBaseRepository<Indicator> indicatorRepository,
            IBaseRepository<DisciplineScore> disciplineScoreRepository, 
            IBaseRepository<IndicatorScore> indicatorScoreRepository, ILogger logger)
        {
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _disciplineRepository = disciplineRepository;
            _indicatorRepository = indicatorRepository;
            _disciplineScoreRepository = disciplineScoreRepository;
            _indicatorScoreRepository = indicatorScoreRepository;
            _logger = logger;
        }
        public async Task<CollectionResult<TeacherDisciplineDto>> GetTeacherDisciplinesAsync(long teacherId)
        {
            var teacher = await _teacherRepository.GetAll()
                .Include(t => t.Disciplines)
                .FirstOrDefaultAsync(t => t.Id == teacherId);

            if (teacher == null)
            {
                _logger.Error("Преподаватель не найден. Id: {TeacherId}", teacherId);
                throw new ExceptionResult(
                    ErrorCodes.TeacherAlreadyExists,
                    ErrorMessage.TeacherAlreadyExists
                );
            }

            var disciplines = teacher.Disciplines
                .Select(d => new TeacherDisciplineDto
                {
                    Id = d.Id,     
                    Name = d.Name   
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

            var allExistingScores = await _indicatorScoreRepository.GetAll()
                .Where(s => studentIds.Contains(s.StudentId) &&
                            indicatorIds.Contains(s.IndicatorId) &&
                            s.TeacherId == filter.TeacherId)
                .ToListAsync();

            foreach (var student in students)
            {
                student.Scores = indicators
                    .Select(indicator => allExistingScores
                        .FirstOrDefault(s => s.StudentId == student.Id &&
                                             s.IndicatorId == indicator.Id)?.ScoreValue)
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

            var existingScores = await _indicatorScoreRepository.GetAll()
                .Where(s => studentIds.Contains(s.StudentId) &&
                            indicatorIds.Contains(s.IndicatorId) &&
                            s.TeacherId == dto.TeacherId)
                .ToListAsync();

            var existingDict = existingScores.ToDictionary(s => (s.StudentId, s.IndicatorId));
            var newScores = new List<IndicatorScore>();

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

                    if (scoreDto.Score < 0 || scoreDto.Score > new IndicatorScore().MaxScore)
                        throw new ExceptionResult(ErrorCodes.InvalidScore, ErrorMessage.InvalidScore);

                    var key = (scoreDto.StudentId, scoreDto.IndicatorId);
                    if (existingDict.TryGetValue(key, out var existingScore))
                    {
                        existingScore.ScoreValue = scoreDto.Score;
                        _indicatorScoreRepository.Update(existingScore);
                    }
                    else
                    {
                        var newScore = new IndicatorScore
                        {
                            StudentId = scoreDto.StudentId,
                            IndicatorId = scoreDto.IndicatorId,
                            TeacherId = dto.TeacherId,
                            ScoreValue = scoreDto.Score,
                        };
                        newScores.Add(newScore);
                        existingDict[key] = newScore;
                    }
                }

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

            foreach (var score in newScores)
            {
                await _indicatorScoreRepository.CreateAsync(score);
            }

            await _indicatorScoreRepository.SaveChangesAsync();
            await _disciplineScoreRepository.SaveChangesAsync();

            _logger.Information("Оценки сохранены. Преподаватель: {TeacherId}, Студентов: {Count}",
                dto.TeacherId, scoresByStudent.Count());

            return new BaseResult<bool>
            {
                Data = true,
            };
        }
    }
}
