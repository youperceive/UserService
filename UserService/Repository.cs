using ModelLibrary.repo;
using SqlSugar;

namespace UserService;

public interface IInfra
{
    void Execute(string query, object? parameters = null);
    dynamic? FetchOne(string query, object? parameters = null);
    List<dynamic> FetchAll(string query, object? parameters = null);
}

public class PostgreSqlInfra(string connectionString) : IInfra, IDisposable
{
    private readonly SqlSugarScope _db = new(new ConnectionConfig()
    {
        ConnectionString = connectionString,
        DbType = DbType.PostgreSQL,
        IsAutoCloseConnection = true,
        InitKeyType = InitKeyType.Attribute
    });

    public void Execute(string query, object? parameters = null)
    {
        _db.Ado.ExecuteCommand(query, parameters);
    }

    public dynamic? FetchOne(string query, object? parameters = null)
    {
        return _db.Ado.SqlQuery<dynamic>(query, parameters).FirstOrDefault();
    }

    public List<dynamic> FetchAll(string query, object? parameters = null)
    {
        return _db.Ado.SqlQuery<dynamic>(query, parameters);
    }

    public void Dispose()
    {
        _db?.Dispose();
    }
}

public class UserRepository : IDisposable
{
    private readonly PostgreSqlInfra _infra;
    private readonly SqlSugarScope _db;

    public UserRepository(string? connectionString)
    {
        _infra = new PostgreSqlInfra(connectionString);
        _db = new SqlSugarScope(new ConnectionConfig()
        {
            ConnectionString = connectionString,
            DbType = DbType.PostgreSQL,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute
        });
        
        // 创建表（如果不存在）
        _db.CodeFirst.InitTables(typeof(User));
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

    public void Dispose()
    {
        _infra?.Dispose();
        _db?.Dispose();
    }
}