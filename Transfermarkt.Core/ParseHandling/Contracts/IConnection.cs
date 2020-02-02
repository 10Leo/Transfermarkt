using System;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IConnection<TNode>
    {
        TNode Connect(string url);
        TNode GetNode();
    }
}