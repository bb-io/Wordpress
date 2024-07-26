using Blackbird.Applications.Sdk.Common;

namespace Apps.Wordpress.Models.Responses;

public class MissingTranslations
{
    [Display("Missing languages")]
    public IEnumerable<string> MissingLanguages { get; set;}
}