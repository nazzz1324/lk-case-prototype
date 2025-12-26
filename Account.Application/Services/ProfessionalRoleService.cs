using Account.Application.Resources;
using Account.Domain.DTO.Discipline;
using Account.Domain.DTO.Indicator;
using Account.Domain.DTO.ProfessionalRole;
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
    public class ProfessionalRoleService : IProfessionalRoleService
    {
        private readonly IBaseRepository<Competence> _competenceRepository;
        private readonly IBaseRepository<ProfessionalRole> _proleRepository;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public ProfessionalRoleService(
            IBaseRepository<Competence> competenceRepository, 
            IBaseRepository<ProfessionalRole> proleRepository,
            ILogger logger,
            IMapper mapper)
        {
            _competenceRepository = competenceRepository;
            _proleRepository = proleRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<BaseResult<ProfessionalRoleDto>> CreateProfessionalRoleAsync(CreateProfessionalRoleDto dto)
        {
            var prole = await _proleRepository.GetAll()
               .FirstOrDefaultAsync(x => x.Name == dto.Name);

            if (prole != null)
            {
                _logger.Error("Профессиональная роль уже существует. Name: {Name}", dto.Name);
                throw new ExceptionResult(
                    ErrorCodes.ProleAlreadyExists,
                    ErrorMessage.ProleAlreadyExists
                );
            }

            prole = new ProfessionalRole
            {
                Name = dto.Name,
                Description = dto.Description,
            };

            await _proleRepository.CreateAsync(prole);
            await _proleRepository.SaveChangesAsync();

            if (dto.CompetenceIds != null && dto.CompetenceIds.Any())
            {
                var competences = await _competenceRepository.GetAll()
                    .Where(x => dto.CompetenceIds.Contains(x.Id))
                    .ToListAsync();
                foreach (var competence in competences)
                {
                    prole.Competences.Add(competence);
                }
                await _proleRepository.SaveChangesAsync();
            }

            _logger.Information("Профессиональная роль создана. Name: {Name}, Привязано компетенций: {Count}",
                prole.Name,
                dto.CompetenceIds?.Count ?? 0);

            return new BaseResult<ProfessionalRoleDto>()
            {
                Data = _mapper.Map<ProfessionalRoleDto>(prole)
            };
        }

        public async Task<BaseResult<ProfessionalRoleDto>> DeleteProfessionalRoleAsync(long id)
        {
            var prole = await _proleRepository.GetAll()
               .FirstOrDefaultAsync(x => x.Id == id);

            if (prole == null)
            {
                _logger.Error("Профессиональная роль не существует. Id: {Id}", id);
                throw new ExceptionResult(
                    ErrorCodes.ProleNotFound,
                    ErrorMessage.ProleNotFound
                );
            }

            _proleRepository.Remove(prole);
            await _proleRepository.SaveChangesAsync();

            _logger.Information("Профессиональная роль удалена. Name: {Name}",
                prole.Name);

            return new BaseResult<ProfessionalRoleDto>()
            {
                Data = _mapper.Map<ProfessionalRoleDto>(prole),
            };
        }

        public async Task<CollectionResult<ProfessionalRolesDto>> GetProfessionalRolesAsync()
        {
            var proles = await _proleRepository.GetAll()
                .Include(x => x.Competences)
                .Select(x => new ProfessionalRolesDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,

                    CompetenceIndexes = x.Competences
                        .Select(d => d.Index)
                        .ToList()
                })
                .ToArrayAsync();

            if (proles.Length == 0)
            {
                _logger.Information("Список профессиональных ролей пуст");
                throw new ExceptionResult(
                    ErrorCodes.ProleNotFound,
                    ErrorMessage.ProleNotFound
                );
            }

            _logger.Information("Получено профессиональных ролей: {Count}", proles.Length);

            return new CollectionResult<ProfessionalRolesDto>
            {
                Data = proles,
            };
        }

        public async Task<BaseResult<ProfessionalRoleDto>> UpdateProfessionalRoleAsync(ProfessionalRoleDto dto)
        {
            var prole = await _proleRepository.GetAll()
                .Include(d => d.Competences)
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (prole == null)
            {
                _logger.Error("Профессиональная роль не существует. Id: {Id}", dto.Id);
                throw new ExceptionResult(
                    ErrorCodes.DisciplineNotFound,
                    ErrorMessage.DisciplineNotFound
                );
            }

            prole.Name = dto.Name;
            prole.Description = dto.Description;

            var competences = await _competenceRepository.GetAll()
                .Where(x => dto.CompetenceIds.Contains(x.Id))
                .ToListAsync();

            prole.Competences.Clear();

            foreach (var competence in competences)
            {
                prole.Competences.Add(competence);
            }

            _proleRepository.Update(prole);
            await _proleRepository.SaveChangesAsync();

            _logger.Information("Профессиональная роль обновлена. Id: {Id}, Привязано компетенций: {Count}",
                prole.Id, competences.Count);

            return new BaseResult<ProfessionalRoleDto>()
            {
                Data = _mapper.Map<ProfessionalRoleDto>(prole),
            };
        }
    }
}
