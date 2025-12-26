using Account.Domain.DTO.Role;
using Account.Domain.DTO.UserRole;
using Account.Domain.DTO.UserRoleDto;
using Account.Domain.Entity.AuthRole;
using Account.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Interfaces.Services
{
    /// <summary>
    /// Сервис для управления ролями
    /// </summary>
    public interface IRoleService 
    {
        /// <summary>
        /// Создание роли
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<Role>> CreateRoleAsync(CreateRoleDto dto);
        /// <summary>
        /// Удаление роли
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BaseResult<RoleDto>> DeleteRoleAsync(long id);
        /// <summary>
        /// Обновление роли
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<RoleDto>> UpdateRoleAsync(RoleDto dto);
        /// <summary>
        /// Добавление роли для пользователя
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto);
        /// <summary>
        /// Удаление роли у пользователя
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<UserRoleDto>> DeleteRoleForUserAsync(DeleteUserRoleDto dto);
        /// <summary>
        /// Обновление роли у пользователя
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<UserRoleDto>> UpdateRoleForUserAsync(UpdateUserRoleDto dto);
    }
}
