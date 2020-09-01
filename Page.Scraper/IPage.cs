using System;
using System.Collections.Generic;

namespace Page.Scraper.Contracts
{
    public interface IPage<TDomain, TNode> where TDomain : IDomain
    {
        ISection this[string name] { get; }
        IReadOnlyList<ISection> Sections { get; set; }

        string Url { get; }
        ParseLevel ParseLevel { get; }
        TDomain Domain { get; set; }

        IConnection<TNode> Connection { get; set; }

        event EventHandler<PageEventArgs> OnBeforeParse;
        event EventHandler<PageEventArgs> OnAfterParse;

        void Connect(string url);

        void Parse(IEnumerable<ISection> sections = null, bool parseChildren = false);
    }

    public class PageEventArgs : EventArgs
    {
        public string Url { get; }
        public string Message { get; }
        public Exception Exception { get; }

        public PageEventArgs(string url, Exception exception = null, string message = null)
        {
            this.Url = url;
            this.Message = message;
            this.Exception = exception;
        }
    }

    public enum ParseLevel
    {
        NotYet,
        Fetched,
        Parsed
    }
}
