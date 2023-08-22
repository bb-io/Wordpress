using Apps.Wordpress.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Wordpress.Models.Requests.User;

public class UserRequest
{
    [Display("User")]
    [DataSource(typeof(UserDataHandler))]
    public string UserId { get; set; }
}