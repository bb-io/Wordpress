using Blackbird.Applications.Sdk.Common;

namespace Apps.Wordpress.Models.Requests;

public class AddUser
{
    public string Email { get; set; }
    [Display("User name")] public string UserName { get; set; }
    public string Password { get; set; }
    public List<string> Roles { get; set; }
}