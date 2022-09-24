using Avn.Domain.Enums;

namespace Avn.Api.Controllers;

public class VerificationController : BaseController
{
    private readonly IVerificationService _verificationService;
    public VerificationController(IVerificationService verificationService)
        => _verificationService = verificationService;

    [HttpPost]
    public async Task<IActionResult> ResendCode()
     => Ok(await _verificationService.SendOtpAsync(CurrentUserId, TemplateType.ResendCode));
}
