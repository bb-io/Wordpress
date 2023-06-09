﻿using System.Text;
using Apps.Wordpress.Extension;
using Apps.Wordpress.Models.Requests;
using Apps.Wordpress.Models.Responses;
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
    
    [Action("Get posts created in last hours", Description = "Get all posts that were created in last specified number of hours")]
    public async Task<AllPostsResponse> GetFilteredPostsByCreateDate(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] [Display("Hours")] int hoursNumber)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        var posts = await client.Posts.GetAllAsync(true, true);

        return new()
        {
            Posts = posts
                .Where(x => x.DateGmt.AddHours(hoursNumber) >= DateTime.Now.ToUniversalTime())
                .Select(p => new WordPressItem(p)).ToList()
        };
    }       
    
    [Action("Get posts modified in last hours", Description = "Get all posts that were modified in last specified number of hours")]
    public async Task<AllPostsResponse> GetFilteredPostsByModifyDate(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] [Display("Hours")] int hoursNumber)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        var posts = await client.Posts.GetAllAsync(true, true);

        return new()
        {
            Posts = posts
                .Where(x => x.ModifiedGmt.AddHours(hoursNumber) >= DateTime.Now.ToUniversalTime())
                .Select(p => new WordPressItem(p)).ToList()
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
    
    [Action("Get post as HTML", Description = "Get post by id as HTML file")]
    public async Task<FileResponse> GetPostByIdAsHtml(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] PostRequest input)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        var post = await client.Posts.GetByIDAsync(input.PostId);

        var html = (post.Title.Rendered, post.Content.Rendered).AsHtml();
        
        return new(Encoding.UTF8.GetBytes(html));
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
    
    [Action("Create post from HTML", Description = "Create post from HTML file")]
    public async Task<WordPressItem> CreatePostFromHtml(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] CreateFromFileRequest request)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        
        var html = Encoding.UTF8.GetString(request.File);
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
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] UpdateRequest request)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        var post = await client.Posts.UpdateAsync(new()
        {
            Id = request.Id,
            Title = new(request.Title),
            Content = new(request.Content)
        });

        return new(post);
    }    
    
    [Action("Update post from HTML", Description = "Update post from HTML file")]
    public async Task<WordPressItem> UpdatePostFromHtml(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] UpdateFromFileRequest request)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        
        var html = Encoding.UTF8.GetString(request.File);
        var htmlDocument = html.AsHtmlDocument();

        var post = await client.Posts.UpdateAsync(new()
        {
            Id = request.Id,
            Title = new(htmlDocument.GetTitle()),
            Content = new(htmlDocument.GetBody())
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