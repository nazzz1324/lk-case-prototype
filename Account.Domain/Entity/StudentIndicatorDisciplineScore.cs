using Account.Domain.Entity.LinkedEntites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Entity
{
    public class StudentIndicatorDisciplineScore
    {
        public long Id { get; set; }

        // Составной контекст (только FK)
        public long StudentId { get; set; }
        public long DisciplineId { get; set; }
        public long IndicatorId { get; set; }
        public long TeacherId { get; set; }

        // Данные
        public decimal Score { get; set; }
    }
}
