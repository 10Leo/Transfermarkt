using System;
namespace Transfermarkt.Core.ParseHandling.Contracts
{
    abstract class ElementParser<TNode> : IElementParser<TNode, IElement, object>
    {
        private bool parsedAlready = false;

        public Predicate<TNode> CanParsePredicate { get; set; }
        public Func<TNode, IElement> ParseFunc { get; set; }

        public abstract IElement Element { get; }

        public IConverter<object> Converter { get; set; }

        public event EventHandler<ParserEventArgs<TNode, IElement>> OnSuccess;
        public event EventHandler<ParserEventArgs<TNode, IElement>> OnFailure;

        public virtual bool CanParse(TNode node)
        {
            //if (parsedAlready) { return false; }
            return (CanParsePredicate != null) ? CanParsePredicate(node) : false;
        }

        public virtual IElement Parse(TNode node)
        {
            try
            {
                var e = ParseFunc(node);
                Element.Value = e.Value;

                OnSuccess?.Invoke(this, new ParserEventArgs<TNode, IElement>(node, Element));
                parsedAlready = true;
            }
            catch (Exception ex)
            {
                OnFailure?.Invoke(this, new ParserEventArgs<TNode, IElement>(node, Element, ex));
            }

            return Element;
        }
    }
}
