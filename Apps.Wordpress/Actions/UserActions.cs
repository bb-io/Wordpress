using Apps.Wordpress.Models.Requests;
using Apps.Wordpress.Models.Responses.All;
using Apps.Wordpress.Models.Responses.Entities;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.Wordpress.Actions;

[ActionList]
public class UserActions
{
    #region Get

    [Action("Get all users", Description = "Get all users")]
    public async Task<AllUsersResponse> GetAllUsers(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        var users = await client.Users.GetAllAsync(true, true);

        return new()
        {
            User = users.Select(x => new WordPressUser(x)).ToList()
        };
    }

    [Action("Get user", Description = "Get user by id")]
    public async Task<WordPressUser> GetUserById(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] [Display("User id")] int id)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        var user = await client.Users.GetByIDAsync(id, true, true);

        return new(user);
    }

    #endregion

    #region Create

    [Action("Add user", Description = "Add user")]
    public async Task<WordPressUser> AddUser(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] AddUser request)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
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