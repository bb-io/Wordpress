using System.Net;
using System.Net.Sockets;
using System.Text;
using Apps.Wordpress.Api;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using RestSharp;

namespace Tests.Wordpress;

[TestClass]
public class WordpressRestClientTests
{
    private const string UnreachableSiteUrl = "http://127.0.0.1:1";

    [TestMethod]
    public async Task Paginate_WhenSiteReturnsNoResponseBody_ThrowsPluginApplicationException()
    {
        var client = CreateClient(UnreachableSiteUrl);

        var exception = await Assert.ThrowsExceptionAsync<PluginApplicationException>(
            () => client.Paginate<object>(new RestRequest("posts")));

        Assert.IsFalse(string.IsNullOrWhiteSpace(exception.Message));
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public async Task Paginate_WhenResultSpansSeveralPages_ReturnsItemsFromEveryPage(int totalPages)
    {
        using var listener = StartFakeSite(totalPages, out var siteUrl);
        var client = CreateClient(siteUrl);

        var items = await client.Paginate<PageItem>(new RestRequest("posts"));

        CollectionAssert.AreEqual(Enumerable.Range(1, totalPages).ToList(), items.Select(x => x.Page).ToList());
    }

    private static WordpressRestClient CreateClient(string siteUrl)
        => new(new[] { new AuthenticationCredentialsProvider("url", siteUrl) });

    private static HttpListener StartFakeSite(int totalPages, out string siteUrl)
    {
        siteUrl = $"http://localhost:{GetFreePort()}";
        var listener = new HttpListener();
        listener.Prefixes.Add($"{siteUrl}/");
        listener.Start();
        _ = ServePages(listener, totalPages);
        return listener;
    }

    private static async Task ServePages(HttpListener listener, int totalPages)
    {
        while (listener.IsListening)
        {
            HttpListenerContext context;
            try
            {
                context = await listener.GetContextAsync();
            }
            catch (HttpListenerException)
            {
                return;
            }
            catch (ObjectDisposedException)
            {
                return;
            }

            var requestedPage = context.Request.QueryString.GetValues("page")!.Last();
            var body = Encoding.UTF8.GetBytes($"[{{\"Page\":{requestedPage}}}]");

            context.Response.ContentType = "application/json";
            context.Response.AddHeader("X-WP-TotalPages", totalPages.ToString());
            await context.Response.OutputStream.WriteAsync(body);
            context.Response.Close();
        }
    }

    private static int GetFreePort()
    {
        using var socket = new TcpListener(IPAddress.Loopback, 0);
        socket.Start();
        return ((IPEndPoint)socket.LocalEndpoint).Port;
    }

    private record PageItem(int Page);
}
