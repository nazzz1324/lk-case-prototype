using Account.Domain.Entity.AuthRole;
using Account.Domain.Interfaces.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Entity
{
    public class TeacherDiscipline
    {
        public long TeacherId { get; set; }//
        public long DisciplineId { get; set; }//
    }
}
