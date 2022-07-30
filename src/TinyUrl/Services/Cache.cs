using System.Collections.Concurrent;
using TinyUrl.Models;
using Timer = System.Timers.Timer;

namespace TinyUrl.Services;

public static class Cache
{
    // The default dictionary class is not thread safe
    private static readonly ConcurrentDictionary<string, UrlTime> _cache = new();
    private static long _numBytes;

    public static string Get(string key)
    {
        ResetTimer(key);
        return _cache[key].Url;
    }
    
    public static void Set(string key, string url)
    {
        if (_cache.ContainsKey(key))
        {
            ResetTimer(key);
            return;
        }

        _numBytes += url.Length; // Assuming 1 char in a string = 1 byte

        while (_numBytes > 1073741824) // Limit memory usage to 1 GB worth of URLs
        {
            RemoveTheOldest();
        }
            
        _cache[key] = new UrlTime
        {
            Url = url,
            Timer = CreateDeletionTimer(key),
            Created = DateTime.Now
        };
    }

    public static bool Contains(string key) => _cache.ContainsKey(key);

    private static void RemoveTheOldest()
    {
        var oldest = _cache.FirstOrDefault();

        foreach (var keyTimer in _cache)
        {
            if (keyTimer.Value.Created > oldest.Value.Created)
            {
                oldest = keyTimer;
            }
        }

        Remove(oldest.Value.Url); 
        _numBytes -= oldest.Value.Url.Length;
    }

    private static Timer CreateDeletionTimer(string key)
    {
        var timer = new Timer(TimeSpan.FromHours(1).TotalMilliseconds);
        
        timer.Elapsed += (source, e) => { _ = Remove(key); };
        timer.AutoReset = false;
        timer.Enabled = true;

        return timer;
    }

    private static void ResetTimer(string key)
    {
        _cache[key].Timer.Stop();
        _cache[key].Timer.Start();
    }

    private static Task Remove(string key) => Task.Run(() => KeepTryingToRemove(key));

    private static void KeepTryingToRemove(string key)
    {
        // Keep trying to remove it until it works
        while (!_cache.TryRemove(key, out UrlTime val))
        {
        }
    }
}
