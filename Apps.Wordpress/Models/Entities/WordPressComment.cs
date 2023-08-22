using Blackbird.Applications.Sdk.Common;
using WordPressPCL.Models;

namespace Apps.Wordpress.Models.Entities;

public class WordPressComment
{
    #region Properties

    public int Id { get; }
    [Display("Post ID")] public int PostId { get; }
    public string Content { get; }
    public string Author { get; }
    public DateTime Date { get; }
    public string Link { get; }

    #endregion

    #region Constructors

    public WordPressComment(Comment comment)
    {
        Id = comment.Id;
        PostId = comment.PostId;
        Date = comment.Date;
        Author = comment.AuthorName;
        Content = comment.Content.Rendered;
        Link = comment.Link;
    }

    #endregion
}