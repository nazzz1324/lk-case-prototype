using Account.Domain.DTO.Competence;
using Account.Domain.DTO.Discipline;
using Account.Domain.DTO.ProfessionalRole;
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
    public class ProfessionalRoleController : ControllerBase
    {
        private readonly IProfessionalRoleService _proleService;

        public ProfessionalRoleController(IProfessionalRoleService proleService)
        {
            _proleService = proleService;
        }

        [HttpGet("getProles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CollectionResult<ProfessionalRolesDto>>> GetProfessionalRolesAsync()
        {
            var response = await _proleService.GetProfessionalRolesAsync();

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("createProle")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ProfessionalRole>>> CreateProfessionalRoleAsync([FromBody] CreateProfessionalRoleDto dto)
        {
            var response = await _proleService.CreateProfessionalRoleAsync(dto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("deleteProle")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ProfessionalRoleDto>>> DeleteProfessionalRoleAsync(long id)
        {
            var response = await _proleService.DeleteProfessionalRoleAsync(id);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("updateProle")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ProfessionalRoleDto>>> UpdateProfessionalRoleAsync([FromBody] ProfessionalRoleDto dto)
        {
            var response = await _proleService.UpdateProfessionalRoleAsync(dto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
