using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling
{
    abstract class ElementParser<TNode> : IElementParser<TNode, IElement, object>
    {
        private bool parsedAlready = false;
        public abstract string DisplayName { get; set; }

        public Predicate<TNode> CanParsePredicate { get; set; }
        public Func<TNode, IElement> ParseFunc { get; set; }

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
