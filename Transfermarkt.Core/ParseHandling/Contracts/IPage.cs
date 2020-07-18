using System;
using System.Collections.Generic;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IPage<TDomain, TValue, TNode> where TDomain : IDomain<TValue> where TValue : IValue
    {
        ISection this[string name] { get; }
        IReadOnlyList<ISection> Sections { get; set; }

        string Url { get; }

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
}
