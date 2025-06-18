using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userMgr;
    private readonly RoleManager<IdentityRole> _roleMgr;
    private readonly IConfiguration _cfg;

    public AuthController(UserManager<ApplicationUser> userMgr,
                          RoleManager<IdentityRole> roleMgr,
                          IConfiguration cfg)
    {
        _userMgr = userMgr;
        _roleMgr = roleMgr;
        _cfg = cfg;
    }
    public record RegisterDto(string UserName, string Password, string DisplayName);

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserName) ||
            string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest("Username and Password are required");

        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            DisplayName = dto.DisplayName ?? dto.UserName
        };

        var result = await _userMgr.CreateAsync(user, dto.Password);

        if (!result.Succeeded)                       // ⬅️ فشل
            return BadRequest(new
            {
                errors = result.Errors.Select(e => e.Description)
            });

        if (!await _roleMgr.RoleExistsAsync("User"))
            await _roleMgr.CreateAsync(new IdentityRole("User"));

        await _userMgr.AddToRoleAsync(user, "User");

        var token = GenerateJwt(user);

        return Ok(new { token });
    }

    private string GenerateJwt(ApplicationUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, "User")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }


    public record LoginDto(string UserName, string Password);

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserName) ||
            string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest("Username and Password are required");

        var user = await _userMgr.FindByNameAsync(dto.UserName);
        if (user == null)
            return Unauthorized("User not found");

        var ok = await _userMgr.CheckPasswordAsync(user, dto.Password);
        if (!ok)
            return Unauthorized("Invalid password");

        var token = GenerateJwt(user);

        return Ok(new { token });
    }
}




