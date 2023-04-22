using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Wordpress
{
    public class WordpressApplication : IApplication
    {
        public string Name
    {
        get => "Wordpress";
        set { }
    }

    public T GetInstance<T>()
    {
        throw new NotImplementedException();
    }
}
}
