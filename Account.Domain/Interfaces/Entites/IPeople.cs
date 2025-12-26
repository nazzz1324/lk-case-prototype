using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Interfaces.Entites
{
    public interface IPeople
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string? Middlename { get; set; }
    }
}
