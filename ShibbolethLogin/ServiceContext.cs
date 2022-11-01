using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace ShibbolethLogin
{
    public class ServiceContext : IServiceContext
    {
        public readonly IHttpContextAccessor _httpContextAccessor;
        public ServiceContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserId => GetValueByClaim(ClaimTypes.Name);

        public string UserIdWithDomain => GetValueByClaim(ClaimTypes.NameIdentifier);

        public string IpAddress => _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();

        private string GetValueByClaim(string claimName)
        {
            var value = _httpContextAccessor?.HttpContext.User?.FindFirst(claimName)?.Value;
            if (!string.IsNullOrEmpty(value))
                return value;

            return "";
        }
    }
}
