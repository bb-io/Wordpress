using Blackbird.Applications.Sdk.Common;
using WordPressPCL.Models;

namespace Apps.Wordpress.Models.Entities;

public class WordPressMedia
{
    #region Properties

    public int Id { get; }
    public string Title { get; }
    public string Link { get; }
    [Display("Source link")] public string SourceLink { get; }

    #endregion

    #region Constructors

    public WordPressMedia(MediaItem media)
    {
        Id = media.Id;
        Title = media.Title.Rendered;
        SourceLink = media.SourceUrl;
        Link = media.Link;
    }
    
    #endregion
}