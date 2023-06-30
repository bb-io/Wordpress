using Apps.Wordpress.Models.Responses.Entities;

namespace Apps.Wordpress.Models.Responses.All;

public class AllUsersResponse
{
    public List<WordPressUser> User { get; init; }
}