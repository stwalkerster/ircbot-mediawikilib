namespace Stwalkerster.Bot.MediaWikiLib.Model
{
    using System;

    public class PageProtection
    {
        public string Type { get; }

        public string Level { get; }

        public DateTime? Expiry { get; }

        public PageProtection(string type, string level, DateTime? expiry)
        {
            this.Type = type;
            this.Level = level;
            this.Expiry = expiry;
        }

        protected bool Equals(PageProtection other)
        {
            return string.Equals(this.Type, other.Type) && string.Equals(this.Level, other.Level) && this.Expiry.Equals(other.Expiry);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((PageProtection) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (this.Type != null ? this.Type.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.Level != null ? this.Level.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.Expiry.GetHashCode();
                return hashCode;
            }
        }
    }
}