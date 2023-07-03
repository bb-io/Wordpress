namespace Apps.Wordpress.Models.Requests;

public class CreateFromFileRequest
{
    public string Title { get; set; }
    public byte[] File { get; set; }
}