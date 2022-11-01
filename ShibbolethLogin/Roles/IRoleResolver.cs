using System.Collections.Generic;
using ShibbolethLogin.Claims;

namespace ShibbolethLogin.Roles
{
    /// <summary>
    /// abstraction for authorization via accessing user roles form different sources, also inherits from IClaimsProcessor
    /// </summary>
    public interface IRoleResolver: IClaimsProcessor
    {
   /// <summary>
   /// Returns roles for give user
   /// </summary>
   /// <param name="userName">user name</param>
   /// <param name="userGroups">already predefined user groups/roles or empty enum for processing, e.g. from group name translation to another ...</param>
   /// <returns>user role names as string</returns>
        IEnumerable<string> GetUserRoles(string userName, IEnumerable<string> userGroups);
        
   /// <summary>
   /// returns whether user has give role using user name and user groups
   /// </summary>
   /// <param name="userName">user name</param>
   /// <param name="role">role name</param>
   /// <param name="userGroups">enumerable of user groups</param>
   /// <returns></returns>
        bool IsInRole(string userName, string role, IEnumerable<string> userGroups);
    }
}
