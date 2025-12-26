using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.DTO.Competence
{
    public class CreateCompetenceDto
    {
        public string Index { get; set; }
        public string Name { get; set; }          
        public string Description { get; set; }
        public List<long>? IndicatorIds { get; set; } = new();
    }
}
