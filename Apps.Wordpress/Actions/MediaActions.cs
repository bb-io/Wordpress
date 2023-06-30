using Apps.Wordpress.Models.Requests;
using Apps.Wordpress.Models.Responses.All;
using Apps.Wordpress.Models.Responses.Entities;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.Wordpress.Actions;

[ActionList]
public class MediaActions
{
    #region Get

    [Action("Get all media", Description = "Get all media content")]
    public async Task<AllMediaResponse> GetAllMedia(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        var medias = await client.Media.GetAllAsync(true, true);

        return new()
        {
            Media = medias.Select(x => new WordPressMedia(x)).ToList()
        };
    }

    [Action("Get media", Description = "Get media by id")]
    public async Task<WordPressMedia> GetMediaById(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] [Display("Media id")] int id)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        var media = await client.Media.GetByIDAsync(id);

        return new(media);
    }

    #endregion

    #region Create

    [Action("Upload media", Description = "Upload media")]
    public async Task<WordPressMedia> UploadMedia(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] UploadMedia request)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        var media = await client.Media.CreateAsync(new MemoryStream(request.FileContent), request.FileName);

        return new(media);
    }

    #endregion

    #region Delete

    [Action("Delete media", Description = "Delete media")]
    public Task DeleteMedia(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] [Display("Media id")] int mediaId)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        return client.Media.DeleteAsync(mediaId);
    }

    #endregion
}