using QAService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "InitialThink QA Service API",
        Version = "v1",
        Description = "答疑系统 - 提供做题前的提示和做题后的解答",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "InitialThink Team"
        }
    });
});

// 注册知识图谱服务
builder.Services.AddSingleton<KnowledgeGraphService>();

// 注册 AI 服务
builder.Services.AddSingleton<AIService>();

// 配置 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "QA Service API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("=========================================");
Console.WriteLine("   InitialThink QA Service");
Console.WriteLine("=========================================");
Console.WriteLine($"  环境: {app.Environment.EnvironmentName}");
Console.WriteLine($"  端口: http://localhost:5274");
Console.WriteLine($"  Swagger: http://localhost:5274/swagger");
Console.WriteLine("=========================================");
Console.WriteLine("  功能:");
Console.WriteLine("  ✓ 做题前提示（3级渐进）");
Console.WriteLine("  ✓ AI题解生成");
Console.WriteLine("  ✓ 代码分析与优化建议");
Console.WriteLine("  ✓ 知识图谱增强（算法识别）");
Console.WriteLine("=========================================");
Console.WriteLine();

app.Run();
