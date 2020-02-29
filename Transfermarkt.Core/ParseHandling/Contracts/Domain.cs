using System.Collections.Generic;
using System.Linq;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public abstract class Domain<TValue> : IDomain<TValue> where TValue : IValue
    {
        public IList<IElement<TValue>> Elements { get; set; }

        public IList<IDomain<TValue>> Children { get; set; }

        public Domain()
        {
            Elements = new List<IElement<TValue>>();
            Children = new List<IDomain<TValue>>();
        }

        public IElement<TValue> SetElement(IElement<TValue> element)
        {
            if (element == null)
            {
                return null;
            }

            var elementType = element.GetType();
            var thisElement = Elements.FirstOrDefault(e => e.GetType() == elementType);
            var index = Elements.IndexOf(thisElement);
            if (index != -1)
                Elements[index] = element;

            if (thisElement == null)
            {
                return null;
            }
            return thisElement;
        }

        public override string ToString()
        {
            return string.Format("{0}\n{1}"
                , string.Join(
                    ", "
                    , Elements.Select(
                        p => p.ToString()
                    )
                )
                , string.Join(
                    ", "
                    , Children.Select(
                        p => p.ToString()
                    )
                )
            );
        }
    }
}
