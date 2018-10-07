namespace Stwalkerster.Bot.MediaWikiLib.Services
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Castle.Core.Logging;
    using Stwalkerster.Bot.MediaWikiLib.Configuration;
    using Stwalkerster.Bot.MediaWikiLib.Services.Interfaces;

    public class WebServiceClient : IWebServiceClient
    {
        private readonly string endpoint;
        private readonly string userAgent;
        private readonly ILogger logger;

        private readonly object lockObject = new object();

        public WebServiceClient(IMediaWikiConfiguration appConfiguration, ILogger logger)
        {
            this.logger = logger;
            this.endpoint = appConfiguration.MediaWikiApiEndpoint;
            this.userAgent = appConfiguration.UserAgent;
        }

        public Stream DoApiCall(NameValueCollection query)
        {
            query.Set("format", "xml");

            var queryFragment = string.Join("&", query.AllKeys.Select(a => a + "=" + WebUtility.UrlEncode(query[a])));
            
            var url = string.Format("{0}?{1}", this.endpoint, queryFragment);
            
            this.logger.DebugFormat("Requesting {0}", url);
            
            var hwr = (HttpWebRequest)WebRequest.Create(url);
            hwr.UserAgent = this.userAgent;
            
            var memoryStream = new MemoryStream();

            lock (this.lockObject)
            {
                using (var resp = (HttpWebResponse) hwr.GetResponse())
                {
                    var responseStream = resp.GetResponseStream();

                    if (responseStream == null)
                    {
                        throw new NullReferenceException("Returned web request response stream was null.");
                    }

                    responseStream.CopyTo(memoryStream);
                    resp.Close();
                }
            }

            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}