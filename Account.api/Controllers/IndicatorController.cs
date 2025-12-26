using Account.Domain.DTO.Indicator;
using Account.Domain.Entity.LinkedEntites;
using Account.Domain.Interfaces.Services;
using Account.Domain.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Account.api.Controllers
{
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]
    public class IndicatorController : ControllerBase
    {
        private readonly IIndicatorService _indicatorService;

        public IndicatorController(IIndicatorService indicatorService)
        {
            _indicatorService = indicatorService;
        }

        [HttpGet("getIndicators")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CollectionResult<IndicatorsDto>>> GetIndicatorsAsync()
        {
            var response = await _indicatorService.GetIndicatorsAsync();

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("searchIndicators")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CollectionResult<IndicatorsDto>>> SearchIndicatorsAsync([FromQuery] string search)
        {
            var response = await _indicatorService.SearchIndicatorsAsync(search);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("createIndicator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<IndicatorDto>>> CreateIndicatorAsync([FromBody] CreateIndicatorDto dto)
        {
            var response = await _indicatorService.CreateIndicatorAsync(dto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("deleteIndicator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<IndicatorDto>>> DeleteIndicatorAsync(long id)
        {
            var response = await _indicatorService.DeleteIndicatorAsync(id);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("updateIndicator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<IndicatorDto>>> UpdateIndicatorAsync([FromBody] IndicatorDto dto)
        {
            var response = await _indicatorService.UpdateIndicatorAsync(dto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
