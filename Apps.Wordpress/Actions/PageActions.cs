using System.Text;
using Apps.Wordpress.Models.Requests;
using Apps.Wordpress.Models.Responses;
using Apps.Wordpress.Models.Responses.All;
using Apps.Wordpress.Models.Responses.Entities;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.Wordpress.Actions;

[ActionList]
public class PageActions
{
    #region Get

    [Action("Get all pages", Description = "Get all pages content")]
    public async Task<AllPagesResponse> GetAllPages(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        var pages = await client.Pages.GetAllAsync(true, true);

        return new()
        {
            Pages = pages.Select(p => new WordPressItem(p)).ToList()
        };
    }

    [Action("Get page", Description = "Get page by id")]
    public async Task<WordPressItem> GetPageById(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] PageRequest input)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        var page = await client.Pages.GetByIDAsync(input.PageId);

        return new(page);
    }    
    
    [Action("Get page as HTML", Description = "Get page by id as HTML file")]
    public async Task<FileResponse> GetPageByIdAsHtml(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] PageRequest input)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        var page = await client.Pages.GetByIDAsync(input.PageId);

        return new(page.Title.Rendered,  Encoding.UTF8.GetBytes(page.Content.Rendered));
    }

    #endregion

    #region Create

    [Action("Create page", Description = "Create page")]
    public async Task<WordPressItem> CreatePage(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] CreateRequest request)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        var page = await client.Pages.CreateAsync(new()
        {
            Title = new(request.Title),
            Content = new(request.Content)
        });

        return new(page);
    }
    
    [Action("Create page from HTML", Description = "Create page from HTML file")]
    public async Task<WordPressItem> CreatePageFromHtml(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] CreateFromFileRequest request)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        var page = await client.Pages.CreateAsync(new()
        {
            Title = new(request.Title),
            Content = new(Encoding.UTF8.GetString(request.File))
        });

        return new(page);
    }

    #endregion

    #region Update

    [Action("Update page", Description = "Update page")]
    public async Task<WordPressItem> UpdatePage(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] UpdateRequest request)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        var page = await client.Pages.UpdateAsync(new()
        {
            Id = request.Id,
            Title = new(request.Title),
            Content = new(request.Content)
        });

        return new(page);
    }    
    
    [Action("Update page from HTML", Description = "Update page from HTML file")]
    public async Task<WordPressItem> UpdatePageFromHtml(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] UpdateFromFileRequest request)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        var page = await client.Pages.UpdateAsync(new()
        {
            Id = request.Id,
            Title = new(request.Title),
            Content = new(Encoding.UTF8.GetString(request.File))
        });

        return new(page);
    }

    #endregion

    #region Delete

    [Action("Delete page", Description = "Delete page")]
    public Task DeletePage(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] [Display("Page id")] int pageId)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        return client.Pages.DeleteAsync(pageId);
    }

    #endregion
}