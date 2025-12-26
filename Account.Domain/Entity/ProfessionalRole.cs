using Account.Domain.Entity.LinkedEntites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Entity
{
    public class ProfessionalRole
    {
        public long Id { get; set; }//
        public string Name { get; set; }//
        public string? Description { get; set; }//

        public ICollection<Competence> Competences { get; set; } = new List<Competence>();//
        public ICollection<Group> Groups { get; set; } = new List<Group>();//
    }
}
