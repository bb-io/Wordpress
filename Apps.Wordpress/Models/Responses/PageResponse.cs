using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Wordpress.Models.Responses
{
    public class PageResponse
    {
        public string Title { get; set; }

        public string HtmlContent { get; set; }

        public string Link { get; set; }
    }
}
