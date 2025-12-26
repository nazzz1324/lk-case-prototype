using Account.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Interfaces
{
    public interface IAuditable
    {
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
