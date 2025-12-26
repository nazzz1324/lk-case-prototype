using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Entity.LinkedEntites
{
    public class Indicator
    {
        public long Id { get; set; }//
        public string Index { get; set; }//
        public string Name { get; set; }//
        public string? Description { get; set; }//
        public long? CompetenceId { get; set; }//

        public ICollection<Discipline> Disciplines { get; set; } = new List<Discipline>();//
        public Competence Competence { get; set; }//
        public ICollection<IndicatorScore> IndicatorScores { get; set; } = new List<IndicatorScore>();//
    }
}//maxScore вставить в индикатор скор