using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace ShibbolethLogin
{
    public interface IClaimsConfig
    {
        /// <summary>
        /// Method to override, can add claims to signing in user.
        /// </summary>
        /// <param name="claims">claims</param>
        /// <param name="context">context from controller to get service</param>
        void EditClaims(ClaimsIdentity claims, HttpContext context);
    }
}
