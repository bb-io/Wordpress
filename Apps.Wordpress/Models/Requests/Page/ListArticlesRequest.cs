using Blackbird.Applications.Sdk.Common;

namespace Apps.Wordpress.Models.Requests.Page;

public class ListArticlesRequest
{
    [Display("Updated in last hours")]
    public int? UpdatedInLastHours { get; set; }
    
    [Display("Created in last hours")]
    public int? CreatedInLastHours { get; set; }
}