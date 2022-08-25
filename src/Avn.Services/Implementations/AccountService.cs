﻿using Avn.Domain.Dtos;
using Avn.Domain.Dtos.Users;
using Avn.Domain.Enums;
using Avn.Services.Interfaces;
using Avn.Shared.Utilities;
using Avn.Services.External.Implementations;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Avn.Services.External.Interfaces;
using Avn.Domain.Entities;

namespace Avn.Services.Implementations;

public class AccountService : IAccountService
{

    private readonly IUsersService _userService;
    private readonly IUserJwtTokensService _userTokenService;
    private readonly IJwtTokensFactory _tokenFactoryService;
    private readonly IConfiguration _configuration;
    private readonly IEmailSenderAdapter _emailGatewayAdapter;

    public AccountService(IUsersService userService,
                          IUserJwtTokensService userTokenService,
                          IJwtTokensFactory tokenFactoryService,
                          IConfiguration configuration)
    {
        _userService = userService;
        _userTokenService = userTokenService;
        _tokenFactoryService = tokenFactoryService;
        _configuration = configuration;
    }

    public async Task<IActionResponse<UserTokenDto>> LoginAsync(UserLoginDto item, CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetByEmailAsync(item.Email, cancellationToken);
        if (user.Data is null) return new ActionResponse<UserTokenDto>(ActionResponseStatusCode.NotFound, AppResource.InvalidUser);

        var checkPasswordResult = _userService.CheckPassword(user.Data, item.Password, cancellationToken);
        if (!checkPasswordResult.Data) return new ActionResponse<UserTokenDto>(ActionResponseStatusCode.NotFound, AppResource.InvalidUser);

        var usertokens = await GenerateTokenAsync(user.Data.UserId, cancellationToken);
        await _userTokenService.AddUserTokenAsync(user.Data.UserId, usertokens.AccessToken, usertokens.RefreshToken, cancellationToken);
        return new ActionResponse<UserTokenDto>(usertokens);
    }
    public async Task<IActionResponse> LogoutAsync(Guid userId, string refreshtoken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(refreshtoken))
            return new ActionResponse(ActionResponseStatusCode.NotFound, AppResource.InvalidUser);

