using Apps.Wordpress.Models.Responses.Entities;

namespace Apps.Wordpress.Models.Responses.All;

public class AllPostsResponse
{
    public List<WordPressItem> Posts { get; init; }
}