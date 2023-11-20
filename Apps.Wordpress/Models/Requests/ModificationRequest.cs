﻿using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Wordpress.Models.Requests
{
    public class ModificationRequest
    {
        [Display("Title")]
        public string? Title { get; set; }

        [Display("Content")]
        public string? Content { get; set; }
    }
}
