using Apps.Wordpress.Api;
using Apps.Wordpress.Constants;
using Apps.Wordpress.Invocables;
using Apps.Wordpress.Models.Entities;
using Apps.Wordpress.Models.Requests.Comment;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Parsers;
using RestSharp;
using WordPressPCL.Models;

namespace Apps.Wordpress.Actions;

[ActionList]
public class CommentActions : WordpressInvocable
{
    public CommentActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    #region Get
    
    // TODO: Make this more usable and related to posts/pages

    //[Action("Get all comments", Description = "Get all comments")]
    //public async Task<AllCommentsResponse> GetAllComments()
    //{
    //    var client = new CustomWordpressClient(Creds);
    //    var comments = await client.Comments.GetAllAsync(false, true);

    //    return new()
    //    {
    //        Comments = comments.Select(x => new WordPressComment(x)).ToList()
    //    };
    //}

    //[Action("Get comment", Description = "Get comment by ID")]
    //public async Task<WordPressComment> GetCommentById([ActionParameter] CommentRequest comment)
    //{
    //    var client = new CustomWordpressClient(Creds);
    //    var response = await client.Comments.GetByIDAsync(comment.CommentId);

    //    return new(response);
    //}

    #endregion

    #region Create

    [Action("Add comment", Description = "Add comment")]
    public async Task<WordPressComment> AddComment([ActionParameter] AddComment input)
    {
        var request = new WordpressRestRequest("comments", Method.Post, Creds)
            .WithJsonBody(new
            {
                Post = IntParser.Parse(input.PostId, nameof(input.PostId))!.Value,
                input.Content
            }, JsonConfig.JsonSettings);
        var comment = await Client.ExecuteWithHandling<Comment>(request);

        return new(comment);
    }

    #endregion

    #region Delete

    [Action("Delete comment", Description = "Delete comment")]
    public Task DeleteComment([ActionParameter] CommentRequest comment)
    {
        var request = new WordpressRestRequest($"comments/{comment.CommentId}", Method.Delete, Creds);
        return Client.ExecuteWithHandling<Comment>(request);
    }

    #endregion
}