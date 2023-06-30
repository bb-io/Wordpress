using Apps.Wordpress.Models.Requests;
using Apps.Wordpress.Models.Responses.All;
using Apps.Wordpress.Models.Responses.Entities;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.Wordpress.Actions;

[ActionList]
public class CommentActions
{
    #region Get

    [Action("Get all comments", Description = "Get all comments")]
    public async Task<AllCommentsResponse> GetAllComments(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        var comments = await client.Comments.GetAllAsync(true, true);

        return new()
        {
            Comments = comments.Select(x => new WordPressComment(x)).ToList()
        };
    }

    [Action("Get comment", Description = "Get comment by id")]
    public async Task<WordPressComment> GetCommentById(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] [Display("Comment id")] int id)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        var comment = await client.Comments.GetByIDAsync(id);

        return new(comment);
    }

    #endregion

    #region Create

    [Action("Add comment", Description = "Add comment")]
    public async Task<WordPressComment> AddComment(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] AddComment request)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        var comment = await client.Comments.CreateAsync(new()
        {
            PostId = request.PostId,
            Content = new(request.Content)
        });

        return new(comment);
    }

    #endregion

    #region Delete

    [Action("Delete comment", Description = "Delete comment")]
    public Task DeleteComment(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] [Display("Comment id")] int commentId)
    {
        var client = new CustomWordpressClient(authenticationCredentialsProviders);
        return client.Comments.DeleteAsync(commentId);
    }

    #endregion
}