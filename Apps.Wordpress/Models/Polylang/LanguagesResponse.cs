using Blackbird.Applications.Sdk.Common;

namespace Apps.Wordpress.Models.Polylang;

public class LanguagesResponse
{
    public IEnumerable<Language> Languages { get; set;}

    [Display("Default locale")]
    public string? DefaultLanguageCode { get; set;}

    [Display("Other locales")]
    public IEnumerable<string> OtherLanguageCodes { get; set;}
}