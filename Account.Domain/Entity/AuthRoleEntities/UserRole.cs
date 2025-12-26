using Account.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Entity.AuthRole
{
    public class UserRole 
    {
        public long UserId { get; set; }
        public long RoleId { get; set; }

    }
}
