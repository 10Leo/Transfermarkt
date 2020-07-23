using System;

namespace Page.Scraper.Contracts
{
    public interface IConnection<TNode>
    {
        bool IsConnected { get; }

        TNode Connect(string url);
        TNode GetNode();
        void Reset();
    }
}