using Apps.Wordpress.Extensions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using WordPressPCL;

namespace Apps.Wordpress.Api
{
    public class CustomWordpressClient : WordPressClient
    {
        public CustomWordpressClient(IEnumerable<AuthenticationCredentialsProvider> creds) : base(creds.GetUrl())
        {
            var login = creds.Get("login").Value;
            var appPassword = creds.Get("applicationPassword").Value;

            Auth.UseBasicAuth(login, appPassword);
        }
    }
}
