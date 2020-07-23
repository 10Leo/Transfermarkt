using System;

namespace Page.Parser.Contracts
{
    public interface IConnection<TNode>
    {
        bool IsConnected { get; }

        TNode Connect(string url);
        TNode GetNode();
        void Reset();
    }
}