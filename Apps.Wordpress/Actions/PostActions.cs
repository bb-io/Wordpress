using Apps.Wordpress.Models.Requests;
using Apps.Wordpress.Models.Responses.All;
using Apps.Wordpress.Models.Responses.Entities;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.Wordpress.Actions;

[ActionList]
public class PostActions
{
    #region Get

    [Action("Get all posts", Description = "Get all posts")]
    public async Task<AllPostsResponse> GetAllPosts(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        var posts = await client.Posts.GetAllAsync(true, true);

        return new()
        {
            Posts = posts.Select(x => new WordPressItem(x)).ToList()
        };
    }

    [Action("Get post", Description = "Get post by id")]
    public async Task<WordPressItem> GetPostById(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] PostRequest input)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        var post = await client.Posts.GetByIDAsync(input.PostId);

        return new(post);
    }

    #endregion

    #region Post

    [Action("Create post", Description = "Create post")]
    public async Task<WordPressItem> CreatePost(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] CreateRequest request)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        var post = await client.Posts.CreateAsync(new()
        {
            Title = new(request.Title),
            Content = new(request.Content)
        });

        return new(post);
    }

    #endregion

    #region Delete

    [Action("Delete post", Description = "Delete post")]
    public Task DeletePost(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] [Display("Post id")] int postId)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        return client.Posts.DeleteAsync(postId);
    }

    #endregion
}