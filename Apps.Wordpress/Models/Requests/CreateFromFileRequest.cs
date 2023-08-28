using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.Wordpress.Models.Requests;

public class CreateFromFileRequest
{
    public File File { get; set; }
}