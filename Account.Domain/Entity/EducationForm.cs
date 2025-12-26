using Account.Domain.Entity.LinkedEntites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Entity
{
    public class EducationForm
    {
        public long Id { get; set; } //
        public string Name { get; set; } //

        public ICollection<Group> Groups { get; set; } = new List<Group>(); //
    }
}
