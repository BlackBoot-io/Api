using System.Threading.Tasks;

namespace Avn.Services.External.Interfaces;

public interface IEmailSenderAdapter : IScopedDependency
{
    /// <summary>
    /// Send a new email to receiver
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    Task<IActionResponse> Send(EmailRequestDto email);
}
