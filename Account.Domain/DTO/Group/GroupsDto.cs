using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.DTO.Group
{
    public class GroupsDto
    {
        public long Id {  get; set; }
        public string Name { get; set; }
        public int StudentsCount { get; set; }
        public string Curator { get; set; }
    }
}