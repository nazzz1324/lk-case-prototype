using Account.Domain.DTO.Competence;
using Account.Domain.DTO.Indicator;
using Account.Domain.DTO.ProfessionalRole;
using Account.Domain.DTO.Role;
using Account.Domain.DTO.UserRole;
using Account.Domain.DTO.UserRoleDto;
using Account.Domain.Entity;
using Account.Domain.Entity.AuthRole;
using Account.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Account.Domain.DTO.Group;
using CreateGroupDto = Account.Domain.DTO.Group.CreateGroupDto;

namespace Account.Domain.Interfaces.Services
{
    public interface IGroupService
    {
        /// <summary>
        /// Получение групп
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<CollectionResult<GroupsDto>> GetGroupsAsync();
        /// <summary>
        /// Создание группы
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<GroupDto>> CreateGroupAsync(CreateGroupDto dto);
        /// <summary>
        /// Удаление группы
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BaseResult<GroupDto>> DeleteGroupAsync(long id);
        /// <summary>
        /// Обновление группы
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<GroupDto>> UpdateGroupAsync(GroupDto dto);
    }
}