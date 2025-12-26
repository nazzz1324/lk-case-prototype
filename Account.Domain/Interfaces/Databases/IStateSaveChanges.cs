using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Interfaces.Databases
{
    public interface IStateSaveChanges
    {
        Task<int> SaveChangesAsync();
    }
}
