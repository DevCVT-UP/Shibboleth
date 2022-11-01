using Microsoft.AspNetCore.Http;
using ShibbolethLogin;
using System.Security.Claims;
using ShibbolethLogin.Claims;

namespace ShibbolethApp
{


    /// <summary>
    /// Example of a custom claims processor
    /// </summary>
    public class CustomClaimsProcessor : IClaimsProcessor
    {
        public void ProcessClaims(ClaimsIdentity claims, HttpContext context, string user)
        {
            var shibbolethConfig = (IShibbolethConfig)context.RequestServices.GetService(typeof(IShibbolethConfig));


            claims.AddClaim(new Claim(ClaimTypes.Email, "email@example.net"));

            // TODO: some other custom claims processor logic
        }
    }
}
