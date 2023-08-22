using Apps.Wordpress.Api;
using Apps.Wordpress.Models.Entities;
using Apps.Wordpress.Models.Requests.User;
using Apps.Wordpress.Models.Responses.All;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Wordpress.Actions;

[ActionList]
public class UserActions : BaseInvocable
{
    private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;

    public UserActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    #region Get

    [Action("Get all users", Description = "Get all users")]
    public async Task<AllUsersResponse> GetAllUsers()
    {
        var client = new CustomWordpressClient(Creds);
        var users = await client.Users.GetAllAsync(false, true);

        return new()
        {
            User = users.Select(x => new WordPressUser(x)).ToList()
        };
    }

    [Action("Get user", Description = "Get user by ID")]
    public async Task<WordPressUser> GetUserById(
        [ActionParameter] UserRequest user)
    {
        var client = new CustomWordpressClient(Creds);
        var response = await client.Users.GetByIDAsync(user.UserId, true, true);

        return new(response);
    }

    #endregion

    #region Create

    [Action("Add user", Description = "Add user")]
    public async Task<WordPressUser> AddUser(
        IEnumerable<AuthenticationCredentialsProvider> Creds,
        [ActionParameter] AddUser request)
    {
        var client = new CustomWordpressClient(Creds);
        var user = await client.Users.CreateAsync(new()
        {
            Email = request.Email,
            UserName = request.UserName,
            Password = request.Password,
            Roles = request.Roles
        });

        return new(user);
    }

    #endregion
}