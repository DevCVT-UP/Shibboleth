using System.Web;
using Microsoft.IdentityModel.Tokens;

namespace ShibbolethLogin
{
    public interface IURLCodec
    {
        string Encode(string url);
        string Decode(string url);
    }

    public class URLCodecHttpUtilityUrl : IURLCodec
    {
        public string Encode(string url) => HttpUtility.UrlEncode(url);

        public string Decode(string url) => HttpUtility.UrlDecode(url);

    }

    public class URLCodecBase64Url : IURLCodec
    {
        public string Encode(string url) => Base64UrlEncoder.Encode(url);

        public string Decode(string url) => Base64UrlEncoder.Decode(url);
    }


}