using Apps.Wordpress.Api;
using Apps.Wordpress.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;
using WordPressPCL.Models;

namespace Apps.Wordpress.DataSourceHandlers;

public class MediaDataHandler : WordpressInvocable, IAsyncDataSourceHandler
{
    public MediaDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new WordpressRestRequest("media", Method.Get, Creds);
        var medias = await Client.Paginate<MediaItem>(request);

        return medias
            .Where(x => context.SearchString == null ||
                        x.Title.Rendered.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.Date)
            .Take(20)
            .ToDictionary(x => x.Id.ToString(), x => x.Title.Rendered);
    }
}