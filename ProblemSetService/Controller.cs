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
}
