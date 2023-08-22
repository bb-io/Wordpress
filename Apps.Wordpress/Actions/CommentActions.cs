using Apps.Wordpress.Api;
using Apps.Wordpress.Models.Entities;
using Apps.Wordpress.Models.Requests.Comment;
using Apps.Wordpress.Models.Responses.All;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Parsers;

namespace Apps.Wordpress.Actions;

[ActionList]
public class CommentActions : BaseInvocable
{
    private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;

    public CommentActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    #region Get

    [Action("Get all comments", Description = "Get all comments")]
    public async Task<AllCommentsResponse> GetAllComments()
    {
        var client = new CustomWordpressClient(Creds);
        var comments = await client.Comments.GetAllAsync(false, true);

        return new()
        {
            Comments = comments.Select(x => new WordPressComment(x)).ToList()
        };
    }

    [Action("Get comment", Description = "Get comment by ID")]
    public async Task<WordPressComment> GetCommentById([ActionParameter] CommentRequest comment)
    {
        var client = new CustomWordpressClient(Creds);
        var response = await client.Comments.GetByIDAsync(comment.CommentId);

        return new(response);
    }

    #endregion

    #region Create

    [Action("Add comment", Description = "Add comment")]
    public async Task<WordPressComment> AddComment([ActionParameter] AddComment request)
    {
        var client = new CustomWordpressClient(Creds);
        var comment = await client.Comments.CreateAsync(new()
        {
            PostId = IntParser.Parse(request.PostId, nameof(request.PostId))!.Value,
            Content = new(request.Content)
        });

        return new(comment);
    }

    #endregion

    #region Delete

    [Action("Delete comment", Description = "Delete comment")]
    public Task DeleteComment([ActionParameter] CommentRequest comment)
    {
        var client = new CustomWordpressClient(Creds);

        var intCommentId = IntParser.Parse(comment.CommentId, nameof(comment.CommentId))!.Value;
        return client.Comments.DeleteAsync(intCommentId);
    }

    #endregion
}