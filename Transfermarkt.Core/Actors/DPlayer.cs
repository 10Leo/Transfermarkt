using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.Actors
{
    class DClub : ID
    {
        public IList<IElement> Elements { get; set; }

        public IList<ID> Children { get; set; }

        public DClub()
        {
            Children = new List<ID>();

            Elements = new List<IElement>();
            Elements.Add(new PlayerName());
            Elements.Add(new Height());
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
                    , Children.Select(
                        p => p.ToString()
                    )
                )
                , string.Join(
                    ", "
                    , Elements.Select(
                        p => p.ToString()
                    )
                )
            );
        }
    }

    class DPlayer : ID
    {
        public IList<IElement> Elements { get; set; }
        public IList<ID> Children { get; set; } = null;

        public DPlayer()
        {
            Elements = new List<IElement>();
            Children = new List<ID>();
            Elements.Add(new MarketValue());
            Elements.Add(new Height());
        }

        public IElement SetElement(IElement element)
        {
            if (element == null)
            {
                return null;
            }
            var elementType = element.GetType();

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
                    , Children.Select(
                        p => p.ToString()
                    )
                )
                , string.Join(
                    ", "
                    , Elements.Select(
                        p => p.ToString()
                    )
                )
            );
        }
    }

    public class PlayerName : IElement
    {
        public string Name { get; set; }
        public dynamic Value { get; set; }

        public PlayerName()
        {
            Name = "Name";
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Name, Value);
        }
    }

    public class MarketValue : IElement
    {
        public string Name { get; }
        public dynamic Value { get; set; }

        public MarketValue()
        {
            Name = "Market Value";
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Name, Value);
        }
    }

    public class Height : IElement
    {
        public string Name { get; set; }
        public dynamic Value { get; set; }

        public Height()
        {
            Name = "Height";
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Name, Value);
        }
    }

    public interface ID
    {
        IList<IElement> Elements { get; set; }

        IList<ID> Children { get; set; }

        IElement SetElement(IElement element);
    }

    public interface IElement
    {
        string Name { get; }
        dynamic Value { get; set; }
    }
}
