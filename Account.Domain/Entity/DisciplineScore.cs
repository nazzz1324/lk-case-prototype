using Account.Domain.Entity.AuthRole;
using Account.Domain.Interfaces.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Entity
{
    public class DisciplineScore
    {
        public long Id { get; set; } //
        public decimal? Score { get; set; } //
        //public byte Semester { get; set; } //

        public long StudentId { get; set; } //
        public long DisciplineId { get; set; } //

        public Student Student { get; set; } //
        public Discipline Discipline { get; set; } // ok

    }
}
