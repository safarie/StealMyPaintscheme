using System.Security.Claims;

namespace StealMyPaintscheme.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int? GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst("userId")?.Value
                    ?? user.FindFirst(c => c.Type.EndsWith("userId"))?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }

    public static bool GetIsAdmin(this ClaimsPrincipal user) =>
        user.HasClaim("isAdmin", "true") ||
        user.HasClaim(c => c.Type.EndsWith("isAdmin") && c.Value.ToLower() == "true");
}
