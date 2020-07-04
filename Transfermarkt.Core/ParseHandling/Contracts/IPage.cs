using System;
using System.Collections.Generic;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IPage<TDomain, TElement, TValue, TNode> where TDomain : IDomain<TValue> where TElement : IElement<TValue> where TValue : IValue
    {
        ISection<TElement, TValue, TNode> this[string name] { get; }
        IReadOnlyList<ISection<TElement, TValue, TNode>> Sections { get; set; }

        TDomain Domain { get; set; }

        IConnection<TNode> Connection { get; set; }

        event EventHandler<PageEventArgs> OnBeforeParse;
        event EventHandler<PageEventArgs> OnAfterParse;

        IEnumerable<Link> Fetch(string url);
        TDomain Parse(string url, string sectionName = null, IEnumerable<Link> links = null);
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
}
