using Microsoft.AspNetCore.Mvc;
using ModelLibrary.api;
using ProblemSetService.MockData;

namespace ProblemSetService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionController : ControllerBase
{
    /// <summary>
    /// 获取题目列表（分页）
    /// </summary>
    [HttpGet("list")]
    public ActionResult<QuestionListResponse> GetQuestionList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 8,
        [FromQuery] string? difficulty = null,
        [FromQuery] string? keyword = null,
        [FromQuery] string? tags = null)
    {
        try
        {
            // 获取假数据
            var allQuestions = QuestionMockData.GetAllQuestions();

            // 应用筛选
            var filteredQuestions = allQuestions.AsEnumerable();
            
            if (!string.IsNullOrEmpty(difficulty))
            {
                filteredQuestions = filteredQuestions.Where(q => 
                    q.Difficulty.Equals(difficulty, StringComparison.OrdinalIgnoreCase));
            }
            
            if (!string.IsNullOrEmpty(keyword))
            {
                filteredQuestions = filteredQuestions.Where(q => 
                    q.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            // 分页
            var total = filteredQuestions.Count();
            var questions = filteredQuestions
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            var response = new QuestionListResponse
            {
                StatusCode = 200,
                Message = "获取成功",
                Data = questions,
                Total = total,
                Page = page,
                PageSize = pageSize
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new BaseResponse(500, $"服务器错误: {ex.Message}"));
        }
    }
    
    /// <summary>
    /// 获取推荐题目
    /// </summary>
    [HttpGet("recommend")]
    public ActionResult<RecommendQuestionsResponse> GetRecommendQuestions(
        [FromQuery] int count = 5,
        [FromQuery] string? userId = null)
    {
        try
        {
            // 获取假数据
            var recommendedQuestions = QuestionMockData.GetRecommendQuestions(count);

            var response = new RecommendQuestionsResponse
            {
                StatusCode = 200,
                Message = "获取成功",
                Data = recommendedQuestions
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new BaseResponse(500, $"服务器错误: {ex.Message}"));
        }
    }
}

#region Response Models

public class QuestionListResponse : BaseResponse
{
    public List<QuestionItem>? Data { get; set; }
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class RecommendQuestionsResponse : BaseResponse
{
    public List<QuestionItem>? Data { get; set; }
}

#endregion
