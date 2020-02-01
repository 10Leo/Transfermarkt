using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public abstract class Section<TNode> : ISection<IDomain, TNode, IElement>
    {
        public IConnection<TNode> Connection { get; set; }

        public IReadOnlyList<IElementParser<TNode, IElement, object>> Parsers { get; set; }
        public IPage<IDomain, TNode, IElement> Page { get; set; }

        public Func<IList<(TNode key, TNode value)>> GetElementsNodes { get; set; }
        public Func<IList<string>> GetUrls { get; set; }
        public Func<IList<(IDomain child, List<(TNode key, TNode value)>)>> GetChildsNodes { get; set; }

        public Section(IConnection<TNode> connection)
        {
            this.Connection = connection;
            this.Parsers = new List<IElementParser<TNode, IElement, object>>();
        }

        public IEnumerable<IElement> ParseElements()
        {
            IList<IElement> parsedElements = new List<IElement>();

            if (Parsers != null && Parsers.Count > 0)
            {
                IList<(TNode key, TNode value)> elementsNodes = GetElementsNodes?.Invoke();

                if (elementsNodes != null && elementsNodes.Count > 0)
                {
                    foreach (var (key, value) in elementsNodes)
                    {
                        foreach (var parser in Parsers)
                        {
                            if (parser.CanParse(key))
                            {
                                var parsedObj = parser.Parse(value);
                                parsedElements.Add(parsedObj);
                            }
                        }
                    }
                }
            }

            return parsedElements;
        }

        public IEnumerable<IDomain> ParseUrls()
        {
            IList<IDomain> parsedDomains = new List<IDomain>();

            if (Page != null)
            {
                IList<string> pagesNodes = GetUrls?.Invoke();

                if (pagesNodes != null && pagesNodes.Count > 0)
                {
                    foreach (var pageUrl in pagesNodes)
                    {
                        var pageDomain = Page.Parse(pageUrl);
                        parsedDomains.Add(pageDomain);

                        Type t = Page.Domain.GetType();
                        Page.Domain = (IDomain)Activator.CreateInstance(t);
                    }
                }
            }

            return parsedDomains;
        }

        public IEnumerable<IDomain> ParseChilds()
        {
            IList<IDomain> parsedChilds = new List<IDomain>();

            {
                IList<(IDomain child, List<(TNode key, TNode value)>)> childDomainNodes = GetChildsNodes?.Invoke();

                if (childDomainNodes != null && childDomainNodes.Count > 0)
                {
                    foreach ((IDomain, List<(TNode key, TNode value)>) childDomainNode in childDomainNodes)
                    {
                        IDomain t = childDomainNode.Item1;
                        parsedChilds.Add(t);

                        foreach ((TNode key, TNode value) in childDomainNode.Item2)
                        {
                            foreach (var parser in Parsers)
                            {
                                if (parser.CanParse(key))
                                {
                                    var parsedObj = parser.Parse(value);
                                    var e = t.SetElement(parsedObj);
                                }
                            }
                        }
                    }
                }
            }

            return parsedChilds;
        }
    }
}
