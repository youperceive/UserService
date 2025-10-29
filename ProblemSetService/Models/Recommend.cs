using System.Text.Json.Serialization;

namespace ProblemSetService.Models;

// 1. 基础 API 响应模型（所有接口响应的基类，处理成功/错误状态）
public class ApiResponseBase
{
    /// <summary>
    /// 接口调用是否成功
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// 错误信息（仅当 Success = false 时存在）
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; set; }
}

// 2. /recommend 接口响应模型
public partial class RecommendResponse : ApiResponseBase
{
    /// <summary>
    /// 请求的用户名
    /// </summary>
    [JsonPropertyName("user_handle")]
    public string? UserHandle { get; set; }

    /// <summary>
    /// 推荐的题目列表
    /// </summary>
    [JsonPropertyName("recommendations")]
    public List<RecommendationItem>? Recommendations { get; set; }

    /// <summary>
    /// 用户已解题目的标签集合
    /// </summary>
    [JsonPropertyName("user_tags")]
    public List<string>? UserTags { get; set; }
}

/// <summary>
/// 单个推荐题目的详细信息（嵌套在 RecommendResponse 中）
/// </summary>
public class RecommendationItem
{
    /// <summary>
    /// 题目 ID
    /// </summary>
    [JsonPropertyName("problem_id")]
    public string? ProblemId { get; set; } // 用 string 兼容可能的非数字 ID，若确定是数字可改为 int

    /// <summary>
    /// 题目名称
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 题目难度评分（可能为 null，对应 Flask 中的 pd.notna 判断）
    /// </summary>
    [JsonPropertyName("rating")]
    public int? Rating { get; set; }

    /// <summary>
    /// 推荐理由解释
    /// </summary>
    [JsonPropertyName("explanation")]
    public string? Explanation { get; set; }
}

// 3. /user_info/<user_handle> 接口响应模型
public class UserInfoResponse : ApiResponseBase
{
    /// <summary>
    /// 请求的用户名
    /// </summary>
    [JsonPropertyName("user_handle")]
    public string? UserHandle { get; set; }

    /// <summary>
    /// 用户已解决的题目数量
    /// </summary>
    [JsonPropertyName("solved_problems_count")]
    public int SolvedProblemsCount { get; set; }

    /// <summary>
    /// 用户已解题目的标签集合
    /// </summary>
    [JsonPropertyName("user_tags")]
    public List<string>? UserTags { get; set; }
}

// 4. /health 接口响应模型
public class HealthCheckResponse
{
    /// <summary>
    /// 服务状态（如 "healthy"）
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// 状态描述信息
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    /// 数据是否加载完成（true/false）
    /// </summary>
    [JsonPropertyName("data_loaded")]
    public bool DataLoaded { get; set; }
}

// 5. /stats 接口响应模型
public class StatsResponse : ApiResponseBase
{
    /// <summary>
    /// 系统统计数据（嵌套对象）
    /// </summary>
    [JsonPropertyName("stats")]
    public SystemStats? Stats { get; set; }
}

/// <summary>
/// 系统统计详情（嵌套在 StatsResponse 中）
/// </summary>
public class SystemStats
{
    /// <summary>
    /// 系统中的用户总数
    /// </summary>
    [JsonPropertyName("users")]
    public int Users { get; set; }

    /// <summary>
    /// 系统中的题目总数
    /// </summary>
    [JsonPropertyName("problems")]
    public int Problems { get; set; }

    /// <summary>
    /// 数据是否加载完成（true/false）
    /// </summary>
    [JsonPropertyName("data_loaded")]
    public bool DataLoaded { get; set; }
}