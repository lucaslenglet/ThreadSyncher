namespace ThreadSyncher;

public interface IThreadSyncher<T>
{
    IThreadSyncherContext<T> GetContext(string key);
    IThreadSyncherContext<T> InitContext(string key, bool force = true);
}