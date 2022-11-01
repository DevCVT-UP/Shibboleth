using System.DirectoryServices.AccountManagement;
using Microsoft.Extensions.Logging;

namespace ShibbolethLogin.ActiveDirectory
{
    public interface IADConfig
    {
        string Server { get; set; }
        string Container { get; set; }
        string User { get; set; }
        string Password { get; set; }
        PrincipalContext GetContext();
        Principal GetUser(string userLogin);
     
        ILogger Logger { get; set; }
        public string DefaultDomain { get; set; }
        string GroupMemberAttributeName { get; set; }
        string CommonNameFragmentStart { get; set; }
    }

  
}
