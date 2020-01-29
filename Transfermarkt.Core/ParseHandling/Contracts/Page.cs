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
        protected static IConfigurationManager config = new ConfigManager();

        public Func<TNode, (TNode key, TNode value)> GetClubsUrl { get; set; }
        public Func<IList<(TNode key, TNode value)>> GetElementsNodes { get; set; }
        public Func<IList<string>> GetUrls { get; set; }

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

            IList<(TNode key, TNode value)> elementsNodes = GetElementsNodes?.Invoke();
            IList<string> pagesNodes = GetUrls?.Invoke();

            if (Sections != null)
            {
                foreach (var section in Sections)
                {
                    if (section.Parsers != null)
                    {
                        foreach (var elementsNode in elementsNodes)
                        {
                            foreach (var parser in section.Parsers)
                            {
                                if (parser.CanParse(elementsNode.key))
                                {
                                    var parsedObj = parser.Parse(elementsNode.value);
                                    var e = this.Domain.SetElement(parsedObj);
                                }
                            }
                        }
                    }

                    if (section.Pages != null)
                    {
                        foreach (var page in section.Pages)
                        {
                            foreach (var pageUrl in pagesNodes)
                            {
                                this.Domain?.Children.Add(page.Parse(pageUrl));
                            }
                            //var node = this.Connection.GetNode();

                            //var dic = GetClubsUrl?.Invoke(node);
                            //string finalClubUrl = "";// GetClubsUrl(node);

                            //this.Domain?.Children.Add(page.Parse(finalClubUrl));
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
