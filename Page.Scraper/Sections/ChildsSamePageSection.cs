using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Page.Scraper.Contracts
{
    /// <summary>
    /// For children domain objects that exist in the same page as the parent domain object.
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public abstract class ChildsSamePageSection<TDomain, TNode> : ISection where TDomain : IDomain, new()
    {
        private IList<List<(TNode key, TNode value)>> childDomainNodes;
        protected Func<IList<List<(TNode key, TNode value)>>> GetChildsNodes { get; set; }
        protected IPage<IDomain, TNode> Page { get; set; }

        public string Name { get; set; }
        public IEnumerable<IElementParser<IElement<IValue, IConverter<IValue>>, IValue, TNode>> Parsers { get; set; }
        public Children ChildrenType { get; private set; }
        public ParseLevel ParseLevel { get; set; }

        public ChildsSamePageSection(string name, IPage<IDomain, TNode> page)
        {
            this.Name = name;
            this.Page = page;
            this.ChildrenType = Children.SAME_PAGE;
        }

        public void Parse(bool parseChildren)
        {
            childDomainNodes = GetChildsNodes?.Invoke();

            if (childDomainNodes == null || childDomainNodes.Count == 0)
            {
                return;
            }

            foreach (List<(TNode key, TNode value)> childDomainNode in childDomainNodes)
            {
                TDomain childType = new TDomain();
                this.Page.Domain?.Children.Add(childType);

                if (Parsers != null)
                {
                    foreach ((TNode key, TNode value) in childDomainNode)
                    {
                        foreach (var parser in Parsers)
                        {
                            if (parser.CanParse(key))
                            {
                                var parsedObj = parser.Parse(value);
                                var e = childType.SetElement(parsedObj);
                            }
                        }
                    }
                }

                this.ParseLevel = (parseChildren ? ParseLevel.Parsed : ParseLevel.Fetched);

                Reset();
            }
        }

        private void Reset()
        {
            Parsers.ToList().ForEach(p =>
            {
                p.Reset();
            });
        }
    }
}