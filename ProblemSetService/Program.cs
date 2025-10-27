using Microsoft.OpenApi.Models;
using ProblemSetService;

var builder = WebApplication.CreateBuilder(args);

// 配置日志
builder.Logging.AddConsole();

// 注册服务
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // 配置camelCase命名策略，与前端保持一致
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();

// CORS配置 - 允许前端访问
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
        {
            // 允许localhost和127.0.0.1
            if (origin.Contains("localhost") || origin.Contains("127.0.0.1"))
                return true;
            
            // 允许局域网访问（192.168.x.x, 10.x.x.x, 172.16-31.x.x）
            var uri = new Uri(origin);
            var host = uri.Host;
            if (host.StartsWith("192.168.") || host.StartsWith("10."))
                return true;
            if (host.StartsWith("172."))
            {
                var parts = host.Split('.');
                if (parts.Length >= 2 && int.TryParse(parts[1], out var second))
                {
                    if (second >= 16 && second <= 31)
                        return true;
                }
            }
                
            return false;
        })
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// Swagger配置
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UserService API",
        Version = "v1",
        Description = "用户服务API"
    });
});

builder.Services.AddScoped<ProblemSetRepository>(provider => 
{
    var config = provider.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("PostgreSQL");
    Console.WriteLine(connectionString);
    return connectionString == null ? 
        new ProblemSetRepository("") : 
        new ProblemSetRepository(connectionString);
});

// 业务服务注册
builder.Services.AddScoped<ProblemSetService.ProblemSetService>();

var app = builder.Build();

// 中间件顺序很重要！
app.UseRouting(); // 必须先使用路由
app.UseCors("AllowAll"); // 然后使用CORS
// 只在开发环境启用Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserService API V1");
        c.RoutePrefix = string.Empty;
    });
}
app.UseAuthorization();


app.MapControllers();
// 端口设置
app.Urls.Add("http://localhost:5171");
app.Urls.Add("https://localhost:7171");

app.Run();