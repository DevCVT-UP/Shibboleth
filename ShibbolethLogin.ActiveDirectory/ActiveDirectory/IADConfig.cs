using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Text;

namespace ShibbolethLogin
{
    public interface IADConfig
    {
        string Server { get; set; }
        string Container { get; set; }
        string User { get; set; }
        string Password { get; set; }
        PrincipalContext GetContext();
        Principal GetUser(string userLogin);
        IEnumerable<ClaimValuePair> Claims { get; set; }
    }
}
