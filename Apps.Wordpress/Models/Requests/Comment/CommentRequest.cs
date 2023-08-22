using Apps.Wordpress.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Wordpress.Models.Requests.Comment;

public class CommentRequest
{
    [Display("Comment")]
    [DataSource(typeof(CommentDataHandler))]
    public string CommentId { get; set; }
}