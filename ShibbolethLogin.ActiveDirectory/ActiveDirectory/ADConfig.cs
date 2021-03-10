using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Text;

namespace ShibbolethLogin
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
        public ADConfig(string server, string container, string user, string password, IEnumerable<ClaimValuePair> claims = null)
        {
            Server = server;
            Container = container;
            User = user;
            Password = password;
            Claims = claims;
        }

        public ADConfig()
        {
            
        }

        public string Server { get; set; }
        public string Container { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public IEnumerable<ClaimValuePair> Claims { get; set; }

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

        public Principal GetUser(string userLogin)
        {
            try
            {
                return UserPrincipal.FindByIdentity(GetContext(), userLogin);
            }
            catch (Exception ex)
            {
               Trace.TraceError(ex.Message + ex.StackTrace);
               return null;
            }
        }
    }
}
