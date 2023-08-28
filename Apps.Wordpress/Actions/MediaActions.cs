using Apps.Wordpress.Api;
using Apps.Wordpress.Models.Entities;
using Apps.Wordpress.Models.Requests.Media;
using Apps.Wordpress.Models.Responses.All;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Parsers;

namespace Apps.Wordpress.Actions;

[ActionList]
public class MediaActions : BaseInvocable
{
    private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;
    
    public MediaActions(InvocationContext invocationContext) : base(invocationContext)
    {
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
        var media = await client.Media.CreateAsync(new MemoryStream(request.File.Bytes), request.FileName ?? request.File.Name);

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