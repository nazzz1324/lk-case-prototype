using Account.Domain.Entity.LinkedEntites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Entity
{
    public class Discipline
    {
        public long Id { get; set; }//
        public string? Index { get; set; }//
        public string Name { get; set; }//
        public string? Description { get; set; }//         

        public ICollection<Indicator> Indicators { get; set; } = new List<Indicator>(); 
        public ICollection<DisciplineScore> DisciplineScores { get; set; } = new List<DisciplineScore>();//
        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();//
        public ICollection<Group> Groups { get; set; } = new List<Group>();// ok
    }
}

