using Apps.Wordpress.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Wordpress.Models.Requests;

public class LanguageOptionalRequest
{
    [Display("Language (P)")]
    [DataSource(typeof(LanguageDataHandler))]
    public string? Language { get; set; }
}