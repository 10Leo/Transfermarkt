using System;
using System.Collections.Generic;
using System.Linq;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public abstract class Page<TValue, TNode> : IPage<IDomain<TValue>, IElement<TValue>, TValue, TNode> where TValue : IValue
    {
        public ISection<IElement<TValue>, TValue, TNode> this[string name] => Sections?.FirstOrDefault(s => s.Name == name);
        public IReadOnlyList<ISection<IElement<TValue>, TValue, TNode>> Sections { get; set; }

        public IDomain<TValue> Domain { get; set; }

        public IConnection<TNode> Connection { get; set; }

        public event EventHandler<PageEventArgs> OnAfterParse;
        public event EventHandler<PageEventArgs> OnBeforeParse;

        public Page(IConnection<TNode> connection)
        {
            this.Connection = connection ?? throw new Exception("Can't use a null connection.");
        }

        #region Contract

        public virtual IEnumerable<Link> Fetch(string url)
        public void Connect(string url)
        {
            this.Connection.Connect(url);
        }


            var urls = new List<Link>();
            if (Sections != null)
            {
                foreach (var section in Sections)
                {
                    if (section is IChildsSection<IDomain<TValue>, IElement<TValue>, TValue, TNode>)
                    {
                        urls.AddRange(((IChildsSection<IDomain<TValue>, IElement<TValue>, TValue, TNode>)section).Fetch(this).ToArray());
                    }
                }
            }

            return urls;
        }

        public virtual IDomain<TValue> Parse(string url, string sectionName = null, IEnumerable<Link> links = null)
        {
            this.Connection.Connect(url);

            OnBeforeParse?.Invoke(this, new PageEventArgs(url));

            if (Sections != null)
            {
                if (sectionName != null && links != null)
                {
                    var section = this[sectionName];

                    if (section is IChildsSection<IDomain<TValue>, IElement<TValue>, TValue, TNode>)
                    {
                        ((IChildsSection<IDomain<TValue>, IElement<TValue>, TValue, TNode>)section).Parse(this, links);
                    }
                    else
                    {
                        section.Parse(this);
                    }
                }
                else
                {
                    foreach (var section in Sections)
                    {
                        section.Parse(this);
                    }
                }
            }

            OnAfterParse?.Invoke(this, new PageEventArgs(url));

            return this.Domain;
        }

        #endregion Contract
    }
}
