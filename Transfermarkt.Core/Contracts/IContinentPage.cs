using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.Contracts
{
    interface IContinentPage<TNode> : IPage<IDomain>
    {
        ICompetitionPage<TNode> Competition { get; set; }
    }
}
