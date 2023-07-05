namespace Apps.Wordpress.Models.Requests;

public class UpdateFromFileRequest
{
    public int Id { get; set; }
    public byte[] File { get; set; }
}