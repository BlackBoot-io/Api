using Avn.Shared.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Avn.Services.External.Implementations;

public interface INftStorageAdaptar: IScopedDependency
{
    Task<object> Upload(object item, CancellationToken cancellationToken = default);
}
