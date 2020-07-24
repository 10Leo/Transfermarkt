using System;

namespace Page.Scraper.Contracts
{
    public abstract class ElementParser<TElement, TValue, TNode> : IElementParser<TElement, TValue, TNode> where TElement : IElement<TValue, IConverter<TValue>>, new() where TValue : IValue
    {
        private bool parsedAlready = false;

        public Predicate<TNode> CanParsePredicate { get; set; }
        public Func<TNode, TElement> ParseFunc { get; set; }

        public event EventHandler<ParserEventArgs<TNode>> OnSuccess;
        public event EventHandler<ParserEventArgs<TNode>> OnFailure;

        public virtual bool CanParse(TNode node)
        {
            if (parsedAlready)
            {
                return false;
            }

            return (CanParsePredicate != null) ? CanParsePredicate(node) : false;
        }

        public virtual TElement Parse(TNode node)
        {
            TElement e = new TElement();

            try
            {
                e = ParseFunc(node);

                OnSuccess?.Invoke(this, new ParserEventArgs<TNode>(node, (e.InternalName, e.Value?.ToString())));
            }
            catch (Exception ex)
            {
                OnFailure?.Invoke(this, new ParserEventArgs<TNode>(node, (e.InternalName, e.Value?.ToString()), ex));
            }
            finally
            {
                // Either this Parser succeeded or not, it was already invoked
                parsedAlready = true;
            }

            return e;
        }

        public void Reset()
        {
            parsedAlready = false;
        }
    }
}
