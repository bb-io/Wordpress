using Apps.Wordpress.Api;
using Apps.Wordpress.Invocables;
using Apps.Wordpress.Models.Entities;
using Apps.Wordpress.Models.Requests.Media;
using Apps.Wordpress.Models.Responses.All;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;
using WordPressPCL.Models;

namespace Apps.Wordpress.Actions;

[ActionList("Media")]
public class MediaActions : WordpressInvocable
{
    private readonly IFileManagementClient _fileManagementClient;

    public MediaActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
        : base(invocationContext)
    {
        _fileManagementClient = fileManagementClient;
    }

    #region Get

    [Action("Get all media", Description = "Get all media content")]
    public async Task<AllMediaResponse> GetAllMedia()
    {
        var request = new WordpressRestRequest("media", Method.Get, Creds);
        var medias = await Client.Paginate<MediaItem>(request);

        return new()
        {
            Media = medias.Select(x => new WordPressMedia(x)).ToList()
        };
    }

    [Action("Get media", Description = "Get media by ID")]
    public async Task<WordPressMedia> GetMediaById([ActionParameter] MediaRequest media)
    {
        var request = new WordpressRestRequest($"media/{media.MediaId}", Method.Get, Creds);
        var response = await Client.ExecuteWithHandling<MediaItem>(request);

        return new(response);
    }

    #endregion

    #region Create

    [Action("Upload media", Description = "Upload media")]
    public async Task<WordPressMedia> UploadMedia([ActionParameter] UploadMedia input)
    {
        var fileStream = await _fileManagementClient.DownloadAsync(input.File);

        var request = new WordpressRestRequest("media", Method.Post, Creds);

        var fileName = input.FileName ?? input.File.Name;
        request.AddFile("file", () => fileStream, fileName);

        var media = await Client.ExecuteWithHandling<MediaItem>(request);
        return new(media);
    }

    #endregion

    #region Delete

    [Action("Delete media", Description = "Delete media")]
    public Task DeleteMedia([ActionParameter] MediaRequest media)
    {
        var request = new WordpressRestRequest($"media/{media.MediaId}?force=true", Method.Delete, Creds);
        return Client.ExecuteWithHandling(request);
    }

    #endregion
}