using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeeManagement.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public sealed class AuthController(IConfiguration configuration) : ControllerBase
{
    [HttpGet("token")]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    public ActionResult<TokenResponse> GetToken([FromQuery] int userId = 1, [FromQuery] string role = "Admin")
    {
        return GenerateJsonWebToken(userId, role);
    }

    [HttpPost("token")]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    public ActionResult<TokenResponse> CreateToken([FromBody] TokenRequest request)
    {
        var role = string.IsNullOrWhiteSpace(request.Role) ? "Admin" : request.Role;
        var userId = request.UserId <= 0 ? 1 : request.UserId;

        return GenerateJsonWebToken(userId, role);
    }

    private TokenResponse GenerateJsonWebToken(int userId, string userRole)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(10);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecurityKey"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, userRole),
            new("UserId", userId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new TokenResponse(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
