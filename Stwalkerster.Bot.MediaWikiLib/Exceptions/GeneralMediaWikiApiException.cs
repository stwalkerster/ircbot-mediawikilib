namespace Stwalkerster.Bot.MediaWikiLib.Exceptions
{
    using System;

    public class GeneralMediaWikiApiException : Exception
    {
        public string ApiResponse { get; }

        public GeneralMediaWikiApiException()
        {
        }

        public GeneralMediaWikiApiException(string message) : base(message)
        {
        }
        
        public GeneralMediaWikiApiException(string message, string apiResponse) : this (message)
        {
            this.ApiResponse = apiResponse;
        }
    }
}