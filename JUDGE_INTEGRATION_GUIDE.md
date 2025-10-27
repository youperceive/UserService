# 判题机集成快速指南

## 概述
判题机已从独立API服务改为库，由ProblemSetService统一调用。

## 前置条件
- Docker Desktop 已安装并运行
- JudgeMachine项目路径: `C:\Users\Uiharukazari\Desktop\InitialThinkJudgeMachine\JudgeMachine`

---

## 集成步骤

### 1. 添加项目引用和NuGet包

在ProblemSetService目录执行：

```bash
cd C:\Users\Uiharukazari\WebstormProjects\UserService\ProblemSetService

# 添加JudgeMachine项目引用
dotnet add reference ..\..\Desktop\InitialThinkJudgeMachine\JudgeMachine\JudgeMachine.csproj

# 添加Docker.DotNet包
dotnet add package Docker.DotNet --version 3.125.15
```

### 2. 创建JudgeController

在 `ProblemSetService/Controllers/` 创建 `JudgeController.cs`，复制以下代码：

```csharp
using JudgeMachine.Judge;
using JudgeMachine.Languages;
using Microsoft.AspNetCore.Mvc;

namespace ProblemSetService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JudgeController : ControllerBase
{
    [HttpPost("run")]
    public async Task<IActionResult> RunCode([FromBody] RunCodeRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Code))
                return BadRequest(new { error = "代码不能为空" });
                
            var language = GetLanguage(request.Lang);
            if (language == null)
                return BadRequest(new { error = $"不支持的语言: {request.Lang}" });
            
            var timeoutValue = int.TryParse(request.Timeout, out var t) ? t : 5;
            var (output, exitCode) = await language.Run(request.Code, request.Input, timeoutValue);
            
            return Ok(new {
                standardOutput = output.StandardOutput,
                errorOutput = output.ErrorOutput,
                exitCode = exitCode,
                executionTimeMs = 0,
                memoryUsageKb = 0
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    [HttpPost("runBatch")]
    public async Task<IActionResult> RunBatch([FromBody] BatchTestRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Code))
                return BadRequest(new { error = "代码不能为空" });
                
            var language = GetLanguage(request.Language);
            if (language == null)
                return BadRequest(new { error = "不支持的语言" });
                
            var results = new List<TestCaseResult>();
            
            foreach (var testCase in request.TestCases)
            {
                try
                {
                    var (output, exitCode) = await language.Run(request.Code, testCase.Input, request.Timeout);
                    
                    results.Add(new TestCaseResult
                    {
                        Input = testCase.Input,
                        ExpectedOutput = testCase.ExpectedOutput,
                        ActualOutput = output.StandardOutput,
                        ErrorOutput = output.ErrorOutput,
                        ExecutionTimeMs = 0,
                        MemoryUsageKb = 0,
                        ExitCode = exitCode,
                        IsAccepted = IsOutputMatched(output.StandardOutput, testCase.ExpectedOutput, testCase.IgnoreWhitespace)
                    });
                    
                    if (!results[^1].IsAccepted && request.StopOnFirstFailure)
                        break;
                }
                catch (Exception ex)
                {
                    results.Add(new TestCaseResult
                    {
                        Input = testCase.Input,
                        ExpectedOutput = testCase.ExpectedOutput,
                        ErrorOutput = $"执行异常: {ex.Message}",
                        IsAccepted = false
                    });
                    
                    if (request.StopOnFirstFailure)
                        break;
                }
            }
            
            return Ok(new BatchTestResponse
            {
                TotalCases = request.TestCases.Count,
                PassedCases = results.Count(r => r.IsAccepted),
                Results = results
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    private Language? GetLanguage(string lang) => lang switch
    {
        "C++" => new CPlusPlus(),
        "C#" => new CSharp(),
        "Java" => new Java(),
        "Python3" => new Python3(),
        "JavaScript" => new JavaScript(),
        "TypeScript" => new TypeScript(),
        "GoLang" => new GoLang(),
        "C" => new C(),
        "Pascal" => new Pascal(),
        _ => null
    };
    
    private bool IsOutputMatched(string actual, string expected, bool ignoreWhitespace)
    {
        if (ignoreWhitespace)
        {
            actual = System.Text.RegularExpressions.Regex.Replace(actual.Trim(), @"\s+", " ");
            expected = System.Text.RegularExpressions.Regex.Replace(expected.Trim(), @"\s+", " ");
        }
        return actual == expected;
    }
}

public class RunCodeRequest
{
    public string Code { get; set; } = "";
    public string Lang { get; set; } = "C++";
    public string Input { get; set; } = "";
    public string Timeout { get; set; } = "5";
}

public class BatchTestRequest
{
    public string Code { get; set; } = "";
    public string Language { get; set; } = "C++";
    public List<TestCase> TestCases { get; set; } = new();
    public int Timeout { get; set; } = 5;
    public bool StopOnFirstFailure { get; set; } = false;
}

public class TestCase
{
    public string Input { get; set; } = "";
    public string ExpectedOutput { get; set; } = "";
    public bool IgnoreWhitespace { get; set; } = true;
}

public class TestCaseResult
{
    public string Input { get; set; } = "";
    public string ExpectedOutput { get; set; } = "";
    public string ActualOutput { get; set; } = "";
    public string ErrorOutput { get; set; } = "";
    public long ExecutionTimeMs { get; set; }
    public long MemoryUsageKb { get; set; }
    public long ExitCode { get; set; }
    public bool IsAccepted { get; set; }
}

public class BatchTestResponse
{
    public int TotalCases { get; set; }
    public int PassedCases { get; set; }
    public List<TestCaseResult> Results { get; set; } = new();
}
```

