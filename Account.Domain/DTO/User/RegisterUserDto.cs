using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.DTO.User
{
    public class RegisterUserDto
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Middlename { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        //public string PasswordConfirm { get; set; }
        public long RoleId { get; set; }
    }
}
