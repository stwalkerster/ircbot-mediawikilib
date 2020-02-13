namespace Stwalkerster.Bot.MediaWikiLib.Exceptions
{
    public class MissingUserException : GeneralMediaWikiApiException
    {
        public MissingUserException() : base("Missing user")
        {
        }
    }
}