using ModelLibrary.repo;
using SqlSugar;

namespace UserService;

public interface IUserRepository
{
    public void CreateUser(string uuid, string email, string password);

    public User GetUserByEmail(string email);
    public User GetUserById(int id);
    
    public bool HasRoleHigherThan(int id, string role);
}


public class UserRepository : IUserRepository
{
    private readonly SqlSugarScope _db;

    private readonly Dictionary<string, int> _roles = new()
    {
        {"root", 1},
        {"admin", 2},
        {"user", 3},
        {"visitor", 4}
    };

    public UserRepository(string connectionString)
    {
        _db = new SqlSugarScope(new ConnectionConfig
        {
            ConnectionString = connectionString,
            DbType = DbType.PostgreSQL,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute
        });
        
        // 创建表（如果不存在）
        _db.CodeFirst.InitTables<User>();
        _db.CodeFirst.InitTables<UserRole>();
    }

    public void CreateUser(string uuid, string email, string password)
    {
        var user = new User
        {
            Uuid = uuid,
            Email = email,
            PasswordHash = Util.HashPassword(password),
        };  
        
        // 使用SQLSugar的插入方法
        _db.Insertable(user).ExecuteCommand();
        
        var u = GetUserByEmail(email);
        var ur = new UserRole()
        {
            UserId = u.Id,
            Role = "user",
        };
        _db.Insertable(ur).ExecuteCommand();
    }

    public User GetUserByEmail(string email)
    {
        return _db.Queryable<User>().Where(u => u.Email == email).First();
    }

    public User GetUserById(int id)
    {
        return _db.Queryable<User>().Where(u => u.Id == id).First();
    }

    private string GetRoleOfUser(int id)
    {
        var userRole = _db.Queryable<UserRole>().Where(u => u.UserId == id).First();
        return userRole.Role;
    }

    public bool HasRoleHigherThan(int id, string role)
    {
        var userIdx = _roles[GetRoleOfUser(id)];
        var targetIdx = _roles[role];
        return userIdx <= targetIdx;
    }
}