// Models/UserModels.cs
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ModelLibrary.api
{
    // 基础响应模型
    public class BaseResponse
    {
        [JsonPropertyName("code")]
        public int StatusCode { get; set; } = 200;

        [JsonPropertyName("message")]
        public string Message { get; set; } = "success";

        public BaseResponse() { }

        public BaseResponse(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }
    }

    // 注册请求模型
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("valid_code")]
        public string ValidCode { get; set; } = string.Empty;

        public RegisterRequest() { }

        public RegisterRequest(string email, string password, string validCode)
        {
            Email = email;
            Password = password;
            ValidCode = validCode;
        }
    }

    // 注册响应模型
    public class RegisterResponse : BaseResponse
    {
        public RegisterResponse() { }

        public RegisterResponse(int statusCode, string message) : base(statusCode, message) { }
    }

    // 验证码请求模型
    public class ValidCodeRequest
    {
        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        public ValidCodeRequest() { }

        public ValidCodeRequest(string email)
        {
            Email = email;
        }
    }

    // 验证码响应模型
    public class ValidCodeResponse : BaseResponse
    {
        [JsonPropertyName("valid_code")]
        public string ValidCode { get; set; } = string.Empty;

        public ValidCodeResponse() { }

        public ValidCodeResponse(string validCode)
        {
            ValidCode = validCode;
        }

        public ValidCodeResponse(int statusCode, string message, string validCode) : base(statusCode, message)
        {
            ValidCode = validCode;
        }
    }

    // 登录请求模型
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        public LoginRequest() { }

        public LoginRequest(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }

    // 登录响应模型
    public class LoginResponse : BaseResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        public LoginResponse() { }

        public LoginResponse(string token)
        {
            Token = token;
        }

        public LoginResponse(int statusCode, string message, string token) : base(statusCode, message)
        {
            Token = token;
        }

        public LoginResponse(int statusCode, string message) : base(statusCode, message) { }
    }

    // 空服务类
    public class Service
    {
        // 这是一个空类，根据您的需要实现业务逻辑
    }
}