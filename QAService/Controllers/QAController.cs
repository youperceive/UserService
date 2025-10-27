using Microsoft.AspNetCore.Mvc;
using QAService.Models;
using QAService.Services;
using Newtonsoft.Json;

namespace QAService.Controllers;

[ApiController]
[Route("api/qa")]
public class QAController : ControllerBase
{
    private readonly AIService _aiService;
    private readonly ILogger<QAController> _logger;
    
    // 简单的题目数据缓存（实际应该从 ProblemSetService 获取）
    private static Dictionary<int, QuestionInfo>? _questionCache;
    
    public QAController(AIService aiService, ILogger<QAController> logger)
    {
        _aiService = aiService;
        _logger = logger;
    }
    
    /// <summary>
    /// 获取题目提示
    /// POST /api/qa/hint
    /// </summary>
    [HttpPost("hint")]
    public async Task<IActionResult> GetHint([FromBody] HintRequest request)
    {
        try
        {
            if (request.QuestionId <= 0)
                return BadRequest(new { error = "无效的题目ID" });
                
            if (request.Level < 1 || request.Level > 3)
                return BadRequest(new { error = "提示等级必须在1-3之间" });
            
            // 获取题目信息
            var question = await GetQuestionInfoAsync(request.QuestionId);
            if (question == null)
                return NotFound(new { error = $"题目 {request.QuestionId} 不存在" });
            
            _logger.LogInformation($"生成提示: QuestionId={request.QuestionId}, Level={request.Level}, Language={request.Language}, ForceRefresh={request.ForceRefresh}");
            
            var hint = await _aiService.GenerateHintAsync(
                request.QuestionId,
                question.Title,
                question.Description,
                request.Level,
                request.UserCode,
                request.Language,
                request.ForceRefresh
            );
            
            return Ok(hint);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取提示时发生错误");
            return StatusCode(500, new { error = "服务器内部错误", details = ex.Message });
        }
    }
    
    /// <summary>
    /// 获取题解
    /// POST /api/qa/solution
    /// </summary>
    [HttpPost("solution")]
    public async Task<IActionResult> GetSolution([FromBody] SolutionRequest request)
    {
        try
        {
            if (request.QuestionId <= 0)
                return BadRequest(new { error = "无效的题目ID" });
            
            // 获取题目信息
            var question = await GetQuestionInfoAsync(request.QuestionId);
            if (question == null)
                return NotFound(new { error = $"题目 {request.QuestionId} 不存在" });
            
            _logger.LogInformation($"生成题解: QuestionId={request.QuestionId}, HasUserCode={!string.IsNullOrEmpty(request.UserCode)}");
            
            var solution = await _aiService.GenerateSolutionAsync(
                request.QuestionId,
                question.Title,
                question.Description,
                request.UserCode,
                request.Language
            );
            
            return Ok(solution);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取题解时发生错误");
            return StatusCode(500, new { error = "服务器内部错误", details = ex.Message });
        }
    }
    
    /// <summary>
    /// 健康检查
    /// GET /api/qa/health
    /// </summary>
    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        return Ok(new
        {
            service = "QAService",
            status = "healthy",
            timestamp = DateTime.Now,
            features = new[]
            {
                "渐进式提示（3级）",
                "AI题解生成",
                "代码分析",
                "优化建议"
            }
        });
    }
    
    /// <summary>
    /// 获取题目信息（简化版，实际应该调用 ProblemSetService 的 API）
    /// </summary>
    private async Task<QuestionInfo?> GetQuestionInfoAsync(int questionId)
    {
        // 懒加载题目数据
        if (_questionCache == null)
        {
            await LoadQuestionsAsync();
        }
        
        return _questionCache?.GetValueOrDefault(questionId);
    }
    
    /// <summary>
    /// 加载题目数据（从 Questions.json）
    /// </summary>
    private async Task LoadQuestionsAsync()
    {
        try
        {
            // 尝试从 ProblemSetService 的 Data 目录读取
            var jsonPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "..", "..", "..", "..", "ProblemSetService", "Data", "Questions.json"
            );
            
            // 规范化路径
            jsonPath = Path.GetFullPath(jsonPath);
            
            if (!System.IO.File.Exists(jsonPath))
            {
                _logger.LogWarning($"Questions.json not found at: {jsonPath}");
                _questionCache = new Dictionary<int, QuestionInfo>();
                return;
            }
            
            var jsonString = await System.IO.File.ReadAllTextAsync(jsonPath);
            var questions = JsonConvert.DeserializeObject<List<QuestionJson>>(jsonString);
            
            if (questions == null)
            {
                _questionCache = new Dictionary<int, QuestionInfo>();
                return;
            }
            
            _questionCache = questions.ToDictionary(
                q => q.Id,
                q => new QuestionInfo
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = StripHtml(q.Description),
                    Difficulty = q.Difficulty
                }
            );
            
            _logger.LogInformation($"Loaded {_questionCache.Count} questions");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading questions");
            _questionCache = new Dictionary<int, QuestionInfo>();
        }
    }
    
    /// <summary>
    /// 去除 HTML 标签
    /// </summary>
    private string StripHtml(string html)
    {
        if (string.IsNullOrEmpty(html))
            return "";
            
        return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", " ")
            .Replace("&nbsp;", " ")
            .Replace("&lt;", "<")
            .Replace("&gt;", ">")
            .Trim();
    }
}

/// <summary>
/// 题目信息（简化）
/// </summary>
public class QuestionInfo
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Difficulty { get; set; } = "";
}

/// <summary>
/// JSON 中的题目格式
/// </summary>
public class QuestionJson
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Input { get; set; } = "";
    public string Output { get; set; } = "";
    public string Limit { get; set; } = "";
    public string Difficulty { get; set; } = "";
    public List<string> Tags { get; set; } = new();
    public List<ExampleJson> Examples { get; set; } = new();
}

public class ExampleJson
{
    public string Input { get; set; } = "";
    public string Output { get; set; } = "";
    public string? Explanation { get; set; }
}

