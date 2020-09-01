using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Page.Scraper.Contracts
{
    public abstract class ElementsSection<TNode> : ISection
    {
        private IList<(TNode key, TNode value)> elementsNodes;
        protected Func<IList<(TNode key, TNode value)>> GetElementsNodes { get; set; }
        protected IPage<IDomain, TNode> Page { get; set; }

        public string Name { get; set; }
        public IEnumerable<IElementParser<IElement<IValue, IConverter<IValue>>, IValue, TNode>> Parsers { get; set; }

        public Children ChildrenType { get; private set; }
        public ParseLevel ParseLevel { get; set; }

        public ElementsSection(string name, IPage<IDomain, TNode> page)
        {
            this.Name = name;
            this.Page = page;
            this.ChildrenType = Contracts.Children.NO;
            this.Parsers = new List<IElementParser<IElement<IValue, IConverter<IValue>>, IValue, TNode>>();
        }

        public void Parse(bool parseChildren)
        {
            if (Parsers == null)
            {
                return;
            }

            elementsNodes = GetElementsNodes?.Invoke();
            if (elementsNodes == null || elementsNodes.Count == 0)
            {
                return;
            }

            foreach (var (key, value) in elementsNodes)
            {
                foreach (var parser in Parsers)
                {
                    if (parser.CanParse(key))
                    {
                        var parsedObj = parser.Parse(value);
                        var e = this.Page.Domain.SetElement(parsedObj);
                    }
                }
            }

            this.ParseLevel = (parseChildren ? ParseLevel.Parsed : ParseLevel.Fetched);

            Parsers.ToList().ForEach(p =>
            {
                //TODO: recreate an instance of the parser? if yes, relocate this logic to the beginning of this method.
                p.Reset();
            });
        }
    }
}