using Blackbird.Applications.Sdk.Common;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.Wordpress.Models.Requests.Media;

public class UploadMedia
{
    [Display("File name")] public string? FileName { get; set; }
    [Display("File content")] public File File { get; set; }
}