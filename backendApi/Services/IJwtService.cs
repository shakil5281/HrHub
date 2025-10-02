using HrHubAPI.Models;
using System.Security.Claims;

namespace HrHubAPI.Services
{
    public interface IJwtService
    {
        Task<string> GenerateJwtTokenAsync(ApplicationUser user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task<bool> ValidateTokenAsync(string token);
        ClaimsPrincipal? GetPrincipalFromToken(string token);
    }
}
