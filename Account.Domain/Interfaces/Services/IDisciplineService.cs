using Account.Domain.DTO.Competence;
using Account.Domain.DTO.Discipline;
using Account.Domain.DTO.Indicator;
using Account.Domain.DTO.Role;
using Account.Domain.DTO.UserRole;
using Account.Domain.DTO.UserRoleDto;
using Account.Domain.Entity;
using Account.Domain.Entity.AuthRole;
using Account.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Interfaces.Services
{
    public interface IDisciplineService
    {
        /// <summary>
        /// Получение индикаторов
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<CollectionResult<DisciplinesDto>> GetDisciplinesAsync();
        /// <summary>
        /// Создание компетенции
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<DisciplineDto>> CreateDisciplineAsync(CreateDisciplineDto dto);
        /// <summary>
        /// Удаление компетенции
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BaseResult<DisciplineDto>> DeleteDisciplineAsync(long id);
        /// <summary>
        /// Обновление компетенции
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<DisciplineDto>> UpdateDisciplineAsync(DisciplineDto dto);
    }
}
