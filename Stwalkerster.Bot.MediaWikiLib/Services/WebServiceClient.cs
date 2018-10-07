namespace Stwalkerster.Bot.MediaWikiLib.Services
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Castle.Core.Logging;
    using Stwalkerster.Bot.MediaWikiLib.Services.Interfaces;

    public class WebServiceClient : IWebServiceClient
    {
        private readonly ILogger logger;
        private readonly object lockObject = new object();

        public WebServiceClient(ILogger logger)
        {
            this.logger = logger;
        }

        public Stream DoApiCall(NameValueCollection query, string endpoint, string userAgent)
        {
            query.Set("format", "xml");

            var queryFragment = string.Join("&", query.AllKeys.Select(a => a + "=" + WebUtility.UrlEncode(query[a])));
            
            var url = string.Format("{0}?{1}", endpoint, queryFragment);
            
            this.logger.DebugFormat("Requesting {0}", url);
            
            var hwr = (HttpWebRequest)WebRequest.Create(url);
            hwr.UserAgent = userAgent;
            
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