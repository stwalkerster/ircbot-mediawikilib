namespace Stwalkerster.Bot.MediaWikiLib.Services.Interfaces
{
    using System.Collections.Specialized;
    using System.IO;

    public interface IWebServiceClient
    {
        Stream DoApiCall(NameValueCollection query);
    }
}