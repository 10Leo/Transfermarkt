using HtmlAgilityPack;
using System;
using System.Net;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Contracts.Page;

namespace Transfermarkt.Core.ParseHandling.Pages
{
    //public class ContinentPage : IContinentPage<HtmlNode>
    //{
    //    private readonly string url;
    //    private HtmlDocument doc;

    //    public IDomain Domain { get; set; }

    //    public ICompetitionPage<HtmlNode> Competition { get; set; }

    //    public ContinentPage(string url)
    //    {
    //        this.url = url;

    //        this.Domain = new Continent();
    //    }

    //    public void Parse()
    //    {
    //    }

    //    public void Save()
    //    {
    //    }

    //    private void Connect()
    //    {
    //        //TODO: transform this in a service (generic) that connects to the page
    //        try
    //        {
    //            string htmlCode = "";
    //            using (WebClient client = new WebClient())
    //            {
    //                client.Encoding = System.Text.Encoding.GetEncoding("UTF-8");
    //                client.Headers.Add(HttpRequestHeader.UserAgent, "AvoidError");
    //                htmlCode = client.DownloadString(url);
    //            }
    //            doc = new HtmlAgilityPack.HtmlDocument();
    //            doc.LoadHtml(htmlCode);
    //        }
    //        catch (System.Net.WebException ex)
    //        {
    //            //Debug.WriteLine(ex.StackTrace);
    //            System.Environment.Exit(-1);
    //        }
    //        catch (Exception ex)
    //        {
    //            //Debug.WriteLine(ex.StackTrace);
    //        }
    //    }

    //    private void LogSuccess(Object o, CustomEventArgs e)
    //    {
    //        Console.WriteLine(".");
    //    }

    //    private void LogFailure(Object o, CustomEventArgs e)
    //    {
    //        Console.WriteLine(e.Message);
    //    }
    //}
}
