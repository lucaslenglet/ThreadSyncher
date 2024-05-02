using System;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadSyncher;

public interface IThreadSyncherContext<T>
{
    string Key { get; }
    bool IsOpen { get; }
    bool IsClosed { get; }

    ValueTask<T> PullAndCloseAsync(CancellationToken cancellationToken = default);
    ValueTask<T> PullAndCloseAsync(TimeSpan timeToCancel);
    ValueTask PushAsync(T value);
    void Close();
}