using System;
using System.Security.Claims;

namespace ShibbolethLogin
{
    /// <summary>
    /// Interface for shiholeth service configuration
    /// </summary>
    public interface IShibbolethConfig
    {
        /// <summary>
        /// url of login service page on shibboleth id provider, expected format "url?target={0}"
        /// </summary>
        string SSOLoginUrlFormatString { get; set; }
        /// <summary>
        /// url of logout service on shibboleth id provider
        /// </summary>
        string SSOLogoutUrl { get; set; }

        /// <summary>
        /// URL of callback after Login redirection to SSO, expected format: "controller/action", default value = "Account/LoginCallback"
        /// </summary>
        string LoginCalbackAction { get; set; }

        /// <summary>
        /// url to contoller action to be called after login is successful
        /// </summary>
        string AfterLoginPath { get; set; }
        /// <summary>
        /// Identifier whether testing capabilities are enabled, true allows fake login in account controller
        /// </summary>
        bool Testing { get; set; }

        /// <summary>
        /// name of injected header with shibboleth user id
        /// </summary>
        string HeaderRemoteUser { get; set; }

        /// <summary>
        /// default domain to add for users without domain
        /// </summary>
        string DefaultDomain { get; set; }
        /// <summary>
        /// name of used authentication cookie
        /// </summary>
        string CookieName { get; set; }
        /// <summary>
        /// cookie expiration
        /// </summary>
        TimeSpan ExpireTimeSpan { get; set; }
        /// <summary>
        /// path to acces denied handler
        /// </summary>
        string AccessDeniedPath { get; set; }
        /// <summary>
        /// url to application login controller action
        /// </summary>
        string AppLoginUrl { get; set; }

        /// <summary>
        /// url to application logout controller action
        /// </summary>
        string AppLogoutUrl { get; set; }

        string ExternalUserClaimType { get; set; }
    }
}
