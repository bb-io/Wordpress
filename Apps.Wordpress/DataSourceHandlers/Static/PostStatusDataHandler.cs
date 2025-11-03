
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Wordpress.DataSourceHandlers.Static;


public class PostStatusDataHandler :  IStaticDataSourceItemHandler
    {
        public IEnumerable<DataSourceItem> GetData() => new List<DataSourceItem>()
            {
                new DataSourceItem("publish", "Publish"),
                new DataSourceItem("future", "Future"),
                new DataSourceItem("draft", "Draft"),
                new DataSourceItem("pending", "Pending"),
                new DataSourceItem("private", "Private")
            };
    }


