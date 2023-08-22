using Apps.Wordpress.Models.Entities;

namespace Apps.Wordpress.Models.Responses.All
{
    public class AllPagesResponse
    {
        public IEnumerable<WordPressItem> Pages { get; set; }
    }
}
