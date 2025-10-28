using Apps.Wordpress.Events.Polling;
using Tests.Wordpress.Base;

namespace Tests.Wordpress
{
    [TestClass]
    public class PollingTests : TestBase
    {
        [TestMethod]
        public async Task OnPostCreated_IsSuccessfull()
        {
            var polling = new PollingList(InvocationContext);

            var response = await polling.OnPostCreated(
                new Blackbird.Applications.Sdk.Common.Polling.PollingEventRequest<Apps.Wordpress.Events.Polling.Models.Memory.ContentCreatedPollingMemory>
                {
                    Memory = new Apps.Wordpress.Events.Polling.Models.Memory.ContentCreatedPollingMemory
                    {
                        LastCreationDate = DateTime.UtcNow.AddDays(-1)
                    }
                },
                new Apps.Wordpress.Models.Requests.LanguageOptionalRequest());

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            Console.WriteLine(json);

            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task OnPostUpdated_IsSuccessfull()
        {
            var polling = new PollingList(InvocationContext);

            var response = await polling.OnPostUpdated(
                new Blackbird.Applications.Sdk.Common.Polling.PollingEventRequest<Apps.Wordpress.Events.Polling.Models.Memory.ContentUpdatedPollingMemory>
                {
                    Memory = new Apps.Wordpress.Events.Polling.Models.Memory.ContentUpdatedPollingMemory
                    {
                         LastModificationTime = DateTime.UtcNow.AddDays(1)
                    }
                }, "506",
                new Apps.Wordpress.Models.Requests.LanguageOptionalRequest());

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            Console.WriteLine(json);

            Assert.IsNotNull(response);
        }
    }
}
