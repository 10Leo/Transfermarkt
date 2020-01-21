using System;

namespace Transfermarkt.Core.Contracts
{
    public interface IElementParser<TNode, TReturn>
    {
        event EventHandler OnSuccess;
        event EventHandler OnFailure;

        bool CanParse(TNode node);
        TReturn Parse(TNode node);
    }
}