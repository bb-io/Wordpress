using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.String;

namespace Apps.Wordpress.Extensions;

public static class CredsExtensions
{
    public static Uri GetUrl(this IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        var url = creds.First(p => p.KeyName == "url").Value;
        return $"{url.TrimEnd('/')}/wp-json/".ToUri();
    }
}