using CollaborativeOffice.Domain;
using CollaborativeOffice.IdentityService.Data;
using CollaborativeOffice.IdentityService.DTOs;
using CollaborativeOffice.IdentityService.Services; // Make sure this using is present
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // 引用授权特性
using System.Security.Claims; // 引用Claims
using BCrypt.Net;

namespace CollaborativeOffice.IdentityService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly TokenService _tokenService;

    // ✅ CORRECT: Only ONE constructor accepting all dependencies.
    public AuthController(ApplicationDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        if (await _context.Users.AnyAsync(u => u.Username.Equals(request.Username)))
        {
            return BadRequest("Username already exists.");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var newUser = new User
        {
            Username = request.Username,
            PasswordHash = passwordHash
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        return Ok(new { UserId = newUser.Id, Username = newUser.Username });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid username or password.");
        }

        var token = _tokenService.CreateToken(user);

        return Ok(new
        {
            Message = "Login successful!",
            UserId = user.Id,
            Username = user.Username,
            Token = token
        });
    }
    
    [HttpGet("me")] // 定义路由为 GET /api/auth/me
    [Authorize] // 关键！这个特性(Attribute)表示此接口必须在授权后才能访问
    public IActionResult GetMyProfile()
    {
        // 当用户通过验证后，他们的信息(Claims)会被保存在HttpContext.User中
        // 我们可以从中读取用户信息，而无需再次查询数据库
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(ClaimTypes.Name)?.Value;

        if (userId == null)
        {
            return Unauthorized();
        }

        // 返回从Token中解析出的用户信息
        return Ok(new { UserId = userId, Username = username });
    }
}