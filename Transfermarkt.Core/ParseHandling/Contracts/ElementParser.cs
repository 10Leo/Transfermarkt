using System;
namespace Transfermarkt.Core.ParseHandling.Contracts
{
    abstract class ElementParser<TElement, TValue, TNode> : IElementParser<TElement, TNode, TValue> where TElement: IElement<TValue>, new()
    {
        private bool parsedAlready = false;

        public Predicate<TNode> CanParsePredicate { get; set; }
        public Func<TNode, TElement> ParseFunc { get; set; }

        public IConverter<TValue> Converter { get; set; }

        public event EventHandler<ParserEventArgs<TNode, TValue>> OnSuccess;
        public event EventHandler<ParserEventArgs<TNode, TValue>> OnFailure;

        public virtual bool CanParse(TNode node)
        {
            //if (parsedAlready) { return false; }
            return (CanParsePredicate != null) ? CanParsePredicate(node) : false;
        }

        public virtual TElement Parse(TNode node)
        {
            TElement e = new TElement();

            try
            {
                e = ParseFunc(node);

                OnSuccess?.Invoke(this, new ParserEventArgs<TNode, TValue>(node, e));
                parsedAlready = true;
            }
            catch (Exception ex)
            {
                OnFailure?.Invoke(this, new ParserEventArgs<TNode, TValue>(node, e, ex));
            }

            return e;
        }
    }
}
