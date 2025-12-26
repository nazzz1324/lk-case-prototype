using Account.Domain.DTO;
using Account.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken);
        Task<BaseResult<TokenDto>> RefreshToken(TokenDto dto);
    }

}
