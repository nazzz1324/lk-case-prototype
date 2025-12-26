using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.DTO.Student
{
    public class StudentDisciplinesDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string TeacherName { get; set; }
        public decimal Score { get; set; }
    }
}
