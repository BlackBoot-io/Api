#nullable disable
using Avn.Data.Context;
using Avn.Data.UnitofWork;
using Avn.Domain.Dtos;
using Avn.Domain.Entities;
using Avn.Domain.Enums;
using Avn.Services.External.Implementations;
using Avn.Services.Interfaces;
using Avn.Shared.Utilities;

namespace Avn.Services.Implementations;

public class UsersService : IUsersService
{
    private readonly IAppUnitOfWork _uow;
    private readonly EmailSenderAdapter _emailGatewayAdapter;

    public UsersService(IAppUnitOfWork uow) => _uow = uow;


    public async Task<IActionResponse<User>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => new ActionResponse<User>(await _uow.UserRepo.GetAll().FirstOrDefaultAsync(x => x.Email == email.ToLower(), cancellationToken));

    public async Task<IActionResponse<User>> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => new ActionResponse<User>(await _uow.UserRepo.FindAsync(new object[] { id }, cancellationToken));

    public async Task<IActionResponse<Guid>> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        #region create random code
        var newCode = RandomStringGenerator.Generate(10);
        user.Code = newCode;
        #endregion

        await _uow.UserRepo.AddAsync(user, cancellationToken);

        var dbResult = await _uow.SaveChangesAsync();
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse<Guid>(ActionResponseStatusCode.ServerError);

        #region send code
        await _emailGatewayAdapter.Send(new EmailRequestDto(EmailTemplate.Verification, user.Email, "Recovery Password", newCode));
        #endregion

        return new ActionResponse<Guid>();
    }
    public async Task<IActionResponse<Guid>> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _uow.UserRepo.Update(user);

        var dbResult = await _uow.SaveChangesAsync();
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse<Guid>(ActionResponseStatusCode.ServerError);

        return new ActionResponse<Guid>();
    }
    public IActionResponse<bool> CheckPassword(User user, string password, CancellationToken cancellationToken = default)
        => new ActionResponse<bool>(HashGenerator.Hash(password) == user.Password);
}