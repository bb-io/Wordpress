using Apps.Wordpress.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Wordpress.Models.Requests.Page;

public class PageOptionalRequest
{
    [Display("Page ID")] 
    [DataSource(typeof(PageDataHandler))]
    public string? Id { get; set; }
}