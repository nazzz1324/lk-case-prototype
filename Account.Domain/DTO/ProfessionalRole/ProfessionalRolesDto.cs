using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.DTO.ProfessionalRole
{
    public class ProfessionalRolesDto
    {
        public long Id {  get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> CompetenceIndexes { get; set; } = new();
    }
}
