﻿using Apps.Wordpress.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Wordpress.Models.Requests.Post;

public class PostOptionalRequest
{
    [Display("Post ID")] 
    [DataSource(typeof(PostDataHandler))]
    public string? Id { get; set; }
}