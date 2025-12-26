using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.DTO.Indicator
{
    public class IndicatorsDto
    {
        public long Id { get; set; }
        public string Index { get; set; }        
        public string Name { get; set; }          

        public List<string> CompetenceIndexes { get; set; } = new();  
        public List<string> DisciplineIndexes { get; set; } = new(); 
    }
}
