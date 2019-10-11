namespace Stwalkerster.Bot.MediaWikiLib.Model
{
    public class PageCategoryProperties
    {
        public PageCategoryProperties(string sortKey, bool hidden)
        {
            this.SortKey = sortKey;
            this.Hidden = hidden;
        }

        public string SortKey { get; internal set; }
        public bool Hidden { get; internal set; }
    }
}