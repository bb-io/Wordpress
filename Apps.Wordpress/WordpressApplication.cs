using Blackbird.Applications.Sdk.Common;

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
