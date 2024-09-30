using Apps.Wordpress.Models.Dtos;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Wordpress.Models.Entities;

public class WordPressItem
{
    #region Properties

    [Display("ID")]
    public string Id { get; }
    public string Title { get; }

    [Display("Html content")] public string HtmlContent { get; }

    [Display("Html excerpt")] public string HtmlExcerpt { get; }

    public string Link { get; }
    
    [Display("Created at")]
    public DateTime CreatedAt { get; }
    
    [Display("Modified at")]
    public DateTime ModifiedAt { get; }

    [Display("Language (P)")]
    public string Language { get; set; }

    #endregion

    #region Constructors

    public WordPressItem(BaseDto post)
    {
        Id = post.Id.ToString();
        Title = post.Title.Rendered;
        HtmlContent = post.Content.Rendered;
        Link = post.Link;
        CreatedAt = post.DateGmt;
        ModifiedAt = post.ModifiedGmt;
        HtmlExcerpt = post.Excerpt.Rendered;
        Language = post.Lang ?? "not available";
    }

    #endregion
}