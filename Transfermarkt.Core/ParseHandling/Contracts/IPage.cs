﻿using System;
using System.Collections.Generic;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IPage<TDomain, TNode, TElement, TValue> where TDomain : IDomain<TValue> where TElement : IElement<TValue>
    {
        TDomain Domain { get; set; }

        IConnection<TNode> Connection { get; set; }

        IReadOnlyList<ISection<TNode, TElement, TValue>> Sections { get; set; }

        event EventHandler<PageEventArgs> OnBeforeParse;
        event EventHandler<PageEventArgs> OnAfterParse;

        TDomain Parse(string url);
        void Save();
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
