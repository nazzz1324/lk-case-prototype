using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.DTO.Teacher
{
    public class SaveScoresDto
    {
        public long DisciplineId { get; set; }
        public long TeacherId { get; set; }
        public List<ScoreItemDto> Scores { get; set; } = new();
    }
}
