using ModelLibrary.api;
using ModelLibrary.repo;
using System.Linq;

namespace ProblemSetService;

public class ProblemSetService(ProblemSetRepository repo)
{
    public CreateProblemResponse CreateProblem(CreateProblemRequest request)
    {
        try
        {
            // 1. 基础参数校验（必填项不能为空）
            if (string.IsNullOrWhiteSpace(request.Uuid))
                return new CreateProblemResponse(400, "题目业务唯一标识（uuid）不能为空");
            if (string.IsNullOrWhiteSpace(request.Description))
                return new CreateProblemResponse(400, "题目描述不能为空");
            if (string.IsNullOrWhiteSpace(request.InputFormat))
                return new CreateProblemResponse(400, "输入格式说明不能为空");
            if (string.IsNullOrWhiteSpace(request.OutputFormat))
                return new CreateProblemResponse(400, "输出格式说明不能为空");
            if (string.IsNullOrWhiteSpace(request.Difficulty))
                return new CreateProblemResponse(400, "难度等级不能为空");

            // 2. 处理时间/内存限制转换（避免格式错误导致崩溃，失败用默认值）
            var timeLimit = 2000; // 默认2000毫秒
            var memoryLimit = 512; // 默认512MB
            if (!string.IsNullOrWhiteSpace(request.TimeLimit))
                int.TryParse(request.TimeLimit.Replace("(默认毫秒)", ""), out timeLimit);
            if (!string.IsNullOrWhiteSpace(request.MemoryLimit))
                int.TryParse(request.MemoryLimit.Replace("(默认MB)", ""), out memoryLimit);

            // 3. 调用Repo创建题目
            repo.CreateProblem(new Problem
            {
                Uuid = request.Uuid,
                Description = request.Description,
                InputFormat = request.InputFormat ?? "",
                OutputFormat = request.OutputFormat ?? "",
                Difficulty = request.Difficulty,
                Explanation = request.Explanation ?? "",
                TimeLimit = timeLimit,
                MemoryLimit = memoryLimit
            });

            return new CreateProblemResponse("题目创建成功");
        }
        catch (Exception ex)
        {
            // 捕获异常，返回服务器错误
            return new CreateProblemResponse(500, $"题目创建失败：{ex.Message}");
        }
    }

    public AddIoFileOfProblemResponse AddIoFileOfProblem(AddIoFileOfProblemRequest request)
    {
        try
        {
            // 1. 基础参数校验
            if (string.IsNullOrWhiteSpace(request.Uuid))
                return new AddIoFileOfProblemResponse(400, "题目业务唯一标识（uuid）不能为空");
            if (string.IsNullOrWhiteSpace(request.InFile))
                return new AddIoFileOfProblemResponse(400, "输入内容（inFile）不能为空");
            if (string.IsNullOrWhiteSpace(request.OutFile))
                return new AddIoFileOfProblemResponse(400, "输出内容（outFile）不能为空");

            // 2. 调用Repo添加样例（Repo中查不到题目会抛异常）
            repo.AddIoFileOfProblem(request.Uuid, request.InFile, request.OutFile);

            return new AddIoFileOfProblemResponse("输入输出样例添加成功");
        }
        catch (System.InvalidOperationException)
        {
            // 对应Repo中 Where(u => u.Uuid == uuid).First() 未找到数据的异常
            return new AddIoFileOfProblemResponse(404, "未找到该uuid对应的题目");
        }
        catch (Exception ex)
        {
            return new AddIoFileOfProblemResponse(500, $"样例添加失败：{ex.Message}");
        }
    }

    public GetIoFileOfProblemResponse GetIoFileOfProblem(GetIoFileOfProblemRequest request)
    {
        try
        {
            // 1. 基础参数校验
            if (string.IsNullOrWhiteSpace(request.Uuid))
                return new GetIoFileOfProblemResponse(400, "题目业务唯一标识（uuid）不能为空");

            
            // 2. 调用Repo查询样例，转换为DTO（隐藏数据库字段）
            var ioFiles = repo.GetIoFileOfProblem(request.Uuid);
            var ioFileDtos = ioFiles.Select(file => new IoFileDto(file.In, file.Out)).ToList();
            
            // 3. 构造响应（区分“有数据”和“无数据”场景）
            var message = ioFileDtos.Any() ? "输入输出样例查询成功" : "该题目暂无输入输出样例";
            return new GetIoFileOfProblemResponse(ioFileDtos, message);
        }
        catch (System.InvalidOperationException)
        {
            // Repo中查不到题目时抛的异常
            return new GetIoFileOfProblemResponse(404, "未找到该uuid对应的题目");
        }
        catch (Exception ex)
        {
            return new GetIoFileOfProblemResponse(500, $"样例查询失败：{ex.Message}");
        }
    }
    
