using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IValue
    {
        Type Type { get; }
        string ToString();
    }

    //public class AValue : IValue
    //{
    //    public Type Type { get; }

    //    public string Value { get; set; }

    //    public override string ToString()
    //    {
    //        return string.Format("{0}", Value);
    //    }
    //}

    public class StringValue : IValue
    {
        public Type Type => typeof(string);
        public string Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", Value);
        }
    }
    public class IntValue : IValue
    {
        public Type Type => typeof(int?);

        public int? Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", Value);
        }
    }
    public class DecimalValue : IValue
    {
        public Type Type => typeof(decimal?);
        public decimal? Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", Value);
        }
    }
    public class DatetimeValue : IValue
    {
        public Type Type => typeof(DateTime?);
        public DateTime? Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", Value);
        }
    }
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
}
