using Microsoft.AspNetCore.Mvc;
using ModelLibrary.api;

namespace UserService;

[ApiController]
[Route("api/[controller]")]
public class UserController(UserService userService) : ControllerBase
{
    /// <summary>
    /// 获取验证码
    /// </summary>
    [HttpPost("valid-code")]
    public ActionResult<ValidCodeResponse> GetValidCode([FromBody] ValidCodeRequest request)
    {
        try
        {
            var response = userService.ValidCode(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new BaseResponse(500, $"服务器错误: {ex.Message}"));
        }
    }
    
    /// <summary>
    /// 用户注册
    /// </summary>
    [HttpPost("register")]
    public ActionResult<RegisterResponse> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var response = userService.Register(request);
            
            return response.StatusCode != 200 ? 
                StatusCode(response.StatusCode, response) : Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new BaseResponse(500, $"服务器错误: {ex.Message}"));
        }
    }
    
    /// <summary>
    /// 用户登录
    /// </summary>
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = userService.Login(request);
            
            return response.StatusCode != 200 ? 
                StatusCode(response.StatusCode, response) : Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new BaseResponse(500, $"服务器错误: {ex.Message}"));
        }
    }
}