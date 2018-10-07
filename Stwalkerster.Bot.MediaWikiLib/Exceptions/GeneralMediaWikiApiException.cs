namespace Stwalkerster.Bot.MediaWikiLib.Exceptions
{
    using System;

    public class GeneralMediaWikiApiException : Exception
    {
        public GeneralMediaWikiApiException()
        {
        }

        public GeneralMediaWikiApiException(string message) : base(message)
        {
        }
    }
}