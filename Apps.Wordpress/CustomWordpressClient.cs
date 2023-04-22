using Blackbird.Applications.Sdk.Common.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordPressPCL;

namespace Apps.Wordpress
{
    public class CustomWordpressClient : WordPressClient
    {
        private static string GetUrl(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        {
            var url = authenticationCredentialsProviders.First(p => p.KeyName == "url").Value;
            return $"{url.TrimEnd('/')}/";
        }

        public CustomWordpressClient(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders) : base(GetUrl(authenticationCredentialsProviders))
        {
            var login = authenticationCredentialsProviders.First(p => p.KeyName == "login").Value;
            var appPassword = authenticationCredentialsProviders.First(p => p.KeyName == "applicationPassword").Value;

            this.Auth.UseBasicAuth(login, appPassword);
        }
    }
}
