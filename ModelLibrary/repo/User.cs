using System.ComponentModel.DataAnnotations;
using SqlSugar;

namespace ModelLibrary.repo;

public class User
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    [SugarColumn(Length = 255)]
    public string Email { get; init; } = "";

    [SugarColumn(Length = 255)]
    public string PasswordHash { get; init; } = "";
}

public class UserRole
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int UserId { get; set; }

    [SugarColumn(Length = 255)] public string Role { get; set; } = "user";
}