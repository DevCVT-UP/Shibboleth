using Microsoft.AspNetCore.Http;
using ShibbolethLogin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShibbolethApp
{
    public class ClaimsConfig : IClaimsConfig
    {
        public void EditClaims(ClaimsIdentity claims, HttpContext context)
        {
            var shibbolethConfig = (IShibbolethConfig)context.RequestServices.GetService(typeof(IShibbolethConfig));
        }
    }
}
