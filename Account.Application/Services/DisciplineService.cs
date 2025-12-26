using Account.Application.Resources;
using Account.Domain.DTO.Competence;
using Account.Domain.DTO.Discipline;
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
    public class DisciplineService : IDisciplineService
    {
        private readonly IBaseRepository<Discipline> _disciplineRepository;
        private readonly IBaseRepository<Indicator> _indicatorRepository;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public DisciplineService(IBaseRepository<Discipline> disciplineRepository, 
            IBaseRepository<Indicator> indicatorRepository, ILogger logger, IMapper mapper)
        {
            _disciplineRepository = disciplineRepository;
            _indicatorRepository = indicatorRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<BaseResult<DisciplineDto>> CreateDisciplineAsync(CreateDisciplineDto dto)
        {
            var discipline = await _disciplineRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Name == dto.Name);

            if (discipline != null)
            {
                _logger.Error("Дисциплина уже существует. Name: {Name}", dto.Name);
                throw new ExceptionResult(
                    ErrorCodes.DisciplineAlreadyExists,
                    ErrorMessage.DisciplineAlreadyExists
                );
            }

            discipline = new Discipline
            {
                Name = dto.Name,
            };

            await _disciplineRepository.CreateAsync(discipline);
            await _disciplineRepository.SaveChangesAsync();

            if (dto.IndicatorIds != null && dto.IndicatorIds.Any())
            {
                var indicators = await _indicatorRepository.GetAll()
                    .Where(x => dto.IndicatorIds.Contains(x.Id))
                    .ToListAsync();
                foreach (var indicator in indicators)
                {
                    discipline.Indicators.Add(indicator);  
                }
                await _disciplineRepository.SaveChangesAsync(); 
            }

            _logger.Information("Дисциплина создана. Id: {Id}, Привязано индикаторов: {Count}",
                discipline.Id,
                dto.IndicatorIds?.Count ?? 0);

            return new BaseResult<DisciplineDto>()
            {
                Data = _mapper.Map<DisciplineDto>(discipline)
            };
        }

        public async Task<BaseResult<DisciplineDto>> DeleteDisciplineAsync(long id)
        {
            var discipline = await _disciplineRepository.GetAll()
               .FirstOrDefaultAsync(x => x.Id == id);

            if (discipline == null)
            {
                _logger.Error("Дисциплина не существует. Id: {Id}", id);
                throw new ExceptionResult(
                    ErrorCodes.DisciplineNotFound,
                    ErrorMessage.DisciplineNotFound
                );
            }

            _disciplineRepository.Remove(discipline);
            await _disciplineRepository.SaveChangesAsync();

            _logger.Information("Дисциплина удалена. Id: {Id}",
                discipline.Id);

            return new BaseResult<DisciplineDto>()
            {
                Data = _mapper.Map<DisciplineDto>(discipline),
            };
        }

        public async Task<CollectionResult<DisciplinesDto>> GetDisciplinesAsync()
        {
            var disciplines = await _disciplineRepository.GetAll()
                .Include(d => d.Indicators)  
                .Select(d => new DisciplinesDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    IndicatorCount = d.Indicators.Count  
                })
                .ToArrayAsync();

            if (disciplines.Length == 0)
            {
                _logger.Information("Список дисциплин пуст");
                return new CollectionResult<DisciplinesDto>
                {
                    Data = disciplines,
                };
            }

            _logger.Information("Получено дисциплин: {Count}", disciplines.Length);

            return new CollectionResult<DisciplinesDto>
            {
                Data = disciplines
            };
        }

        public async Task<BaseResult<DisciplineDto>> UpdateDisciplineAsync(DisciplineDto dto)
        {
            var discipline = await _disciplineRepository.GetAll()
                .Include(d => d.Indicators)
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (discipline == null)
            {
                _logger.Error("Дисциплина не существует. Id: {Id}", dto.Id);
                throw new ExceptionResult(
                    ErrorCodes.DisciplineNotFound,
                    ErrorMessage.DisciplineNotFound
                );
            }

            discipline.Name = dto.Name;

            var indicatorsToAttach = await _indicatorRepository.GetAll()
                .Where(x => dto.IndicatorIds.Contains(x.Id))
                .ToListAsync();

            discipline.Indicators.Clear();

            foreach (var indicator in indicatorsToAttach)
            {
                discipline.Indicators.Add(indicator);
            }

            _disciplineRepository.Update(discipline);
            await _disciplineRepository.SaveChangesAsync();

            _logger.Information("Дисциплина обновлена. Id: {Id}, Привязано индикаторов: {Count}",
                discipline.Id, indicatorsToAttach.Count);

            return new BaseResult<DisciplineDto>()
            {
                Data = _mapper.Map<DisciplineDto>(discipline),
            };
        }
    }
}
