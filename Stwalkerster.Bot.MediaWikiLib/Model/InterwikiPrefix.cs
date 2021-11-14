namespace Stwalkerster.Bot.MediaWikiLib.Model
{
    public class InterwikiPrefix
    {
        public string Prefix { get; set; }
        public string Url { get; set; }
        public string Language { get; set; }
        public bool ProtocolRelative { get; set; }
        public bool Local { get; set; }
    }
}