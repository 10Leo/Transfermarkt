using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    //public interface IVal : IValue<>
    //{

    //}

    public interface IValue<T>
    {
        T Value { get; set; }
    }

    public interface StringValue: IValue<string> { }
    public interface IntValue: IValue<int> { }

    interface ITyp { }

    public class Int : ITyp
    {
        int Value { get; set; }
    }
}
