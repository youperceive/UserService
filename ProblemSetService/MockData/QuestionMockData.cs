namespace ProblemSetService.MockData;

/// <summary>
/// 题目数据的假数据生成器
/// </summary>
public static class QuestionMockData
{
    /// <summary>
    /// 获取所有题目的假数据
    /// </summary>
    public static List<QuestionItem> GetAllQuestions()
    {
        return new List<QuestionItem>
        {
            new QuestionItem
            {
                Id = 1,
                Title = "两数之和",
                Difficulty = "简单",
                AcceptanceRate = 52.3,
                Tags = new List<string> { "数组", "哈希表" }
            },
            new QuestionItem
            {
                Id = 2,
                Title = "反转链表",
                Difficulty = "简单",
                AcceptanceRate = 73.5,
                Tags = new List<string> { "链表", "递归" }
            },
            new QuestionItem
            {
                Id = 3,
                Title = "最长回文子串",
                Difficulty = "中等",
                AcceptanceRate = 36.8,
                Tags = new List<string> { "字符串", "动态规划" }
            },
            new QuestionItem
            {
                Id = 4,
                Title = "三数之和",
                Difficulty = "中等",
                AcceptanceRate = 34.2,
                Tags = new List<string> { "数组", "双指针" }
            },
            new QuestionItem
            {
                Id = 5,
                Title = "正则表达式匹配",
                Difficulty = "困难",
                AcceptanceRate = 27.5,
                Tags = new List<string> { "字符串", "动态规划", "递归" }
            },
            new QuestionItem
            {
                Id = 6,
                Title = "合并K个升序链表",
                Difficulty = "困难",
                AcceptanceRate = 56.8,
                Tags = new List<string> { "链表", "分治", "堆" }
            },
            new QuestionItem
            {
                Id = 7,
                Title = "二叉树的最大深度",
                Difficulty = "简单",
                AcceptanceRate = 77.2,
                Tags = new List<string> { "树", "深度优先搜索", "递归" }
            },
            new QuestionItem
            {
                Id = 8,
                Title = "环形链表",
                Difficulty = "简单",
                AcceptanceRate = 51.4,
                Tags = new List<string> { "链表", "双指针" }
            },
            new QuestionItem
            {
                Id = 9,
                Title = "合并两个有序数组",
                Difficulty = "简单",
                AcceptanceRate = 52.8,
                Tags = new List<string> { "数组", "双指针" }
            },
            new QuestionItem
            {
                Id = 10,
                Title = "买卖股票的最佳时机",
                Difficulty = "简单",
                AcceptanceRate = 58.3,
                Tags = new List<string> { "数组", "动态规划" }
            }
        };
    }
    
    /// <summary>
    /// 获取推荐题目假数据
    /// </summary>
    public static List<QuestionItem> GetRecommendQuestions(int count = 5)
    {
        return new List<QuestionItem>
        {
            new QuestionItem
            {
                Id = 1,
                Title = "两数之和",
                Difficulty = "简单",
                AcceptanceRate = 52.3,
                Tags = new List<string> { "数组", "哈希表" }
            },
            new QuestionItem
            {
                Id = 3,
                Title = "最长回文子串",
                Difficulty = "中等",
                AcceptanceRate = 36.8,
                Tags = new List<string> { "字符串", "动态规划" }
            },
            new QuestionItem
            {
                Id = 7,
                Title = "二叉树的最大深度",
                Difficulty = "简单",
                AcceptanceRate = 77.2,
                Tags = new List<string> { "树", "深度优先搜索", "递归" }
            },
            new QuestionItem
            {
                Id = 4,
                Title = "三数之和",
                Difficulty = "中等",
                AcceptanceRate = 34.2,
                Tags = new List<string> { "数组", "双指针" }
            },
            new QuestionItem
            {
                Id = 10,
                Title = "买卖股票的最佳时机",
                Difficulty = "简单",
                AcceptanceRate = 58.3,
                Tags = new List<string> { "数组", "动态规划" }
            }
        };
    }
}

#region Model Classes

public class QuestionItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public double AcceptanceRate { get; set; }
    public List<string> Tags { get; set; } = new();
}

#endregion

