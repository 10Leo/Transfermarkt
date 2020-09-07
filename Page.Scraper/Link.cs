using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Page.Scraper.Contracts
{
    public class Link<TNode, TPage> where TPage : IPage<IDomain, TNode>, new()
    {
        public string Title { get; set; }
        public IDictionary<string, string> Identifiers { get; set; } = new Dictionary<string, string>();

        public string Url { get; set; }
        public TPage Page { get; set; }

        public override string ToString()
        {
            return $"{(Title ?? Url)}";
        }

        #region Equality

        public override bool Equals(object obj)
        {
            if (!(obj is Link<TNode, TPage> item))
            {
                return false;
            }

            return this.Title.Equals(item.Title);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + Title.GetHashCode();
                return hash;
            }
        }

        #endregion Equality
    }
}
