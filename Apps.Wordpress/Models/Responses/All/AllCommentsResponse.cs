using Apps.Wordpress.Models.Entities;

namespace Apps.Wordpress.Models.Responses.All;

public class AllCommentsResponse
{
    public List<WordPressComment> Comments { get; set; }
}