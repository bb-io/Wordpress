﻿using System.Net.Mime;
using System.Text;
using Apps.Wordpress.Api;
using Apps.Wordpress.Extensions;
using Apps.Wordpress.Invocables;
using Apps.Wordpress.Models.Dtos;
using Apps.Wordpress.Models.Entities;
using Apps.Wordpress.Models.Polylang;
using Apps.Wordpress.Models.Requests;
using Apps.Wordpress.Models.Requests.Page;
using Apps.Wordpress.Models.Responses;
using Apps.Wordpress.Models.Responses.All;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.Extensions.System;
using Blackbird.Applications.Sdk.Utils.Html.Extensions;
using Newtonsoft.Json;
using RestSharp;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Wordpress.Actions;

[ActionList]
public class PageActions : WordpressInvocable
{
    private const string Endpoint = "pages";

    private readonly IFileManagementClient _fileManagementClient;
    
    public PageActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
        : base(invocationContext)
    {
        _fileManagementClient = fileManagementClient;
    }

    #region Get

    [Action("Search pages", Description = "Search pages given created or updated times. Optionally use the language input to filter by language (with Polylang)")]
    public async Task<AllPagesResponse> GetAllPages([ActionParameter] ListArticlesRequest input)
    {
        var client = new WordpressRestClient(Creds);

        var query = new Dictionary<string, string>()
        {
            { "after", input.CreatedInLastHours.GetPastHoursDate()},
            { "modified_after", input.UpdatedInLastHours.GetPastHoursDate()},
        }.AllIsNotNull();

        if (input.Language != null)
            query["lang"] = input.Language;

        var endpoint = Endpoint.WithQuery(query);
        var request = new WordpressRestRequest(endpoint, Method.Get, Creds);

        var items = await client.Paginate<BaseDto>(request);

        return new()
        {
            Pages = items.Select(p => new WordPressItem(p)).ToList()
        };
    }

    [Action("Get page", Description = "Get page by ID")]
    public async Task<WordPressItem> GetPageById([ActionParameter] PageRequest input)
    {
        var client = new WordpressRestClient(Creds);
        var request = new WordpressRestRequest(Endpoint + $"/{input.Id}", Method.Get, Creds);
        var post = await client.ExecuteWithHandling(request);

        var dto = JsonConvert.DeserializeObject<BaseDto>(post.Content!)!;
        return new(dto);
    }

    [Action("Get page missing translations (P)", Description = "Gets all the languages that are missing for this page.")]
    public async Task<MissingTranslations> GetPageMissingTranslations([ActionParameter] PageRequest input)
    {
        var client = new WordpressRestClient(Creds);
        var request = new WordpressRestRequest(Endpoint + $"/{input.Id}", Method.Get, Creds);
        var post = await client.ExecuteWithHandling<BaseDto>(request);

        var polylang = new PolylangActions(InvocationContext);
        var allLanguagesResponse = await polylang.GetLanguages();
        var allLanguages = allLanguagesResponse.Languages.Select(x => x.Slug);
        var translatedLanguages = new List<string>(post.Translations.Keys);

        var missingLanguages = allLanguages.Where(x => translatedLanguages.All(y => y != x))!;

        return new MissingTranslations { MissingLanguages = missingLanguages };
    }

    [Action("Get page translation (P)", Description = "Get the translation of a page. Polylang required.")]
    public async Task<WordPressItem> GetTranslationByPage([ActionParameter] PageRequest input, [ActionParameter] LanguageRequest lang)
    {
        var client = new WordpressRestClient(Creds);
        var request = new WordpressRestRequest(Endpoint + $"/{input.Id}", Method.Get, Creds);
        var post = await client.ExecuteWithHandling<BaseDto>(request);

        if (!post.Translations.ContainsKey(lang.Language))
            throw new PluginMisconfigurationException("This page does not have a translation in " + lang.Language);

        var translationId = post.Translations[lang.Language];
        return await GetPageById(new PageRequest { Id = translationId.ToString() });
    }

