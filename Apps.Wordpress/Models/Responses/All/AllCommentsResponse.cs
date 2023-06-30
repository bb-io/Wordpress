using Apps.Wordpress.Models.Responses.Entities;

namespace Apps.Wordpress.Models.Responses.All;

public class AllCommentsResponse
{
    public List<WordPressComment> Comments { get; set; }
}