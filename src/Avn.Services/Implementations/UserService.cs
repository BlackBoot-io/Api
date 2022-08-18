#nullable disable
using Avn.Data.Context;
using Avn.Domain.Entities;
using Avn.Services.Interfaces;

namespace BlackBoot.Services.Implementations;

public class UserService : IUserService
{
    private readonly DbSet<User> _users;
    private readonly ApplicationDBContext _context;

    public UserService(ApplicationDBContext context)
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
        await _users.AddAsync(user);



        var dbResult = await _context.SaveChangesAsync();
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse<Guid>(ActionResponseStatusCode.ServerError);

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