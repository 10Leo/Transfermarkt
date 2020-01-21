using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core.Pages
{
    public interface IHAPClubPage : IClubPage<HtmlNode, IDomain>
    {
    }
}
