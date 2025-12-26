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
using Account.Domain.DTO.Group;

namespace Account.Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly IBaseRepository<Student> _studentRepository;
        private readonly IBaseRepository<Group> _groupRepository;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public GroupService(IBaseRepository<Student> studentRepository,
            IBaseRepository<Group> groupRepository,
            ILogger logger,
            IMapper mapper)
        {
            _studentRepository = studentRepository;
            _groupRepository = groupRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<CollectionResult<GroupsDto>> GetGroupsAsync()
        {
            var groups = await _groupRepository.GetAll()
                .Include(x => x.Students)
                .Select(x => new GroupsDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Curator = x.Curator,
                    studentCount = x.Students.Count,
                    students = x.Students.Select(s => s.Id).ToList()
                })
                .ToArrayAsync();

            if (groups.Length == 0)
            {
                _logger.Information("Список групп пуст");
                throw new ExceptionResult(
                    ErrorCodes.GroupNotFound,
                    ErrorMessage.GroupNotFound
                );
            }

            _logger.Information("Получено групп: {Count}", groups.Length);

            return new CollectionResult<GroupsDto>
            {
                Data = groups,
            };
        }

        public async Task<BaseResult<GroupDto>> CreateGroupAsync(CreateGroupDto dto)
        {
            var group = await _groupRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Name == dto.Name);

            if (group != null)
            {
                _logger.Error("Группа уже существует. Name: {Name}", dto.Name);
                throw new ExceptionResult(
                    ErrorCodes.GroupAlreadyExists,
                    ErrorMessage.GroupAlreadyExists
                );
            }

            group = new Group
            {
                Name = dto.Name,
                Curator = dto.Curator,
            };

            await _groupRepository.CreateAsync(group);
            await _groupRepository.SaveChangesAsync();

            if (dto.students != null && dto.students.Any())
            {
                var students = await _studentRepository.GetAll()
                    .Where(x => dto.students.Contains(x.Id))
                    .ToListAsync();
                foreach (var student in students)
                {
                    group.Students.Add(student);
                }
                await _groupRepository.SaveChangesAsync();
            }

            _logger.Information("Группа создана. Name: {Name}, Привязано студентов: {Count}",
                group.Name,
                dto.students?.Count ?? 0);

            return new BaseResult<GroupDto>()
            {
                Data = _mapper.Map<GroupDto>(group)
            };
        }

        public async Task<BaseResult<GroupDto>> DeleteGroupAsync(long id)
        {
            var group = await _groupRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (group == null)
            {
                _logger.Error("Группа не существует. Id: {Id}", id);
                throw new ExceptionResult(
                    ErrorCodes.GroupNotFound,
                    ErrorMessage.GroupNotFound
                );
            }

            _groupRepository.Remove(group);
            await _groupRepository.SaveChangesAsync();

            _logger.Information("Группа удалена. Name: {Name}",
                group.Name);

            return new BaseResult<GroupDto>()
            {
                Data = _mapper.Map<GroupDto>(group),
            };
        }

        public async Task<BaseResult<GroupDto>> UpdateGroupAsync(GroupDto dto)
        {
            var group = await _groupRepository.GetAll()
                .Include(g => g.Students)
                .FirstOrDefaultAsync(g => g.Id == dto.Id);

            if (group == null)
            {
                _logger.Error("Группа не существует. Id: {Id}", dto.Id);
                throw new ExceptionResult(
                    ErrorCodes.GroupNotFound,
                    ErrorMessage.GroupNotFound
                );
            }

            group.Name = dto.Name;
            group.Curator = dto.Curator;

            var students = await _studentRepository.GetAll()
                .Where(x => dto.students.Contains(x.Id))
                .ToListAsync();

            group.Students.Clear();

            foreach (var student in students)
            {
                group.Students.Add(student);
            }

            _groupRepository.Update(group);
            await _groupRepository.SaveChangesAsync();

            _logger.Information("Группа обновлена. Id: {Id}, Привязано студентов: {Count}",
                group.Id, students.Count);

            return new BaseResult<GroupDto>()
            {
                Data = _mapper.Map<GroupDto>(group),
            };
        }
    }
}