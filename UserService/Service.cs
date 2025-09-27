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
        
        redis.DeleteValidCode(req.Email);
        db.CreateUser(req.Email, req.Password);
        return new RegisterResponse();
    }
    
    // 事实上用邮箱充当了 userId，不过我认为无伤大雅
    public LoginResponse Login(LoginRequest req)
    {
        var user = db.GetUserByEmail(req.Email);

        if (user.PasswordHash != Util.HashPassword(req.Password))
        {
            return new LoginResponse(401, "密码错误");
        }

        var token = Util.GenerateToken(req.Email);
        redis.SetToken(req.Email, token, TimeSpan.FromMinutes(5));
        return new LoginResponse(200, "success", token);
    }

    // 给网关用，检测某用户是否已登录
    public bool IsLogin(string userId, string token)
    {
        return redis.Token(userId) == token;
    }
}