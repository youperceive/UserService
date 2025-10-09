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
}
