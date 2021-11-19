namespace Stwalkerster.Bot.MediaWikiLib.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Stwalkerster.Bot.MediaWikiLib.Model;

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
        IEnumerable<string> GetPagesInCategory(string category, string limit);
        string GetArticlePath();
        string GetMaxLag();
        DateTime? GetRegistrationDate(string username);
        int GetEditCount(string username);
        PageInformation GetPageInformation(string title);
        IDictionary<string, PageCategoryProperties> GetCategoriesOfPage(string title);
        IEnumerable<BlockInformation> GetBlockInformation(string username);
        IEnumerable<Contribution> GetContributions(string user, int limit);
        IEnumerable<string> GetPagesInCategory(string category, bool fetchAll);
        IDictionary<string, string> GetPagesInCategory(string category, string limit, bool fetchAll);
        string ShortenUrl(string url);
        Tuple<string,string> ShortenUrlWithAlt(string url);
        IEnumerable<InterwikiPrefix> GetInterwikiPrefixes();
        IEnumerable<string> PrefixSearch(string prefix, int pageNamespace);
    }
}