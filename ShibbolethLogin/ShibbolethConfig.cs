using System;
using System.Collections.Generic;
using System.Text;

namespace ShibbolethLogin
{
    public class ShibbolethConfig : IShibbolethConfig
    {
        /// <summary>
        /// Shibboleth configuration
        /// </summary>
        /// <param name="loginUrl">Shibboleth login url</param>
        /// <param name="logoutUrl">Shibboleth log out url</param>
        /// <param name="afterLoginPath">Redirect to this url after login</param>
        /// <param name="claims">List of ClaimValuePairs which are used to create claims from Shibboleth headers by value name</param>
        /// <param name="useAD">Specifies whether to use the Active Directory</param>
        /// <param name="useLogger">Specifies whether to use the Logger to write informations about user logins</param>
        /// <param name="testing">Specifies if Shibboleth is in testing mode and anyone can use fakelogin</param>
        public ShibbolethConfig(string loginUrl, string logoutUrl, string afterLoginPath, string accessDeniedPath, string cookieName, TimeSpan expireTimeSpan, List<ClaimValuePair> claims = null, bool useLogger = false, bool testing = false)
        {
            LoginUrl = loginUrl;
            LogoutUrl = logoutUrl;
            AfterLoginPath = afterLoginPath;
            Claims = claims;
            Testing = testing;
            UseLogger = useLogger;
            AccessDeniedPath = accessDeniedPath;
            CookieName = cookieName;
            ExpireTimeSpan = expireTimeSpan;
        }

        public ShibbolethConfig()
        {
            
        }

        public string LoginUrl { get; set; }
        public string LogoutUrl { get; set; }
        public string AfterLoginPath { get; set; }
        public bool Testing { get; set; } = false;
        public bool UseLogger { get; set; } = false;
        public IEnumerable<ClaimValuePair> Claims { get; set; }
        public string DefaultDomain { get; set; }
        public string CookieName { get; set; }
        public TimeSpan ExpireTimeSpan { get; set; }
        public string AccessDeniedPath { get; set; }
    }
}
