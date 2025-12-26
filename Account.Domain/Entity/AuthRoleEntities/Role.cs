using Account.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Entity.AuthRole
{
    public class Role : IEntityID<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }

    }
}
