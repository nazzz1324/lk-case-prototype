using Account.Domain.DTO.ProfessionalRole;
using Account.Domain.DTO.Teacher;
using Account.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Interfaces.Services
{
    public interface ITeacherService
    {
        Task<CollectionResult<TeacherDisciplineDto>> GetTeacherDisciplinesAsync(long teacherId);
        Task<BaseResult<ScoringDataDto>> GetScoringDataAsync(ScoringFilterDto filter);
        Task<BaseResult<bool>> SaveScoresAsync(SaveScoresDto dto);
    }
}
