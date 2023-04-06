using RestSharp;
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
        public CustomWordpressClient(string url, string login, string appPassword) : base($"{url.TrimEnd('/')}/")
        {
            this.Auth.UseBasicAuth(login, appPassword);
        }
    }
}
