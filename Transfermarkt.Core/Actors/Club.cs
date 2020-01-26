using System.Collections.Generic;
using System.Linq;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Club;

namespace Transfermarkt.Core.Actors
{
    public class Club : IDomain
    {
        public IList<IElement> Elements { get; set; }

        public IList<IDomain> Children { get; set; }

        public Club()
        {
            Elements = new List<IElement>
            {
                new Name()
            };

            Children = new List<IDomain>();
        }

        public IElement SetElement(IElement element)
        {
            if (element == null)
            {
                return null;
            }
            var elementType = element.GetType();

            //var thisElement = Elements.FirstOrDefault(p => p.GetType() == elementType);
            //thisElement.Value = element.Value;

            foreach (var e in Elements)
            {
                if (e.GetType() == elementType)
                {
                    e.Value = element.Value;
                    return e;
                }
            }

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
