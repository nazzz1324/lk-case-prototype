using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.DTO.Users
{
    public class UsersDto
    {
        public long Id { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Role {  get; set; }
        public bool IsActive { get; set; }
    }
}
