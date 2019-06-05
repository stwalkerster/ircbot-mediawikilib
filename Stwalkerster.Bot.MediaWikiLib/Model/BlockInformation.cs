namespace Stwalkerster.Bot.MediaWikiLib.Model
{
    public class BlockInformation
    {
        public string Id { get; set; }
        public bool AllowUserTalk { get; set; }
        public bool AnonOnly { get; set; }
        public bool AutoBlock { get; set; }
        public string BlockReason { get; set; }
        public string BlockedBy { get; set; }
        public string Expiry { get; set; }
        public bool NoCreate { get; set; }
        public bool NoEmail { get; set; }
        public string Start { get; set; }
        public string Target { get; set; }

        /// <remarks>
        ///     TODO: fixes for context, localisation, etc,
        /// </remarks>
        public override string ToString()
        {
            const string Format = "Block {0} targeting {1} blocked by {2} for {3} starting at {4} because {5} Flags: {6}";
            var emptyMessage = string.Format(Format,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty);

            var flags = string.Empty;
            if (this.NoCreate) flags += "NOCREATE ";
            if (this.AutoBlock) flags += "AUTOBLOCK ";
            if (this.NoEmail) flags += "NOEMAIL ";
            if (this.AllowUserTalk) flags += "ALLOWUSERTALK ";
            if (this.AnonOnly) flags += "ANONONLY ";

            var message = string.Format(Format,
                this.Id,
                this.Target,
                this.BlockedBy,
                this.Expiry,
                this.Start,
                this.BlockReason,
                flags);

            if (message == emptyMessage)
            {
                message = "Cannot find any current blocks for the specified user.";
            }

            return message;
        }


    }
}