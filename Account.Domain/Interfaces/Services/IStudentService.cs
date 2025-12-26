using Account.Domain.DTO.Student;
using Account.Domain.DTO.Teacher;
using Account.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Interfaces.Services
{
    public interface IStudentService
    {
        Task<CollectionResult<StudentDisciplinesDto>> GetStudentDisciplinesAsync(long studentId);
        Task<CollectionResult<StudentCompetencesDto>> GetStudentCompetencesAsync(long studentId);
        Task<BaseResult<StudentDisciplineScoresDto>> GetDisciplineScoresAsync(long disciplineId, long studentId);
        Task<BaseResult<StudentCompetenceScoresDto>> GetCompetenceScoresAsync(long competenceId, long studentId);
        Task<BaseResult<StudentProlesDto>> GetStudentProlesAsync(long studentId);
    }
}
