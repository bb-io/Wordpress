using Apps.Wordpress.Api;
using Apps.Wordpress.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Apps.Wordpress.Models.Polylang;

namespace Apps.Wordpress.DataSourceHandlers;

public class LanguageDataHandler : WordpressInvocable, IAsyncDataSourceHandler
{
    public LanguageDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new WordpressRestRequest("/languages", RestSharp.Method.Get, Creds);
        var result = await PolylangClient.ExecuteWithHandling<List<Language>>(request);

        return result
            .Where(x => context.SearchString == null ||
                        x.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(x => x.Slug, x => x.Name);
    }
}