namespace QAService.Models;

/// <summary>
/// 题解请求
/// </summary>
public class SolutionRequest
{
    /// <summary>
    /// 题目ID
    /// </summary>
    public int QuestionId { get; set; }
    
    /// <summary>
    /// 用户的代码（用于AI分析）
    /// </summary>
    public string UserCode { get; set; } = "";
    
    /// <summary>
    /// 编程语言
    /// </summary>
    public string Language { get; set; } = "C++";
}

/// <summary>
/// 题解响应
/// </summary>
public class SolutionResponse
{
    /// <summary>
    /// 题目ID
    /// </summary>
    public int QuestionId { get; set; }
    
    /// <summary>
    /// 标准题解
    /// </summary>
    public StandardSolution? StandardSolution { get; set; }
    
    /// <summary>
    /// 用户代码分析
    /// </summary>
    public CodeAnalysis? UserCodeAnalysis { get; set; }
    
    /// <summary>
    /// 生成时间
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// 标准题解
/// </summary>
public class StandardSolution
{
    /// <summary>
    /// 解题思路
    /// </summary>
    public string Approach { get; set; } = "";
    
    /// <summary>
    /// 算法说明
    /// </summary>
    public string Algorithm { get; set; } = "";
    
    /// <summary>
    /// 时间复杂度
    /// </summary>
    public string TimeComplexity { get; set; } = "";
    
    /// <summary>
    /// 空间复杂度
    /// </summary>
    public string SpaceComplexity { get; set; } = "";
    
    /// <summary>
    /// 示例代码
    /// </summary>
    public Dictionary<string, string> SampleCodes { get; set; } = new();
    
    /// <summary>
    /// 关键点
    /// </summary>
    public List<string> KeyPoints { get; set; } = new();
}

/// <summary>
/// 代码分析
/// </summary>
public class CodeAnalysis
{
    /// <summary>
    /// 整体评价
    /// </summary>
    public string OverallReview { get; set; } = "";
    
    /// <summary>
    /// 正确性分析
    /// </summary>
    public string Correctness { get; set; } = "";
    
    /// <summary>
    /// 时间复杂度
    /// </summary>
    public string TimeComplexity { get; set; } = "";
    
    /// <summary>
    /// 空间复杂度
    /// </summary>
    public string SpaceComplexity { get; set; } = "";
    
    /// <summary>
    /// 优化建议
    /// </summary>
    public List<string> Suggestions { get; set; } = new();
    
    /// <summary>
    /// 评分（0-100）
    /// </summary>
    public int Score { get; set; }
}

