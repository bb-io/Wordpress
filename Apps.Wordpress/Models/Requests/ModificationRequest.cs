using Blackbird.Applications.Sdk.Common;

namespace Apps.Wordpress.Models.Requests;

public class ModificationRequest
{
    [Display("Title")]
    public string? Title { get; set; }

    [Display("Content")]
    public string? Content { get; set; }
}