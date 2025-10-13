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
        _db.CodeFirst.InitTables<Submission>();
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
    
    #region 提交记录方法（返回实体）
    public Submission CreateSubmission(Submission submission)
    {
        // 插入并返回带自增ID的实体
        return _db.Insertable(submission).ExecuteReturnEntity();
    }

    public Submission? GetSubmissionEntityByUuid(string submissionUuid)
    {
        // 关联查询：加载User和Problem导航属性
        return _db.Queryable<Submission>()
            .Includes(s => s.User) // 加载关联的User实体（需确保Submission有User导航属性）
            .Includes(s => s.Problem) // 加载关联的Problem实体
            .Where(s => s.Uuid == submissionUuid)
            .First();
    }

    public List<Submission> GetSubmissionEntityListByFilter(string? userUuid, string? problemUuid, int page, int pageSize, out int total)
    {
        var query = _db.Queryable<Submission>()
            .Includes(s => s.User)
            .Includes(s => s.Problem);

        // 筛选条件（UUID转内部ID）
        if (!string.IsNullOrWhiteSpace(userUuid))
        {
            int userId = GetUserIdByUuid(userUuid);
            query = query.Where(s => s.UserId == userId);
        }
        if (!string.IsNullOrWhiteSpace(problemUuid))
        {
            int problemId = GetProblemIdByUuid(problemUuid);
            query = query.Where(s => s.ProblemId == problemId);
        }

        // 总记录数
        total = query.Count();

        // 分页查询（按提交时间倒序）
        return query.OrderBy(s => s.SubmissionTime, OrderByType.Desc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }
    #endregion
    
    #region 辅助方法：UUID转内部ID
    /// <summary>
    /// 用户UUID转内部ID（查不到抛异常，与题目转换逻辑一致）
    /// </summary>
    public int GetUserIdByUuid(string userUuid)
    {
        var user = _db.Queryable<User>()
            .Where(u => u.Uuid == userUuid)
            .First();

        if (user == null)
            throw new InvalidOperationException($"未找到UUID为【{userUuid}】的用户");

        return user.Id;
    }

    /// <summary>
    /// 题目UUID转内部ID（复用之前逻辑，确保实现）
    /// </summary>
    public int GetProblemIdByUuid(string problemUuid)
    {
        var problem = _db.Queryable<Problem>()
            .Where(p => p.Uuid == problemUuid)
            .First();

        if (problem == null)
            throw new InvalidOperationException($"未找到UUID为【{problemUuid}】的题目");

        return problem.Id;
    }
    #endregion
}