using Timer = System.Timers.Timer;

namespace TinyUrl.Models;

public class UrlTime
{
    public string Url { get; init; }
    public Timer Timer { get; init; }
    public DateTime Created { get; init; }
}
