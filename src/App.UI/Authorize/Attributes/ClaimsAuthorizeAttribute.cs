using App.UI.Authorize.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace App.UI.Authorize.Attributes
{
    public class ClaimsAuthorizeAttribute : TypeFilterAttribute
    {
        public ClaimsAuthorizeAttribute(string claimName, string claimValue) : base(typeof(RequestClaimFilter))
        {
            Arguments = new object[] { new Claim(claimName, claimValue) };
        }
    }
}