using Account.Domain.Entity.LinkedEntites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Entity
{
    public class Competence
    {
        public long Id { get; set; }//
        public string Index { get; set; }//
        public string Name { get; set; }//
        public string Description { get; set; }//

        public ICollection<Indicator> Indicators { get; set; } = new List<Indicator>();//
        public ICollection<CompetenceScore> CompetenceScores { get; set; } = new List<CompetenceScore>();//
        public ICollection<ProfessionalRole> ProfessionalRoles { get; set; } = new List<ProfessionalRole>();//ok
    }

}
