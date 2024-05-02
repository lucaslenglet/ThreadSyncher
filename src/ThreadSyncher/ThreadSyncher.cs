using System;
using System.Collections.Concurrent;

namespace ThreadSyncher;

internal class ThreadSyncher<T> : IThreadSyncher<T>
{
    private readonly ConcurrentDictionary<string, IThreadSyncherContext<T>> _contexts = [];

    public IThreadSyncherContext<T> InitContext(string key, bool force = true)
    {
        var ctx = new ThreadSyncherContext<T>(key, this);

        if (force && _contexts.TryGetValue(key, out var toBeReplaced))
        {
            toBeReplaced.Close();
        }

        if (_contexts.TryAdd(key, ctx))
        {
            return ctx;
        }

        throw new InvalidOperationException("Thread syncher has already been initialized.");
    }

    public IThreadSyncherContext<T> GetContext(string key)
    {
        if (_contexts.TryGetValue(key, out var ctx))
        {
            return ctx;
        }

        throw new InvalidOperationException($"No thread syncher initialized for key '{key}'.");
    }

    internal void RemoveContext(string key)
    {
        _ = _contexts.TryRemove(key, out _);
    }
}
