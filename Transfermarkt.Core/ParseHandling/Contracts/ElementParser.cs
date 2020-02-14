using System;
namespace Transfermarkt.Core.ParseHandling.Contracts
{
    abstract class ElementParser<TElement, TNode> : IElementParser<TElement, TNode, object> where TElement: IElement, new()
    {
        private bool parsedAlready = false;

        public Predicate<TNode> CanParsePredicate { get; set; }
        public Func<TNode, TElement> ParseFunc { get; set; }

        public IConverter<object> Converter { get; set; }

        public event EventHandler<ParserEventArgs<TNode>> OnSuccess;
        public event EventHandler<ParserEventArgs<TNode>> OnFailure;

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
                e.Value = ParseFunc(node).Value;

                OnSuccess?.Invoke(this, new ParserEventArgs<TNode>(node, e));
                parsedAlready = true;
            }
            catch (Exception ex)
            {
                OnFailure?.Invoke(this, new ParserEventArgs<TNode>(node, e, ex));
            }

            return e;
        }
    }
}
