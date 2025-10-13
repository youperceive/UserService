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
}