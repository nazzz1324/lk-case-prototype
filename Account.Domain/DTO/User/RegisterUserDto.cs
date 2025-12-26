using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.DTO.User
{
    public record RegisterUserDto (string Login, string Password, string PasswordConfirm) { }
}
