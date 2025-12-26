using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Account.Domain.Entity
{
    public class Group
    {
        public long Id { get; set; } //
        public string Name { get; set; } //
        //public long FacultyId { get; set; }
        public string Curator { get; set; }//
        public long ProleId { get; set; } //
        //public long EducationFormId { get; set; }

        public ProfessionalRole ProfessionalRole { get; set; }//
        public ICollection<Student> Students { get; set; } = new List<Student>();//
        public ICollection<Discipline> Disciplines { get; set; } = new List<Discipline>();//
    }
}
