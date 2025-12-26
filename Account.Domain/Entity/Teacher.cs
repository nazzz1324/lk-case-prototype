using Account.Domain.Entity.AuthRole;
using Account.Domain.Interfaces.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Entity
{
    public class Teacher : IPeople
    {
        public long Id { get; set; }//
        public string Firstname { get; set; }//
        public string Lastname { get; set; }//
        public string? Middlename { get; set; }//

        public User User { get; set; }//
        public ICollection<Discipline> Disciplines { get; set; } = new List<Discipline>();//
        public ICollection<IndicatorScore> IndicatorScores { get; set; } = new List<IndicatorScore>();//

    }
}
