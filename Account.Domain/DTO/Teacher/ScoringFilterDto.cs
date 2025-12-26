using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.DTO.Teacher
{
    public class ScoringFilterDto
    {
        public long DisciplineId { get; set; }
        public long GroupId { get; set; }
        public long TeacherId { get; set; }
    }
}
