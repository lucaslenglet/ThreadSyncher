using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ThreadSyncher;

internal class ThreadSyncherContext<T>(string key, ThreadSyncher<T> threadSyncher) : IThreadSyncherContext<T>
{
    private readonly Channel<T> _channel = Channel.CreateBounded<T>(new BoundedChannelOptions(1)
    {
        SingleReader = true,
        SingleWriter = true,
    });
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ThreadSyncher<T> _threadSyncher = threadSyncher;

    public string Key { get; } = key;
    public bool IsOpen { get; private set; }
    public bool IsClosed => !IsOpen;

    public async ValueTask PushAsync(T v)
    {
        await _channel.Writer.WriteAsync(v, _cancellationTokenSource.Token);
    }

    public async ValueTask<T> PullAndCloseAsync(CancellationToken cancellationToken = default)
    {
        var ct = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);
        try
        {
            return await _channel.Reader.ReadAsync(ct.Token);
        }
        finally
        {
            Close();
        }
    }

    public ValueTask<T> PullAndCloseAsync(TimeSpan timeToCancel)
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(timeToCancel);
        return PullAndCloseAsync(cts.Token);
    }

    public void Close()
    {
        IsOpen = false;
        _ = _channel?.Writer.TryComplete();
        if (_cancellationTokenSource is not { IsCancellationRequested: true })
        {
            _cancellationTokenSource.Cancel();
        }
        _threadSyncher.RemoveContext(Key);
    }
}