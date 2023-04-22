using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Apps.Wordpress.Models.Requests;
using Apps.Wordpress.Models.Responses;
using Apps.Wordpress.Dtos;
using Newtonsoft.Json.Linq;
using WordPressPCL;
using System;
using Blackbird.Applications.Sdk.Common.Actions;

namespace Apps.Wordpress
{
    [ActionList]
    public class Actions
    {
        [Action("Get all pages", Description = "Get all pages content")]
        public AllPagesResponse GetAllPages(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, 
            [ActionParameter] AllPagesRequest input)
        {
            var client = new CustomWordpressClient(authenticationCredentialsProviders);
            var pages = client.Pages.GetAllAsync().Result;
            var pagesDtos = pages.Select(p => new PageDto()
            {
                Title = p.Title.Rendered,
                HtmlContent = p.Content.Rendered,
                Link = p.Link,
            }).ToList();

            return new AllPagesResponse()
            {
                Pages = pagesDtos,
            };
        }

        [Action("Get page", Description = "Get page by id")]
        public PageResponse GetPageById(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] PageRequest input)
        {
            var client = new CustomWordpressClient(authenticationCredentialsProviders);
            var page = client.Pages.GetByIDAsync(input.PageId).Result;

            return new PageResponse()
            {
                Title = page.Title.Rendered,
                HtmlContent = page.Content.Rendered,
                Link = page.Link
            };
        }

        [Action("Get post", Description = "Get post by id")]
        public PostResponse GetPostById(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] PostRequest input)
        {
            var client = new CustomWordpressClient(authenticationCredentialsProviders);
            var page = client.Posts.GetByIDAsync(input.PostId).Result;

            return new PostResponse()
            {
                Title = page.Title.Rendered,
                HtmlContent = page.Content.Rendered,
                Link = page.Link
            };
        }
    }
}
