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
                    //if (section.Parsers != null && section.Parsers.Count > 0)
                    {
                        IList<(TNode key, TNode value)> elementsNodes = ((Section<TNode>)section).GetElementsNodes?.Invoke();

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

                    //if (section.Page != null)
                    {
                        IList<string> pagesNodes = ((Section<TNode>)section).GetUrls?.Invoke();

                        if (pagesNodes != null && pagesNodes.Count > 0)
                        {
                            foreach (var pageUrl in pagesNodes)
                            {
                                this.Domain?.Children.Add(section.Page.Parse(pageUrl));
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
