using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core.Parsers.HtmlAgilityPack.Player
{
    abstract class ElementParser<TNode, TValue> : IElementParser<TNode, TValue>
    {
        public IConverter<TValue> Converter { get; set; }

        public event EventHandler<CustomEventArgs> OnSuccess;
        public event EventHandler<CustomEventArgs> OnFailure;

        protected bool parsedAlready = false;

        public virtual bool CanParse(TNode node) {
            return !parsedAlready;
        }

        protected abstract TValue OnParse();

        public TValue Parse(TNode node)
        {
            try
            {
                TValue value = OnParse();
                OnSuccess?.Invoke(this, new CustomEventArgs("Success"));
                return value;
            }
            catch (Exception)
            {
                OnSuccess?.Invoke(this, new CustomEventArgs("Success"));
            }
            return default(TValue);
        }
    }
}
