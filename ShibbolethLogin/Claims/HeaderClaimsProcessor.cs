using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ShibbolethLogin.Claims
{
    /// <summary>
    /// A IClams Processor implementation to add claims from request headers
    /// </summary>
    public class HeaderClaimsProcessor : IClaimsProcessor
    {

        public HeaderClaimsProcessor(ILogger logger, IEnumerable<ClaimEntry> entries = null)
        {
            Logger = logger;
            Entries = entries;
        }

        /// <summary>
        /// collection of claims to be created from given headers
        /// </summary>
        public IEnumerable<ClaimEntry> Entries { get; set; }
        public ILogger Logger { get; set; }
        public void ProcessClaims(ClaimsIdentity claims, HttpContext context, string user)
        {
            var headers = context?.Request?.Headers;
            if (headers == null) return;

            if (Entries != null)
            {
                try
                {
                    claims.AddClaims(Entries
                        .Where(p => !(string.IsNullOrEmpty(p.ClaimType) || string.IsNullOrEmpty(p.ValueName)))
                        .Select(p => new Claim(p.ClaimType, headers[p.ValueName])).Where(p => !string.IsNullOrEmpty(p.Value)));
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, ex.Message);
                }
            }
        }
    }
}