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

        //public IReadOnlyList<IElementParser<TNode, IElement, object>> Parsers { get; set; }
        //public IPage<IDomain, TNode, IElement> Page { get; set; }

        //public Func<IList<(TNode key, TNode value)>> GetElementsNodes { get; set; }
        //public Func<IList<string>> GetUrls { get; set; }
        //public Func<IList<(IDomain child, List<(TNode key, TNode value)>)>> GetChildsNodes { get; set; }

        public Section(IConnection<TNode> connection)
        {
            this.Connection = connection;
            //this.Parsers = new List<IElementParser<TNode, IElement, object>>();
        }

        public abstract void Parse(IPage<IDomain, TNode, IElement> page);
        //{
        //    if (Parsers != null && Parsers.Count > 0)
        //    {
        //        IList<(TNode key, TNode value)> elementsNodes = GetElementsNodes?.Invoke();

        //        if (elementsNodes != null && elementsNodes.Count > 0)
        //        {
        //            foreach (var (key, value) in elementsNodes)
        //            {
        //                foreach (var parser in Parsers)
        //                {
        //                    if (parser.CanParse(key))
        //                    {
        //                        var parsedObj = parser.Parse(value);
        //                        var e = page.Domain.SetElement(parsedObj);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    if (Page != null)
        //    {
        //        IList<string> pagesNodes = GetUrls?.Invoke();

        //        if (pagesNodes != null && pagesNodes.Count > 0)
        //        {
        //            foreach (var pageUrl in pagesNodes)
        //            {
        //                var pageDomain = Page.Parse(pageUrl);
        //                page.Domain?.Children.Add(pageDomain);

        //                Type t = Page.Domain.GetType();
        //                Page.Domain = (IDomain)Activator.CreateInstance(t);
        //            }
        //        }
        //    }

        //    {
        //        IList<(IDomain child, List<(TNode key, TNode value)>)> childDomainNodes = GetChildsNodes?.Invoke();

        //        if (childDomainNodes != null && childDomainNodes.Count > 0)
        //        {
        //            foreach ((IDomain, List<(TNode key, TNode value)>) childDomainNode in childDomainNodes)
        //            {
        //                IDomain childType = childDomainNode.Item1;
        //                page.Domain?.Children.Add(childType);

        //                foreach ((TNode key, TNode value) in childDomainNode.Item2)
        //                {
        //                    foreach (var parser in Parsers)
        //                    {
        //                        if (parser.CanParse(key))
        //                        {
        //                            var parsedObj = parser.Parse(value);
        //                            var e = childType.SetElement(parsedObj);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    }
}
