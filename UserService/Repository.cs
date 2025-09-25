using ModelLibrary.repo;
using SqlSugar;

namespace UserService;

public interface IUserRepository
{
    public void CreateUser(string email, string password);

    public User GetUserByEmail(string email);
}


public class UserRepository : IUserRepository
{
    private readonly SqlSugarScope _db;

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
    }

    public void CreateUser(string email, string password)
    {
        var user = new User
        {
            Email = email,
            PasswordHash = Util.HashPassword(password),
        };  
        
        // 使用SQLSugar的插入方法
        _db.Insertable(user).ExecuteCommand();
    }

    public User GetUserByEmail(string email)
    {
        return _db.Queryable<User>().Where(u => u.Email == email).First();
    }
}