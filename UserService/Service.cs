using ModelLibrary.api;

namespace UserService;

public class UserService(IRedisService redis, UserRepository db)
{
    private readonly IRedisService _redis = redis;
    private readonly UserRepository _db = db;

    public ValidCodeResponse ValidCode(ValidCodeRequest req)
    {
        var validCode = Util.GenerateValidCode();
        _redis.Save(req.Email, validCode);
        return new ValidCodeResponse(validCode);
    }
    
    public RegisterResponse Register(RegisterRequest req)
    {
        var code = _redis.Load(req.Email);
        if (code != req.ValidCode)
        {
            return new RegisterResponse(401, "验证码不正确或不存在");
        }
        
        _redis.Delete(req.Email);
        _db.CreateUser(req.Email, req.Password);
        return new RegisterResponse();
    }
    
    public LoginResponse Login(LoginRequest req)
    {
        var user = _db.GetUserByEmail(req.Email);

        return user.PasswordHash != Util.HashPassword(req.Password) ? 
            new LoginResponse(401, "密码错误") : new LoginResponse();
    }
}