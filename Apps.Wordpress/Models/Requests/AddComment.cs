using Blackbird.Applications.Sdk.Common;

namespace Apps.Wordpress.Models.Requests;

public class AddComment
{
    [Display("Post id")] public int PostId { get; set; }
    public string Content { get; set; }
}