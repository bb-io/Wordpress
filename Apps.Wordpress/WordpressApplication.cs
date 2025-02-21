using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Metadata;

namespace Apps.Wordpress;

public class WordpressApplication : IApplication, ICategoryProvider
{
        
    public IEnumerable<ApplicationCategory> Categories
    {
        get => [ApplicationCategory.Cms];
        set { }
    }

    public T GetInstance<T>()
    {
        throw new NotImplementedException();
    }
}