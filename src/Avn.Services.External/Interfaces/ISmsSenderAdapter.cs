using System.Threading.Tasks;

namespace Avn.Services.External.Interfaces;

public interface ISmsSenderAdapter : IScopedDependency
{
    Task<IActionResponse> Send(object email);
}
