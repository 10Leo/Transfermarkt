using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core.Parsers.HtmlAgilityPack.Competition
{
    class ImgUrlParser : IElementParser<HtmlNode, string>
    {
        private string displayName = "Img Url";
        private bool parsedAlready = false;

        public IConverter<string> Converter { get; set; }

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

        public string Parse(HtmlNode node)
        {
            string parsedObj = null;

            try
            {
                var parsedStr = node.SelectSingleNode("//div[@id='wettbewerb_head']//div[@class='headerfoto']/img")?.GetAttributeValue<string>("src", null);

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

    static class HtmlAgilityPackUtil
    {
        public static T GetAttributeValue<T>(this HtmlNode td, string key, T defaultValue) where T : IConvertible
        {
            T result = defaultValue;//default(T);

            if (td.Attributes[key] == null)// || String.IsNullOrEmpty(td.Attributes[key].Value) == false)
            {
                return defaultValue;
            }

            string value = td.Attributes[key].Value;

            try
            {
                result = (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                result = defaultValue;//default(T);
            }

            return result;
        }
    }
}
