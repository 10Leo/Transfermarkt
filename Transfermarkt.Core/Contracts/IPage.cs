using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.Contracts
{
    public interface IPage<TReturn>
    {
        TReturn Value { get; set; }

        void Parse();
        void Save();
    }
}
