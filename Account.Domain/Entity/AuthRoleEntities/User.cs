using Account.Domain.Interfaces;
using System.Collections.Generic;

namespace Account.Domain.Entity.AuthRole
{
    public class User : IEntityID<long>
    {
        public long Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public UserToken UserToken { get; set; }
        public List<Role> Roles { get; set; }
        public Teacher Teacher { get; set; }
        public Student Student { get; set; }
    }
}

