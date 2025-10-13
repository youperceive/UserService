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
        _db.CodeFirst.InitTables<IoExampleProblem>();
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
    
    public void AddIoExampleOfProblem(string uuid, string inFile, string outFile, string explanation)
    {
        var id = _db.Queryable<Problem>().Where(u => u.Uuid == uuid).First().Id;

        var ioExample = new IoExampleProblem()
        {
            ProblemId = id,
            InExample = inFile,
            OutExample = outFile,
            Explanation = explanation
        };
        
        _db.Insertable(ioExample).ExecuteCommand();
    }
    
    public List<IoExampleProblem> GetIoExampleOfProblem(string uuid)
    {
        var id = _db.Queryable<Problem>().Where(u => u.Uuid == uuid).First().Id;

        var ps = _db.Queryable<IoExampleProblem>().
            Where(u => u.ProblemId == id).ToArray();
        
        
        return ps.ToList();
    }
    
    /// <summary>
    /// 根据uuid查询题目基本信息
    /// </summary>
    public Problem? GetProblemByUuid(string uuid)
    {
        return _db.Queryable<Problem>()
            .Where(p => p.Uuid == uuid)
            .First();
    }

    /// <summary>
    /// 根据题目ID查询所有标签
    /// </summary>
    public List<string> GetTagsOfProblem(string uuid)
    {
        var problemId = GetProblemByUuid(uuid)!.Id;
        
        return _db.Queryable<TagOfProblem>()
            .Where(t => t.ProblemId == problemId)
            .Select(t => t.Tag)
            .ToList();
    }
    
    public bool AddTagOfProblem(string uuid, string tag)
    {
        // 1. 根据uuid查题目ID（查不到会抛InvalidOperationException，与现有逻辑一致）
        var problem = _db.Queryable<Problem>().Where(p => p.Uuid == uuid).First();
        if (problem == null)
            throw new InvalidOperationException("未找到该uuid对应的题目");

        // 2. 校验该题目是否已存在此标签（避免重复添加）
        var tagExists = _db.Queryable<TagOfProblem>()
            .Where(t => t.ProblemId == problem.Id && t.Tag == tag)
            .Any();
        if (tagExists)
            return false; // 标签已存在，返回false

        // 3. 插入新标签
        _db.Insertable(new TagOfProblem
        {
            ProblemId = problem.Id,
            Tag = tag
        }).ExecuteCommand();

        return true; // 添加成功
    }
}