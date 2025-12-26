using Account.Application.Services;
using Account.Domain.DTO;
using Account.Domain.Interfaces.Services;
using Account.Domain.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Account.api.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class TokenController : ControllerBase
    {
        private ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<TokenDto>>> RefreshToken([FromBody] TokenDto tokenDto)
        {
            var response = await _tokenService.RefreshToken(tokenDto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
