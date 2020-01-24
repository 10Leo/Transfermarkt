using HtmlAgilityPack;
using System;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Club
{
    class CountryParser// : IElementParser<HtmlNode, Nationality?>
    {
        private string displayName = "Country";
        private bool parsedAlready = false;

        public IConverter<Nationality?> Converter { get; set; }

        public event EventHandler<CustomEventArgs> OnSuccess;
        public event EventHandler<CustomEventArgs> OnFailure;

        public bool CanParse(HtmlNode node)
        {
            //if (parsedAlready)
            //{
            //    return false;
            //}
            return true;
        }

        public Nationality? Parse(HtmlNode node)
        {
            Nationality? parsedObj = null;

            try
            {
                HtmlNode countryNode = node.SelectSingleNode("//div[@id='verein_head']//span[@class='mediumpunkt']//img[@class='flaggenrahmen vm']");
                string parsedStr = countryNode?.GetAttributeValue<string>("title", null);

                parsedObj = Converter.Convert(parsedStr);

                OnSuccess?.Invoke(this, new CustomEventArgs($"Success parsing {displayName}."));
                parsedAlready = true;
            }
            catch (Exception)
            {
                OnFailure?.Invoke(this, new CustomEventArgs($"Error parsing {displayName}."));
                throw;
            }

            return parsedObj;
        }
    }
}
