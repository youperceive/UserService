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
            int timeLimit = 2000; // 默认2000毫秒
            int memoryLimit = 512; // 默认512MB
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
}