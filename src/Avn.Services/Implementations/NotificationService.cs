namespace Avn.Services.Implementations;

public class NotificationService : INotificationService
{
    public Task<IActionResponse> SendAsync(TemplateType template, string receiver, string content = "")
    {
        throw new NotImplementedException();
    }
}
