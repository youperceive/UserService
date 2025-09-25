using ModelLibrary.api;

namespace UserService;

public class UserService(IRedisService redis, IUserRepository db)
{
    public ValidCodeResponse ValidCode(ValidCodeRequest req)
    {
        var validCode = Util.GenerateValidCode();
        redis.SetValidCode(req.Email, validCode, TimeSpan.FromMinutes(5));
        return new ValidCodeResponse(validCode);
    }
    
    public RegisterResponse Register(RegisterRequest req)
    {
        var code = redis.ValidCode(req.Email);
        if (code != req.ValidCode)
        {
            return new RegisterResponse(401, "验证码不正确或不存在");
        }
        
        redis.Delete(req.Email);
        db.CreateUser(req.Email, req.Password);
        return new RegisterResponse();
    }
    
    public LoginResponse Login(LoginRequest req)
    {
        var user = db.GetUserByEmail(req.Email);

        return user.PasswordHash != Util.HashPassword(req.Password) ? 
            new LoginResponse(401, "密码错误") : new LoginResponse();
    }
}