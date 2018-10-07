namespace Stwalkerster.Bot.MediaWikiLib.Configuration
{
    public interface IMediaWikiConfiguration
    {
        string MediaWikiApiEndpoint { get; }
        string UserAgent { get; }
    }
}