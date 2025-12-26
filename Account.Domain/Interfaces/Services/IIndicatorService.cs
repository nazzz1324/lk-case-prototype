using Account.Domain.DTO.Competence;
using Account.Domain.DTO.Indicator;
using Account.Domain.Entity;
using Account.Domain.Entity.LinkedEntites;
using Account.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Interfaces.Services
{
    public interface IIndicatorService
    {
        /// <summary>
        /// Поиск индикаторов
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<CollectionResult<IndicatorsDto>> SearchIndicatorsAsync(string search);
        /// <summary>
        /// Получение индикаторов
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<CollectionResult<IndicatorsDto>> GetIndicatorsAsync();
        /// <summary>
        /// Создание индикатора
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<IndicatorDto>> CreateIndicatorAsync(CreateIndicatorDto dto);
        /// <summary>
        /// Удаление индикатора
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BaseResult<IndicatorDto>> DeleteIndicatorAsync(long id);
        /// <summary>
        /// Обновление индикатора
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<IndicatorDto>> UpdateIndicatorAsync(IndicatorDto dto);
        
    }
}
