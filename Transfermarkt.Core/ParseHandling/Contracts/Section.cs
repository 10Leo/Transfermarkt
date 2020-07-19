using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Logging;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public abstract class ElementsSection<TNode, TValue> : ISection where TValue : IValue
    {
        private IList<(TNode key, TNode value)> elementsNodes;
        protected Func<IList<(TNode key, TNode value)>> GetElementsNodes { get; set; }
        protected IPage<IDomain<TValue>, TValue, TNode> Page { get; set; }

        public string Name { get; set; }
        public IEnumerable<IElementParser<IElement<TValue>, TValue, TNode>> Parsers { get; set; }

        public Children ChildrenType { get; private set; }

        public ElementsSection(string name, IPage<IDomain<TValue>, TValue, TNode> page)
        {
            this.Name = name;
            this.Page = page;
            this.ChildrenType = Contracts.Children.NO;
            this.Parsers = new List<IElementParser<IElement<TValue>, TValue, TNode>>();
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

            Parsers.ToList().ForEach(p =>
            {
                //TODO: recreate an instance of the parser? if yes, relocate this logic to the beginning of this method.
                p.Reset();
            });
        }
    }

    public abstract class ChildsSection<TNode, TValue> : ISection where TValue : IValue
    {
        public IPage<IDomain<TValue>, TValue, TNode> this[string name]
        {
            get
            {
                var index = 0;
                for (int i = 0; i < Children.Count; i++)
                {
                    if (Children[i].Title == name)
                    {
                        index = i;
                        break;
                    }
                }

                if (index == 0)
                {
                    return null;
                }

                return ChildrenPages[pagesParsed[Children[index]]];
            }
        }

        private bool fetched = false;
        private readonly IDictionary<Link, bool> linksParsed = new Dictionary<Link, bool>();
        private readonly IDictionary<Link, int> pagesParsed = new Dictionary<Link, int>();

        protected IPage<IDomain<TValue>, TValue, TNode> Page { get; set; }
        protected IPage<IDomain<TValue>, TValue, TNode> ChildPage { get; set; }
        protected Func<IList<Link>> GetUrls { get; set; }

        public string Name { get; set; }
        public IList<Link> Children { get; set; }
        public IList<IPage<IDomain<TValue>, TValue, TNode>> ChildrenPages { get; set; }
        public Children ChildrenType { get; private set; }

        public ILogger Logger { get; set; }
        public IConnection<TNode> Connection { get; set; }

        public ChildsSection(string name, IPage<IDomain<TValue>, TValue, TNode> page, ILogger logger, IConnection<TNode> connection)
        {
            this.Name = name;
            this.Page = page;
            this.ChildrenType = Contracts.Children.DIFF_PAGE;
            this.Logger = logger;
            this.Connection = connection;
        }

        public IList<Link> Fetch()
        {
            if (fetched)
            {
                return Children;
            }
            if (!this.Page.Connection.IsConnected)
            {
                throw new Exception("No connection to the page made yet.");
            }

            Children = GetUrls?.Invoke();
            
            fetched = true;
            Children?.ToList().ForEach(l => linksParsed.Add(l, false));
            Children?.ToList().ForEach(l => pagesParsed.Add(l, -1));
            ChildrenPages = new List<IPage<IDomain<TValue>, TValue, TNode>>(Children.Count);

            return Children;
        }

        public void Parse(bool parseChildren)
        {
            if (this.Page == null)
            {
                return;
            }
            if (!this.Page.Connection.IsConnected)
            {
                throw new Exception("No connection to the page made yet.");
            }
            if (fetched == false)
            {
                Fetch();
            }
            if (Children == null || Children.Count == 0)
            {
                return;
            }

            if (!parseChildren)// || this.ChildrenType == Contracts.Children.DIFF_PAGE)
            {
                return;
            }

            foreach (var pageUrl in Children)
            {
                if (linksParsed.ContainsKey(pageUrl) && linksParsed[pageUrl])
                {
                    // Page already parsed
                    continue;
                }

                Type tChildPage = this.ChildPage.GetType();
                this.ChildPage.Connection.Reset();
                var childPage = (IPage<IDomain<TValue>, TValue, TNode>)Activator.CreateInstance(tChildPage, this.ChildPage.Connection, this.Logger, 2009);
                childPage.Connect(pageUrl.Url);
                childPage.Parse(parseChildren: parseChildren);

                this.ChildrenPages.Add(childPage);
                this.linksParsed[pageUrl] = true;
                this.pagesParsed[pageUrl] = ChildrenPages.Count - 1;

                this.Page.Domain?.Children.Add(childPage.Domain);
            }
        }

        public void Parse(IEnumerable<Link> links)
        {
            if (this.Page == null)
            {
                return;
            }
            if (!this.Page.Connection.IsConnected)
            {
                throw new Exception("No connection to the page made yet.");
            }
            if (fetched == false)
            {
                Fetch();
            }
            if (Children == null || Children.Count == 0)
            {
                return;
            }

            foreach (Link pageUrl in links)
            {
                Link pageToParse = Children.FirstOrDefault(c => c.Equals(pageUrl));
                if (pageToParse == null)
                {
                    continue;
                }

                if (linksParsed.ContainsKey(pageToParse) && linksParsed[pageToParse])
                {
                    // Page already parsed
                    continue;
                }

                //if (this.HasChildren == Contracts.Children.DIFF_PAGE)
                {
                    Type tChildPage = this.ChildPage.GetType();
                    this.ChildPage.Connection.Reset();
                    var childPage = (IPage<IDomain<TValue>, TValue, TNode>)Activator.CreateInstance(tChildPage, this.ChildPage.Connection, this.Logger, 2009);
                    childPage.Connect(pageUrl.Url);
                    childPage.Parse(parseChildren: false);

                    this.ChildrenPages.Add(childPage);
                    this.linksParsed[pageUrl] = true;
                    this.pagesParsed[pageUrl] = ChildrenPages.Count - 1;

                    this.Page.Domain?.Children.Add(childPage.Domain);
                }
            }
        }
    }

    public abstract class ChildsSamePageSection<TDomain, TValue, TNode> : ISection where TDomain : IDomain<TValue>, new() where TValue : IValue
    {
        private IList<List<(TNode key, TNode value)>> childDomainNodes;
        protected Func<IList<List<(TNode key, TNode value)>>> GetChildsNodes { get; set; }
        protected IPage<IDomain<TValue>, TValue, TNode> Page { get; set; }

        public string Name { get; set; }
        public IEnumerable<IElementParser<IElement<TValue>, TValue, TNode>> Parsers { get; set; }
        public Children ChildrenType { get; private set; }

        public ChildsSamePageSection(string name, IPage<IDomain<TValue>, TValue, TNode> page)
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

                Parsers.ToList().ForEach(p =>
                {
                    p.Reset();
                });
            }
        }
    }
}