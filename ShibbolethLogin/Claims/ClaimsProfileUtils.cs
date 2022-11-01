using System.Linq;

namespace ShibbolethLogin.Claims
{
    /// <summary>
    /// utility class for claims profile
    /// </summary>
    public static class ClaimsProfileUtils
    {
        /// <summary>
        /// Adds claim processors to a profile. Fluent
        /// </summary>
        /// <param name="profile">claims profile object</param>
        /// <param name="procs">params of processors to add</param>
        /// <returns>claims profile for fluency</returns>
        public static ClaimsProfile AddProcessors(this ClaimsProfile profile,params IClaimsProcessor[] procs)
        {
            var lst = profile.ClaimsProcessors.ToList();
            lst.AddRange(procs);
            profile.ClaimsProcessors = lst.ToArray();
            return profile;
        }

    }
}