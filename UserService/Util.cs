using System.Security.Cryptography;
using System.Text;

namespace UserService;

public static class Util
{
    public static string GenerateValidCode()
    {
        return Random.Shared.Next(100000, 999999).ToString();
    }

    public static string HashPassword(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA512.HashData(bytes);
        return Convert.ToHexString(hash).ToLower();
    }

    public static string GenerateToken(string userId)
    {
        var bytes = Encoding.UTF8.GetBytes(userId + DateTime.Now.Ticks);
        var hash = SHA512.HashData(bytes);
        return Convert.ToHexString(hash).ToLower();
    }
}