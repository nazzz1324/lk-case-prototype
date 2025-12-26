using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.DTO.Student
{
    public class StudentDisciplineScoresDto
    {
        public string Name { get; set; }
        public List<DisciplineIndicatorScoreDto> Indicators { get; set; } = new();
        public decimal? DisciplineScore { get; set; }
    }
}
