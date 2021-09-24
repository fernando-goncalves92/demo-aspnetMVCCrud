using Microsoft.AspNetCore.Http;
using System.Linq;

namespace App.UI.Authorize
{
    public static class Authorization
    {
        public static bool CheckUserClaims(HttpContext context, string claimName, string claimValue)
        {
            return context.User.Identity.IsAuthenticated && context.User.Claims.Any(c => c.Type == claimName && c.Value.Contains(claimValue));
        }
    }
}
