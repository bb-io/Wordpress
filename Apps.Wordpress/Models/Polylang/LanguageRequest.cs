using Apps.Wordpress.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Wordpress.Models.Polylang;

public class LanguageRequest
{
    [Display("Language")]
    [DataSource(typeof(LanguageDataHandler))]
    public string Language { get; set; }
}