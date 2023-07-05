using System.Web;
using Apps.Wordpress.Constants;
using HtmlAgilityPack;

namespace Apps.Wordpress.Extension;

public static class HtmlExtensions
{
    public static string GetTitle(this HtmlDocument doc)
        => GetHtmlText(doc, HtmlXPaths.Title);

    public static string GetBody(this HtmlDocument doc)
        => GetHtmlText(doc, HtmlXPaths.Body);

    public static HtmlDocument AsHtmlDocument(this string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        return doc;
    }

    private static string GetHtmlText(HtmlDocument doc, string xPath)
        => HttpUtility.HtmlDecode(doc.DocumentNode.SelectSingleNode(xPath).InnerText);
}