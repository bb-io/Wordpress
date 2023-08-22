using Apps.Wordpress.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Wordpress.Models.Requests.Media;

public class MediaRequest
{
    [Display("Media")]
    [DataSource(typeof(MediaDataHandler))]
    public string MediaId { get; set; }
}