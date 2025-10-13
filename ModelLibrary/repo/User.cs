using System.ComponentModel.DataAnnotations;
using SqlSugar;

namespace ModelLibrary.repo;

public class User
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    
    public string Uuid { get; set; } = string.Empty;

    [SugarColumn(Length = 255)] // 添加 unique 约束
    public string Email { get; init; } = "";

    [SugarColumn(Length = 255)]
    public string PasswordHash { get; init; } = "";
    
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.OneToMany, nameof(Submission.UserId))]
    public List<Submission>? Submissions { get; set; }
}

public class UserRole
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int UserId { get; set; }

    [SugarColumn(Length = 255)] public string Role { get; set; } = "user";
}