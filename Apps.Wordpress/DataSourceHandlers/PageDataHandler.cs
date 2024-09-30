using Apps.Wordpress.Api;
using Apps.Wordpress.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;
using WordPressPCL.Models;

namespace Apps.Wordpress.DataSourceHandlers;

public class PageDataHandler : WordpressInvocable, IAsyncDataSourceHandler
{
    public PageDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var request = new WordpressRestRequest("pages", Method.Get, Creds);
        var pages = await Client.Paginate<Page>(request);

        return pages
            .Where(x => context.SearchString == null ||
                        x.Title.Rendered.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.Date)
            .Take(20)
            .ToDictionary(x => x.Id.ToString(), x => x.Title.Rendered);
    }
}