using Apps.Wordpress.Constants;
using Apps.Wordpress.Extensions;
using Apps.Wordpress.Models.Dtos;
using Apps.Wordpress.Models.Entities;
using Apps.Wordpress.Models.Requests.Post;
using Apps.Wordpress.Models.Requests;
using Apps.Wordpress.Models.Responses;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.Extensions.System;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Wordpress.Api.RestSharp;

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
        return JsonConvert.DeserializeObject<T>(response.Content);
    }    
    
    public async Task<RestResponse> ExecuteWithHandling(RestRequest request)
    {
        var response = await ExecuteAsync(request);

        if (response.IsSuccessStatusCode)
            return response;

        var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
        throw new(error.Message);
    }


    private static RestClientOptions GetOptions(IEnumerable<AuthenticationCredentialsProvider> creds, string baseResource)
    {
        var url = creds.GetUrl().Append(baseResource);

        return new()
        {
            BaseUrl = url
        };
    }
}