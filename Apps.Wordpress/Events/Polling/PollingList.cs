using Apps.Wordpress.Api.RestSharp;
using Apps.Wordpress.Constants;
using Apps.Wordpress.DataSourceHandlers;
using Apps.Wordpress.Events.Polling.Models;
using Apps.Wordpress.Events.Polling.Models.Memory;
using Apps.Wordpress.Models.Dtos;
using Apps.Wordpress.Models.Entities;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using RestSharp;

namespace Apps.Wordpress.Events.Polling;

[PollingEventList]
public class PollingList : BaseInvocable
{
    public PollingList(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [PollingEvent("On post created", "On a new post created")]
    public Task<PollingEventResponse<ContentCreatedPollingMemory, ContentPollingResult>> OnPostCreated(
        PollingEventRequest<ContentCreatedPollingMemory> request)
        => PollContentCreation(request, new()
        {
            ["after"] = request.Memory?.LastCreationDate.ToString(Formats.ISO8601) ?? string.Empty
        }, "posts", () => new()
        {
            LastCreationDate = DateTime.UtcNow
        });


    [PollingEvent("On post updated", "On a specific post updated")]
    public Task<PollingEventResponse<ContentUpdatedPollingMemory, ContentPollingResult>> OnPostUpdated(
        PollingEventRequest<ContentUpdatedPollingMemory> request,
        [PollingEventParameter] [Display("Post ID")] [DataSource(typeof(PostDataHandler))]
        string? postId)
        => PollContentChanges(request, postId, new()
        {
            ["modified_after"] = request.Memory?.LastModificationTime.ToString(Formats.ISO8601) ?? string.Empty
        }, "posts", () => new()
        {
            LastModificationTime = DateTime.UtcNow
        });

    [PollingEvent("On page created", "On a new page created")]
    public Task<PollingEventResponse<ContentCreatedPollingMemory, ContentPollingResult>> OnPageCreated(
        PollingEventRequest<ContentCreatedPollingMemory> request)
        => PollContentCreation(request, new()
        {
            ["after"] = request.Memory?.LastCreationDate.ToString(Formats.ISO8601) ?? string.Empty
        }, "pages", () => new()
        {
            LastCreationDate = DateTime.UtcNow
        });


    [PollingEvent("On page updated", "On a specific page updated")]
    public Task<PollingEventResponse<ContentUpdatedPollingMemory, ContentPollingResult>> OnPageUpdated(
        PollingEventRequest<ContentUpdatedPollingMemory> request,
        [PollingEventParameter] [Display("Page ID")] [DataSource(typeof(PageDataHandler))]
        string? pageId)
        => PollContentChanges(request, pageId, new()
        {
            ["modified_after"] = request.Memory?.LastModificationTime.ToString(Formats.ISO8601) ?? string.Empty
        }, "pages", () => new()
        {
            LastModificationTime = DateTime.UtcNow
        });

    private async Task<PollingEventResponse<T, ContentPollingResult>> PollContentCreation<T>(
        PollingEventRequest<T> request, Dictionary<string, string> query, string endpoint, Func<T> createMemory)
    {
        if (request.Memory is null)
        {
            return new()
            {
                FlyBird = false,
                Memory = createMemory()
            };
        }

        var items = await ListContentItems(endpoint.WithQuery(query));

        if (!items.Any())
        {
            return new()
            {
                FlyBird = false,
                Memory = createMemory()
            };
        }

        return new()
        {
            FlyBird = true,
            Result = new()
            {
                Items = items.Select(x => new WordPressItem(x)).ToList()
            },
            Memory = createMemory()
        };
    }

    private async Task<PollingEventResponse<T, ContentPollingResult>> PollContentChanges<T>(
        PollingEventRequest<T> request, string? resourceId, Dictionary<string, string> query, string endpoint,
        Func<T> createMemory) where T : ContentUpdatedPollingMemory
    {
        if (request.Memory is null)
        {
            return new()
            {
                FlyBird = false,
                Memory = createMemory()
            };
        }

        var items = string.IsNullOrWhiteSpace(resourceId)
            ? await ListContentItems(endpoint.WithQuery(query))
            : await GetContentItem($"{endpoint}/{resourceId}", request.Memory);

        if (!items.Any())
        {
            return new()
            {
                FlyBird = false,
                Memory = createMemory()
            };
        }

        return new()
        {
            FlyBird = true,
            Result = new()
            {
                Items = items.Select(x => new WordPressItem(x)).ToList()
            },
            Memory = createMemory()
        };
    }

    private Task<List<BaseDto>> ListContentItems(string endpoint)
    {
        var client = new WordpressRestClient(InvocationContext.AuthenticationCredentialsProviders);
        var listRequest =
            new WordpressRestRequest(endpoint, Method.Get, InvocationContext.AuthenticationCredentialsProviders);

        return client.Paginate<BaseDto>(listRequest);
    }

    private async Task<List<BaseDto>> GetContentItem(string endpoint, ContentUpdatedPollingMemory memory)
    {
        var client = new WordpressRestClient(InvocationContext.AuthenticationCredentialsProviders);
        var listRequest =
            new WordpressRestRequest(endpoint, Method.Get, InvocationContext.AuthenticationCredentialsProviders);

        var item = await client.ExecuteWithHandling<BaseDto>(listRequest);

        return item.ModifiedGmt > memory.LastModificationTime ? [item] : [];
    }
}