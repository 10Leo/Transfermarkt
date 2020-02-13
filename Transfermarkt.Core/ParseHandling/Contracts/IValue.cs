using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IValue<T>
    {
        T Value { get; set; }
    }

    public interface StringValue: IValue<string> { }
    public interface IntValue: IValue<int> { }
}
