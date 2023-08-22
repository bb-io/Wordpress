using Apps.Wordpress.Models.Entities;

namespace Apps.Wordpress.Models.Responses.All;

public class AllPostsResponse
{
    public List<WordPressItem> Posts { get; init; }
}