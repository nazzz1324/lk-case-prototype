using Account.Domain.DTO.Student;
using Account.Domain.Interfaces.Services;
using Account.Domain.Result;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Account.api.Controllers
{
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Student")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet("getDisciplines")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CollectionResult<StudentDisciplinesDto>>> GetStudentDisciplinesAsync(long studentId)
        {
            var response = await _studentService.GetStudentDisciplinesAsync(studentId);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("getCompetences")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CollectionResult<StudentCompetencesDto>>> GetStudentCompetencesAsync(long studentId)
        {
            var response = await _studentService.GetStudentCompetencesAsync(studentId);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("getDisciplineScores")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<StudentDisciplineScoresDto>>> GetDisciplineScoresAsync(long studentId, long disciplineId)
        {
            var response = await _studentService.GetDisciplineScoresAsync(disciplineId, studentId);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet("getCompetenceScores")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] 
        public async Task<ActionResult<BaseResult<StudentCompetenceScoresDto>>> GetCompetenceScoresAsync(long studentId, long competenceId)
        {
            var response = await _studentService.GetCompetenceScoresAsync(competenceId, studentId);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("getStudentProles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] 
        public async Task<ActionResult<BaseResult<StudentProlesDto>>> GetStudentProlesAsync(long studentId)
        {
            var response = await _studentService.GetStudentProlesAsync(studentId);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
