using System.Text; // 用于编码
using CollaborativeOffice.IdentityService.Data;
using CollaborativeOffice.IdentityService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer; // 引用JWT认证包
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; // 引用Token验证参数
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// --- 服务注册区域 ---

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);
builder.Services.AddScoped<TokenService>();

// ▼▼▼ 1. 添加JWT认证服务 ▼▼▼
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // 以下是验证JWT时的关键参数设置:
            ValidateIssuer = true,                         // 验证签发者
            ValidateAudience = true,                       // 验证受众
            ValidateLifetime = true,                       // 验证生命周期（是否过期）
            ValidateIssuerSigningKey = true,               // 验证签名密钥
            ValidIssuer = builder.Configuration["Jwt:Issuer"],       // 从appsettings读取有效的签发者
            ValidAudience = builder.Configuration["Jwt:Audience"],   // 从appsettings读取有效的受众
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)) // 从appsettings读取密钥并加密
        };
    });

// ▼▼▼ 2. 添加授权服务 ▼▼▼
builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // 1. 定义一个名为 "Bearer" 的安全方案
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", // Header的名称
        Type = SecuritySchemeType.ApiKey, // 方案类型
        Scheme = "Bearer", // 方案名称
        BearerFormat = "JWT", // Bearer的格式
        In = ParameterLocation.Header, // Token所在的位置
        Description = "请输入'Bearer'后加一个空格，然后粘贴你的JWT Token。例如：\"Bearer {token}\""
    });

    // 2. 将上面定义的 "Bearer" 安全方案应用到所有的API操作上
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// --- HTTP请求处理管道 ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

// ▼▼▼ 3. 启用认证和授权中间件 ▼▼▼
// 注意：顺序非常重要，必须在MapControllers之前
app.UseAuthentication(); // 先启用认证
app.UseAuthorization();  // 再启用授权
app.MapControllers();

app.Run();
