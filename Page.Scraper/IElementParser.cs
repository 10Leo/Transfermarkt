using System;

namespace Page.Scraper.Contracts
{
    public interface IElementParser<out TElement, out TValue, TNode> where TElement : IElement<TValue, IConverter<TValue>> where TValue : IValue
    {
        //IConverter<TValue> Converter { get; set; }

        event EventHandler<ParserEventArgs<TNode>> OnSuccess;
        event EventHandler<ParserEventArgs<TNode>> OnFailure;

        bool CanParse(TNode node);
        TElement Parse(TNode node);
        void Reset();
    }

    public class ParserEventArgs<TNode> : EventArgs
    {
        public TNode Node { get; }
        public (string name, string value) Element { get; }
        public string Message { get; }
        public Exception Exception { get; }

        public ParserEventArgs(TNode node, (string name, string value) parsedElement, Exception exception = null, string message = null)
        {
            this.Node = node;
            this.Element = parsedElement;
            this.Message = message;
            this.Exception = exception;
        }
    }
}