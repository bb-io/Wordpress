using Apps.Wordpress.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Wordpress.Models.Requests.Page;

public class ListArticlesRequest
{
    [Display("Updated in last hours")]
    public int? UpdatedInLastHours { get; set; }
    
    [Display("Created in last hours")]
    public int? CreatedInLastHours { get; set; }

    [Display("Language (P)")]
    [DataSource(typeof(LanguageDataHandler))]
    public string? Language { get; set; }

}