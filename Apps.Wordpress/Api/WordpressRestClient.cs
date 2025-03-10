﻿using Apps.Wordpress.Constants;
using Apps.Wordpress.Extensions;
using Apps.Wordpress.Models.Responses;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.Extensions.System;
using Newtonsoft.Json;
using RestSharp;

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

        var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);

        if (error.Message.Contains("No route was found matching the URL and request method") && Options.BaseUrl.AbsolutePath.Contains("pll/v1"))
        {
            throw new PluginMisconfigurationException("Could not find Polylang. Please make sure the Polylang plugin is installed.");
        }

        throw new PluginApplicationException(error.Message);
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