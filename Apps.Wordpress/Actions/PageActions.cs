using System.Globalization;
using System.Net.Mime;
using System.Text;
using Apps.Wordpress.Api;
using Apps.Wordpress.Api.RestSharp;
using Apps.Wordpress.Constants;
using Apps.Wordpress.Extensions;
using Apps.Wordpress.Models.Entities;
using Apps.Wordpress.Models.Requests;
using Apps.Wordpress.Models.Requests.Page;
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
public class PageActions : BaseInvocable
{
    private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;

    public PageActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    #region Get

    [Action("Get all pages", Description = "Get all pages content")]
    public async Task<AllPagesResponse> GetAllPages([ActionParameter] ListArticlesRequest input)
    {
        var client = new WordpressRestClient(Creds);

        var query = new Dictionary<string, string>()
        {
            { "after", input.CreatedInLastHours.GetPastHoursDate()},
            { "modified_after", input.UpdatedInLastHours.GetPastHoursDate()},
        }.AllIsNotNull();
        
        var endpoint = ApiEndpoints.Pages.WithQuery(query);
        var request = new WordpressRestRequest(endpoint, Method.Get, Creds);

        var items = await client.Paginate<Page>(request);
        return new()
        {
            Pages = items.Select(p => new WordPressItem(p)).ToList()
        };
    }

    [Action("Get page", Description = "Get page by ID")]
    public async Task<WordPressItem> GetPageById(
        [ActionParameter] PageRequest input)
    {
        var client = new CustomWordpressClient(Creds);
        var page = await client.Pages.GetByIDAsync(input.PageId);

        return new(page);
    }

    [Action("Get page as HTML", Description = "Get page by id as HTML file")]
    public async Task<FileResponse> GetPageByIdAsHtml([ActionParameter] PageRequest input)
    {
        var client = new CustomWordpressClient(Creds);
        var page = await client.Pages.GetByIDAsync(input.PageId);

        var html = (page.Title.Rendered, page.Content.Rendered).AsHtml();

        return new(new(Encoding.UTF8.GetBytes(html))
        {
            Name = page.Title.Rendered,
            ContentType = MediaTypeNames.Text.Html
        });
    }

    #endregion

    #region Create

    [Action("Create page", Description = "Create page")]
    public async Task<WordPressItem> CreatePage([ActionParameter] CreateRequest request)
    {
        var client = new CustomWordpressClient(Creds);
        var page = await client.Pages.CreateAsync(new()
        {
            Title = new(request.Title),
            Content = new(request.Content)
        });

        return new(page);
    }

    [Action("Create page from HTML", Description = "Create page from HTML file")]
    public async Task<WordPressItem> CreatePageFromHtml([ActionParameter] CreateFromFileRequest request)
    {
        var client = new CustomWordpressClient(Creds);

        var html = Encoding.UTF8.GetString(request.File.Bytes);
        var htmlDocument = html.AsHtmlDocument();

        var page = await client.Pages.CreateAsync(new()
        {
            Title = new(htmlDocument.GetTitle()),
            Content = new(htmlDocument.GetBody())
        });

        return new(page);
    }

    #endregion

    #region Update

    [Action("Update page", Description = "Update page")]
    public async Task<WordPressItem> UpdatePage(
        [ActionParameter] PageRequest page,
        [ActionParameter] UpdateRequest request)
    {
        var client = new CustomWordpressClient(Creds);
        var response = await client.Pages.UpdateAsync(new()
        {
            Id = IntParser.Parse(page.PageId, nameof(page.PageId))!.Value,
            Title = new(request.Title),
            Content = new(request.Content)
        });

        return new(response);
    }

    [Action("Update page from HTML", Description = "Update page from HTML file")]
    public async Task<WordPressItem> UpdatePageFromHtml(
        [ActionParameter] PageRequest page,
        [ActionParameter] UpdateFromFileRequest request)
    {
        var client = new CustomWordpressClient(Creds);

        var html = Encoding.UTF8.GetString(request.File.Bytes);
        var htmlDocument = html.AsHtmlDocument();

        var response = await client.Pages.UpdateAsync(new()
        {
            Id = IntParser.Parse(page.PageId, nameof(page.PageId))!.Value,
            Title = new(htmlDocument.GetTitle()),
            Content = new(htmlDocument.GetBody())
        });

        return new(response);
    }

    #endregion

    #region Delete

    [Action("Delete page", Description = "Delete page")]
    public Task DeletePage([ActionParameter] PageRequest page)
    {
        var client = new CustomWordpressClient(Creds);

        var intPageId = IntParser.Parse(page.PageId, nameof(page.PageId))!.Value;
        return client.Pages.DeleteAsync(intPageId);
    }

    #endregion
}