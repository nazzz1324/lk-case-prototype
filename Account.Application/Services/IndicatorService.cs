using Account.Application.Resources;
using Account.Domain.DTO.Indicator;
using Account.Domain.DTO.Role;
using Account.Domain.DTO.User;
using Account.Domain.Entity.AuthRole;
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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Services
{
    public class IndicatorService : IIndicatorService
    {
        private readonly IBaseRepository<Indicator> _indicatorRepository;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public IndicatorService(IBaseRepository<Indicator> indicatorRepository, IMapper mapper, ILogger logger)
        {
            _indicatorRepository = indicatorRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BaseResult<IndicatorDto>> CreateIndicatorAsync(CreateIndicatorDto dto)
        {
            var indicator = await _indicatorRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Index == dto.Index || x.Name == dto.Name);

            if (indicator != null)
            {
                _logger.Error("Индикатор уже существует. Index: {Index}, Name: {Name}", dto.Index, dto.Name);
                throw new ExceptionResult(
                    ErrorCodes.IndicatorAlreadyExists,
                    ErrorMessage.IndicatorAlreadyExists
                );
            }

            indicator = new Indicator
            {
                Index = dto.Index,
                Name = dto.Name,
            };

            await _indicatorRepository.CreateAsync(indicator);
            await _indicatorRepository.SaveChangesAsync();

            _logger.Information("Индикатор создан. Id: {Id}, Index: {Index}",
                indicator.Id, indicator.Index);

            return new BaseResult<IndicatorDto>()
            {
                Data = _mapper.Map<IndicatorDto>(indicator)
            };
        }

        public async Task<BaseResult<IndicatorDto>> DeleteIndicatorAsync(long id)
        {
            var indicator = await _indicatorRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == id );

            if (indicator == null)
            {
                _logger.Error("Индикатор не существует. Id: {Id}", id);
                throw new ExceptionResult(
                    ErrorCodes.IndicatorNotFound,
                    ErrorMessage.IndicatorNotFound
                );
            }

            _indicatorRepository.Remove(indicator);
            await _indicatorRepository.SaveChangesAsync();

            _logger.Information("Индикатор удален. Id: {Id}, Index: {Index}",
                indicator.Id, indicator.Index);

            return new BaseResult<IndicatorDto>()
            {
                Data = _mapper.Map<IndicatorDto>(indicator),
            };
        }

        public async Task<CollectionResult<IndicatorsDto>> SearchIndicatorsAsync(string search)
        {
            var indicators = await _indicatorRepository.GetAll()
                .Include(x => x.Competence)
                .Include(x => x.Disciplines)
                .Where(x =>
                    string.IsNullOrWhiteSpace(search) ||
                    x.Index.Contains(search) ||
                    x.Name.Contains(search))
                .Select(x => new IndicatorsDto
                {
                    Id = x.Id,
                    Index = x.Index,
                    Name = x.Name,
                    CompetenceIndexes = x.Competence != null
                        ? new List<string> { x.Competence.Index }
                        : new List<string>(),
                    DisciplineIndexes = x.Disciplines.Select(d => d.Index).ToList()
                })
                .ToArrayAsync(); 

            _logger.Information("Поиск индикаторов. Запрос: '{Search}', Найдено: {Count}",
                search ?? "(все)", indicators.Length);

            return new CollectionResult<IndicatorsDto>
            {
                Data = indicators
            };
        }

        public async Task<CollectionResult<IndicatorsDto>> GetIndicatorsAsync()
        {
            var indicators = await _indicatorRepository.GetAll()
                .Include(x => x.Competence)   
                .Include(x => x.Disciplines)        
                .Select(x => new IndicatorsDto
                {
                    Id = x.Id,
                    Index = x.Index,
                    Name = x.Name,
                    
                    CompetenceIndexes = x.Competence != null
                        ? new List<string> { x.Competence.Index }
                        : new List<string>(),
                 
                    DisciplineIndexes = x.Disciplines
                        .Select(d => d.Index)
                        .ToList()
                })
                .ToArrayAsync();

            if (indicators.Length == 0)
            {
                _logger.Information("Список индикаторов пуст");
                throw new ExceptionResult(
                    ErrorCodes.IndicatorNotFound,
                    ErrorMessage.IndicatorNotFound
                );
            }

            _logger.Information("Получено индикаторов: {Count}", indicators.Length);

            return new CollectionResult<IndicatorsDto>
            {
                Data = indicators,
                Message = indicators.Length == 0 ? "Нет индикаторов" : null
            };
        }

        public async Task<BaseResult<IndicatorDto>> UpdateIndicatorAsync(IndicatorDto dto)
        {
            var indicator = await _indicatorRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (indicator == null)
            {
                _logger.Error("Индикатор не существует. Id: {Id}", dto.Id);
                throw new ExceptionResult(
                    ErrorCodes.IndicatorNotFound,
                    ErrorMessage.IndicatorNotFound
                );
            }

            var existing = await _indicatorRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id != dto.Id && (x.Index == dto.Index || x.Name == dto.Name));


            if (existing != null)
            {
                _logger.Error("Индикатор уже существует. Index: {Index}, Name: {Name}", dto.Index, dto.Name);
                throw new ExceptionResult(
                   ErrorCodes.IndicatorAlreadyExists,
                   ErrorMessage.IndicatorAlreadyExists
                );
            }

            indicator.Name = dto.Name;
            indicator.Index = dto.Index;

            var updatedIndicator = _indicatorRepository.Update(indicator);
            await _indicatorRepository.SaveChangesAsync();

            _logger.Information("Индикатор изменен. Id: {Id}, Index: {Index}",
                indicator.Id, indicator.Index);

            return new BaseResult<IndicatorDto>()
            {
                Data = _mapper.Map<IndicatorDto>(updatedIndicator),
            };
        }
    }
}
