using Account.Application.Resources;
using Account.Domain.DTO.Competence;
using Account.Domain.DTO.Indicator;
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
    public class CompetenceService : ICompetenceService
    {
        private readonly IBaseRepository<Competence> _competenceRepository;
        private readonly IBaseRepository<Indicator> _indicatorRepository;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public CompetenceService(IBaseRepository<Competence> competenceRepository, ILogger logger, IMapper mapper,
            IBaseRepository<Indicator> indicatorRepository)
        {
            _competenceRepository = competenceRepository;
            _logger = logger;
            _mapper = mapper;
            _indicatorRepository = indicatorRepository;
        }

        public async Task<BaseResult<CompetenceDto>> CreateCompetenceAsync(CreateCompetenceDto dto)
        {
            var existingCompetence = await _competenceRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Index == dto.Index || x.Name == dto.Name);

            if (existingCompetence != null)
            {
                _logger.Error("Компетенция уже существует. Index: {Index}, Name: {Name}", dto.Index, dto.Name);
                throw new ExceptionResult(
                    ErrorCodes.CompetenceAlreadyExists,
                    ErrorMessage.CompetenceAlreadyExists
                );
            }

            var competence = new Competence
            {
                Index = dto.Index,
                Name = dto.Name,
                Description = dto.Description
            };

            await _competenceRepository.CreateAsync(competence);
            await _competenceRepository.SaveChangesAsync();

            if (dto.IndicatorIds != null && dto.IndicatorIds.Any())
            {
                var indicators = await _indicatorRepository.GetAll()
                    .Where(x => dto.IndicatorIds.Contains(x.Id))
                    .ToListAsync();

                foreach (var indicator in indicators)
                {
                    indicator.CompetenceId = competence.Id;
                }

                await _indicatorRepository.SaveChangesAsync();
            }

            _logger.Information("Компетенция создана. Id: {Id}, Index: {Index}, Привязано индикаторов: {Count}",
                competence.Id, competence.Index,
                dto.IndicatorIds?.Count ?? 0);

            return new BaseResult<CompetenceDto>()
            {
                Data = _mapper.Map<CompetenceDto>(competence)
            };
        }

        public async Task<BaseResult<CompetenceDto>> DeleteCompetenceAsync(long id)
        {
            var competence = await _competenceRepository.GetAll()
               .FirstOrDefaultAsync(x => x.Id == id);

            if (competence == null)
            {
                _logger.Error("Компетенция не существует. Id: {Id}", id);
                throw new ExceptionResult(
                    ErrorCodes.IndicatorNotFound,
                    ErrorMessage.IndicatorNotFound
                );
            }

            _competenceRepository.Remove(competence);
            await _competenceRepository.SaveChangesAsync();

            _logger.Information("Компетенция удалена. Id: {Id}, Index: {Index}",
                competence.Id, competence.Index);

            return new BaseResult<CompetenceDto>()
            {
                Data = _mapper.Map<CompetenceDto>(competence),
            };
        }

        public async Task<CollectionResult<CompetencesDto>> GetCompetencesAsync()
        {
            var competences = await _competenceRepository.GetAll()
                .ToArrayAsync();

            if (competences.Length == 0)
            {
                _logger.Information("Список компетенций пуст");
                throw new ExceptionResult(
                    ErrorCodes.CompetenceNotFound,
                    ErrorMessage.CompetenceNotFound
                );
            }

            _logger.Information("Получено компетенций: {Count}", competences.Length);

            var competencesDto = competences.Select(c => new CompetencesDto
            {
                Id = c.Id,
                Index = c.Index,
                Name = c.Name,
                Description = c.Description
            }).ToArray();

            return new CollectionResult<CompetencesDto>
            {
                Data = competencesDto,
            };
        }

        public async Task<BaseResult<CompetenceDto>> UpdateCompetenceAsync(CompetenceDto dto)
        {
            var competence = await _competenceRepository.GetAll()
                .Include(c => c.Indicators)
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (competence == null)
            {
                _logger.Error("Компетенция не существует. Id: {Id}", dto.Id);
                throw new ExceptionResult(
                    ErrorCodes.CompetenceNotFound,
                    ErrorMessage.CompetenceNotFound
                );
            }

            competence.Index = dto.Index;
            competence.Name = dto.Name;
            competence.Description = dto.Description;

            var indicatorsToAttach = await _indicatorRepository.GetAll()
                .Where(x => dto.IndicatorIds.Contains(x.Id)) 
                .ToListAsync();

            foreach (var currentIndicator in competence.Indicators.ToList())
            {
                currentIndicator.CompetenceId = null;
            }

            foreach (var indicator in indicatorsToAttach)
            {
                indicator.CompetenceId = competence.Id;
            }

            _competenceRepository.Update(competence);
            await _competenceRepository.SaveChangesAsync();

            _logger.Information("Компетенция обновлена. Id: {Id}, Index: {Index}, Привязано индикаторов: {Count}",
                competence.Id, competence.Index, indicatorsToAttach.Count);

            return new BaseResult<CompetenceDto>()
            {
                Data = _mapper.Map<CompetenceDto>(competence),
            };
        }
    }
}
