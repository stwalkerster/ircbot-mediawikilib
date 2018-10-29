namespace Stwalkerster.Bot.MediaWikiLib.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PageInformation
    {
        public List<string> RedirectedFrom { get; private set; }
        public List<PageProtection> Protection { get; private set; }
        public string Title { get; private set; }
        public string LastRevComment { get; private set; }
        public string LastRevUser { get; private set; }
        public DateTime? Touched { get; private set; }
        public bool Missing { get; private set; }
        public uint Size { get; private set; }

        public PageInformation(List<string> redirectedFrom)
        {
            this.RedirectedFrom = redirectedFrom;
            this.Protection = new List<PageProtection>();
        }

        public PageInformation(
            List<string> redirectedFrom,
            List<PageProtection> protection,
            string title,
            uint size,
            string lastRevComment,
            string lastRevUser,
            DateTime? touched)
        {
            this.RedirectedFrom = redirectedFrom;
            this.Protection = protection;
            this.Title = title;
            this.Size = size;
            this.LastRevComment = lastRevComment;
            this.LastRevUser = lastRevUser;
            this.Touched = touched;
            this.Missing = false;
        }

        public PageInformation(List<string> redirectedFrom, List<PageProtection> protection, string title, bool missing)
        {
            this.RedirectedFrom = redirectedFrom;
            this.Protection = protection;
            this.Title = title;
            this.Missing = missing;
        }

        protected bool Equals(PageInformation other)
        {
            return Enumerable.SequenceEqual(this.RedirectedFrom, other.RedirectedFrom)
                   && Enumerable.SequenceEqual(this.Protection, other.Protection)
                   && string.Equals(this.Title, other.Title)
                   && string.Equals(this.LastRevComment, other.LastRevComment)
                   && string.Equals(this.LastRevUser, other.LastRevUser)
                   && this.Touched.Equals(other.Touched) 
                   && this.Missing == other.Missing && string.Equals(this.Size, other.Size);
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

            return Equals((PageInformation) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (this.RedirectedFrom != null ? this.RedirectedFrom.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.Protection != null ? this.Protection.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.Title != null ? this.Title.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.LastRevComment != null ? this.LastRevComment.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.LastRevUser != null ? this.LastRevUser.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.Touched.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Missing.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.Size != null ? this.Size.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}