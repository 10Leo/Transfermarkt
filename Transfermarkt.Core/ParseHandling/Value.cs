using Page.Parser.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.ParseHandling
{
    public class NationalityValue : IValue
    {
        public Type Type => typeof(Actors.Nationality?);
        public Actors.Nationality? Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", Value);
        }
    }
    public class PositionValue : IValue
    {
        public Type Type => typeof(Actors.Position?);
        public Actors.Position? Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", Value);
        }
    }
    public class FootValue : IValue
    {
        public Type Type => typeof(Actors.Foot?);
        public Actors.Foot? Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", Value);
        }
    }

    public class ContinentCodeValue : IValue
    {
        public Type Type => typeof(Actors.ContinentCode?);
        public Actors.ContinentCode? Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", Value);
        }
    }
}
