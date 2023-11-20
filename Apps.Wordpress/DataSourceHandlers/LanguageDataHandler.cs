using Apps.Wordpress.Api;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apps.Wordpress.Api.RestSharp;
using Apps.Wordpress.Models.Polylang;

namespace Apps.Wordpress.DataSourceHandlers
{
    public class LanguageDataHandler : BaseInvocable, IAsyncDataSourceHandler
    {
        private IEnumerable<AuthenticationCredentialsProvider> Creds =>
            InvocationContext.AuthenticationCredentialsProviders;

        public LanguageDataHandler(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
        {
            var client = new WordpressRestClient(Creds, "pll/v1");
            var request = new WordpressRestRequest("/languages", RestSharp.Method.Get, Creds);
            var result = await client.ExecuteWithHandling<List<Language>>(request);

            return result
                .Where(x => context.SearchString == null ||
                            x.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(x => x.Slug, x => x.Name);
        }
    }
}