        await _userTokenService.RevokeUserTokensAsync(userId, refreshtoken, cancellationToken);
        return new ActionResponse();
    }
    public async Task<IActionResponse<UserTokenDto>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var refreshTokenModel = await _userTokenService.GetRefreshTokenAsync(refreshToken, cancellationToken);
        if (refreshTokenModel.Data is null)
            return new ActionResponse<UserTokenDto>(ActionResponseStatusCode.NotFound, AppResource.InvalidUser);

        var usertokens = await GenerateTokenAsync(refreshTokenModel.Data.UserId.Value, cancellationToken);
        await _userTokenService.AddUserTokenAsync(refreshTokenModel.Data.UserId.Value, usertokens.AccessToken, usertokens.RefreshToken, cancellationToken);
        return new ActionResponse<UserTokenDto>(usertokens);
    }
    public async Task<IActionResponse<UserDto>> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetAsync(userId, cancellationToken);
        return new ActionResponse<UserDto>(new UserDto
        {
            Email = user.Data.Email,
            FullName = user.Data.FullName,
            UserType = user.Data.UserType,
            OrganizationName = user.Data.OrganizationName,
            WalletAddress = user.Data.WalletAdress,
            UserId = user.Data.UserId,
            EmailIsApproved=user.Data.EmailIsApproved,
            IsActive= user.Data.IsActive
        });
    }
    private async Task<UserTokenDto> GenerateTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userService.GetAsync(userId, cancellationToken);
        var accessToken = _tokenFactoryService.CreateToken(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Data.UserId.ToString()),
                new Claim(ClaimTypes.Email,user.Data.Email)
            }, JwtTokenType.AccessToken);

        var refreshToken = _tokenFactoryService.CreateToken(new List<Claim>
            {
               new Claim("AccessToken",accessToken.Data.Token)

            }, JwtTokenType.RefreshToken);
        UserTokenDto result = new()
        {
            AccessToken = accessToken.Data.Token,
            AccessTokenExpireTime = DateTimeOffset.UtcNow.AddMinutes(accessToken.Data.TokenExpirationMinutes),
            RefreshToken = refreshToken.Data.Token,
            RefreshTokenExpireTime = DateTimeOffset.UtcNow.AddMinutes(refreshToken.Data.TokenExpirationMinutes),
            User = new UserDto
            {
                Email = user.Data.Email,
                FullName = user.Data.FullName,
                UserType = user.Data.UserType,
                OrganizationName = user.Data.OrganizationName,
                WalletAddress = user.Data.WalletAdress,
                UserId = user.Data.UserId,
                EmailIsApproved = user.Data.EmailIsApproved,
                IsActive = user.Data.IsActive
            }
        };

        return result;
    }
    public async Task<IActionResponse<UserTokenDto>> SignupAsync(User user, CancellationToken cancellationToken = default)
    {
        user.Password = HashGenerator.Hash(user.Password);
        var addedUser = await _userService.AddAsync(user, cancellationToken);

        #region create random code
        var newCode = RandomStringGenerator.Generate(10);
        user.Code = newCode;
        #endregion

        if (!addedUser.IsSuccess)
            return new ActionResponse<UserTokenDto>(ActionResponseStatusCode.BadRequest, AppResource.TransactionFailed);
        var usertokens = await GenerateTokenAsync(user.UserId, cancellationToken);
        await _userTokenService.AddUserTokenAsync(user.UserId, usertokens.AccessToken, usertokens.RefreshToken, cancellationToken);

        #region send code
        await _emailGatewayAdapter.Send(new EmailRequestDto(EmailTemplate.Verification, user.Email, "Recovery Password", newCode));
        #endregion

        return new ActionResponse<UserTokenDto>(usertokens);
    }
    public async Task<IActionResponse<bool>> VerifyAsync(Guid userId, string code, CancellationToken cancellationToken = default)
    {
        #region get user
        var foundUserResponse = await _userService.GetAsync(userId);
        var foundUser = foundUserResponse.Data;
        if (foundUser == null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound);
        #endregion

        #region check if the user sent the same code, then make IsVerified true
        if (foundUser.Code != code)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound);

        foundUser.EmailIsApproved = true;

        var dbResult = await _userService.UpdateAsync(foundUser, cancellationToken);

        if (!dbResult.IsSuccess)
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError);
        #endregion

        return new ActionResponse<bool>();
    }
    public async Task<IActionResponse<bool>> ResendOtpAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        #region get user
        var foundUserResponse = await _userService.GetAsync(userId);
        var foundUser = foundUserResponse.Data;
        if (foundUser == null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound);
        #endregion

        #region generate random code for user and save it
        var newCode = RandomStringGenerator.Generate(10);

        foundUser.Code = newCode;
        var dbResult = await _userService.UpdateAsync(foundUser, cancellationToken);

        if (!dbResult.IsSuccess)
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError);
        #endregion

        #region send the generated code to the user via email
        await _emailGatewayAdapter.Send(new EmailRequestDto(EmailTemplate.Verification, foundUser.Email, "Recovery Password", newCode));

        #endregion

        return new ActionResponse<bool>();
    }
    public async Task<IActionResponse<bool>> UpdateEmailAsync(Guid userId, string email, CancellationToken cancellationToken = default)
    {
        #region validate email
        if (!Regex.Match(email, @"^[\w -\.] +@([\w -] +\.)+[\w -]{ 2,4}$").Success)
            return new ActionResponse<bool>(ActionResponseStatusCode.BadRequest);
        #endregion

        #region get user
        var foundUserResponse = await _userService.GetAsync(userId);
        var foundUser = foundUserResponse.Data;
        if (foundUser == null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound);
        #endregion

        #region Update Email field
        foundUser.Email = email;

        var dbResult = await _userService.UpdateAsync(foundUser, cancellationToken);

        if (!dbResult.IsSuccess)
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError);
        #endregion

        return new ActionResponse<bool>();
    }
}