    public AddIoExampleOfProblemResponse AddIoExampleOfProblem(AddIoExampleOfProblemRequest request)
    {
        try
        {
            repo.AddIoExampleOfProblem(request.Uuid, request.InExample, request.OutExample, request.Explanation);
            return new AddIoExampleOfProblemResponse(200, "样例添加成功");
        }
        catch (Exception ex)
        {
            return new AddIoExampleOfProblemResponse(500, ex.Message);
        }
    }

    public GetIoExampleOfProblemResponse GetIoExampleOfProblem(GetIoExampleOfProblemRequest request)
    {
        try
        {
            var examples = repo.GetIoExampleOfProblem(request.Uuid);
            
            var examplesDto = examples.Select(e => new 
                IoExampleDto(e.Index, e.InExample, e.OutExample, e.Explanation)).ToList();
        
            return new GetIoExampleOfProblemResponse(examplesDto, "获取成功");
        }
        catch (Exception ex)
        {
            return new GetIoExampleOfProblemResponse(500, ex.Message);
        }
    }
    
    /// <summary>
    /// 获取题目完整信息（包含基本信息、标签和带解释的样例）
    /// </summary>
    public GetProblemResponse GetProblem(GetProblemRequest request)
    {
        try
        {
            // 1. 基础参数校验
            if (string.IsNullOrWhiteSpace(request.Uuid))
                return new GetProblemResponse(400, "题目业务唯一标识（uuid）不能为空");

            // 2. 查询题目基本信息
            var problem = repo.GetProblemByUuid(request.Uuid);
            if (problem == null)
                return new GetProblemResponse(404, "未找到该uuid对应的题目");

            // 3. 查询题目标签
            var tags = repo.GetTagsOfProblem(problem.Uuid);

            // 4. 查询带解释的样例并转换格式
            var examples = repo.GetIoExampleOfProblem(request.Uuid);
            var ioExamples = examples.Select(e => new List<string>
            {
                e.InExample,
                e.OutExample,
                e.Explanation
            }).ToList();

            // 5. 构造完整响应对象
            return new GetProblemResponse(
                uuid: problem.Uuid,
                title: problem.Title,  // 注意：此处假设Problem实体有Title属性，需与实际实体匹配
                description: problem.Description,
                inputFormat: problem.InputFormat,
                outputFormat: problem.OutputFormat,
                ioExample: ioExamples,
                tags: tags,
                difficulty: problem.Difficulty,
                timeLimit: $"{problem.TimeLimit}(默认毫秒)",
                memoryLimit: $"{problem.MemoryLimit}(默认MB)",
                message: "题目详情获取成功"
            );
        }
        catch (InvalidOperationException)
        {
            return new GetProblemResponse(404, "未找到该uuid对应的题目");
        }
        catch (Exception ex)
        {
            return new GetProblemResponse(500, $"获取题目详情失败：{ex.Message}");
        }
    }
    
    /// <summary>
    /// 为题目添加标签（含参数校验和重复处理）
    /// </summary>
    public AddTagOfProblemResponse AddTagOfProblem(AddTagOfProblemRequest request)
    {
        try
        {
            // 1. 基础参数校验（必填项不能为空）
            if (string.IsNullOrWhiteSpace(request.Uuid))
                return new AddTagOfProblemResponse(400, "题目业务唯一标识（uuid）不能为空");
            if (string.IsNullOrWhiteSpace(request.Tag))
                return new AddTagOfProblemResponse(400, "标签名称（tag）不能为空");

            // 2. 调用Repo添加标签（Repo返回bool表示是否新增成功）
            var addSuccess = repo.AddTagOfProblem(request.Uuid, request.Tag);
            if (!addSuccess)
                return new AddTagOfProblemResponse(400, $"该题目已存在标签：{request.Tag}");

            // 3. 添加成功
            return new AddTagOfProblemResponse();
        }
        catch (InvalidOperationException ex)
        {
            // 捕获Repo中“题目不存在”的异常
            return new AddTagOfProblemResponse(404, ex.Message);
        }
        catch (Exception ex)
        {
            // 通用服务器异常
            return new AddTagOfProblemResponse(500, $"标签添加失败：{ex.Message}");
        }
    }
    
