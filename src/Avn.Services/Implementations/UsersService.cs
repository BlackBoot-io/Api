#nullable disable
using Avn.Domain.Dtos;
using Avn.Domain.Entities;
using Avn.Domain.Enums;
using Avn.Services.External.Implementations;
using Avn.Services.Interfaces;
using Avn.Shared.Utilities;

namespace Avn.Services.Implementations;

public class UsersService : IUsersService
{
    private readonly DbSet<User> _users;
    private readonly ApplicationDBContext _context;
    private readonly EmailGatewayAdapter _emailGatewayAdapter;


    public UsersService(ApplicationDBContext context)
    {
        _context = context;
        _users = context.Set<User>();
    }

    public async Task<IActionResponse<User>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => new ActionResponse<User>(await _users.FirstOrDefaultAsync(x => x.Email == email.ToLower(), cancellationToken));
    public async Task<IActionResponse<User>> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => new ActionResponse<User>(await _users.FindAsync(new object[] { id }, cancellationToken));
    public async Task<IActionResponse<Guid>> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        #region create random code
        var newCode = RandomStringGenerator.Generate(10);
        user.Code = newCode;
        #endregion

        await _users.AddAsync(user);

        var dbResult = await _context.SaveChangesAsync();
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse<Guid>(ActionResponseStatusCode.ServerError);

        #region send code
        _emailGatewayAdapter.Send(new EmailDto
        {
            Content = newCode,
            Receiver = user.Email,
            Subject = "Recovery Password",
            Template = EmailTemplate.Verification
        });
        #endregion

        return new ActionResponse<Guid>();
    }
    public async Task<IActionResponse<Guid>> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _users.Update(user);

        var dbResult = await _context.SaveChangesAsync();
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse<Guid>(ActionResponseStatusCode.ServerError);

        return new ActionResponse<Guid>();
    }
    public IActionResponse<bool> CheckPassword(User user, string password, CancellationToken cancellationToken = default)
        => new ActionResponse<bool>(HashGenerator.Hash(password) == user.Password);
}