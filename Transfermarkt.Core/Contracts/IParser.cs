using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Converters;

namespace Transfermarkt.Core.Contracts
{
    public interface IParser
    {
        IPageConnector Connector { get; set; }
        ConvertersCollection Converters { get; set; }

        Competition ParseSquadsFromCompetition(string url);
        Club ParseSquad(string url);
    }
}
