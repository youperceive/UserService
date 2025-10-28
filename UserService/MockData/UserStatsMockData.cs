namespace UserService.MockData;

/// <summary>
/// 用户统计数据的假数据生成器
/// </summary>
public static class UserStatsMockData
{
    /// <summary>
    /// 获取成就卡片假数据
    /// </summary>
    public static List<AchievementCard> GetAchievementCards(string userId)
    {
        return new List<AchievementCard>
        {
            new()
            {
                Description = "等级",
                Data = 10,
                Type = "level"
            },
            new()
            {
                Description = "阅读总数",
                Data = 350,
                YesterdayData = 50,
                Type = "read"
            },
            new()
            {
                Description = "获得点赞",
                Data = 100,
                YesterdayData = 30,
                Type = "like"
            },
            new()
            {
                Description = "获得收藏",
                Data = 80,
                YesterdayData = 20,
                Type = "favorite"
            }
        };
    }

    /// <summary>
    /// 获取最近通过题目假数据
    /// </summary>
    public static List<RecentSolvedItem> GetRecentSolved(string userId)
    {
        return [
            new()
            {
                Id = 1,
                Title = "两数之和",
                LastSolvedAt = DateTime.Parse("2025-09-20T12:30:00")
            },
            new()
            {
                Id = 2,
                Title = "反转链表",
                LastSolvedAt = DateTime.Parse("2025-09-21T15:45:00")
            },
            new()
            {
                Id = 3,
                Title = "最长回文子串",
                LastSolvedAt = DateTime.Parse("2025-09-25T08:10:00")
            }
        ];
    }

    /// <summary>
    /// 获取语言统计假数据
    /// </summary>
    public static List<LanguageStatItem> GetLanguageStats(string userId)
    {
        return new List<LanguageStatItem>
        {
            new() { Name = "C++", Value = 14 },
            new() { Name = "Python", Value = 28 },
            new() { Name = "Java", Value = 22 },
            new() { Name = "JavaScript", Value = 31 },
            new() { Name = "Go", Value = 7 },
            new() { Name = "Rust", Value = 12 }
        };
    }

    /// <summary>
    /// 获取题目类型统计假数据
    /// </summary>
    public static List<TypeStatItem> GetSolvedTypeStats(string userId)
    {
        return new List<TypeStatItem>
        {
            new() { Name = "数据结构", Value = 35 },
            new() { Name = "图论", Value = 20 },
            new() { Name = "数论", Value = 15 },
            new() { Name = "双指针", Value = 25 },
            new() { Name = "动态规划", Value = 30 },
            new() { Name = "贪心", Value = 18 }
        };
    }

    /// <summary>
    /// 获取难度统计假数据
    /// </summary>
    public static DifficultyStats GetDifficultyStats(string userId)
    {
        return new DifficultyStats
        {
            Easy = new DifficultyStatItem { Solved = 120, Total = 150 },
            Medium = new DifficultyStatItem { Solved = 80, Total = 200 },
            Hard = new DifficultyStatItem { Solved = 30, Total = 100 }
        };
    }
}

#region Model Classes

public class AchievementCard
{
    public string Description { get; set; } = string.Empty;
    public int Data { get; set; }
    public int? YesterdayData { get; set; }
    public string Type { get; set; } = string.Empty;
}

public class RecentSolvedItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime LastSolvedAt { get; set; }
}

public class LanguageStatItem
{
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
}

public class TypeStatItem
{
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
}

public class DifficultyStats
{
    public DifficultyStatItem Easy { get; set; } = new();
    public DifficultyStatItem Medium { get; set; } = new();
    public DifficultyStatItem Hard { get; set; } = new();
}

public class DifficultyStatItem
{
    public int Solved { get; set; }
    public int Total { get; set; }
}

#endregion

