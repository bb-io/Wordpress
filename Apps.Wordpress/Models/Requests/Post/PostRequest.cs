using Apps.Wordpress.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Wordpress.Models.Requests.Post
{
    public class PostRequest
    {
        [Display("Post")] 
        [DataSource(typeof(PostDataHandler))]
        public string PostId { get; set; }
    }
}
