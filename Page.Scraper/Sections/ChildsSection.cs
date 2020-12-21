using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Page.Scraper.Contracts
{
    /// <summary>
    /// Pages that contain links to other (child) pages.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TChildPage"></typeparam>
    public abstract class ChildsSection<TNode, TChildPage> : ISection where TChildPage : IPage<IDomain, TNode>, new()
    {
        public TChildPage this[IDictionary<string, string> ids]
        {
            get
            {
                if (ids.ContainsKey("URL"))
                {
                    return Children.FirstOrDefault(c => c.Url == ids["URL"].ToString()).Page;
                }
                else if (ids.ContainsKey("Title"))
                {
                    return Children.FirstOrDefault(c => c.Title == ids["Title"].ToString()).Page;
                }
                else
                {
                    Link<TNode, TChildPage> child = Children.FirstOrDefault(c =>
                    {
                        foreach (KeyValuePair<string, string> id in ids)
                        {
                            if (c.Identifiers.ContainsKey(id.Key))
                            {
                                var k = c.Identifiers[id.Key];
                                if (k != id.Value)
                                {
                                    return false;
                                }

                            }
                        }

                        return true;
                    });

                    return child.Page;
                }
            }
        }

        private bool fetched = false;

        protected IPage<IDomain, TNode> Page { get; set; }
        protected IPage<IDomain, TNode> ChildPage { get; set; }
        protected Func<IList<Link<TNode, TChildPage>>> GetUrls { get; set; }

        public string Name { get; set; }
        public IList<Link<TNode, TChildPage>> Children { get; set; }
        public Children ChildrenType { get; private set; }

        public IConnection<TNode> Connection { get; set; }
        public ParseLevel ParseLevel { get; set; }

        public ChildsSection(string name, IPage<IDomain, TNode> page, IConnection<TNode> connection)
        {
            this.Name = name;
            this.Page = page;
            this.ChildrenType = Contracts.Children.DIFF_PAGE;
            this.Connection = connection;
        }

        public IList<Link<TNode, TChildPage>> Peek()
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
            this.ParseLevel = ParseLevel.Fetched;

            return Children;
        }

        public void Parse(bool parseChildren)
        {
            Validate(Children);
            Parse(Children, parseChildren, parseChildren);
        }

        public void Parse(IEnumerable<Link<TNode, TChildPage>> links, bool parseChildren = false)
        {
            var linksToParse = links?.Where(s => Children.Contains(s));
            
            Validate(linksToParse);
            Parse(linksToParse, true, parseChildren);
        }

        private void Parse(IEnumerable<Link<TNode, TChildPage>> linksToParse, bool parseChildren, bool child)
        {
            if (!parseChildren)// || this.ChildrenType == Contracts.Children.DIFF_PAGE)
            {
                return;
            }

            foreach (Link<TNode, TChildPage> pageUrl in linksToParse)
            {
                Link<TNode, TChildPage> found = Children.FirstOrDefault(c => c == pageUrl);
                if (found == null)
                {
                    // Link pretended not found
                    continue;
                }

                if (found.Page != null)
                {
                    // Page already parsed
                    continue;
                }

                TChildPage childPage = new TChildPage();

                childPage.Connect(pageUrl.Url);
                childPage.Parse(parseChildren: child);

                found.Page = childPage;

                this.Page.Domain?.Children.Add(childPage.Domain);
            }

            this.ParseLevel = (parseChildren ? ParseLevel.Parsed : ParseLevel.Fetched);
        }

        private void Validate(IEnumerable<Link<TNode, TChildPage>> linksToParse)
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
                Peek();
            }
            if (linksToParse == null || linksToParse.Count() == 0)
            {
                return;
            }
        }
    }
}