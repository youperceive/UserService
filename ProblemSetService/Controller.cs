using Microsoft.AspNetCore.Mvc;
using ModelLibrary.api;

namespace ProblemSetService;

[ApiController]
[Route("api/[controller]")]
public class ProblemController(ProblemSetService problemSetService) : ControllerBase
{
    /// <summary>
    /// 创建新题目
    /// </summary>
    [HttpPost("create")]
    public ActionResult<CreateProblemResponse> CreateProblem([FromBody] CreateProblemRequest request)
    {
        try
        {
            var response = problemSetService.CreateProblem(request);
            return response.StatusCode != 200 ? 
                StatusCode(response.StatusCode, response) : Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new BaseResponse(500, $"服务器错误: {ex.Message}"));
        }
    }
    
    /// <summary>
    /// 获取题目完整详情（包含基本信息、标签、带解释的样例等）
    /// </summary>
    [HttpGet("get-problem")]
    public ActionResult<GetProblemResponse> GetProblem([FromQuery] string uuid)
    {
        try
        {
            // 1. 构造请求对象（与其他查询接口保持一致）
            var request = new GetProblemRequest { Uuid = uuid };
            // 2. 调用Service层获取题目详情
            var response = problemSetService.GetProblem(request);
        
            // 3. 根据响应状态码返回对应结果（非200状态码返回自定义状态码）
            return response.StatusCode != 200 
                ? StatusCode(response.StatusCode, response) 
                : Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            // 捕获“题目不存在”类异常，返回404
            return NotFound(new BaseResponse(404, ex.Message));
        }
        catch (Exception ex)
        {
            // 捕获通用异常，返回500服务器错误
            return StatusCode(500, new BaseResponse(500, $"服务器错误: {ex.Message}"));
        }
    }
    
    /// <summary>
    /// 为题目添加输入输出样例
    /// </summary>
    [HttpPost("add-io-file")]
    public ActionResult<AddIoFileOfProblemResponse> AddIoFileOfProblem([FromBody] AddIoFileOfProblemRequest request)
    {
        try
        {
            var response = problemSetService.AddIoFileOfProblem(request);
            return response.StatusCode != 200 ? 
                StatusCode(response.StatusCode, response) : Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new BaseResponse(500, $"服务器错误: {ex.Message}"));
        }
    }
    
    /// <summary>
    /// 获取题目所有输入输出样例
    /// </summary>
    [HttpGet("get-io-files")]
    public ActionResult<GetIoFileOfProblemResponse> GetIoFileOfProblem([FromQuery] string uuid)
    {
        try
        {
            var request = new GetIoFileOfProblemRequest { Uuid = uuid };
            var response = problemSetService.GetIoFileOfProblem(request);
            return response.StatusCode != 200 ? 
                StatusCode(response.StatusCode, response) : Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new BaseResponse(404, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new BaseResponse(500, $"服务器错误: {ex.Message}"));
        }
    }
    
    /// <summary>
    /// 为题目添加带解释的输入输出样例
    /// </summary>
    [HttpPost("add-io-example")]
    public ActionResult<AddIoExampleOfProblemResponse> AddIoExampleOfProblem([FromBody] AddIoExampleOfProblemRequest request)
    {
        try
        {
            var response = problemSetService.AddIoExampleOfProblem(request);
            return response.StatusCode != 200 ? 
                StatusCode(response.StatusCode, response) : Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new BaseResponse(404, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new BaseResponse(500, $"服务器错误: {ex.Message}"));
        }
    }

    /// <summary>
    /// 获取题目所有带解释的输入输出样例
    /// </summary>
    [HttpGet("get-io-examples")]
    public ActionResult<GetIoExampleOfProblemResponse> GetIoExampleOfProblem([FromQuery] string uuid)
    {
        try
        {
            var request = new GetIoExampleOfProblemRequest { Uuid = uuid };
            var response = problemSetService.GetIoExampleOfProblem(request);
            return response.StatusCode != 200 ? 
                StatusCode(response.StatusCode, response) : Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new BaseResponse(404, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new BaseResponse(500, $"服务器错误: {ex.Message}"));
        }
    }
    
    /// <summary>
    /// 为题目添加标签（同一题目不能重复添加相同标签）
    /// </summary>
    [HttpPost("add-tag")]
    public ActionResult<AddTagOfProblemResponse> AddTagOfProblem([FromBody] AddTagOfProblemRequest request)
    {
        try
        {
            var response = problemSetService.AddTagOfProblem(request);
            // 根据响应状态码返回结果（与现有接口逻辑一致）
            return response.StatusCode != 200 
                ? StatusCode(response.StatusCode, response) 
                : Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            // 兼容可能的“资源不存在”异常（与现有接口一致）
            return NotFound(new BaseResponse(404, ex.Message));
        }
        catch (Exception ex)
        {
            // 通用服务器错误
            return StatusCode(500, new BaseResponse(500, $"服务器错误: {ex.Message}"));
        }
    }
    
    #region 提交记录相关接口（新增）
    /// <summary>
    /// 创建代码提交记录
    /// </summary>
    [HttpPost("submission/create")]
    public ActionResult<CreateSubmissionResponse> CreateSubmission([FromBody] CreateSubmissionRequest request)
    {
        try
        {
            // 调用Service层创建提交（返回含实体的响应）
            var response = problemSetService.CreateSubmission(request);
            
            // 统一响应处理：非200状态码返回对应StatusCode，成功返回Ok
            return response.StatusCode != 200 
                ? StatusCode(response.StatusCode, response) 
                : Ok(response);
        }
        catch (Exception ex)
        {
            // 通用异常处理（与其他接口保持一致）
            return StatusCode(500, new BaseResponse(500, $"服务器错误: {ex.Message}"));
        }
    }

    /// <summary>
    /// 获取单个提交记录详情
    /// </summary>
    [HttpGet("submission/get")]
    public ActionResult<GetSubmissionResponse> GetSubmission([FromQuery] string submissionUuid)
    {
        try
        {
            // 1. 构造请求对象（与GetProblem接口参数处理逻辑一致）
            var request = new GetSubmissionRequest { SubmissionUuid = submissionUuid };
            
            // 2. 调用Service层查询提交
            var response = problemSetService.GetSubmission(request);
            
            // 3. 响应处理（与其他查询接口逻辑统一）
            return response.StatusCode != 200 
                ? StatusCode(response.StatusCode, response) 
                : Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            // 捕获“提交记录不存在”异常（与GetProblem接口异常处理对齐）
            return NotFound(new BaseResponse(404, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new BaseResponse(500, $"服务器错误: {ex.Message}"));
        }
    }

    /// <summary>
    /// 获取提交记录列表（支持按用户/题目筛选、分页）
    /// </summary>
    [HttpGet("submission/list")]
    public ActionResult<GetSubmissionListResponse> GetSubmissionList(
        [FromQuery] string? userId,
        [FromQuery] string? problemId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            // 1. 构造分页查询请求（参数映射与现有分页逻辑一致）
            var request = new GetSubmissionListRequest
            {
                UserId = userId,
                ProblemUuid = problemId,
                Page = page,
                PageSize = pageSize
            };
            
            // 2. 调用Service层查询列表
            var response = problemSetService.GetSubmissionList(request);
            
            // 3. 响应处理（与其他列表查询接口统一）
            return response.StatusCode != 200 
                ? StatusCode(response.StatusCode, response) 
                : Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            // 筛选条件中用户/题目不存在时的异常处理
            return NotFound(new BaseResponse(404, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new BaseResponse(500, $"服务器错误: {ex.Message}"));
        }
    }
    #endregion
}
