using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.DTO.Group
{
    public class CreateGroupDto
    {
        public string Name { get; set; }
        public string Curator { get; set; }
        public List<long> StudentIds { get; set; } = new();
    }
}