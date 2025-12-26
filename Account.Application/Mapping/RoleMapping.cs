using AutoMapper;
using Account.Domain.DTO.Role;
using Account.Domain.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Account.Domain.Entity.AuthRole;

namespace Account.Application.Mapping
{
    public class RoleMapping : Profile
    {
        public RoleMapping()
        {
            CreateMap<Role, RoleDto>().ReverseMap();
        }
    }
}
