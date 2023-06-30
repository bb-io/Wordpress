using Blackbird.Applications.Sdk.Common;
using WordPressPCL.Models;

namespace Apps.Wordpress.Models.Responses.Entities;

public class WordPressItem
{
    #region Properties

    public int Id { get; }
    public string Title { get; }

    [Display("Html content")] public string HtmlContent { get; }

    public string Link { get; }

    #endregion

    #region Constructors

    public WordPressItem(Post post)
    {
        Id = post.Id;
        Title = post.Title.Rendered;
        HtmlContent = post.Content.Rendered;
        Link = post.Link;
    }
    
    public WordPressItem(Page page)
    {
        Id = page.Id;
        Title = page.Title.Rendered;
        HtmlContent = page.Content.Rendered;
        Link = page.Link;
    }

    #endregion
}