    [Action("Get page as HTML", Description = "Get page by id as HTML file")]
    public async Task<FileResponse> GetPageByIdAsHtml([ActionParameter] PageRequest input)
    {
        var client = new WordpressRestClient(Creds);
        var request = new WordpressRestRequest(Endpoint + $"/{input.Id}", Method.Get, Creds);
        var post = await client.ExecuteWithHandling<BaseDto>(request);
        
        var html = (post.Title.Rendered, post.Content.Rendered).AsHtml();
        
        var metaTag = $"<meta name=\"blackbird-page-id\" content=\"{input.Id}\">";
        var headIndex = html.IndexOf("<head>", StringComparison.Ordinal) + "<head>".Length;
        html = html.Insert(headIndex, metaTag);
        
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));
        var file = await _fileManagementClient.UploadAsync(stream, MediaTypeNames.Text.Html,
            $"{post.Title.Rendered}.html");
        return new(file);
    }

    #endregion

    #region Post & Update

    [Action("Create page", Description = "Create a new page. With Polylang enabled it can also be used to create translations of other pages.")]
    public Task<WordPressItem> CreatePage([ActionParameter] ModificationRequest input, [ActionParameter] PageTranslationOptions translationOptions)
    {
        return ExecuteModification(input, translationOptions, null);
    }

    [Action("Create page from HTML", Description = "Create a new page from an HTML file. With Polylang enabled it can also be used to create translations of other pages.")]
    public async Task<WordPressItem> CreatePageFromHtml([ActionParameter] FileModificationRequest input, [ActionParameter] PageTranslationOptions translationOptions)
    {
        var fileStream = await _fileManagementClient.DownloadAsync(input.File);
        var fileBytes = await fileStream.GetByteData();
        var html = Encoding.UTF8.GetString(fileBytes);
        return await ExecuteModification(html, translationOptions, null);
    }

    [Action("Update page", Description = "Update page. With Polylang enabled it can also be used to set the language and update its associations.")]
    public Task<WordPressItem> UpdatePage(
        [ActionParameter] PageRequest page,
        [ActionParameter] ModificationRequest input,
        [ActionParameter] PageTranslationOptions translationOptions
        )
    {
        return ExecuteModification(input, translationOptions, page.Id);
    }

    [Action("Update page from HTML", Description = "Update a page from an HTML file. With Polylang enabled it can also be used to set the language and update its associations.")]
    public async Task<WordPressItem> UpdatePageFromHtml(
        [ActionParameter] PageOptionalRequest page,
        [ActionParameter] FileModificationRequest input,
        [ActionParameter] PageTranslationOptions translationOptions
        )
    {
        var fileStream = await _fileManagementClient.DownloadAsync(input.File);
        var fileBytes = await fileStream.GetByteData();
        var html = Encoding.UTF8.GetString(fileBytes);
        var htmlDocument = html.AsHtmlDocument();

        var metaTag = htmlDocument.DocumentNode.SelectSingleNode("//meta[@name='blackbird-page-id']");
        var pageIdValue = metaTag?.GetAttributeValue("content", null);

        var pageId = page.Id ?? pageIdValue
            ?? throw new PluginMisconfigurationException("Page ID not found in HTML file. Please make sure the file was created with the 'Get page as HTML' action, or provide the Page ID.");

        return await ExecuteModification(html, translationOptions, pageId);
    }

    private async Task<WordPressItem> ExecuteModification(string html, PageTranslationOptions translationOptions, string? id)
    {
        var htmlDocument = html.AsHtmlDocument();
        var title = htmlDocument.GetTitle();
        var body = htmlDocument.GetBody();
        return await ExecuteModification(new ModificationRequest { Title = title, Content = body }, translationOptions, id);
    }

    private async Task<WordPressItem> ExecuteModification(ModificationRequest input, PageTranslationOptions translationOptions, string? id)
    {
        var client = new WordpressRestClient(Creds);
        var request = new WordpressRestRequest(Endpoint + (id == null ? "" : $"/{id}"), Method.Post, Creds);

        request.AddJsonBody(new
        {
            title = input.Title,
            content = input.Content
        });

        if (translationOptions.Language != null)
            request.AddQueryParameter("lang", translationOptions.Language);

        if (translationOptions.ParentId != null)
        {
            var parent = await GetPageById(new PageRequest { Id = translationOptions.ParentId });
            request.AddQueryParameter($"translations[{parent.Language}]", translationOptions.ParentId);
        }

        var result = await client.ExecuteWithHandling<BaseDto>(request);

        return new(result);
    }

    #endregion

    #region Delete

    [Action("Delete page", Description = "Delete page")]
    public Task DeletePage([ActionParameter] PageRequest page)
    {
        var request = new WordpressRestRequest($"pages/{page.Id}", Method.Delete, Creds);
        return Client.ExecuteWithHandling(request);
    }

    #endregion
}