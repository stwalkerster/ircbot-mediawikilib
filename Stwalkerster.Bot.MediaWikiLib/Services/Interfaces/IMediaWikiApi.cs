namespace Stwalkerster.Bot.MediaWikiLib.Services.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IMediaWikiApi
    {
        IEnumerable<string> GetUserGroups(string user);
        bool PageIsInCategory(string page, string category);
        void Login();
        string GetPageContent(string pageName, out string timestamp);
        bool WritePage(string pageName, string content, string editSummary, string timestamp, bool bot, bool minor);
        void DeletePage(string pageName, string reason);
        IEnumerable<string> PrefixSearch(string prefix);
        int GetCategorySize(string categoryName);
        IEnumerable<string> GetPagesInCategory(string category);
        string GetArticlePath();
        string GetMaxLag();
        DateTime? GetRegistrationDate(string username);
        int GetEditCount(string username);
    }
}