using System.Security.Claims;

namespace IotSmartHome.Extensions;

public static class UserExtensions
{
    public static int GetUserId(this HttpContext httpContext)
    {
        var stringUserId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(stringUserId))
        {
            throw new ArgumentException("User not logged in", nameof(httpContext));
        }
        
        if (!int.TryParse(stringUserId, out var userId))
        {
            throw new ArgumentException("Invalid user id", nameof(httpContext));
        }
        
        return userId;
    }
}