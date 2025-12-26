using Account.Domain.DTO.Indicator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.DTO.Teacher
{
    public class ScoringDataDto
    {
        public string DisciplineName { get; set; }
        public string GroupName { get; set; }
        public List<StudentScoreDto> Students { get; set; } = new();
        public List<TeacherIndicatorDto> Indicators { get; set; } = new();
    }
}
