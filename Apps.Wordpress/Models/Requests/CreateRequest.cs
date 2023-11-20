using Apps.Wordpress.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Wordpress.Models.Requests;

public class CreateRequest
{
    public string Title { get; set; }
    public string Content { get; set; }
}