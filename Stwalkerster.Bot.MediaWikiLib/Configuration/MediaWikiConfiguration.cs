namespace Stwalkerster.Bot.MediaWikiLib.Configuration
{
    using System;

    public class MediaWikiConfiguration : IMediaWikiConfiguration
    {
        public MediaWikiConfiguration(string mediaWikiApiEndpoint, string userAgent)
        {
            if (mediaWikiApiEndpoint == null)
            {
                throw new ArgumentNullException(nameof(mediaWikiApiEndpoint));
            }
            
            if (userAgent == null)
            {
                throw new ArgumentNullException(nameof(userAgent));
            }

            this.MediaWikiApiEndpoint = mediaWikiApiEndpoint;
            this.UserAgent = userAgent;
        }

        public string MediaWikiApiEndpoint { get; }
        public string UserAgent { get; }
    }
}