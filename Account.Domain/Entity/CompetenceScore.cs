using Account.Domain.Entity.AuthRole;
using Account.Domain.Interfaces.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Entity
{
    public class CompetenceScore
    {
        public long Id { get; set; }//
        public decimal? Score { get; set; }//
        //public byte Semester { get; set; }//

        public long StudentId { get; set; }//
        public long CompetenceId { get; set; }//

        public Student Student { get; set; }//
        public Competence Competence { get; set; }//
    }
}
