﻿using System;
namespace Transfermarkt.Core.ParseHandling.Contracts
{
    abstract class ElementParser<TElement, TNode> : IElementParser<TElement, TNode, object> where TElement: IElement
    {
        private bool parsedAlready = false;

        public Predicate<TNode> CanParsePredicate { get; set; }
        public Func<TNode, TElement> ParseFunc { get; set; }


        public IConverter<object> Converter { get; set; }

        public event EventHandler<ParserEventArgs<TNode, TElement>> OnSuccess;
        public event EventHandler<ParserEventArgs<TNode, TElement>> OnFailure;

        public virtual bool CanParse(TNode node)
        {
            //if (parsedAlready) { return false; }
            return (CanParsePredicate != null) ? CanParsePredicate(node) : false;
        }

        public virtual TElement Parse(TNode node)
        {
            TElement e = default;
            try
            {
                e.Value = ParseFunc(node).Value;

                OnSuccess?.Invoke(this, new ParserEventArgs<TNode, TElement>(node, Element));
                parsedAlready = true;
            }
            catch (Exception ex)
            {
                OnFailure?.Invoke(this, new ParserEventArgs<TNode, TElement>(node, Element, ex));
            }

            return e;
        }
    }
}
