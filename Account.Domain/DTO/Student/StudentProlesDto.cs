using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.DTO.Student
{
    public class StudentProlesDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Index { get; set; }
        public decimal? Score { get; set; }
        public List<long> Competencies { get; set; } = new List<long>();
        public int CompletedCompetenciesCount { get; set; }

    }
}
