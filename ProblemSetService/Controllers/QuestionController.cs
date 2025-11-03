using Microsoft.AspNetCore.Mvc;
using ProblemSetService.Models;
using System.Text.Json;

namespace ProblemSetService.Controllers;

[ApiController]
[Route("api/question")]  // 明确指定小写路由，与前端保持一致
public class QuestionController(ProblemSetService service) : ControllerBase
{
    private static List<Question>? _cachedQuestions;
    private static readonly object _lock = new();
    
    /// <summary>
    /// 加载并缓存所有题目数据
    /// </summary>
    private List<Question> LoadQuestions()
    {
        if (_cachedQuestions != null)
            return _cachedQuestions;
            
        lock (_lock)
        {
            if (_cachedQuestions != null)
                return _cachedQuestions;
                
            try
            {
                var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Questions.json");
                var jsonString = System.IO.File.ReadAllText(jsonPath);
                var questionsJson = JsonSerializer.Deserialize<List<QuestionJson>>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                if (questionsJson == null)
                {
                    Console.WriteLine("[QuestionController] Failed to deserialize questions.json");
                    _cachedQuestions = new List<Question>();
                    return _cachedQuestions;
                }
                
                // 转换为Question对象
                _cachedQuestions = questionsJson.Select(q => ConvertToQuestion(q)).ToList();
                Console.WriteLine($"[QuestionController] Loaded {_cachedQuestions.Count} questions");
                return _cachedQuestions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QuestionController] Error loading questions: {ex.Message}");
                _cachedQuestions = new List<Question>();
                return _cachedQuestions;
            }
        }
    }
    
    /// <summary>
    /// 将JSON格式转换为Question对象
    /// </summary>
    private Question ConvertToQuestion(QuestionJson qJson)
    {
        var tags = new List<QuestionTag>();
        
        // 添加难度标签
        tags.Add(qJson.Difficulty.ToLower() switch
        {
            "easy" => new QuestionTag { Name = "简单", Color = "var(--color-text-1)", BackgroundColor = "rgb(var(--green-5))" },
            "normal" => new QuestionTag { Name = "中等", Color = "var(--color-text-1)", BackgroundColor = "rgb(var(--orange-5))" },
            "hard" => new QuestionTag { Name = "困难", Color = "var(--color-text-1)", BackgroundColor = "rgb(var(--red-5))" },
            _ => new QuestionTag { Name = "未知", Color = "var(--color-text-1)", BackgroundColor = "rgb(var(--gray-5))" }
        });
        
        // 解析其他标签 (格式: "名称;颜色;背景色")
        foreach (var tagStr in qJson.Tags)
        {
            var parts = tagStr.Split(';');
            if (parts.Length == 3)
            {
                tags.Add(new QuestionTag
                {
                    Name = parts[0],
                    Color = parts[1],
                    BackgroundColor = parts[2]
                });
            }
        }
        
        return new Question
        {
            Id = qJson.Id,
            Title = qJson.Title,
            DescriptionH5 = qJson.Description,
            InputFormatH5 = qJson.Input,
            OutputFormatH5 = qJson.Output,
            NoticeH5 = qJson.Limit,
            Tags = tags,
            Examples = qJson.Examples.Select(e => new QuestionExample
            {
                Input = e.Input,
                Output = e.Output,
                Explanation = e.Explanation
            }).ToList(),
            EnabledLanguages = new List<string> { "C", "C++", "Python3", "Java", "JavaScript", "C#" }, // 默认支持的语言
            AllowTemplate = true
        };
    }
    
    /// <summary>
    /// 获取题目详情
    /// GET /api/question/{id}
    /// </summary>
    [HttpGet("{id}")]
    public IActionResult GetQuestionDetail(int id)
    {
        try
        {
            var questions = LoadQuestions();
            var question = questions.FirstOrDefault(q => q.Id == id);
            
            if (question == null)
                return NotFound(new { error = $"题目 {id} 不存在" });
                
            return Ok(question);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// 获取题目列表（支持筛选和分页）
    /// GET /api/question/list
    /// </summary>
    [HttpGet("list")]
    public IActionResult GetQuestionList(
        [FromQuery] string? difficulty = null,
        [FromQuery] string? tags = null,
        [FromQuery] string? keyword = null,
        [FromQuery] int? minId = null,
        [FromQuery] int? maxId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var questions = LoadQuestions();
            var query = questions.AsEnumerable();
            
            // 难度筛选（支持多选，逗号分隔）
            if (!string.IsNullOrEmpty(difficulty))
            {
                var difficulties = difficulty.Split(',').Select(d => d.Trim().ToLower()).ToList();
                query = query.Where(q => 
                {
                    var qDiff = q.Tags.FirstOrDefault(t => 
                        t.Name == "简单" || t.Name == "中等" || t.Name == "困难"
                    )?.Name.ToLower();
                    
                    return qDiff != null && difficulties.Any(d =>
                        (d == "easy" && qDiff == "简单") ||
                        (d == "medium" && qDiff == "中等") ||
                        (d == "hard" && qDiff == "困难")
                    );
                });
            }
            
            // 标签筛选（支持多选，逗号分隔）
            if (!string.IsNullOrEmpty(tags))
            {
                var tagList = tags.Split(',').Select(t => t.Trim()).ToList();
                query = query.Where(q => 
                    tagList.Any(tag => q.Tags.Any(t => t.Name.Contains(tag, StringComparison.OrdinalIgnoreCase)))
                );
            }
            
            // 关键词搜索（标题）
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(q => 
                    q.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                );
            }
            
            // ID范围筛选
            if (minId.HasValue)
                query = query.Where(q => q.Id >= minId.Value);
                
            if (maxId.HasValue)
                query = query.Where(q => q.Id <= maxId.Value);
            
            // 总数
            var total = query.Count();
            
            // 分页
            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(q => new QuestionListItem
                {
                    Id = q.Id,
                    Title = q.Title,
                    Difficulty = q.Tags.FirstOrDefault(t => 
                        t.Name == "简单" || t.Name == "中等" || t.Name == "困难"
                    )?.Name ?? "未知",
                    Tags = q.Tags.Where(t => t.Name != "简单" && t.Name != "中等" && t.Name != "困难")
                                  .Select(t => t.Name).ToList(),
                    AcceptedCount = new Random().Next(10, 1000),  // Mock数据
                    SubmissionCount = new Random().Next(100, 2000) // Mock数据
                })
                .ToList();
            
            return Ok(new
            {
                total,
                page,
                pageSize,
                items
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// 搜索题目（按标题）
    /// GET /api/question/search
    /// </summary>
    [HttpGet("search")]
    public IActionResult SearchQuestions([FromQuery] string keyword)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { error = "搜索关键词不能为空" });
                
            var questions = LoadQuestions();
            var results = questions
                .Where(q => q.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .Take(10)
                .Select(q => new { q.Id, q.Title })
                .ToList();
                
            return Ok(results);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

