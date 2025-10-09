using ModelLibrary.repo;
using SqlSugar;

namespace ProblemSetService;

public interface IProblemSetRepository
{
}


public class ProblemSetRepository : IProblemSetRepository
{
    private readonly SqlSugarScope _db;
    
    public ProblemSetRepository(string connectionString)
    {
        _db = new SqlSugarScope(new ConnectionConfig
        {
            ConnectionString = connectionString,
            DbType = DbType.PostgreSQL,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute
        });
        
        // 创建表（如果不存在）
        _db.CodeFirst.InitTables<Problem>();
        _db.CodeFirst.InitTables<TagOfProblem>();
        _db.CodeFirst.InitTables<IoFileOfProblem>();
    }
}