namespace ShibbolethLogin
{
    public static class StringUserUtils
    {

        /// <summary>
        /// splits user name into user and domain part, if exists
        /// </summary>
        /// <param name="user">user name</param>
        /// <returns>(user, domain)</returns>
        public static (string username, string domain) UserDomain(this string user)
        {
            if (string.IsNullOrEmpty(user)) return (string.Empty, string.Empty);
            var index = user.IndexOf('@');
            if (index < 0) return (user, string.Empty);
            return (user.Substring(0, index), user.Substring(index + 1));
        }

        /// <summary>
        /// splits user name into user and domain parts, sets domain to default domain if domain part is missing
        /// </summary>
        /// <param name="user">user name </param>
        /// <param name="defaultdomain">default domain to replace missing domain </param>
        /// <returns></returns>
        public static (string username, string domain) CanonizeUserDomainPair(this string user, string defaultdomain = null)
        {
            (string name, string domain) u = user.UserDomain();
            if (!string.IsNullOrEmpty(u.domain)) return u;
            return (u.name, defaultdomain);
        }
        /// <summary>
        /// returns user in format user@domain, replacing missing domain part with default domain if missing 
        /// </summary>
        /// <param name="user"> user name in (user) or (user@somedomain) forms</param>
        /// <param name="defaultdomain">dafault domain to replace missing domain</param>
        /// <returns>return string in user@somedomain form or user@defaultdomain if domain is missing</returns>
        public static string CanonizeUserDomainString(this string user, string defaultdomain = null)
        {
            (string name,string domain) u = user.UserDomain();
            if (!string.IsNullOrEmpty(u.domain)) return user;
            return $"{u.name}@{defaultdomain}";
        }

    }
}