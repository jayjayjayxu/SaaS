using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CollaborativeOffice.Domain;
using Microsoft.IdentityModel.Tokens;

namespace CollaborativeOffice.IdentityService.Services;

public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateToken(User user)
    {
        // 1. 定义需要放入JWT的“声明”(Claims)，比如用户ID和用户名
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username), // <- 修改为 ClaimTypes.Name
        };

        // 2. 从配置中读取密钥、签发者和受众
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        // 3. 创建签名凭证
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 4. 创建Token描述
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2), // Token有效期2小时
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = creds
        };

        // 5. 创建Token处理器并生成最终的Token字符串
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}