using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.Wordpress.Models.Requests
{
    public class FileModificationRequest
    {
        public File File { get; set; }
    }
}
