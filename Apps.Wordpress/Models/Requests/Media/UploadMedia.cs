using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Wordpress.Models.Requests.Media;

public class UploadMedia
{
    [Display("File name")] public string? FileName { get; set; }
    [Display("File content")] public FileReference File { get; set; }
}