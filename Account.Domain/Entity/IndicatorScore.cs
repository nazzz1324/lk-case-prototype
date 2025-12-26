using Account.Domain.Entity.AuthRole;
using Account.Domain.Entity.LinkedEntites;
using Account.Domain.Interfaces.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Entity
{
    public class IndicatorScore
    {
        public long Id { get; set; }//
        public decimal? ScoreValue { get; set; }//
        public decimal MaxScore { get; set; } = 5;//
        //public byte Semester { get; set; }//
        public long StudentId { get; set; }//
        public long IndicatorId { get; set; }
        public long TeacherId { get; set; }//

        public Student Student { get; set; }//
        public Indicator Indicator { get; set; }//
        public Teacher Teacher { get; set; }//
    }
}
