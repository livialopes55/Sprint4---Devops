using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MottuApi.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _cfg;
    public AuthController(IConfiguration cfg) { _cfg = cfg; }

    /// <summary>Login para obter JWT (usuário demo: admin / 123456)</summary>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        if (dto.Username != "admin" || dto.Password != "123456")
            return Unauthorized();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _cfg["Jwt:Issuer"],
            audience: null,
            claims: new[] { new Claim(ClaimTypes.Name, dto.Username) },
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }

    public record LoginDto(string Username, string Password);
}
