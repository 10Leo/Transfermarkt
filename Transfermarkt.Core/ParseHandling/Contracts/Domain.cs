using System.Collections.Generic;
using System.Linq;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public abstract class Domain : IDomain
    {
        public IList<IElement> Elements { get; set; }

        public IList<IDomain> Children { get; set; }

        public Domain()
        {
            Elements = new List<IElement>();
            Children = new List<IDomain>();
        }

        public IElement SetElement(IElement element)
        {
            if (element == null)
            {
                return null;
            }

            var elementType = element.GetType();
            var thisElement = Elements.FirstOrDefault(e => e.GetType() == elementType);

            if (thisElement == null)
            {
                return null;
            }
            thisElement.Value = element.Value;
            return thisElement;

            //foreach (var e in Elements)
            //{
            //    if (e.GetType() == elementType)
            //    {
            //        e.Value = element.Value;
            //        return e;
            //    }
            //}

            //return null;
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
