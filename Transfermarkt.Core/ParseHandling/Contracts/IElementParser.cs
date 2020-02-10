using System;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IElementParser<TNode, TElement, TValue> where TElement : IElement
    {
        TElement Element { get; }
        IConverter<TValue> Converter { get; set; }

        event EventHandler<ParserEventArgs<TNode, TElement>> OnSuccess;
        event EventHandler<ParserEventArgs<TNode, TElement>> OnFailure;

        bool CanParse(TNode node);
        TElement Parse(TNode node);
    }

    public class ParserEventArgs<TNode, TElement> : EventArgs
    {
        public TNode Node { get; }
        public TElement Element { get; }
        public string Message { get; }
        public Exception Exception { get; }

        public ParserEventArgs(TNode node, TElement parsedObj, Exception exception = null, string message = null)
        {
            this.Node = node;
            this.Element = parsedObj;
            this.Message = message;
            this.Exception = exception;
        }
    }
}