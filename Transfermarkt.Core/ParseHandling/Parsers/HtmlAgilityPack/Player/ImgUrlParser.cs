using HtmlAgilityPack;
using System.Linq;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class ImgUrlParser : ElementParser<ImgUrl, StringValue, HtmlNode>
    {
        public ImgUrlParser()
        {
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == ParsersConfig.GetLabel(this.GetType(), ConfigType.PLAYER);

            this.ParseFunc = node =>
            {
                var parsedStr = node
                    .SelectNodes("table//td/a/img")
                    .FirstOrDefault(n => n.Attributes["class"]?.Value == "bilderrahmen-fixed")
                    .Attributes["src"].Value;

                return new ImgUrl { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
