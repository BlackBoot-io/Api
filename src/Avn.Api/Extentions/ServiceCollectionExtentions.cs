#nullable disable

using Avn.Data.Repository;
using Avn.Data.UnitofWork;
using Avn.Services.Resources;
using Avn.Shared.Extentions;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Avn.Api.Extentions;

public static class ServiceCollectionExtentions
{
    public static void AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEFSecondLevelCache(options =>
        {
            options.UseMemoryCacheProvider();
            options.CacheAllQueries(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(30));
            options.DisableLogging(true);
            options.UseCacheKeyPrefix("Avn_");
        });

        services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
        services.AddScoped<IAppUnitOfWork, AppUnitOfWork>();
        services.AddDbContext<ApplicationDbContext>((IServiceProvider serviceProvider, DbContextOptionsBuilder options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("ApplicationDbContext"))
                    .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>());
        });
    }
    public static void AddApplicationAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        services.AddAuthorization();
        _ = services.AddAuthentication(options =>
        {
            options.DefaultChallengeScheme = "Bearer";
            options.DefaultSignInScheme = "Bearer";
            options.DefaultAuthenticateScheme = "Bearer";
        })
        .AddJwtBearer(cfg =>
        {
            cfg.RequireHttpsMetadata = false;
            cfg.SaveToken = true;
            cfg.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = jwtSettings.Issuer,
                ValidateIssuer = true,
                ValidAudience = jwtSettings.Audience,
                ValidateAudience = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                TokenDecryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.EncryptionKey))
            };

            static async Task validate(TokenValidatedContext context)
            {

                var token = ((JwtSecurityToken)context.SecurityToken).RawData;
                var userTokenService = context.HttpContext.RequestServices.GetRequiredService<IJwtTokensService>();
                var userId = context?.Principal?.Identity?.GetUserIdAsGuid();
                if (userId is null || userId == Guid.Empty)
                {
                    context?.Fail(BusinessMessage.InvalidUser);
                    return;
                }
                var validate = await userTokenService.VerifyTokenAsync(userId.Value, token, context.HttpContext.RequestAborted);
                if (!validate.Data)
                {
                    context.Fail(BusinessMessage.InvalidUser);
                    return;
                }
                context.HttpContext.User = context.Principal;
            }

            cfg.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    return validate(context);
                }
            };
            cfg.SecurityTokenValidators.Add(new RequireEncryptedTokenHandler());
        });
    }


}