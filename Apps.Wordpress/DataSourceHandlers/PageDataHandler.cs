using Apps.Wordpress.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Wordpress.DataSourceHandlers;

public class PageDataHandler : BaseInvocable, IAsyncDataSourceHandler
{
    private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;

    public PageDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var client = new CustomWordpressClient(Creds);
        var comments = await client.Pages.GetAllAsync(false, true);

        return comments
            .Where(x => context.SearchString == null ||
                        x.Title.Rendered.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.Date)
            .Take(20)
            .ToDictionary(x => x.Id.ToString(), x => x.Title.Rendered);
    }
}