### 3. 配置CORS

在 `ProblemSetService/Program.cs` 中添加/修改CORS配置：

```csharp
// 在 var builder = WebApplication.CreateBuilder(args); 之后

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
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

// 在 var app = builder.Build(); 之后

app.UseCors("AllowFrontend");
```

### 4. JSON命名策略

确保在 `Program.cs` 中配置camelCase：

```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
```

### 5. 构建并运行

```bash
# 构建
dotnet build

# 运行
dotnet run
```

服务应该在 `http://localhost:5000` 启动。

---

## 测试

### 使用Swagger测试

访问: `http://localhost:5000/swagger`

找到 `Judge` 分组，测试 `/api/Judge/run` 端点：

```json
{
  "code": "#include <iostream>\nusing namespace std;\nint main() {\n    int a, b;\n    cin >> a >> b;\n    cout << a + b << endl;\n    return 0;\n}",
  "lang": "C++",
  "input": "1 2",
  "timeout": "5"
}
```

预期输出：
```json
{
  "standardOutput": "3\n",
  "errorOutput": "",
  "exitCode": 0,
  "executionTimeMs": 0,
  "memoryUsageKb": 0
}
```

### 使用前端测试

1. 确保前端已更新（`useApi.ts` 已修改）
2. 启动前端: `npm run dev`
3. 打开题目页面，点击"运行"按钮
4. 查看浏览器控制台和网络请求

---

## 常见问题

### 1. 编译错误：找不到JudgeMachine

**原因**: 项目引用路径不正确

**解决**:
```bash
# 先编译JudgeMachine
cd C:\Users\Uiharukazari\Desktop\InitialThinkJudgeMachine\JudgeMachine
dotnet build

# 然后编译ProblemSetService
cd C:\Users\Uiharukazari\WebstormProjects\UserService\ProblemSetService
dotnet build
```

### 2. Docker连接失败

**错误**: `Cannot connect to Docker daemon`

**解决**:
1. 确认Docker Desktop正在运行
2. 检查Windows: Docker是否使用npipe
3. 如果使用WSL，可能需要修改 `JudgeMachine/Settings.cs`：
   ```csharp
   public static string DockerUrl { set; get; } = "unix:///var/run/docker.sock";
   ```

### 3. CORS错误

**错误**: `Access-Control-Allow-Origin`

**解决**: 确保已配置CORS（步骤3）并且前端URL在允许列表中

### 4. 404错误

**错误**: `GET http://localhost:5000/Judge/run 404`

**原因**: 路径错误，新路径是 `/api/Judge/run`

**解决**: 前端应该已自动修改，检查 `useApi.ts` 第248行

---

## 端口和地址

- **主后端**: `http://localhost:5000`
- **判题API**: `http://localhost:5000/api/Judge/run`
- **前端**: `http://localhost:3000`

---

## 下一步

集成成功后，可以：
1. 删除独立的判题机API项目（`InitialThinkJudgeMachine/Api`）
2. 实现异步评测队列（推荐使用Hangfire或RabbitMQ）
3. 添加评测记录存储到数据库
4. 实现题目的测试用例管理

