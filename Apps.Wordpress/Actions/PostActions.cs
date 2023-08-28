using System.Net.Mime;
using System.Text;
using Apps.Wordpress.Api;
using Apps.Wordpress.Api.RestSharp;
using Apps.Wordpress.Constants;
using Apps.Wordpress.Extensions;
using Apps.Wordpress.Models.Entities;
using Apps.Wordpress.Models.Requests;
using Apps.Wordpress.Models.Requests.Page;
using Apps.Wordpress.Models.Requests.Post;
using Apps.Wordpress.Models.Responses;
using Apps.Wordpress.Models.Responses.All;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.Extensions.System;
using Blackbird.Applications.Sdk.Utils.Html.Extensions;
using Blackbird.Applications.Sdk.Utils.Parsers;
using RestSharp;
using WordPressPCL.Models;

namespace Apps.Wordpress.Actions;

[ActionList]
public class PostActions : BaseInvocable
{
    private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;
    
    public PostActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }
    
    #region Get

    [Action("Get all posts", Description = "Get all posts")]
    public async Task<AllPostsResponse> GetAllPosts([ActionParameter] ListArticlesRequest input)
    {
        var client = new WordpressRestClient(Creds);

        var query = new Dictionary<string, string>()
        {
            { "after", input.CreatedInLastHours.GetPastHoursDate()},
            { "modified_after", input.UpdatedInLastHours.GetPastHoursDate()},
        }.AllIsNotNull();
        
        var endpoint = ApiEndpoints.Posts.WithQuery(query);
        var request = new WordpressRestRequest(endpoint, Method.Get, Creds);

        var items = await client.Paginate<Post>(request);

        return new()
        {
            Posts = items.Select(x => new WordPressItem(x)).ToList()
        };
    }

    [Action("Get post", Description = "Get post by ID")]
    public async Task<WordPressItem> GetPostById([ActionParameter] PostRequest input)
    {
        var client = new CustomWordpressClient(Creds);
        var post = await client.Posts.GetByIDAsync(input.PostId);

        return new(post);
    }
    
    [Action("Get post as HTML", Description = "Get post by id as HTML file")]
    public async Task<FileResponse> GetPostByIdAsHtml([ActionParameter] PostRequest input)
    {
        var client = new CustomWordpressClient(Creds);
        var post = await client.Posts.GetByIDAsync(input.PostId);

        var html = (post.Title.Rendered, post.Content.Rendered).AsHtml();
        
        return new(new(Encoding.UTF8.GetBytes(html))
        {
            Name = post.Title.Rendered,
            ContentType = MediaTypeNames.Text.Html
        });
    }

    #endregion

    #region Post

    [Action("Create post", Description = "Create post")]
    public async Task<WordPressItem> CreatePost([ActionParameter] CreateRequest request)
    {
        var client = new CustomWordpressClient(Creds);
        var post = await client.Posts.CreateAsync(new()
        {
            Title = new(request.Title),
            Content = new(request.Content)
        });

        return new(post);
    }    
    
    [Action("Create post from HTML", Description = "Create post from HTML file")]
    public async Task<WordPressItem> CreatePostFromHtml([ActionParameter] CreateFromFileRequest request)
    {
        var client = new CustomWordpressClient(Creds);
        
        var html = Encoding.UTF8.GetString(request.File.Bytes);
        var htmlDocument = html.AsHtmlDocument();
        
        var post = await client.Posts.CreateAsync(new()
        {
            Title = new(htmlDocument.GetTitle()),
            Content = new(htmlDocument.GetBody())
        });

        return new(post);
    }

    #endregion
    
    #region Update

    [Action("Update post", Description = "Update post")]
    public async Task<WordPressItem> UpdatePost(
        [ActionParameter] PostRequest post,
        [ActionParameter] UpdateRequest request)
    {
        var client = new CustomWordpressClient(Creds);
        var response = await client.Posts.UpdateAsync(new()
        {
            Id = IntParser.Parse(post.PostId, nameof(post.PostId))!.Value,
            Title = new(request.Title),
            Content = new(request.Content)
        });

        return new(response);
    }    
    
    [Action("Update post from HTML", Description = "Update post from HTML file")]
    public async Task<WordPressItem> UpdatePostFromHtml(
        [ActionParameter] PostRequest post,
        [ActionParameter] UpdateFromFileRequest request)
    {
        var client = new CustomWordpressClient(Creds);
        
        var html = Encoding.UTF8.GetString(request.File.Bytes);
        var htmlDocument = html.AsHtmlDocument();

        var response = await client.Posts.UpdateAsync(new()
        {
            Id = IntParser.Parse(post.PostId, nameof(post.PostId))!.Value,
            Title = new(htmlDocument.GetTitle()),
            Content = new(htmlDocument.GetBody())
        });

        return new(response);
    }

    #endregion

    #region Delete

    [Action("Delete post", Description = "Delete post")]
    public Task DeletePost([ActionParameter] PostRequest post)
    {
        var client = new CustomWordpressClient(Creds);

        var intPostId = IntParser.Parse(post.PostId, nameof(post.PostId))!.Value;
        return client.Posts.DeleteAsync(intPostId);
    }

    #endregion
}