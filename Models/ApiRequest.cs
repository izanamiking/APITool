using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APITool.Models
{
    public class ApiRequest
    {
        public string Url { get; set; }
        public string Body { get; set; }
        public HttpMethodType Method { get; set; } = HttpMethodType.GET;
    }
}
