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
            return this.DoApiCall(query, endpoint, userAgent, new CookieContainer(), false);
        }

        public Stream DoApiCall(NameValueCollection query, string endpoint, string userAgent, CookieContainer cookieJar, bool post)
        {
            query.Set("format", "xml");

            var queryFragment = string.Join("&", query.AllKeys.Select(a => a + "=" + WebUtility.UrlEncode(query[a])));
            
            var url = endpoint;

            if (!post)
            {
                url = string.Format("{0}?{1}", endpoint, queryFragment);
            }
            
            var hwr = (HttpWebRequest)WebRequest.Create(url);
            hwr.CookieContainer = cookieJar;
            hwr.UserAgent = userAgent;
            hwr.Method = post ? "POST" : "GET";
            
            this.logger.DebugFormat("Requesting {1} {0}", url, hwr.Method);

            if (post)
            {
                hwr.ContentType = "application/x-www-form-urlencoded";
                hwr.ContentLength = queryFragment.Length;

                using (var requestWriter = new StreamWriter(hwr.GetRequestStream()))
                {
                    requestWriter.Write(queryFragment);
                    requestWriter.Flush();
                }
            }
            
            var memoryStream = new MemoryStream();

            lock (this.lockObject)
            {
                using (var resp = (HttpWebResponse) hwr.GetResponse())
                {
                    cookieJar.Add(resp.Cookies);
                    
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