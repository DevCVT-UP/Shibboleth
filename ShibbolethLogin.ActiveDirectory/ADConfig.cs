using System;
using System.DirectoryServices.AccountManagement;
using Microsoft.Extensions.Logging;

namespace ShibbolethLogin.ActiveDirectory
{
    public class ADConfig : IADConfig
    {
        /// <summary>
        /// AD configuration
        /// </summary>
        /// <param name="server">AD server</param>
        /// <param name="container">AD container</param>
        /// <param name="user">AD user credentials</param>
        /// <param name="password">AD password credentials</param>
        /// <param name="claims">List of ClaimValuePairs which are used to create claims from AD headers by value name</param>
        public ADConfig(string server, string container, string user, string password,string defaultDomain, ILogger logger):this(logger)
        {
            Server = server;
            Container = container;
            User = user;
            Password = password;
            
            DefaultDomain = defaultDomain;
        }

        public ADConfig(ILogger logger)
        {
            Logger = logger;
        }
        public ILogger Logger { get; set; }
        public string Server { get; set; }
        public string Container { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string DefaultDomain { get; set; }
        public string GroupMemberAttributeName { get; set; } = "memberof";
        public string CommonNameFragmentStart { get; set; } = "CN=";

        public PrincipalContext GetContext()
        {
            return new PrincipalContext(
                ContextType.Domain,
                Server,
                Container,
                User,
                Password
            );
        }

        /// <summary>
        /// gets user principal from AD
        /// </summary>
        /// <param name="userLogin">user login</param>
        /// <returns></returns>
        public Principal GetUser(string userLogin)
        {
            try
            {
                return UserPrincipal.FindByIdentity(GetContext(), userLogin);
            }
            catch (Exception ex)
            {
            
               Logger?.LogError(ex,ex.Message);
               return null;
            }
        }
    }
}
