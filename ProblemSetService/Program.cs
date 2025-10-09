using Microsoft.OpenApi.Models;
using ProblemSetService;

var builder = WebApplication.CreateBuilder(args);

// 配置日志
builder.Logging.AddConsole();

// 注册服务
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// CORS配置
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
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
app.Urls.Add("http://localhost:5170");
app.Urls.Add("https://localhost:7170");

app.Run();