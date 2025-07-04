var builder = WebApplication.CreateBuilder(args);

// 1. 定义CORS策略的名称
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// 2. 添加并配置CORS服务
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            // 确保这个端口号(5173)和你Vue开发服务器的端口号一致
            policy.WithOrigins("http://localhost:5173", "http://localhost:5174") // 同时允许两个可能的端口
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// 3. 添加YARP反向代理服务
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// --- 关键：中间件的注册顺序非常重要 ---

// 4. 首先启用路由
app.UseRouting();

// 5. 在路由之后、端点之前，启用CORS策略
app.UseCors(MyAllowSpecificOrigins);

// 6. 最后映射反向代理的端点
app.MapReverseProxy();

app.Run();