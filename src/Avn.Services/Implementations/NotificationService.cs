namespace Avn.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly IUsersService _usersService;
    public NotificationService(IUsersService usersService)
    {
        _usersService = usersService;
    }

    public async Task<IActionResponse> SendAsync(Guid userId, TemplateType template, string content = "", byte[] file = null)
    {
        var currentUser = await _usersService.GetCurrentUserAsync(userId);

        throw new NotImplementedException();
    }
}
