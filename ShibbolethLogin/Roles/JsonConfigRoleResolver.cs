using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ShibbolethLogin
{
    public class JsonConfigRoleResolver : IRoleResolver
    {
        public class JsonRoleConfig
        {

            public string Path { get; set; }

            public List<Role> GetRoles()
            {
                using (StreamReader sr = new StreamReader(Path))
                {
                    string json = sr.ReadToEnd();
                    List<Role> roles = JsonConvert.DeserializeObject<List<Role>>(json);
                    return roles;
                }
            }
        }

        public class Role
        {
            public string Name { get; set; }
            public List<string> ADGroups { get; set; }
            public List<string> Users { get; set; }

            public bool IsInRole(string userName, IEnumerable<string> userGroups)
            {
                return Users.Contains(userName) || userGroups.Any(userGroup => ADGroups.Contains(userGroup));
            }
        }


        readonly JsonRoleConfig config;

        private List<Role> roleRules;

        /// <summary>
        /// Config role resolver which get user roles from json file
        /// </summary>
        /// <param name="path">Path to json file with roles</param>
        public JsonConfigRoleResolver(string path)
        {
            config = new JsonRoleConfig() { Path = path };
            Reload();
        }

        public void Reload()
        {
            roleRules = config.GetRoles();
        }

        public IEnumerable<string> GetUserRoles(string userName, IEnumerable<string> userGroups, ClaimsIdentity claims = null)
        {
            return roleRules.Where(p => IsInRole(userName, p.Name, userGroups)).Select(p => p.Name);
        }

        public IEnumerable<string> Roles
        {
            get { return roleRules.Select(p => p.Name); }
        }

        public bool IsInRole(string userName, string role, IEnumerable<string> userGroups)
        {
            var found = roleRules.FirstOrDefault(p => p.Name == role);
            if (found == null) return false;
            return found.IsInRole(userName, userGroups);
        }
    }
}
