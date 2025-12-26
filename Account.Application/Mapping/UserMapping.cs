using AutoMapper;
using Account.Domain.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Account.Domain.Entity.AuthRole;

namespace Account.Application.Mapping
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
