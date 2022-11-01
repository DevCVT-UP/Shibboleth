using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ShibbolethLogin.Claims
{
    public class ConstClaimsProcessor : IClaimsProcessor
    {

        public ConstClaimsProcessor() { }

        public ConstClaimsProcessor(string key,string value=null)
        {
            Values = new[] {new KeyValuePair<string, string>(key, value ?? string.Empty)};
        }

        public ConstClaimsProcessor(IEnumerable<KeyValuePair<string, string>> values)
        {
            Values = values ?? Enumerable.Empty<KeyValuePair<string, string>>();
        }

        public IEnumerable<KeyValuePair<string, string>> Values { get; set; } = Enumerable.Empty<KeyValuePair<string, string>>();

        public void ProcessClaims(ClaimsIdentity claims, HttpContext context, string user)
        {
            claims.AddClaims(Values?.Select(p=>new Claim(p.Key,p.Value)) );
        }
    }
}