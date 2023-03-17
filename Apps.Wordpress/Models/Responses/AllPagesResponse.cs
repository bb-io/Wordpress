using Apps.Wordpress.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Wordpress.Models.Responses
{
    public class AllPagesResponse
    {
        public IEnumerable<PageDto> Pages { get; set; }
    }
}
