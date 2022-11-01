using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ShibbolethLogin.Roles
{
    /// <summary>
    /// implementation of role provider via configuration in a json file
    /// </summary>
    public class JsonConfigRoleResolver : RoleResolverBase
    {
        public JsonConfigRoleResolver(ILogger logger)
        {
            Logger = logger;
        }
        private ILogger Logger { get; set; }
        public string DefaultDomain { get; set; }

        public class JsonRoleLoader
        {

            public JsonRoleLoader(ILogger logger, string defaultDomain)
            {
                Logger = logger;
                DefaultDomain = defaultDomain;
            }
            private ILogger Logger { get; set; }
            public string Path { get; set; }
            public string DefaultDomain { get; set; }

            public IEnumerable<Role> GetRoles()
            {
                try
                {
                    using StreamReader sr = new StreamReader(Path);
                    string json = sr.ReadToEnd();
                    List<Role> roles = JsonConvert.DeserializeObject<List<Role>>(json);

                    foreach (var role in roles)
                    {
                        role.Name = role.Name.ToLowerInvariant();
                        role.Users = role.Users.Select(p => p.CanonizeUserDomainString(DefaultDomain).ToLowerInvariant()).ToList();
                    }
                    return roles;
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex, ex.Message);
                    return Enumerable.Empty<Role>();
                }
            }
        }

        /// <summary>
        /// Serialization object
        /// </summary>
        public class Role
        {
            public string Name { get; set; }
            public List<string> Groups { get; set; } = new List<string>();
            public List<string> Users { get; set; } = new List<string>();

            public bool IsInRole(string userName, IEnumerable<string> userGroups)
            {
                return Users.Contains(userName) || userGroups.Any(userGroup => Groups.Contains(userGroup));
            }
        }


        readonly JsonRoleLoader loader;

        private IEnumerable<Role> roleRules;

        /// <summary>
        /// Config role resolver which get user roles from json file
        /// </summary>
        /// <param name="path">Path to json file with roles</param>
        public JsonConfigRoleResolver(string path, ILogger logger, string defaultDomain = null)
        {
            Logger = logger;
            DefaultDomain = defaultDomain;
            loader = new JsonRoleLoader(Logger, DefaultDomain) { Path = path };
            Reload();
        }
        /// <summary>
        /// refreshes data form the configuration file
        /// </summary>
        public void Reload()
        {
            roleRules = loader.GetRoles().ToArray();
        }
        /// <summary>
        /// Returns user roles for give user name
        /// </summary>
        /// <param name="userName">user name</param>
        /// <param name="userGroups">already obtained user groups, can be empty</param>
        /// <returns></returns>
        public override IEnumerable<string> GetUserRoles(string userName, IEnumerable<string> userGroups)
        {
            userGroups ??= Enumerable.Empty<string>();
            return roleRules.Where(p => IsInRole(userName, p.Name, userGroups)).Select(p => p.Name);
        }

        public IEnumerable<string> Roles
        {
            get { return roleRules.Select(p => p.Name); }
        }

        public override bool IsInRole(string userName, string role, IEnumerable<string> userGroups)
        {
            var found = roleRules.FirstOrDefault(p => p.Name == role);
            if (found == null) return false;
            return found.IsInRole(userName, userGroups);
        }



    }
}
