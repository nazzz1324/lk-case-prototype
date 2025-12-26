using Account.Domain.Entity.AuthRole;
using Account.Domain.Interfaces.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Entity
{
    public class Student : IPeople
    {
        public long Id { get; set; } // Номер зачетки, PK for Student and FK for User
        public string Firstname { get; set ; }//
        public string Lastname { get; set; }//
        public string? Middlename { get; set; }//
        //public string? DateOfBirth { get; set; }
        //public string StudentStatus { get; set; }
        //public int EnrollmentYear { get; set; } = DateTime.Now.Year;
        public long? GroupId { get; set; }//
        public bool IsActive { get; set; } = true;

        public ICollection<DisciplineScore> DisciplineScores { get; set; } = new List<DisciplineScore>();//
        public ICollection<CompetenceScore> CompetenceScores { get; set; } = new List<CompetenceScore>();//
        public ICollection<IndicatorScore> IndicatorScores { get; set; } = new List<IndicatorScore>();//

        public User User { get; set; }//
        public Group Group { get; set; }//
    }
}
