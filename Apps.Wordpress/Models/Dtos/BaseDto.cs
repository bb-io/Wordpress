using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apps.Wordpress.Models.Dtos;

public class BaseDto
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("date")]
    public DateTime Date { get; set; }

    [JsonProperty("date_gmt")]
    public DateTime DateGmt { get; set; }

    [JsonProperty("modified")]
    public DateTime Modified { get; set; }

    [JsonProperty("modified_gmt")]
    public DateTime ModifiedGmt { get; set; }

    [JsonProperty("slug")]
    public string Slug { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("link")]
    public string Link { get; set; }

    [JsonProperty("title")]
    public Title Title { get; set; }

    [JsonProperty("content")]
    public Content Content { get; set; }

    [JsonProperty("excerpt")]
    public Excerpt Excerpt { get; set; }

    [JsonProperty("author")]
    public int Author { get; set; }

    [JsonProperty("featured_media")]
    public int FeaturedMedia { get; set; }

    [JsonProperty("comment_status")]
    public string CommentStatus { get; set; }

    [JsonProperty("ping_status")]
    public string PingStatus { get; set; }

    [JsonProperty("template")]
    public string Template { get; set; }

    [JsonProperty("lang")]
    public string Lang { get; set; }

    [JsonProperty("meta")]
    public JToken Meta { get; set; }

    [JsonProperty("translations")]
    public Dictionary<string, int> Translations { get; set; }
}

public class Title
{
    [JsonProperty("rendered")]
    public string Rendered { get; set; }
}

public class Excerpt
{
    [JsonProperty("rendered")]
    public string Rendered { get; set; }

    [JsonProperty("protected")]
    public bool Protected { get; set; }
}

public class Content
{
    [JsonProperty("rendered")]
    public string Rendered { get; set; }

    [JsonProperty("protected")]
    public bool Protected { get; set; }
}