using System.Collections.Generic;
using System.Linq;

namespace Page.Parser.Contracts
{
    public abstract class Domain : IDomain
    {
        public IList<IElement<IValue>> Elements { get; set; }

        public IList<IDomain> Children { get; set; }

        public Domain()
        {
            Elements = new List<IElement<IValue>>();
            Children = new List<IDomain>();
        }

        public IElement<IValue> SetElement(IElement<IValue> element)
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
