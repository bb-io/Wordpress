using Apps.Wordpress.Constants;

namespace Apps.Wordpress.Extension;

public static class StringExtensions
{
    public static string AsHtml(this (string title, string body) tuple)
        => string.Format(HtmlFormats.Html, tuple.title, tuple.body);
}