using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public abstract class Page<TNode, TValue> : IPage<IDomain<TValue>, TNode, IElement<TValue>, TValue>
    {
        public IDomain<TValue> Domain { get; set; }

        public IConnection<TNode> Connection { get; set; }

        public IReadOnlyList<ISection<TNode, IElement<TValue>, TValue>> Sections { get; set; }
        
        public event EventHandler<PageEventArgs> OnAfterParse;
        public event EventHandler<PageEventArgs> OnBeforeParse;

        public Page(IConnection<TNode> connection)
        {
            this.Connection = connection ?? throw new Exception("Can't use a null connection.");
        }

        #region Contract

        public virtual IDomain<TValue> Parse(string url)
        {
            this.Connection.Connect(url);

            OnBeforeParse?.Invoke(this, new PageEventArgs(url));

            if (Sections != null)
            {
                foreach (var section in Sections)
                {
                    section.Parse(this);
                }
            }

            OnAfterParse?.Invoke(this, new PageEventArgs(url));

            return this.Domain;
        }

        public virtual void Save()
        {
        }

        #endregion Contract
    }
}
