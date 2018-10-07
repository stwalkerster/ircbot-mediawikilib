namespace Stwalkerster.Bot.MediaWikiLib.Services.Interfaces
{
    using System.Collections.Generic;

    public interface IMediaWikiApi
    {
        IEnumerable<string> GetUserGroups(string user);
        bool PageIsInCategory(string page, string category);
    }
}