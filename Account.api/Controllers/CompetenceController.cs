using Account.Domain.DTO.Competence;
using Account.Domain.Entity;
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
    public class CompetenceController : ControllerBase
    {
        private readonly ICompetenceService _competenceService;

        public CompetenceController(ICompetenceService competenceService)
        {
            _competenceService = competenceService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(CollectionResult<CompetencesDto>), 200)]
        public async Task<ActionResult<CollectionResult<CompetencesDto>>> GetCompetencesAsync()
        {
            var response = await _competenceService.GetCompetencesAsync();

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResult<Competence>), 200)]
        [ProducesResponseType(typeof(BaseResult<Competence>), 400)]
        public async Task<ActionResult<BaseResult<Competence>>> CreateCompetenceAsync([FromBody] CreateCompetenceDto dto)
        {
            var response = await _competenceService.CreateCompetenceAsync(dto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(BaseResult<CompetenceDto>), 200)]
        [ProducesResponseType(typeof(BaseResult<CompetenceDto>), 400)]
        public async Task<ActionResult<BaseResult<CompetenceDto>>> DeleteCompetenceAsync(long id)
        {
            var response = await _competenceService.DeleteCompetenceAsync(id);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut]
        [ProducesResponseType(typeof(BaseResult<CompetenceDto>), 200)]
        [ProducesResponseType(typeof(BaseResult<CompetenceDto>), 400)]
        public async Task<ActionResult<BaseResult<CompetenceDto>>> UpdateCompetenceAsync([FromBody] CompetenceDto dto)
        {
            var response = await _competenceService.UpdateCompetenceAsync(dto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}