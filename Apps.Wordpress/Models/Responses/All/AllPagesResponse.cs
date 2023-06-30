using Apps.Wordpress.Models.Responses.Entities;

namespace Apps.Wordpress.Models.Responses.All
{
    public class AllPagesResponse
    {
        public IEnumerable<WordPressItem> Pages { get; set; }
    }
}
