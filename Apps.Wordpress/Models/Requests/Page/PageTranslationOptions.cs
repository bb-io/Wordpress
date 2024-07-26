using Apps.Wordpress.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Wordpress.Models.Requests.Page;

public class PageTranslationOptions
{
    [Display("Language (P)")]
    [DataSource(typeof(LanguageDataHandler))]
    public string? Language { get; set; }

    [Display("As translation of (P)")]
    [DataSource(typeof(PageDataHandler))]
    public string? ParentId { get; set; }
}