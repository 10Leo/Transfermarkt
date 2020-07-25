using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Page.Scraper.Contracts
{
    public abstract class ChildsSection<TNode, TChildPage> : ISection where TChildPage : IPage<IDomain, TNode>, new()
    {
        public IPage<IDomain, TNode> this[string name]
        {
            get
            {
                var index = -1;
                for (int i = 0; i < Children.Count; i++)
                {
                    if (Children[i].Title == name)
                    {
                        index = i;
                        break;
                    }
                }

                if (index == -1)
                {
                    return null;
                }

                return ChildrenPages[pagesParsed[Children[index]]];
            }
        }

        private bool fetched = false;
        private readonly IDictionary<Link, bool> linksParsed = new Dictionary<Link, bool>();
        private readonly IDictionary<Link, int> pagesParsed = new Dictionary<Link, int>();

        protected IPage<IDomain, TNode> Page { get; set; }
        protected IPage<IDomain, TNode> ChildPage { get; set; }
        protected Func<IList<Link>> GetUrls { get; set; }

        public string Name { get; set; }
        public IList<Link> Children { get; set; }
        public IList<IPage<IDomain, TNode>> ChildrenPages { get; set; }
        public Children ChildrenType { get; private set; }

        public IConnection<TNode> Connection { get; set; }

        public ChildsSection(string name, IPage<IDomain, TNode> page, IConnection<TNode> connection)
        {
            this.Name = name;
            this.Page = page;
            this.ChildrenType = Contracts.Children.DIFF_PAGE;
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
            ChildrenPages = new List<IPage<IDomain, TNode>>(Children.Count);

            return Children;
        }

        public void Parse(bool parseChildren)
        {
            Validate(Children);
            P(Children, parseChildren, parseChildren);
        }

        public void Parse(IEnumerable<Link> links, bool parseChildren = false)
        {
            var linksToParse = links?.Where(s => Children.Contains(s));
            
            Validate(linksToParse);
            P(linksToParse, true, parseChildren);
        }

        private void P(IEnumerable<Link> linksToParse, bool parseChildren, bool child)
        {
            if (!parseChildren)// || this.ChildrenType == Contracts.Children.DIFF_PAGE)
            {
                return;
            }

            foreach (Link pageUrl in linksToParse)
            {
                if (linksParsed.ContainsKey(pageUrl) && linksParsed[pageUrl])
                {
                    // Page already parsed
                    continue;
                }

                TChildPage childPage = new TChildPage();

                childPage.Connect(pageUrl.Url);
                childPage.Parse(parseChildren: child);

                this.ChildrenPages.Add(childPage);
                this.linksParsed[pageUrl] = true;
                this.pagesParsed[pageUrl] = ChildrenPages.Count - 1;

                this.Page.Domain?.Children.Add(childPage.Domain);
            }
        }

        private void Validate(IEnumerable<Link> linksToParse)
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
            if (linksToParse == null || linksToParse.Count() == 0)
            {
                return;
            }
        }
    }
}