using Apps.Wordpress.Api;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using RestSharp;

namespace Apps.Wordpress.Connections;

public class ConnectionValidator : IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        CancellationToken cancellationToken)
    {
        var creds = authenticationCredentialsProviders.ToArray();
        var client = new WordpressRestClient(creds);

        var request = new WordpressRestRequest("users", Method.Get, creds);

        try
        {
            var w = await client.ExecuteWithHandling(request);
            return new()
            {
                IsValid = true
            };
        }
        catch (Exception)
        {
            return new ConnectionValidationResponse
            {
                IsValid = false,
                Message = "Invalid connection parameters"
            };
        }
    }
}