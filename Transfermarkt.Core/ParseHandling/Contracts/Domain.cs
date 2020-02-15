using System.Collections.Generic;
using System.Linq;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public abstract class Domain : IDomain<object>
    {
        public IEnumerable<IElement<object>> Elements { get; set; }

        public IEnumerable<IDomain<object>> Children { get; set; }

        public Domain()
        {
            Elements = new List<IElement<object>>();
            Children = new List<IDomain<object>>();
        }

        public IElement<object> SetElement(IElement<object> element)
        {
            if (element == null)
            {
                return null;
            }
            var elementType = element.GetType();

            //var thisElement = Elements.FirstOrDefault(p => p.GetType() == elementType);
            //thisElement.Value = element.Value;

            //foreach (var e in Elements)
            //{
            //    if (e.GetType() == elementType)
            //    {
            //        e.Value = element.Value;
            //        return e;
            //    }
            //}

            return null;
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
