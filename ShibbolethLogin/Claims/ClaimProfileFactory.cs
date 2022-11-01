using Microsoft.Extensions.Logging;

namespace ShibbolethLogin.Claims
{
    /// <summary>
    /// A little helper object to simplify default scenario
    /// </summary>
    public static class ClaimProfileFactory
    {
        public static IClaimsProcessor HeadersClaimsProcessor(ILogger logger) => new HeaderClaimsProcessor(logger);
  
        public static ClaimsProfile DefaultProfile(ILogger logger = null, string defaultDomain = null) =>
            new ClaimsProfile()
                {Logger = logger, DefaultDomain = defaultDomain, ClaimsProcessors = new[] {HeadersClaimsProcessor(logger)}};

    }
}