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

    public void CreateProblem(Problem p)
    {
        _db.Insertable(p).ExecuteCommand();
    }
    
    public void AddIoFileOfProblem(string uuid, string inFile, string outFile)
    {
        var id = _db.Queryable<Problem>().Where(u => u.Uuid == uuid).First().Id;

        var ioFile = new IoFileOfProblem()
        {
            ProblemId = id,
            In = inFile,
            Out = outFile
        };
        
        _db.Insertable(ioFile).ExecuteCommand();
    }
    
    public List<IoFileOfProblem> GetIoFileOfProblem(string uuid)
    {
        var id = _db.Queryable<Problem>().Where(u => u.Uuid == uuid).First().Id;

        var ps = _db.Queryable<IoFileOfProblem>().
            Where(u => u.ProblemId == id).ToArray();
        
        
        return ps.ToList();
    }

    public void DelIoFileOfProblem(int id)
    {
        _db.Deleteable<IoFileOfProblem>().Where(u => u.Id == id);
    }
}