using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public abstract class Page<TNode> : IPage<IDomain, TNode, IElement>
    {
        public IDomain Domain { get; set; }

        public IConnection<TNode> Connection { get; set; }

        public IReadOnlyList<ISection<IDomain, TNode, IElement>> Sections { get; set; }

        public Page(IConnection<TNode> connection)
        {
            this.Connection = connection ?? throw new Exception("Can't pass a null connection.");
        }

        #region Contract

        public virtual IDomain Parse(string url)
        {
            this.Connection.Connect(url);

            if (Sections != null)
            {
                foreach (var section in Sections)
                {
                    if (section.Parsers != null && section.Parsers.Count > 0)
                    {
                        IList<(TNode key, TNode value)> elementsNodes = section.ElementsNodes();

                        if (elementsNodes != null && elementsNodes.Count > 0)
                        {
                            foreach (var (key, value) in elementsNodes)
                            {
                                foreach (var parser in section.Parsers)
                                {
                                    if (parser.CanParse(key))
                                    {
                                        var parsedObj = parser.Parse(value);
                                        var e = this.Domain.SetElement(parsedObj);
                                    }
                                }
                            }
                        }
                    }

                    if (section.Page != null)
                    {
                        IList<string> pagesNodes = section.Urls();

                        if (pagesNodes != null && pagesNodes.Count > 0)
                        {
                            foreach (var pageUrl in pagesNodes)
                            {
                                var r = section.Page.Parse(pageUrl);
                                this.Domain?.Children.Add(r);

                                Type t = section.Page.Domain.GetType();
                                section.Page.Domain = (IDomain)Activator.CreateInstance(t);
                            }
                        }
                    }

                    {
                        IList<(IDomain child, List<(TNode key, TNode value)>)> childDomainNodes = section.ChildsNodes();

                        if (childDomainNodes != null && childDomainNodes.Count > 0)
                        {
                            foreach ((IDomain, List<(TNode key, TNode value)>) childDomainNode in childDomainNodes)
                            {
                                IDomain t = childDomainNode.Item1;
                                this.Domain.Children.Add(t);

                                foreach ((TNode key, TNode value) in childDomainNode.Item2)
                                {
                                    foreach (var parser in section.Parsers)
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
                }
            }

            return this.Domain;
        }

        public virtual void Save()
        {
        }

        #endregion Contract
    }
}
