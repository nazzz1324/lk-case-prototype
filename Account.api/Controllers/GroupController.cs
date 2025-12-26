using Account.Domain.DTO.Competence;
using Account.Domain.DTO.Discipline;
using Account.Domain.DTO.ProfessionalRole;
using Account.Domain.Entity;
using Account.Domain.Interfaces.Services;
using Account.Domain.Result;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Account.Domain.DTO.Group;

namespace Account.api.Controllers
{
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CollectionResult<GroupsDto>>> GetGroupsAsync()
        {
            var response = await _groupService.GetGroupsAsync();

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<Group>>> CreateGroupAsync([FromBody] CreateGroupDto dto)
        {
            var response = await _groupService.CreateGroupAsync(dto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<GroupDto>>> DeleteGroupAsync(long id)
        {
            var response = await _groupService.DeleteGroupAsync(id);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<GroupDto>>> UpdateGroupAsync([FromBody] GroupDto dto)
        {
            var response = await _groupService.UpdateGroupAsync(dto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
