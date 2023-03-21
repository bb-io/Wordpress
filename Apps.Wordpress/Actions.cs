using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Apps.Wordpress.Models.Requests;
using Apps.Wordpress.Models.Responses;
using Apps.Wordpress.Dtos;
using Newtonsoft.Json.Linq;
using WordPressPCL;
using System;

namespace Apps.Wordpress
{
    [ActionList]
    public class Actions
    {
        [Action("Get all pages", Description = "Get all pages content")]
        public AllPagesResponse GetAllPages(string url, string login, AuthenticationCredentialsProvider authenticationCredentialsProvider, 
            [ActionParameter] AllPagesRequest input)
        {
            var client = new CustomWordpressClient(url, login, authenticationCredentialsProvider.Value);
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
        public PageResponse GetPageById(string url, string login, AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] PageRequest input)
        {
            var client = new CustomWordpressClient(url, login, authenticationCredentialsProvider.Value);
            var page = client.Pages.GetByIDAsync(input.PageId).Result;

            return new PageResponse()
            {
                Title = page.Title.Rendered,
                HtmlContent = page.Content.Rendered,
                Link = page.Link
            };
        }

        [Action("Get post", Description = "Get post by id")]
        public PostResponse GetPostById(string url, string login, AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] PostRequest input)
        {
            var client = new CustomWordpressClient(url, login, authenticationCredentialsProvider.Value);
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
