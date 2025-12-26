using Account.Domain.DTO.Competence;
using Account.Domain.DTO.Discipline;
using Account.Domain.Entity;
using Account.Domain.Interfaces.Services;
using Account.Domain.Result;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Account.api.Controllers
{
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]
    public class DisciplineController : ControllerBase
    {
        private readonly IDisciplineService _disciplineService;

        public DisciplineController(IDisciplineService disciplineService)
        {
            _disciplineService = disciplineService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CollectionResult<CompetencesDto>>> GetDisciplinesAsync()
        {
            var response = await _disciplineService.GetDisciplinesAsync();

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<Competence>>> CreateDisciplineAsync([FromBody] CreateDisciplineDto dto)
        {
            var response = await _disciplineService.CreateDisciplineAsync(dto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<CompetenceDto>>> DeleteDisciplineAsync(long id)
        {
            var response = await _disciplineService.DeleteDisciplineAsync(id);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<DisciplineDto>>> UpdateDisciplineAsync([FromBody] DisciplineDto dto)
        {
            var response = await _disciplineService.UpdateDisciplineAsync(dto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
