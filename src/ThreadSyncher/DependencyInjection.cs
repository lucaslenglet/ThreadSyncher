using Microsoft.Extensions.DependencyInjection;

namespace ThreadSyncher;

public static class DependencyInjection
{
    public static IServiceCollection AddThreadSyncher(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IThreadSyncher<>), typeof(ThreadSyncher<>));

        return services;
    }
}