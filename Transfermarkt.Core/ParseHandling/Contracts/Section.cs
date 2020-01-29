using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public abstract class Section<TNode> : ISection<IDomain, TNode, IElement>
    {
        public Func<IList<(TNode key, TNode value)>> GetElementsNodes { get; set; }
        public Func<IList<string>> GetUrls { get; set; }

        public IConnection<TNode> Connection { get; set; }

        public IReadOnlyList<IElementParser<TNode, IElement, object>> Parsers { get; set; }
        public IReadOnlyList<IPage<IDomain, TNode, IElement>> Pages { get; set; }

        public Section(IConnection<TNode> connection)
        {
            this.Connection = connection;
            this.Parsers = new List<IElementParser<TNode, IElement, object>>();
            this.Pages = new List<IPage<IDomain, TNode, IElement>>();
        }
    }
}
