using Apps.Wordpress.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Tests.Wordpress.Base;

namespace Tests.Wordpress;

[TestClass]
public class DataHandlerTests : TestBase
{
    [TestMethod]
    public async Task Get_languages_works()
    {
        var handler = new LanguageDataHandler(InvocationContext);
        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);
        foreach(var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.DisplayName}");
        }
        Assert.IsTrue(result.Count() > 0);
    }
}