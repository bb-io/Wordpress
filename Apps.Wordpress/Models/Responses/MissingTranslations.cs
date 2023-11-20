using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Wordpress.Models.Responses
{
    public class MissingTranslations
    {
        [Display("Missing languages")]
        public IEnumerable<string> MissingLanguages { get; set;}
    }
}
