using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using CollaborativeOffice.Domain;
using CollaborativeOffice.IdentityService.Repositories;
using Microsoft.IdentityModel.Tokens;


namespace CollaborativeOffice.IdentityService.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _configuration;

    // 通过构造函数注入“仓库”用于数据访问，注入“配置”用于读取JWT设置
    public AuthService(IAuthRepository authRepository, IConfiguration configuration)
    {
        _authRepository = authRepository;
        _configuration = configuration;
    }

    /// <summary>
    /// 处理用户注册的业务逻辑
    /// </summary>
    public async Task<(bool Succeeded, string? Error)> RegisterAsync(string username, string password)
    {
        // 1. 检查用户名是否已被占用
        var userExists = await _authRepository.UserExistsAsync(username);
        if (userExists)
        {
            return (false, "用户名已存在。");
        }

        // 2. 使用BCrypt对密码进行哈希加密
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        // 3. 创建新的User领域对象
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        // 4. 通过仓库将新用户添加到数据库
        await _authRepository.AddUserAsync(newUser);
        await _authRepository.SaveChangesAsync();

        return (true, null); // 返回成功并不带错误信息
    }

    /// <summary>
    /// 处理用户登录的业务逻辑
    /// </summary>
    public async Task<(string? Token, string? Error)> LoginAsync(string username, string password)
    {
        // 1. 通过仓库根据用户名查找用户
        var user = await _authRepository.GetUserByUsernameAsync(username);

        // 2. 如果用户不存在，或者密码哈希值不匹配，则返回错误
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return (null, "用户名或密码无效。");
        }

        // 3. 如果凭证有效，则生成一个JWT Token
        var token = GenerateJwtToken(user);
        
        return (token, null); // 返回Token并不带错误信息
    }

    /// <summary>
    /// 一个私有的辅助方法，专门用于生成JWT
    /// </summary>
    private string GenerateJwtToken(User user)
    {
        // 定义需要放入Token中的“声明”(Claims)
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // Sub是标准的“主题”，通常是用户ID
            new(ClaimTypes.Name, user.Username) // Name是标准的“名称”声明
        };

        // 从配置文件(appsettings.json)中读取密钥，并创建一个安全密钥对象
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        
        // 使用密钥和HMAC-SHA256算法创建签名凭证
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        // 设置Token的过期时间，例如2小时后
        var expires = DateTime.UtcNow.AddHours(2);

        // 创建Token的描述对象，包含了所有需要的信息
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = creds
        };

        // 创建一个Token处理器来生成最终的Token字符串
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}