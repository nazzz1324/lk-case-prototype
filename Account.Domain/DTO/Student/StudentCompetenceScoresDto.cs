using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.DTO.Student
{
    public class StudentCompetenceScoresDto
    {
        public string Name { get; set; }
        public List<CompetenceIndicatorScore> Indicators { get; set; } = new();
        public decimal? Score { get; set; }
    }
}
