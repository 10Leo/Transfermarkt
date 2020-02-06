﻿using System;
namespace Transfermarkt.Core.ParseHandling.Contracts
{
    abstract class ElementParser<TNode> : IElementParser<TNode, IElement, object>
    {
        private bool parsedAlready = false;

        public Predicate<TNode> CanParsePredicate { get; set; }
        public Func<TNode, IElement> ParseFunc { get; set; }

        public abstract IElement Element { get; }
        public IConverter<object> Converter { get; set; }

        public event EventHandler<CustomEventArgs> OnSuccess;
        public event EventHandler<CustomEventArgs> OnFailure;

        public virtual bool CanParse(TNode node)
        {
            //if (parsedAlready) { return false; }
            return (CanParsePredicate != null) ? CanParsePredicate(node) : false;
        }

        public virtual IElement Parse(TNode node)
        {
            IElement parsedObj = default;

            try
            {
                parsedObj = ParseFunc(node);

                OnSuccess?.Invoke(this, new CustomEventArgs($"Success parsing {DisplayName}."));
                parsedAlready = true;
            }
            catch (Exception)
            {
                OnFailure?.Invoke(this, new CustomEventArgs($"Error parsing {DisplayName}."));
            }

            return parsedObj;
        }
    }
}
