using System.Text;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;

namespace Apps.Wordpress.Api.RestSharp;

public class WordpressRestRequest : RestRequest
{
    public WordpressRestRequest(string resource, Method method, IEnumerable<AuthenticationCredentialsProvider> creds) :
        base(resource, method)
    {
        var login = creds.Get("login").Value;
        var password = creds.Get("applicationPassword").Value;
        
        var authHeader = Convert
            .ToBase64String(Encoding.GetEncoding("ISO-8859-1")
            .GetBytes(login + ":" + password));
        
        this.AddHeader("Authorization", $"Basic {authHeader}");
    }
}