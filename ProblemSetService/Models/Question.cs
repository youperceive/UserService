namespace ProblemSetService.Models;

/// <summary>
/// 题目示例
/// </summary>
public class QuestionExample
{
    public string Input { get; set; } = "";
    public string Output { get; set; } = "";
    public string? Explanation { get; set; }
}

/// <summary>
/// 标签
/// </summary>
public class QuestionTag
{
    public string Name { get; set; } = "";
    public string Color { get; set; } = "";
    public string BackgroundColor { get; set; } = "";
}

/// <summary>
/// 题目完整信息（用于题目详情页）
/// </summary>
public class Question
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string DescriptionH5 { get; set; } = "";  // HTML格式的题目描述
    public string InputFormatH5 { get; set; } = "";  // HTML格式的输入格式
    public string OutputFormatH5 { get; set; } = ""; // HTML格式的输出格式
    public string NoticeH5 { get; set; } = "";       // HTML格式的限制说明
    public List<QuestionTag> Tags { get; set; } = new();
    public List<QuestionExample> Examples { get; set; } = new();
    public List<string> EnabledLanguages { get; set; } = new();
    public bool AllowTemplate { get; set; } = true;
}

/// <summary>
/// 题目列表项（用于题目列表页，数据量较小）
/// </summary>
public class QuestionListItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Difficulty { get; set; } = "easy"; // easy, normal, hard
    public List<string> Tags { get; set; } = new();   // 简化的标签（只有名称）
    public int AcceptedCount { get; set; }
    public int SubmissionCount { get; set; }
    public double AcceptanceRate => SubmissionCount > 0 ? (double)AcceptedCount / SubmissionCount * 100 : 0;
}

/// <summary>
/// JSON文件中的题目格式（用于解析Questions.json）
/// </summary>
public class QuestionJson
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Input { get; set; } = "";
    public string Output { get; set; } = "";
    public string Limit { get; set; } = "";
    public string Difficulty { get; set; } = "easy";
    public List<string> Tags { get; set; } = new();
    public List<QuestionExampleJson> Examples { get; set; } = new();
}

public class QuestionExampleJson
{
    public string Input { get; set; } = "";
    public string Output { get; set; } = "";
    public string? Explanation { get; set; }
}

public class RecommendRequest
{
    public string UserName { get; set; } = "";
}

public partial class RecommendResponse
{
    public string Message { get; set; } = "";
}