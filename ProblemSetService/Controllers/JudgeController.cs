using JudgeMachine.Judge;
using JudgeMachine.Languages;
using Microsoft.AspNetCore.Mvc;

namespace ProblemSetService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JudgeController : ControllerBase
{
    private readonly ILogger<JudgeController> _logger;

    public JudgeController(ILogger<JudgeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 运行单个测试用例
    /// </summary>
    [HttpPost("run")]
    public async Task<IActionResult> RunCode([FromBody] RunCodeRequest request)
    {
        try
        {
            _logger.LogInformation("收到代码评测请求: Language={Lang}, CodeLength={Length}", 
                request.Lang, request.Code?.Length ?? 0);

            if (string.IsNullOrEmpty(request.Code))
            {
                _logger.LogWarning("代码为空");
                return BadRequest(new { error = "代码不能为空" });
            }
                
            var language = GetLanguage(request.Lang);
            if (language == null)
            {
                _logger.LogWarning("不支持的语言: {Lang}", request.Lang);
                return BadRequest(new { error = $"不支持的语言: {request.Lang}" });
            }
            
            var timeoutValue = int.TryParse(request.Timeout, out var t) ? t : 5;
            
            _logger.LogInformation("开始执行代码: Language={Lang}, Timeout={Timeout}s", 
                request.Lang, timeoutValue);
            
            var (output, exitCode) = await language.Run(request.Code, request.Input, timeoutValue);
            
            _logger.LogInformation("代码执行完成: ExitCode={ExitCode}, OutputLength={Length}", 
                exitCode, output.StandardOutput?.Length ?? 0);
            
            return Ok(new {
                standardOutput = output.StandardOutput ?? "",
                errorOutput = output.ErrorOutput ?? "",
                exitCode = exitCode,
                executionTimeMs = 0,
                memoryUsageKb = 0
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "代码执行异常");
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// 批量运行测试用例
    /// </summary>
    [HttpPost("runBatch")]
    public async Task<IActionResult> RunBatch([FromBody] BatchTestRequest request)
    {
        try
        {
            _logger.LogInformation("收到批量测试请求: Language={Lang}, TestCases={Count}", 
                request.Language, request.TestCases?.Count ?? 0);

            if (string.IsNullOrEmpty(request.Code))
            {
                _logger.LogWarning("代码为空");
                return BadRequest(new { error = "代码不能为空" });
            }
                
            var language = GetLanguage(request.Language);
            if (language == null)
            {
                _logger.LogWarning("不支持的语言: {Lang}", request.Language);
                return BadRequest(new { error = "不支持的语言" });
            }
            
            var results = new List<TestCaseResult>();
            
            for (int i = 0; i < request.TestCases.Count; i++)
            {
                var testCase = request.TestCases[i];
                try
                {
                    _logger.LogInformation("执行测试用例 {Index}/{Total}", i + 1, request.TestCases.Count);
                    
                    var (output, exitCode) = await language.Run(
                        request.Code, 
                        testCase.Input, 
                        request.Timeout
                    );
                    
                    var isAccepted = IsOutputMatched(
                        output.StandardOutput ?? "", 
                        testCase.ExpectedOutput, 
                        testCase.IgnoreWhitespace
                    );
                    
                    results.Add(new TestCaseResult
                    {
                        Input = testCase.Input,
                        ExpectedOutput = testCase.ExpectedOutput,
                        ActualOutput = output.StandardOutput ?? "",
                        ErrorOutput = output.ErrorOutput ?? "",
                        ExecutionTimeMs = 0,
                        MemoryUsageKb = 0,
                        ExitCode = exitCode,
                        IsAccepted = isAccepted
                    });
                    
                    if (!isAccepted && request.StopOnFirstFailure)
                    {
                        _logger.LogInformation("测试失败且StopOnFirstFailure=true，停止后续测试");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "测试用例 {Index} 执行异常", i + 1);
                    
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
            
            var passedCount = results.Count(r => r.IsAccepted);
            _logger.LogInformation("批量测试完成: {Passed}/{Total} 通过", passedCount, results.Count);
            
            return Ok(new BatchTestResponse
            {
                TotalCases = request.TestCases.Count,
                PassedCases = passedCount,
                Results = results
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量测试异常");
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// 根据语言名称获取Language实例
    /// </summary>
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
    
    /// <summary>
    /// 判断输出是否匹配
    /// </summary>
    private bool IsOutputMatched(string actual, string expected, bool ignoreWhitespace)
    {
        if (ignoreWhitespace)
        {
            // 去除首尾空白，并将多个空白字符替换为单个空格
            actual = System.Text.RegularExpressions.Regex.Replace(actual.Trim(), @"\s+", " ");
            expected = System.Text.RegularExpressions.Regex.Replace(expected.Trim(), @"\s+", " ");
        }
        return actual == expected;
    }
}

#region 请求/响应模型

/// <summary>
/// 单次运行代码请求
/// </summary>
public class RunCodeRequest
{
    public string Code { get; set; } = "";
    public string Lang { get; set; } = "C++";
    public string Input { get; set; } = "";
    public string Timeout { get; set; } = "5";
}

/// <summary>
/// 批量测试请求
/// </summary>
public class BatchTestRequest
{
    public string Code { get; set; } = "";
    public string Language { get; set; } = "C++";
    public List<TestCase> TestCases { get; set; } = new();
    public int Timeout { get; set; } = 5;
    public bool StopOnFirstFailure { get; set; } = false;
}

/// <summary>
/// 测试用例
/// </summary>
public class TestCase
{
    public string Input { get; set; } = "";
    public string ExpectedOutput { get; set; } = "";
    public bool IgnoreWhitespace { get; set; } = true;
}

/// <summary>
/// 测试用例结果
/// </summary>
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

/// <summary>
/// 批量测试响应
/// </summary>
public class BatchTestResponse
{
    public int TotalCases { get; set; }
    public int PassedCases { get; set; }
    public List<TestCaseResult> Results { get; set; } = new();
}

#endregion

