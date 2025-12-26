using Account.Domain.DTO.Competence;
using Account.Domain.DTO.Discipline;
using Account.Domain.Entity;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Mapping
{
    internal class DisciplineMapping : Profile
    {
        public DisciplineMapping()
        {
            CreateMap<Discipline, DisciplineDto>();
            CreateMap<CreateDisciplineDto, Discipline>();
        }
    }
}
