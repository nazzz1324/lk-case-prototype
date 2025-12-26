
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
            CreateMap<Competence, CompetenceDto>()
                .ForMember(dest => dest.Indicators,
                    opt => opt.MapFrom(src => src.Indicators.Select(i => i.Id).ToList()));

            CreateMap<CompetenceDto, Competence>()
                .ForMember(dest => dest.Indicators,
                    opt => opt.MapFrom(src => src.Indicators.Select(id => new Indicator { Id = id }).ToList()));

            CreateMap<CreateCompetenceDto, Competence>();
        }
    }
}
