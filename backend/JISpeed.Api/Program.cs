<<<<<<< HEAD
using JISpeed.Core.Entities.Common; // 引入 ApplicationUser 和 ApplicationRole
using JISpeed.Infrastructure.Data; // 引入 OracleDbContext
using Microsoft.EntityFrameworkCore; // 用于配置 DbContext
using Microsoft.AspNetCore.Identity;
=======
using JISpeed.Infrastructure.Data;
using JISpeed.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
>>>>>>> origin/dev-backend-login

var builder = WebApplication.CreateBuilder(args);

// 1. 读取连接字符串（从 appsettings.json）
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// 2. 注册 Oracle 数据库上下文
builder.Services.AddDbContext<AppDbContext>(options => options.UseOracle(connectionString)
    .LogTo(Console.WriteLine)
    .EnableSensitiveDataLogging());
    
// 3. 注册依赖注入（仓储、服务）
// 注册Identity服务（它内部会注册PasswordHasher等）
builder.Services.AddIdentity<UUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


// 4. 控制器和日志等默认配置
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // 如果你需要 OpenAPI / Swagger（Apifox 不必需）

// 配置数据库连接
builder.Services.AddDbContext<OracleDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

// 添加 Identity 服务
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options => 
{
    // 配置密码策略
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    
    // 配置用户策略
    options.User.RequireUniqueEmail = true;
    
    // 配置锁定策略
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<OracleDbContext>()
.AddDefaultTokenProviders();

// 配置身份验证和授权
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

<<<<<<< HEAD
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 启用身份验证和授权中间件
app.UseAuthentication();
app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();
=======
// 5. 中间件配置
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
>>>>>>> origin/dev-backend-login

app.Run();
