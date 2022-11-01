using System;

namespace ShibbolethLogin
{
    public class ShibbolethConfig : IShibbolethConfig
    {
        //    "Shibboleth": {
        //    "SSOLoginUrlFormatString": "https://shibbolethurl/Login?target={0}",
        // "SSOLoginUrlCallbackFormatString": "https://applicationurl/Account/LoginCallback?returnURl={0}"
        //    "SSOLogoutUrl": "https://shibbolethurl/Logout?return=https://idp.upol.cz/idp/profile/Logout",
        //    "AfterLoginPath": "/",

        //},

        public ShibbolethConfig()
        {
            
        }

        public string SSOLoginUrlFormatString { get; set; }
        public string SSOLogoutUrl { get; set; }
        public string LoginCalbackAction { get; set; } = "Account/LoginCallback";
        public string AfterLoginPath { get; set; }
        public bool Testing { get; set; } = false;
        public bool UseLogger { get; set; } = false;
        public string DefaultDomain { get; set; }
        public string CookieName { get; set; } 
        public TimeSpan ExpireTimeSpan { get; set; }
        public string AccessDeniedPath { get; set; }
        public string HeaderRemoteUser { get; set; } = "remoteuser";
        public string AppLoginUrl { get; set; } = "/Account/Login";
        public string  AppLogoutUrl { get; set; } = "/Account/Logout";
        public string ExternalUserClaimType { get; set; } = "https://upol.cz/ExternalUser";
    }
}
