using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.DTO.UserRoleDto
{
    public class DeleteUserRoleDto
    {
        public string Login { get; set; }
        public long RoleId { get; set; }
    }
}
