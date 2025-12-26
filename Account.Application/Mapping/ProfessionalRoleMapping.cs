using Account.Domain.DTO.Indicator;
using Account.Domain.DTO.ProfessionalRole;
using Account.Domain.Entity;
using Account.Domain.Entity.LinkedEntites;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Mapping
{
    internal class ProfessionalRoleMapping : Profile
    {
        public ProfessionalRoleMapping()
        {
            CreateMap<ProfessionalRole, ProfessionalRoleDto>();
            CreateMap<CreateProfessionalRoleDto, ProfessionalRole>();
        }
    }
}
