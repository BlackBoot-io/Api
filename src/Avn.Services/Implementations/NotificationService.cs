namespace Avn.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly IUsersService _usersService;
    private readonly IEmailSenderAdapter _emailSenderAdapter;
    public NotificationService(IUsersService usersService, IEmailSenderAdapter emailSenderAdapter)
    {
        _usersService = usersService;
        _emailSenderAdapter = emailSenderAdapter;
    }

    /// <summary>
    /// send a notification to user such as sms or email or etc.
    /// </summary>
    /// <param name="template"></param>
    /// <param name="receiver"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public async Task<IActionResponse> SendAsync(Guid userId, Dictionary<string, string> extraData, TemplateType template, byte[] file = null)
    {
        var currentUser = await _usersService.GetCurrentUserAsync(userId);
        //fetch email Template then replace data then send email
        var content = "Hi";
        return await _emailSenderAdapter.SendAsync(new EmailRequestDto
           (
               Receiver: currentUser.Data.Email,
               Subject: template.ToString(),
               Content: content,
               File: file
           ));
    }
}
