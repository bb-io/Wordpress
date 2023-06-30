using WordPressPCL.Models;

namespace Apps.Wordpress.Models.Responses.Entities;

public class WordPressUser
{
    #region Properties

    public int Id { get; }
    public string Name { get; }
    public string Email { get; }
    public string Link { get; }

    #endregion

    #region Constructors

    public WordPressUser(User media)
    {
        Id = media.Id;
        Name = media.Name;
        Email = media.Email;
        Link = media.Link;
    }

    #endregion
}