using Apps.Wordpress.Api;
using Apps.Wordpress.Models.Entities;
using Apps.Wordpress.Models.Requests.Media;
using Apps.Wordpress.Models.Responses.All;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Parsers;

namespace Apps.Wordpress.Actions;

[ActionList]
public class MediaActions : BaseInvocable
{
    private readonly IFileManagementClient _fileManagementClient;
    
    private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;
    
    public MediaActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
        : base(invocationContext)
    {
        _fileManagementClient = fileManagementClient;
    }
    
    #region Get

    [Action("Get all media", Description = "Get all media content")]
    public async Task<AllMediaResponse> GetAllMedia()
    {
        var client = new CustomWordpressClient(Creds);
        var medias = await client.Media.GetAllAsync(false, true);

        return new()
        {
            Media = medias.Select(x => new WordPressMedia(x)).ToList()
        };
    }

    [Action("Get media", Description = "Get media by ID")]
    public async Task<WordPressMedia> GetMediaById([ActionParameter] MediaRequest media)
    {
        var client = new CustomWordpressClient(Creds);
        var response = await client.Media.GetByIDAsync(media.MediaId);

        return new(response);
    }

    #endregion

    #region Create

    [Action("Upload media", Description = "Upload media")]
    public async Task<WordPressMedia> UploadMedia([ActionParameter] UploadMedia request)
    {
        var client = new CustomWordpressClient(Creds);
        var fileStream = await _fileManagementClient.DownloadAsync(request.File);
        var fileBytes = await fileStream.GetByteData();
        var media = await client.Media.CreateAsync(new MemoryStream(fileBytes), request.FileName ?? request.File.Name);
        return new(media);
    }

    #endregion

    #region Delete

    [Action("Delete media", Description = "Delete media")]
    public Task DeleteMedia([ActionParameter] MediaRequest media)
    {
        var client = new CustomWordpressClient(Creds);

        var intMediaId = IntParser.Parse(media.MediaId, nameof(media.MediaId))!.Value;
        return client.Media.DeleteAsync(intMediaId);
    }

    #endregion
}