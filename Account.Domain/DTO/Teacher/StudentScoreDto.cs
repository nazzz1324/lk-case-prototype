using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.DTO.Teacher
{
    public class StudentScoreDto
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public List<decimal?> Scores { get; set; } = new(); // оценки за 3 индикатора
    }
}
