using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.DTO.UserRole
{
    public class UpdateUserRoleDto
    {
        public string Login { get; set; }
        public long FromRoleId { get; set; }
        public long ToRoleId { get; set; }


    }
}
