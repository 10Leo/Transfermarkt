using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.Actors
{
    class DPlayer : ID
    {
        public IList<IElement> Properties { get; set; }

        public DPlayer()
        {
            Properties.Add(new PlayerName());
            Properties.Add(new Height());
        }
    }

    public class PlayerName : IElement
    {
        public string Name { get; set; }
        public dynamic Value { get; set; }
    }

    public class Height : IElement
    {
        public string Name { get; set; }
        public dynamic Value { get; set; }
    }

    public interface ID
    {
        IList<IElement> Properties { get; set; }

    }

    public interface IElement
    {
        string Name { get; set; }
        dynamic Value { get; set; }
    }
}
