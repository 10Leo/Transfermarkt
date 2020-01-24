using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    abstract class ElementParser<TNode, TValue> : IElementParser<TNode, TValue>
    {
        private bool parsedAlready = false;
        public abstract string DisplayName { get; set; }

        public Predicate<TNode> CanParsePredicate { get; set; }
        public Func<TNode, TValue> ParseFunc { get; set; }

        public IConverter<TValue> Converter { get; set; }

        public event EventHandler<CustomEventArgs> OnSuccess;
        public event EventHandler<CustomEventArgs> OnFailure;

        public virtual bool CanParse(TNode node)
        {
            //if (parsedAlready)
            //{
            //    return false;
            //}

            if (CanParsePredicate != null)
            {
                return CanParsePredicate(node);
            }
            return false;
        }

        public virtual TValue Parse(TNode node)
        {
            TValue parsedObj = default;

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
