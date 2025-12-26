using Account.Domain.DTO.Teacher;
using Account.Domain.Interfaces.Services;
using Account.Domain.Result;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Account.api.Controllers
{
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Teacher")]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpGet("getDisciplines")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CollectionResult<TeacherDisciplineDto>>> GetTeacherDisciplinesAsync(long teacherId)
        {
            try
            {
                var response = await _teacherService.GetTeacherDisciplinesAsync(teacherId);

                if (response.IsSuccess)
                {
                    return Ok(response);
                }
                
                // Если преподаватель не найден, возвращаем 404
                if (response.ErrorCode == 92) // TeacherNotFound
                {
                    return NotFound(response);
                }
                
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { isSuccess = false, message = ex.Message });
            }
        }

        [HttpGet("getScoringData")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ScoringDataDto>>> GetScoringDataAsync([FromQuery] ScoringFilterDto filter)
        {
            var response = await _teacherService.GetScoringDataAsync(filter);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("saveScores")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<bool>>> SaveScoresAsync([FromBody] SaveScoresDto dto)
        {
            var response = await _teacherService.SaveScoresAsync(dto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
