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
using Apps.Wordpress.Models.Requests.Post;
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
using RestSharp;
using WordPressPCL.Models;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Wordpress.Actions;

[ActionList]
public class PostActions : WordpressInvocable
{
    private const string Endpoint = "posts";
    
    private readonly IFileManagementClient _fileManagementClient;
    
    public PostActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
        : base(invocationContext)
    {
        _fileManagementClient = fileManagementClient;
    }
    
    #region Get

    [Action("Search posts", Description = "Search posts given created or updated times. Optionally use the language input to filter by language (with Polylang)")]
    public async Task<AllPostsResponse> SearchPosts([ActionParameter] ListArticlesRequest input)
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
            Posts = items.Select(x => new WordPressItem(x)).ToList()
        };
    }

    [Action("Get post", Description = "Get post by ID")]
    public async Task<WordPressItem> GetPostById([ActionParameter] PostRequest input)
    {
        var client = new WordpressRestClient(Creds);
        var request = new WordpressRestRequest(Endpoint + $"/{input.Id}", Method.Get, Creds);
        var post = await client.ExecuteWithHandling<BaseDto>(request);

        return new(post);
    }

    [Action("Get post missing translations (P)", Description = "Gets all the languages that are missing for this post.")]
    public async Task<MissingTranslations> GetPostMissingTranslations([ActionParameter] PostRequest input)
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

    [Action("Get post translation (P)", Description = "Get the translation of a post. Polylang required.")]
    public async Task<WordPressItem> GetTranslationByPost([ActionParameter] PostRequest input, [ActionParameter] LanguageRequest lang)
    {
        var client = new WordpressRestClient(Creds);
        var request = new WordpressRestRequest(Endpoint + $"/{input.Id}", Method.Get, Creds);
        var post = await client.ExecuteWithHandling<BaseDto>(request);

        if (!post.Translations.ContainsKey(lang.Language))
            throw new PluginMisconfigurationException("This post does not have a translation in " + lang.Language);

        var translationId = post.Translations[lang.Language];
        return await GetPostById(new PostRequest { Id = translationId.ToString()});
    }

    [Action("Get post as HTML", Description = "Get post by id as HTML file")]
    public async Task<FileResponse> GetPostByIdAsHtml([ActionParameter] PostRequest input)
    {
        var request = new WordpressRestRequest($"posts/{input.Id}", Method.Get, Creds);
        var post = await Client.ExecuteWithHandling<Post>(request);

        var html = (post.Title.Rendered, post.Content.Rendered).AsHtml();
        
        var metaTag = $"<meta name=\"blackbird-post-id\" content=\"{post.Id}\">";
        var headIndex = html.IndexOf("<head>", StringComparison.Ordinal) + "<head>".Length;
        html = html.Insert(headIndex, metaTag);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));
        var file = await _fileManagementClient.UploadAsync(stream, MediaTypeNames.Text.Html,
            $"{post.Title.Rendered}.html");
        return new(file);
    }

    #endregion

    #region Post & Update

    [Action("Create post", Description = "Create a new post. With Polylang enabled it can also be used to create translations of other posts.")]
    public Task<WordPressItem> CreatePost([ActionParameter] ModificationRequest input, [ActionParameter] PostTranslationOptions translationOptions)
    {
        return ExecuteModification(input, translationOptions, null);       
    }

    [Action("Create post from HTML", Description = "Create a new post from an HTML file. With Polylang enabled it can also be used to create translations of other posts.")]
    public async Task<WordPressItem> CreatePostFromHtml([ActionParameter] FileModificationRequest input, [ActionParameter] PostTranslationOptions translationOptions)
    {
        var fileStream = await _fileManagementClient.DownloadAsync(input.File);
        var fileBytes = await fileStream.GetByteData();
        var html = Encoding.UTF8.GetString(fileBytes);
        
        return await ExecuteModification(html, translationOptions, null);
    }

    [Action("Update post", Description = "Update post. With Polylang enabled it can also be used to set the language and update its associations.")]
    public Task<WordPressItem> UpdatePost(
        [ActionParameter] PostRequest post, 
        [ActionParameter] ModificationRequest input, 
        [ActionParameter] PostTranslationOptions translationOptions
        )
    {
        return ExecuteModification(input, translationOptions, post.Id);
    }

    [Action("Update post from HTML", Description = "Update a post from an HTML file. With Polylang enabled it can also be used to set the language and update its associations.")]
    public async Task<WordPressItem> UpdatePostFromHtml(
        [ActionParameter] PostOptionalRequest post,
        [ActionParameter] FileModificationRequest input,
        [ActionParameter] PostTranslationOptions translationOptions
        )
    {
        var fileStream = await _fileManagementClient.DownloadAsync(input.File);
        var fileBytes = await fileStream.GetByteData();
        var html = Encoding.UTF8.GetString(fileBytes);
        var htmlDocument = html.AsHtmlDocument();
        
        var metaTag = htmlDocument.DocumentNode.SelectSingleNode("//meta[@name='blackbird-post-id']");
        var postIdValue = metaTag?.GetAttributeValue("content", null);
        
        var postId = post.Id ?? postIdValue 
            ?? throw new PluginMisconfigurationException("Post ID not found in HTML file. Please make sure the file was created with the 'Get post as HTML' action. Or add optional Post ID parameter.");
        
        return await ExecuteModification(html, translationOptions, postId);
    }    

    private async Task<WordPressItem> ExecuteModification(string html, PostTranslationOptions translationOptions, string? id)
    {
        var htmlDocument = html.AsHtmlDocument();
        var title = htmlDocument.GetTitle();
        var body = htmlDocument.GetBody();
        return await ExecuteModification(new ModificationRequest { Title = title, Content = body }, translationOptions, id);
    }

    private async Task<WordPressItem> ExecuteModification(ModificationRequest input, PostTranslationOptions translationOptions, string? id)
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
            var parent = await GetPostById(new PostRequest { Id = translationOptions.ParentId });
            request.AddQueryParameter($"translations[{parent.Language}]", translationOptions.ParentId);
        }

        var result = await client.ExecuteWithHandling<BaseDto>(request);

        return new(result);
    }

    #endregion

    #region Delete

    [Action("Delete post", Description = "Delete post")]
    public Task DeletePost([ActionParameter] PostRequest post)
    {
        var request = new WordpressRestRequest($"posts/{post.Id}", Method.Delete, Creds);
        return Client.ExecuteWithHandling(request);
    }

    #endregion
}