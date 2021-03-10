namespace ShibbolethLogin
{
    public class ClaimValuePair
    {
        /// <summary>
        /// Claim value pair which contains claim type and value name in shibboleth or AD headers
        /// </summary>
        /// <param name="claimType">claim type</param>
        /// <param name="valueName">value name in Shibboleth or AD headers</param>
        public ClaimValuePair(string claimType, string valueName)
        {
            ClaimType = claimType;
            ValueName = valueName;
        }

        public string ClaimType { get; set; }
        public string ValueName { get; set; }
    }
}
