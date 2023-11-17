using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Wordpress.Models.Polylang
{
    public class Language
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        //[JsonProperty("term_group")]
        //public int? TermGroup { get; set; }

        //[Display("Term ID")]
        //[JsonProperty("term_id")]
        //public int? TermId { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [Display("Home URL")]
        [JsonProperty("home_url")]
        public string HomeUrl { get; set; }

        [Display("Search URL")]
        [JsonProperty("search_url")]
        public string SearchUrl { get; set; }

        [Display("Flag code")]
        [JsonProperty("flag_code")]
        public string FlagCode { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [Display("Is default")]
        [JsonProperty("is_default")]
        public bool IsDefault { get; set; }
    }
}
