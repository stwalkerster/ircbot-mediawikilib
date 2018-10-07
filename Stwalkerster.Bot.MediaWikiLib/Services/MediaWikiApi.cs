namespace Stwalkerster.Bot.MediaWikiLib.Services
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Xml.XPath;
    using Castle.Core.Logging;
    using Stwalkerster.Bot.MediaWikiLib.Configuration;
    using Stwalkerster.Bot.MediaWikiLib.Exceptions;
    using Stwalkerster.Bot.MediaWikiLib.Services.Interfaces;

    public class MediaWikiApi : IMediaWikiApi
    {
        private readonly ILogger logger;
        private readonly IWebServiceClient wsClient;
        private readonly IMediaWikiConfiguration config;

        private readonly Dictionary<string, List<string>> rightsCache;
        private readonly CookieContainer cookieJar;

        public MediaWikiApi(ILogger logger, IWebServiceClient wsClient, IMediaWikiConfiguration mediaWikiConfiguration)
        {
            this.logger = logger;
            this.wsClient = wsClient;
            this.config = mediaWikiConfiguration;
            this.cookieJar = new CookieContainer();

            this.rightsCache = new Dictionary<string, List<string>>();
        }

        private string GetToken(string type = "csrf")
        {
            var queryParameters = new NameValueCollection
            {
                {"action", "query"},
                {"meta", "tokens"},
                {"type", type}
            };

            var apiResult = this.wsClient.DoApiCall(
                queryParameters,
                this.config.MediaWikiApiEndpoint,
                this.config.UserAgent,
                this.cookieJar,
                false);
            
            var nav = new XPathDocument(apiResult).CreateNavigator();

            var token = nav.SelectSingleNode("//tokens/@" + type + "token");

            if (token == null)
            {
                throw new GeneralMediaWikiApiException("Error getting token!");
            }

            return token.Value;
        }
        private bool IsLoggedIn()
        {
            var queryParameters = new NameValueCollection
            {
                {"action", "query"},
                {"meta", "userinfo"}
            };

            var apiResult = this.wsClient.DoApiCall(
                queryParameters,
                this.config.MediaWikiApiEndpoint,
                this.config.UserAgent,
                this.cookieJar,
                false);
            var nav = new XPathDocument(apiResult).CreateNavigator();
            var userId = nav.SelectSingleNode("//userinfo/@id");

            if (userId == null)
            {
                return false;
            }
            
            return userId.ValueAsInt > 0;
        }
        
        public void Login()
        {
            if (this.IsLoggedIn())
            {
                return;
            }
            
            var token = this.GetToken("login");
            
            var queryParameters = new NameValueCollection
            {
                {"action", "login"},
                {"lgname", this.config.Username},
                {"lgpassword", this.config.Password},
                {"lgtoken", token}
            };

            var apiResult = this.wsClient.DoApiCall(
                queryParameters,
                this.config.MediaWikiApiEndpoint,
                this.config.UserAgent,
                this.cookieJar,
                true);
         
            var nav = new XPathDocument(apiResult).CreateNavigator();

            var loginResult = nav.SelectSingleNode("//login/@result");

            if (loginResult == null)
            {
                throw new GeneralMediaWikiApiException("Error logging in!");
            }

            if (loginResult.Value != "Success")
            {
                throw new GeneralMediaWikiApiException("Error logging in, service returned " + loginResult.Value);
            }
        }
        
        public string GetPageContent(string pageName, out string timestamp)
        {
            var queryParameters = new NameValueCollection
            {
                {"action", "query"},
                {"prop", "info|revisions"},
                {"titles", pageName},
                {"rvprop", "timestamp|content"},
                // {"rvslot", "main"}
            };

            var apiResult = this.wsClient.DoApiCall(
                queryParameters,
                this.config.MediaWikiApiEndpoint,
                this.config.UserAgent,
                this.cookieJar,
                false);
            var nav = new XPathDocument(apiResult).CreateNavigator();

            var missing = nav.SelectSingleNode("//page/@missing");
            if (missing != null)
            {
                timestamp = null;
                return null;
            }

            var timestampNode = nav.SelectSingleNode("//rev/@timestamp");
            timestamp = timestampNode == null ? string.Empty : timestampNode.Value;

            var content = nav.SelectSingleNode("//rev");
            if (content == null)
            {
                throw new GeneralMediaWikiApiException("No content found!");
            }
            
            return content.Value;
        }
        
        public bool WritePage(string pageName, string content, string editSummary, string timestamp, bool bot, bool minor)
        {
            var token = this.GetToken();
            
            var queryParameters = new NameValueCollection
            {
                {"action", "edit"},
                {"title", pageName},
                {"text", content},
                {"summary", editSummary}
            };

            if (timestamp != null)
            {
                queryParameters.Add("basetimestamp", timestamp);
                queryParameters.Add("starttimestamp", timestamp);
            }

            if (bot)
            {
                queryParameters.Add("bot", null);
            }

            queryParameters.Add(minor ? "minor" : "notminor", null);
            
            // must be last
            queryParameters.Add("token", token);

            var apiResult = this.wsClient.DoApiCall(
                queryParameters,
                this.config.MediaWikiApiEndpoint,
                this.config.UserAgent,
                this.cookieJar,
                true);
            var nav = new XPathDocument(apiResult).CreateNavigator();

            var result = nav.SelectSingleNode("//edit/@result");
            if (result == null)
            {
                throw new GeneralMediaWikiApiException("No result available - something went wrong.");
            }
            
            return result.Value == "Success";
        }
        
        public void DeletePage(string pageName, string reason)
        {
            var token = this.GetToken();
            
            var queryParameters = new NameValueCollection
            {
                {"action", "delete"},
                {"title", pageName},
                {"reason", reason},
                {"token", token}
            };

            this.wsClient.DoApiCall(
                queryParameters,
                this.config.MediaWikiApiEndpoint,
                this.config.UserAgent,
                this.cookieJar,
                false);
        }
        
        
        public IEnumerable<string> GetUserGroups(string user)
        {
            if (this.rightsCache.ContainsKey(user))
            {
                this.logger.DebugFormat("Getting groups for {0} from cache", user);
                return this.rightsCache[user];
            }

            this.logger.InfoFormat("Getting groups for {0} from webservice", user);

            var queryParameters = new NameValueCollection
            {
                {"action", "query"},
                {"list", "users"},
                {"usprop", "groups"},
                {"ususers", user}
            };

            var userGroups = this.GetGroups(
                    this.wsClient.DoApiCall(queryParameters, this.config.MediaWikiApiEndpoint, this.config.UserAgent))
                .ToList();
            this.rightsCache.Add(user, userGroups);

            return userGroups;
        }

        public IEnumerable<string> PrefixSearch(string prefix)
        {
            bool continuePresent;
            
            var queryParameters = new NameValueCollection
            {
                {"action", "query"},
                {"list", "allpages"},
                {"apprefix", prefix},
                {"aplimit", "max"}
            };

            do
            {
                var apiResult = this.wsClient.DoApiCall(
                    queryParameters,
                    this.config.MediaWikiApiEndpoint,
                    this.config.UserAgent,
                    this.cookieJar,
                    false);
                
                var nav = new XPathDocument(apiResult).CreateNavigator();

                var contNode = nav.SelectSingleNode("//continue");

                if (contNode != null)
                {
                    continuePresent = true;

                    var attrIt = contNode.Select("@*");
                    while (attrIt.MoveNext())
                    {
                        queryParameters.Remove(attrIt.Current.Name);
                        queryParameters.Add(attrIt.Current.Name, attrIt.Current.Value);
                    }
                }
                else
                {
                    continuePresent = false;
                }

                foreach (var page in nav.Select("//allpages/p/@title"))
                {
                    yield return page.ToString();
                }
                
            } while (continuePresent);
        }
        
        public bool PageIsInCategory(string page, string category)
        {
            this.logger.InfoFormat("Getting category {1} for {0} from webservice", page, category);

            var queryParameters = new NameValueCollection
            {
                {"action", "query"},
                {"prop", "categories"},
                {"titles", page},
                {"clcategories", category},
            };

            var result = this.GetCategories(
                    this.wsClient.DoApiCall(queryParameters, this.config.MediaWikiApiEndpoint, this.config.UserAgent))
                .ToList();
            return result.Any();
        }

        private IEnumerable<string> GetCategories(Stream apiResult)
        {
            var nav = new XPathDocument(apiResult).CreateNavigator();
            var groups = new List<string>();
            foreach (var node in nav.Select("//categories/cl/@title"))
            {
                groups.Add(node.ToString());
            }

            return groups;
        }

        private IEnumerable<string> GetGroups(Stream apiResult)
        {
            var nav = new XPathDocument(apiResult).CreateNavigator();
            if (nav.SelectSingleNode("//user/@invalid") != null || nav.SelectSingleNode("//user/@missing") != null)
            {
                return new List<string> {"*"};
            }

            var groups = new List<string>();
            foreach (var node in nav.Select("//user/groups/g"))
            {
                groups.Add(node.ToString());
            }

            return groups;
        }
    }
}