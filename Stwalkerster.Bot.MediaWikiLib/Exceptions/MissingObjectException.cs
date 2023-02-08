namespace Stwalkerster.Bot.MediaWikiLib.Exceptions
{
    public class MissingObjectException : GeneralMediaWikiApiException
    {
        public MissingObjectException() : base("Object not found")
        {
        }
    }
}