using System;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IElementParser<out TElement, TNode, TValue> where TElement : IElement
    {
        IConverter<TValue> Converter { get; set; }

        event EventHandler<ParserEventArgs<TNode>> OnSuccess;
        event EventHandler<ParserEventArgs<TNode>> OnFailure;

        bool CanParse(TNode node);
        TElement Parse(TNode node);
    }

    public class ParserEventArgs<TNode> : EventArgs
    {
        public TNode Node { get; }
        public IElement Element { get; }
        public string Message { get; }
        public Exception Exception { get; }

        public ParserEventArgs(TNode node, IElement parsedElement, Exception exception = null, string message = null)
        {
            this.Node = node;
            this.Element = parsedElement;
            this.Message = message;
            this.Exception = exception;
        }
    }
}