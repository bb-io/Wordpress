using Apps.Wordpress.Api;
using Apps.Wordpress.Constants;
using Apps.Wordpress.Invocables;
using Apps.Wordpress.Models.Entities;
using Apps.Wordpress.Models.Requests.User;
using Apps.Wordpress.Models.Responses.All;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;
using WordPressPCL.Models;

namespace Apps.Wordpress.Actions;

[ActionList("Users")]
public class UserActions : WordpressInvocable
{
    public UserActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    #region Get

    [Action("Get all users", Description = "Get all users")]
    public async Task<AllUsersResponse> GetAllUsers()
    {
        var request = new WordpressRestRequest("users", Method.Get, Creds);
        var users = await Client.Paginate<User>(request);

        return new()
        {
            User = users.Select(x => new WordPressUser(x)).ToList()
        };
    }

    [Action("Get user", Description = "Get user by ID")]
    public async Task<WordPressUser> GetUserById(
        [ActionParameter] UserRequest user)
    {
        var request = new WordpressRestRequest($"users/{user.UserId}", Method.Get, Creds);
        var response = await Client.ExecuteWithHandling<User>(request);

        return new(response);
    }

    #endregion

    #region Create

    [Action("Add user", Description = "Add user")]
    public async Task<WordPressUser> AddUser([ActionParameter] AddUser input)
    {
        var request = new WordpressRestRequest("users", Method.Post, Creds)
            .WithJsonBody(new
            {
                input.Email,
                input.Username,
                input.Password,
                input.Roles
            }, JsonConfig.JsonSettings);
        var user = await Client.ExecuteWithHandling<User>(request);

        return new(user);
    }

    #endregion
}