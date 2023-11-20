using Apps.Wordpress.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Wordpress.Models.Polylang
{
    public class LanguageRequest
    {
        [Display("Language")]
        [DataSource(typeof(LanguageDataHandler))]
        public string Language { get; set; }
    }
}
