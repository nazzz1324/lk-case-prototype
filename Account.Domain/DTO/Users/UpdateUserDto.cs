using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.DTO.Users
{
    public class UpdateUserDto
    {
        public long Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string? Middlename { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public long RoleId { get; set; }
    }
}
