namespace Stwalkerster.Bot.MediaWikiLib.Model
{
    public class Contribution
    {
        public Contribution(string user, string title, string comment, string timestamp, string revid)
        {
            this.User = user;
            this.Title = title;
            this.Comment = comment;
            this.Timestamp = timestamp;
            this.RevId = revid;
        }

        public string User { get; private set; }
        public string Title { get; private set; }
        public string Comment { get; private set; }
        public string Timestamp { get; private set; }
        public string RevId { get; private set; }
    }
}