using System.Text;

namespace SchedulePlannerBack.Util;

public class LinkGenerator
{
    private static string _pool = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private static readonly Random _random = new Random();

    public static string GenerateLink()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 7; i++)
        {
            sb.Append(_pool[_random.Next(0, _pool.Length)]);
        }
        return sb.ToString();
    }
}