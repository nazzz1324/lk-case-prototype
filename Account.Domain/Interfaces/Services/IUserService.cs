using Account.Domain.DTO.Group;
using Account.Domain.DTO.User;
using Account.Domain.DTO.Users;
using Account.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<CollectionResult<UsersDto>> GetUsersAsync();
        Task<BaseResult<bool>> DeleteUserAsync(long id);
        Task<BaseResult<bool>> UpdateUserAsync(UpdateUserDto dto);
    }
    
}
