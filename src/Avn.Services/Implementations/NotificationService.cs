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

    public async Task<IActionResponse> SendAsync(Guid userId, TemplateType template, byte[] file = null)
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
