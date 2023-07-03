namespace Apps.Wordpress.Models.Requests;

public class UpdateRequest
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
}