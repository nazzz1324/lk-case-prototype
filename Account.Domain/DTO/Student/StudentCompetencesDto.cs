using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.DTO.Student
{
    public class StudentCompetencesDto
    {
        public long Id { get; set; }
        public string Index {  get; set; }
        public string Name { get; set; }
        public decimal Progress { get; set; }
    }
}
