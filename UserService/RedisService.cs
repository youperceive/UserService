namespace UserService;

public interface IRedisService
{
    void Save(string key, string value);
    string Load(string key);
    void Delete(string key);
}

public class RedisFaker : IRedisService
{
    private Dictionary<string, string> repo = new Dictionary<string, string>();
    
    public void Save(string key, string value)
    {
        if (!repo.TryAdd(key, value))
        {
            Console.WriteLine(key + "已存在");
            // 后续替换为 error 实现
        }
    }
    
    public string Load(string key)
    {
        if (repo.TryGetValue(key, out var load))
        {
            return load;
        }
        else
        {
            Console.WriteLine(key + "不存在");
            return string.Empty;
        }
    }
    
    public void Delete(string key)
    {
        repo.Remove(key);
    }
}