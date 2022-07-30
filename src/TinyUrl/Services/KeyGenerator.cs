namespace TinyUrl.Services;

public static class KeyGenerator
{
    private static readonly Random _random = new();
    private const string _chars = "abcdefghijklmnopqrstuvwxyz0123456789";

    public static string Generate(int length)
    {
        var key = new char[length];

        for (int i = 0; i < length; i++)
        {
            key[i] = _chars[_random.Next(_chars.Length)];
        }

        return new String(key);
    }
}