using Apps.Wordpress.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Wordpress.Invocables;

public class WordpressInvocable : BaseInvocable
{
    protected AuthenticationCredentialsProvider[] Creds =>
        InvocationContext.AuthenticationCredentialsProviders.ToArray();

    protected WordpressRestClient Client { get; }
    protected WordpressRestClient PolylangClient { get; }

    public WordpressInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new(Creds);
        PolylangClient = new WordpressRestClient(Creds, "pll/v1");
    }
}