using Apps.Wordpress.Api;
using Apps.Wordpress.Invocables;
using Apps.Wordpress.Models.Polylang;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Wordpress.Actions;

[ActionList]
public class PolylangActions : WordpressInvocable
{
    public PolylangActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("Get languages (P)", Description = "Get all languages configured in Polylang")]
    public async Task<LanguagesResponse> GetLanguages()
    {
        var request = new WordpressRestRequest("/languages", RestSharp.Method.Get, Creds);
        var result = await PolylangClient.ExecuteWithHandling<List<Language>>(request);

        return new()
        {
            Languages = result,
            DefaultLanguageCode = result.FirstOrDefault(x => x.IsDefault)?.Locale,
            OtherLanguageCodes = result.Select(x => x.Locale),
        };
    }
}