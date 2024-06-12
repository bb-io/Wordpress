using Apps.Wordpress.Models.Entities;

namespace Apps.Wordpress.Events.Polling.Models;

public class ContentPollingResult
{
    public IEnumerable<WordPressItem> Items { get; set; }
}