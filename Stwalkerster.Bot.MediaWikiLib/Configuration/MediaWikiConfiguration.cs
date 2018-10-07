namespace Stwalkerster.Bot.MediaWikiLib.Configuration
{
    using System;

    public class MediaWikiConfiguration : IMediaWikiConfiguration
    {
        public MediaWikiConfiguration(string mediaWikiApiEndpoint, string userAgent, string username, string password)
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
            this.Username = username;
            this.Password = password;
        }

        public string MediaWikiApiEndpoint { get; }
        public string UserAgent { get; }
        public string Username { get; }
        public string Password { get; }
    }
}