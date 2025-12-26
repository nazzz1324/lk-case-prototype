using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.DTO.Teacher
{
    public class ScoreItemDto
    {
        public long StudentId { get; set; }
        public long IndicatorId { get; set; }
        public decimal Score { get; set; }
    }
}
