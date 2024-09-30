using Apps.Wordpress.Api;
using Apps.Wordpress.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;
using WordPressPCL.Models;

namespace Apps.Wordpress.DataSourceHandlers;

public class CommentDataHandler : WordpressInvocable, IAsyncDataSourceHandler
{
    public CommentDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new WordpressRestRequest("comments", Method.Get, Creds);
        var comments = await Client.Paginate<Comment>(request);

        return comments
            .Where(x => context.SearchString == null ||
                        x.Content.Rendered.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.Date)
            .Take(20)
            .ToDictionary(x => x.Id.ToString(), x => x.Content.Rendered);
    }
}