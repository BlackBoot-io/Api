using Microsoft.Extensions.DependencyInjection;

namespace Avn.Shared.Core.Services;
public class Lazier<T> : Lazy<T> where T : class
{
    public Lazier(IServiceProvider provider)
        : base(() => provider.GetRequiredService<T>())
    {
    }
}