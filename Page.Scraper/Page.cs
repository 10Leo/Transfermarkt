using System;
using System.Collections.Generic;
using System.Linq;

namespace Page.Scraper.Contracts
{
    public abstract class Page<TValue, TNode> : IPage<IDomain, TNode> where TValue : IValue
    {
        public ISection this[string name] => Sections?.FirstOrDefault(s => s.Name == name);

        public string Url { get; private set; }

        private ParseLevel parseLevel = ParseLevel.NotYet;
        public ParseLevel ParseLevel
        {
            get
            {
                if (Sections.All(s => s.ParseLevel == ParseLevel.Parsed))
                {
                    parseLevel = ParseLevel.Parsed;
                }
                else if (Sections.All(s => s.ParseLevel == ParseLevel.Peeked))
                {
                    parseLevel = ParseLevel.Peeked;
                }
                else
                {
                    parseLevel = ParseLevel.NotYet;
                }

                return parseLevel;
            }
            protected set { parseLevel = value; }
        }

        public IReadOnlyList<ISection> Sections { get; set; }

        public IDomain Domain { get; set; }

        public IConnection<TNode> Connection { get; set; }

        public event EventHandler<PageEventArgs> OnAfterParse;
        public event EventHandler<PageEventArgs> OnBeforeParse;

        public Page(IConnection<TNode> connection)
        {
            this.Connection = connection ?? throw new Exception("Can't use a null connection.");
        }

        #region Contract

        public void Connect(string url)
        {
            this.Url = url;
            if (!this.Connection.IsConnected)
            {
                this.Connection.Connect(url);
            }
        }

        public virtual void Parse(IEnumerable<ISection> sections = null, bool parseChildren = false)
        {
            var sectionsToParse = sections == null ? Sections : sections?.Where(s => Sections.Contains(s));
            ParseWith(sectionsToParse ?? Sections, parseChildren);
        }

        #endregion Contract

        #region Private

        private void ParseWith(IEnumerable<ISection> sectionsToParse, bool parseChildren = false)
        {
            OnBeforeParse?.Invoke(this, new PageEventArgs(this.Url));

            this.Connect(this.Url);

            if (sectionsToParse == null)
            {
                return;
            }

            foreach (ISection section in sectionsToParse)
            {
                section.Parse(parseChildren);
            }

            OnAfterParse?.Invoke(this, new PageEventArgs(this.Url));
        }

        #endregion Private
    }
}
