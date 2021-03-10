using System;
using System.Collections.Generic;
using System.Text;

namespace ShibbolethLogin
{
    public interface IShibbolethConfig
    {
        string LoginUrl { get; set; }
        string LogoutUrl { get; set; }
        string AfterLoginPath { get; set; }
        bool Testing { get; set; }
        bool UseLogger { get; set; }
        IEnumerable<ClaimValuePair> Claims { get; set; }
        string DefaultDomain { get; set; }
        string CookieName { get; set; }
        TimeSpan ExpireTimeSpan { get; set; }
        string AccessDeniedPath { get; set; }
    }
}
