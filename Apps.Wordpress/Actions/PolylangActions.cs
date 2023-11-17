using Apps.Wordpress.Api;
using Apps.Wordpress.Api.RestSharp;
using Apps.Wordpress.Models.Entities;
using Apps.Wordpress.Models.Polylang;
using Apps.Wordpress.Models.Responses.All;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Wordpress.Actions
{
    [ActionList]
    public class PolylangActions : BaseInvocable
    {
        private IEnumerable<AuthenticationCredentialsProvider> Creds => InvocationContext.AuthenticationCredentialsProviders;

        public PolylangActions(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        [Action("Get languages (P)", Description = "Get all languages configured in Polylang")]
        public async Task<LanguagesResponse> GetLanguages()
        {
            var client = new WordpressRestClient(Creds, "pll/v1");
            var request = new WordpressRestRequest("/languages", RestSharp.Method.Get, Creds);
            var result = await client.ExecuteWithHandling<List<Language>>(request);

            return new()
            {
                Languages = result,
            };
        }


    }
}
