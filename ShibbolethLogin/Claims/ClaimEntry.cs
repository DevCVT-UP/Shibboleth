namespace ShibbolethLogin.Claims
{
    public class ClaimEntry
    {
        /// <summary>
        /// Claim value pair which contains claim type and value name for configuration IClaimsProcessor. Decribes which underlaying names gave to be mapped to claims of which type. 
        /// </summary>
        /// <param name="claimType">claim type</param>
        /// <param name="valueName">value name in Shibboleth or AD headers</param>
        public ClaimEntry(string claimType, string valueName)
        {
            ClaimType = claimType;
            ValueName = valueName;
        }

        /// <summary>
        /// type identified
        /// </summary>
        public string ClaimType { get; set; }
        /// <summary>
        /// value name
        /// </summary>
        public string ValueName { get; set; }
    }
}
