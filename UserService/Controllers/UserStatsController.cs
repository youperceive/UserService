using Microsoft.AspNetCore.Mvc;
using ModelLibrary.api;
using UserService.MockData;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserStatsController : ControllerBase
{
    /// <summary>
    /// 获取用户成就数据卡片（等级、阅读数、点赞数、收藏数）
    /// </summary>
    [HttpGet("achievement-cards/{userId}")]
    public ActionResult<AchievementCardsResponse> GetAchievementCards(string userId)
    {
        try
        {
            var response = new AchievementCardsResponse
            {
                StatusCode = 200,
                Message = "获取成功",
                Data = UserStatsMockData.GetAchievementCards(userId)
            };
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new BaseResponse(500, $"服务器错误: {ex.Message}"));
        }
    }
    
    /// <summary>
    /// 获取用户最近通过的题目列表
    /// </summary>
    [HttpGet("recent-solved/{userId}")]
    public ActionResult<RecentSolvedResponse> GetRecentSolved(string userId)
    {
        try
        {
            var response = new RecentSolvedResponse
            {
                StatusCode = 200,
                Message = "获取成功",
                Data = UserStatsMockData.GetRecentSolved(userId)
            };
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new BaseResponse(500, $"服务器错误: {ex.Message}"));
        }
    }
    
    /// <summary>
    /// 获取用户使用语言统计数据
    /// </summary>
    [HttpGet("language-stats/{userId}")]
    public ActionResult<LanguageStatsResponse> GetLanguageStats(string userId)
    {
        try
        {
            var response = new LanguageStatsResponse
            {
                StatusCode = 200,
                Message = "获取成功",
                Data = UserStatsMockData.GetLanguageStats(userId)
            };
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new BaseResponse(500, $"服务器错误: {ex.Message}"));
        }
    }
    
    /// <summary>
    /// 获取用户解题类型统计数据
    /// </summary>
    [HttpGet("solved-type-stats/{userId}")]
    public ActionResult<SolvedTypeStatsResponse> GetSolvedTypeStats(string userId)
    {
        try
        {
            var response = new SolvedTypeStatsResponse
            {
                StatusCode = 200,
                Message = "获取成功",
                Data = UserStatsMockData.GetSolvedTypeStats(userId)
            };
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new BaseResponse(500, $"服务器错误: {ex.Message}"));
        }
    }
    
    /// <summary>
    /// 获取用户题目难度统计数据
    /// </summary>
    [HttpGet("difficulty-stats/{userId}")]
    public ActionResult<DifficultyStatsResponse> GetDifficultyStats(string userId)
    {
        try
        {
            var response = new DifficultyStatsResponse
            {
                StatusCode = 200,
                Message = "获取成功",
                Data = UserStatsMockData.GetDifficultyStats(userId)
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

public class AchievementCardsResponse : BaseResponse
{
    public List<AchievementCard>? Data { get; set; }
}

public class RecentSolvedResponse : BaseResponse
{
    public List<RecentSolvedItem>? Data { get; set; }
}

public class LanguageStatsResponse : BaseResponse
{
    public List<LanguageStatItem>? Data { get; set; }
}

public class SolvedTypeStatsResponse : BaseResponse
{
    public List<TypeStatItem>? Data { get; set; }
}

public class DifficultyStatsResponse : BaseResponse
{
    public DifficultyStats? Data { get; set; }
}

#endregion

