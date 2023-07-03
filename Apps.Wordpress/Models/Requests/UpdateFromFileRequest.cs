namespace Apps.Wordpress.Models.Requests;

public class UpdateFromFileRequest
{
    public int Id { get; set; }
    public string Title { get; set; }
    public byte[] File { get; set; }
}