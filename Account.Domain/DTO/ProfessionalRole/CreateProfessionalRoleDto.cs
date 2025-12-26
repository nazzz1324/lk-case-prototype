using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.DTO.ProfessionalRole
{
    public class CreateProfessionalRoleDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<long> CompetenceIds { get; set; } = new();
    }
}
