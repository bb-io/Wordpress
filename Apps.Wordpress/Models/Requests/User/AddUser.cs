using Blackbird.Applications.Sdk.Common;

namespace Apps.Wordpress.Models.Requests.User;

public class AddUser
{
    public string Email { get; set; }
    [Display("User name")] public string Username { get; set; }
    public string Password { get; set; }
    public List<string> Roles { get; set; }
}