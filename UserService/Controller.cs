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
    
    /// <summary>
    /// 检查用户是否已登录
    /// </summary>
    [HttpGet("is-login")]
    public ActionResult<bool> IsLogin([FromQuery] string userId, [FromQuery] string token)
    {
        try
        {
            var isLoggedIn = userService.IsLogin(userId, token);
            return Ok(isLoggedIn);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new BaseResponse(500, $"服务器错误: {ex.Message}"));
        }
    }
    
    /// <summary>
    /// 检查用户是否拥有比指定角色更高的权限
    /// </summary>
    [HttpGet("has-higher-role")]
    public ActionResult<BaseResponse> HasHigherRoleThan([FromQuery] int id, [FromQuery] string role)
    {
        try
        {
            var hasHigherRole = userService.HasHigherRoleThan(id, role);
            return Ok(hasHigherRole);
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