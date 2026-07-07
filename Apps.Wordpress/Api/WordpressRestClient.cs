using Apps.Wordpress.Constants;
using Apps.Wordpress.Extensions;
using Apps.Wordpress.Models.Responses;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.Extensions.System;
using Blackbird.Applications.Sdk.Utils.Html.Extensions;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using System.Text.RegularExpressions;

namespace Apps.Wordpress.Api;

public class WordpressRestClient : RestClient
{
    public WordpressRestClient(IEnumerable<AuthenticationCredentialsProvider> creds) : base(GetOptions(creds, "wp/v2/"))
    {
    }

    public WordpressRestClient(IEnumerable<AuthenticationCredentialsProvider> creds, string baseResource) : base(GetOptions(creds, baseResource))
    {
    }

    public async Task<List<T>> Paginate<T>(RestRequest request)
    {
        var limit = 100;
        var page = 1;
        int? totalPages = null;
        
        var result = new List<T>();
        do
        {
            request.Resource = request.Resource.WithQuery(new()
            {
                { "page", (page++).ToString() },
                { "per_page", limit.ToString() },
            });

            var response = await ExecuteWithHandling(request);
            totalPages ??= int.Parse(response.Headers.First(x => x.Name!.Equals("X-Wp-Totalpages", StringComparison.OrdinalIgnoreCase)).Value.ToString());
            var content = response.Content;

            var data = JsonConvert.DeserializeObject<T[]>(response.Content);
            result.AddRange(data);
        } while (page < totalPages);

        return result;
    }
    
    public async Task<T> ExecuteWithHandling<T>(RestRequest request)
    {
        var response = await ExecuteWithHandling(request);
        return JsonConvert.DeserializeObject<T>(response.Content, JsonConfig.JsonSettings);
    }    
    
    public async Task<RestResponse> ExecuteWithHandling(RestRequest request)
    {
        var response = await ExecuteAsync(request);

        if (response.IsSuccessStatusCode)
            return response;

        var errorMessage = GetErrorMessage(response);

        if (errorMessage.Contains("No route was found matching the URL and request method") &&
            Options.BaseUrl.AbsolutePath.Contains("pll/v1"))
        {
            throw new PluginMisconfigurationException("Could not find Polylang. Please make sure the Polylang plugin is installed.");
        }

        throw new PluginApplicationException(errorMessage);
    }


    private static RestClientOptions GetOptions(IEnumerable<AuthenticationCredentialsProvider> creds, string baseResource)
    {
        var url = creds.GetUrl().Append(baseResource);

        return new()
        {
            BaseUrl = url
        };
    }

    private static string GetErrorMessage(RestResponse response)
    {
        var content = response.Content;
        if (string.IsNullOrWhiteSpace(content))
            return response.ErrorMessage ?? $"Request failed with status code {(int)response.StatusCode}";

        if (TryGetJsonErrorMessage(content, out var jsonErrorMessage))
            return jsonErrorMessage;

        if (IsHtmlResponse(response, content))
            return GetHtmlErrorMessage(response.StatusCode, content);

        return response.ErrorMessage ?? content;
    }

    private static bool TryGetJsonErrorMessage(string content, out string errorMessage)
    {
        try
        {
            var error = JsonConvert.DeserializeObject<ErrorResponse>(content);
            if (!string.IsNullOrWhiteSpace(error?.Message))
            {
                errorMessage = error.Message;
                return true;
            }
        }
        catch (JsonException)
        {
        }

        errorMessage = string.Empty;
        return false;
    }

    private static bool IsHtmlResponse(RestResponse response, string content)
    {
        return response.ContentType?.Contains("html", StringComparison.OrdinalIgnoreCase) == true ||
               content.TrimStart().StartsWith("<", StringComparison.Ordinal);
    }

    private static string GetHtmlErrorMessage(HttpStatusCode statusCode, string content)
    {
        var document = content.AsHtmlDocument();
        var title = document.DocumentNode.SelectSingleNode("//title")?.InnerText?.Trim();
        var bodyText = document.DocumentNode.SelectSingleNode("//body")?.InnerText;
        var normalizedBody = NormalizeWhitespace(WebUtility.HtmlDecode(bodyText ?? string.Empty));

        if (!string.IsNullOrWhiteSpace(title) && normalizedBody.Contains(title, StringComparison.OrdinalIgnoreCase))
        {
            normalizedBody = NormalizeWhitespace(Regex.Replace(normalizedBody, Regex.Escape(title), string.Empty, RegexOptions.IgnoreCase));
        }

        var details = string.Join(": ", new[] { title, normalizedBody }.Where(x => !string.IsNullOrWhiteSpace(x)));

        return string.IsNullOrWhiteSpace(details)
            ? $"Request failed with status code {(int)statusCode}"
            : $"Request failed with status code {(int)statusCode}: {details}";
    }

    private static string NormalizeWhitespace(string value)
    {
        return Regex.Replace(value, "\\s+", " ").Trim();
    }
}