    #region 提交记录相关逻辑（新增）
    /// <summary>
    /// 创建提交记录（返回提交实体，供Controller转DTO）
    /// </summary>
    public CreateSubmissionResponse CreateSubmission(CreateSubmissionRequest request)
    {
        try
        {
            // 1. 基础参数校验（与现有方法校验风格一致）
            if (string.IsNullOrWhiteSpace(request.UserId))
                return new CreateSubmissionResponse(400, "用户标识（userId）不能为空");
            if (string.IsNullOrWhiteSpace(request.ProblemUuid))
                return new CreateSubmissionResponse(400, "题目标识（problemId）不能为空");
            if (string.IsNullOrWhiteSpace(request.Lang))
                return new CreateSubmissionResponse(400, "编程语言（lang）不能为空");
            if (string.IsNullOrWhiteSpace(request.Code))
                return new CreateSubmissionResponse(400, "提交代码不能为空");

            // 2. UUID转内部ID（复用Repo辅助方法，查不到抛InvalidOperationException）
            int userId = repo.GetUserIdByUuid(request.UserId);
            int problemId = repo.GetProblemIdByUuid(request.ProblemUuid);

            // 3. 构造提交实体（匹配Submission实体字段，默认状态为待评测）
            var submissionEntity = new Submission
            {
                Uuid = Guid.NewGuid().ToString("N"), // 生成短哈希值作为对外标识
                UserId = userId,
                ProblemId = problemId,
                SubmissionTime = DateTime.Now,
                Lang = request.Lang,
                State = "Pending", // 初始状态：待评测
                Time = 0, // 未运行时时间为0
                Memory = 0, // 未运行时内存为0
                Code = request.Code
            };

            // 4. 调用Repo保存提交记录
            var savedEntity = repo.CreateSubmission(submissionEntity);

            // 5. 返回业务响应（携带实体给Controller，不做DTO转换）
            return new CreateSubmissionResponse(200, "提交成功", savedEntity);
        }
        catch (InvalidOperationException ex)
        {
            // 捕获Repo中“用户/题目不存在”的异常（与AddIoFileOfProblem异常逻辑一致）
            return new CreateSubmissionResponse(404, ex.Message);
        }
        catch (Exception ex)
        {
            // 通用服务器异常（错误信息格式与现有方法统一）
            return new CreateSubmissionResponse(500, $"提交失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 获取单个提交记录（返回实体，供Controller转DTO）
    /// </summary>
    public GetSubmissionResponse GetSubmission(GetSubmissionRequest request)
    {
        try
        {
            // 1. 基础参数校验
            if (string.IsNullOrWhiteSpace(request.SubmissionUuid))
                return new GetSubmissionResponse(400, "提交标识（id）不能为空");

            // 2. 调用Repo查询提交实体（含关联的User/Problem）
            var submissionEntity = repo.GetSubmissionEntityByUuid(request.SubmissionUuid);
            if (submissionEntity == null)
                return new GetSubmissionResponse(404, "未找到该提交记录");

            // 3. 返回实体（携带实体给Controller）
            return new GetSubmissionResponse(submissionEntity, "查询成功");
        }
        catch (Exception ex)
        {
            return new GetSubmissionResponse(500, $"查询提交失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 获取提交记录列表（支持筛选、分页，返回实体列表+总记录数）
    /// </summary>
    public GetSubmissionListResponse GetSubmissionList(GetSubmissionListRequest request)
    {
        try
        {
            // 1. 分页参数校验（避免非法分页请求）
            int page = Math.Max(request.Page, 1); // 页码最小为1
            int pageSize = Math.Clamp(request.PageSize, 1, 100); // 每页条数限制1-100条

            // 2. 调用Repo查询实体列表（out参数获取总记录数）
            repo.GetSubmissionEntityListByFilter(
                userUuid: request.UserId,
                problemUuid: request.ProblemUuid,
                page: page,
                pageSize: pageSize,
                out int total
            );

            var submissionEntities = repo.GetSubmissionEntityListByFilter(
                request.UserId, 
                request.ProblemUuid, 
                page, 
                pageSize, 
                out total
            );

            // 3. 返回实体列表+分页信息（给Controller处理DTO和分页响应）
            return new GetSubmissionListResponse(submissionEntities, total, pageSize);
        }
        catch (InvalidOperationException ex)
        {
            // 若筛选时用户/题目不存在（Repo抛异常），返回404
            return new GetSubmissionListResponse(404, ex.Message);
        }
        catch (Exception ex)
        {
            return new GetSubmissionListResponse(500, $"查询提交列表失败：{ex.Message}");
        }
    }
    #endregion
}