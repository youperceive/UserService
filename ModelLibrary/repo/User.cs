using SqlSugar;

namespace ModelLibrary.repo;

public class User
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    [SugarColumn(Length = 255)]
    public string Email { get; init; } = "";

    [SugarColumn(Length = 255)]
    public string PasswordHash { get; set; } = "";
}