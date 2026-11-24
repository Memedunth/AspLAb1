using System.Collections.Concurrent;

namespace AspLAb1.Middleware;

/// <summary>
/// Thread-safe singleton tracking total and per-path HTTP request counts.
/// </summary>
public class RequestStats
{
    private long _total;
    private readonly ConcurrentDictionary<string, long> _perPath = new(StringComparer.OrdinalIgnoreCase);

    public long Total => Interlocked.Read(ref _total);

    public long Hit() => Interlocked.Increment(ref _total);

    public long Hit(string path)
    {
        var key = string.IsNullOrEmpty(path) ? "/" : path;
        _perPath.AddOrUpdate(key, 1, (_, current) => current + 1);
        return Interlocked.Increment(ref _total);
    }

    public IReadOnlyList<KeyValuePair<string, long>> TopPaths(int count = 5) =>
        _perPath
            .OrderByDescending(kv => kv.Value)
            .ThenBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase)
            .Take(count)
            .ToList();
}
