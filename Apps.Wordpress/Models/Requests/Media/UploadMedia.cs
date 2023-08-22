using Blackbird.Applications.Sdk.Common;

namespace Apps.Wordpress.Models.Requests.Media;

public class UploadMedia
{
    [Display("File name")] public string FileName { get; set; }
    [Display("File content")] public byte[] FileContent { get; set; }
}