namespace QAService.Models;

/// <summary>
/// 提示请求
/// </summary>
public class HintRequest
{
    /// <summary>
    /// 题目ID
    /// </summary>
    public int QuestionId { get; set; }
    
    /// <summary>
    /// 提示等级 (1=轻度, 2=中度, 3=深度)
    /// </summary>
    public int Level { get; set; } = 1;
    
    /// <summary>
    /// 用户的代码（可选，用于更精准的提示）
    /// </summary>
    public string? UserCode { get; set; }
    
    /// <summary>
    /// 编程语言
    /// </summary>
    public string? Language { get; set; }
    
    /// <summary>
    /// 强制刷新缓存
    /// </summary>
    public bool ForceRefresh { get; set; } = false;
}

/// <summary>
/// 提示响应
/// </summary>
public class HintResponse
{
    /// <summary>
    /// 题目ID
    /// </summary>
    public int QuestionId { get; set; }
    
    /// <summary>
    /// 提示等级
    /// </summary>
    public int Level { get; set; }
    
    /// <summary>
    /// 提示内容
    /// </summary>
    public string Content { get; set; } = "";
    
    /// <summary>
    /// 是否有下一级提示
    /// </summary>
    public bool HasNextLevel { get; set; }
    
    /// <summary>
    /// 生成时间
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.Now;
    
    /// <summary>
    /// 是否来自缓存
    /// </summary>
    public bool FromCache { get; set; }
}

