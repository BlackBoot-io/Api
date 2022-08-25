using Avn.Domain.Dtos;
using Avn.Shared.Core;
using System.Threading.Tasks;

namespace Avn.Services.External.Interfaces;

public interface IEmailSenderAdapter: IScopedDependency
{
    Task<IActionResponse> Send(EmailRequestDto email);
}
