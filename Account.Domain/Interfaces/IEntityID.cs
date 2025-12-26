using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Interfaces
{
    public interface IEntityID<T> where T : struct
    {
        public T Id { get; set; }
    }
}
