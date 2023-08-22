using Blackbird.Applications.Sdk.Common;
using WordPressPCL.Models;

namespace Apps.Wordpress.Models.Entities;

public class WordPressItem
{
    #region Properties

    public int Id { get; }
    public string Title { get; }

    [Display("Html content")] public string HtmlContent { get; }

    public string Link { get; }
    
    [Display("Created at")]
    public DateTime CreatedAt { get; }
    
    [Display("Modified at")]
    public DateTime ModifiedAt { get; }

    #endregion

    #region Constructors

    public WordPressItem(Post post)
    {
        Id = post.Id;
        Title = post.Title.Rendered;
        HtmlContent = post.Content.Rendered;
        Link = post.Link;
        CreatedAt = post.DateGmt;
        ModifiedAt = post.ModifiedGmt;
    }
    
    public WordPressItem(Page page)
    {
        Id = page.Id;
        Title = page.Title.Rendered;
        HtmlContent = page.Content.Rendered;
        Link = page.Link;
        CreatedAt = page.DateGmt;
        ModifiedAt = page.ModifiedGmt;
    }

    #endregion
}