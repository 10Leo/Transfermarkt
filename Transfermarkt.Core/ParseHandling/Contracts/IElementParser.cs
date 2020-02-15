using System;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IElementParser<out TElement, TNode, out TValue> where TElement : IElement<TValue>
    {
        //IConverter<TValue> Converter { get; set; }

        //event EventHandler<ParserEventArgs<TNode, TValue>> OnSuccess;
        //event EventHandler<ParserEventArgs<TNode, TValue>> OnFailure;

        bool CanParse(TNode node);
        TElement Parse(TNode node);
    }

    public class ParserEventArgs<TNode, TValue> : EventArgs
    {
        public TNode Node { get; }
        public IElement<TValue> Element { get; }
        public string Message { get; }
        public Exception Exception { get; }

        public ParserEventArgs(TNode node, IElement<TValue> parsedElement, Exception exception = null, string message = null)
        {
            this.Node = node;
            this.Element = parsedElement;
            this.Message = message;
            this.Exception = exception;
        }
    }
}