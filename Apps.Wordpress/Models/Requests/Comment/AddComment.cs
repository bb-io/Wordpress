using Apps.Wordpress.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Wordpress.Models.Requests.Comment;

public class AddComment
{
    [Display("Post ID")] 
    [DataSource(typeof(PostDataHandler))]
    public string PostId { get; set; }
    
    public string Content { get; set; }
}