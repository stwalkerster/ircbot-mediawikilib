namespace Stwalkerster.Bot.MediaWikiLib.Services.Interfaces
{
    using System.Collections.Specialized;
    using System.IO;
    using System.Net;

    public interface IWebServiceClient
    {
        Stream DoApiCall(
            NameValueCollection query,
            string endpoint,
            string userAgent,
            CookieContainer cookieJar,
            bool post);

        Stream DoApiCall(NameValueCollection query, string endpoint, string userAgent);
    }
}