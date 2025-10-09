using StackExchange.Redis;

namespace UserService;

public interface IRedisService
{
    void SetValidCode(string email, string code, TimeSpan expiry);
    string ValidCode(string email);
    
    void DeleteValidCode(string email);
    void SetToken(string userId, string token, TimeSpan expiry);
    string Token(string userId);
    
    void DeleteToken(string userId);
    
    void Set(string pre, string key, string value, TimeSpan expiry);
    string Value(string pre, string key);
    void Delete(string pre, string key);
}

public class RedisService : IRedisService
{
    private readonly IDatabase _db;

    public RedisService(string connectionString)
    {
        var redis =
            // 创建 Redis 连接
            ConnectionMultiplexer.Connect(connectionString);
        _db = redis.GetDatabase(); 
    }

    public void SetValidCode(string email, string code, TimeSpan expiry)
    {
        _db.StringSet("valid_code:" + email, code, expiry);
    }

    public string ValidCode(string email)
    {
        var res = _db.StringGet("valid_code:" + email).ToString(); 
        return res;
    }

    public void SetToken(string userId, string token, TimeSpan expiry)
    {
        _db.StringSet("token:" + userId, token, expiry);
    }

    public string Token(string userId)
    {
        var res =  _db.StringGet("token:" + userId).ToString();
        return res;
    }

    public void DeleteValidCode(string email)
    {
        _db.KeyDelete("valid_code:" + email);
    }

    public void DeleteToken(string userId)
    {
        _db.KeyDelete("token:" + userId);
    }

    public void Set(string pre, string key, string value, TimeSpan expiry)
    {
        _db.StringSet(pre + key, value, expiry);
    }

    public string Value(string pre, string key)
    {
        return _db.StringGet(pre + key).ToString();
    }

    public void Delete(string pre, string key)
    {
        _db.KeyDelete(pre + key);
    }
}

