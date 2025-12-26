
using Account.Domain.DTO.Competence;
using Account.Domain.DTO.Indicator;
using Account.Domain.DTO.Role;
using Account.Domain.Entity;
using Account.Domain.Entity.AuthRole;
using Account.Domain.Entity.LinkedEntites;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Mapping
{
    public class CompetenceMapping : Profile
    {
        public CompetenceMapping() 
        {
            CreateMap<Competence, CompetenceDto>();
            CreateMap<CreateCompetenceDto, Competence>();
        }
    }
}
