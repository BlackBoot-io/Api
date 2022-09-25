#nullable disable

using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;

namespace Avn.Shared.Extentions;

public static class IdentityExtensions
{
    public static string FindFirstValue(this ClaimsIdentity identity, string claimType) => identity?.FindFirst(claimType)?.Value;

    public static string FindFirstValue(this IIdentity identity, string claimType)
    {
        var claimsIdentity = identity as ClaimsIdentity;
        return claimsIdentity?.FindFirstValue(claimType);
    }

    public static string GetUserId(this IIdentity identity) => identity?.FindFirstValue(ClaimTypes.NameIdentifier);

    public static Guid? GetUserIdAsGuid(this IIdentity identity)
    {
        _ = Guid.TryParse(identity?.FindFirstValue(ClaimTypes.NameIdentifier), out var id);
        return id == new Guid() ? null : id;
    }

    public static T GetUserId<T>(this IIdentity identity) where T : IConvertible
    {
        var userId = identity?.GetUserId();
        return userId.HasValue()
            ? (T)Convert.ChangeType(userId, typeof(T), CultureInfo.InvariantCulture)
            : default;
    }
    public static string GetUserName(this IIdentity identity) => identity.FindFirstValue(ClaimTypes.Name);
}