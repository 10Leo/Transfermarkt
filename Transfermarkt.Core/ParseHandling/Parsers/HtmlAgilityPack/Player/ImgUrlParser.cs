using HtmlAgilityPack;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class ImgUrlParser : ElementParser<ImgUrl, StringValue, HtmlNode>
    {
        public ImgUrlParser()
        {
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == ParsersConfig.Get(this.GetType(), ConfigType.PLAYER);

            this.ParseFunc = node =>
            {
                //TODO
                var parsedStr = string.Empty;

                return new ImgUrl { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
