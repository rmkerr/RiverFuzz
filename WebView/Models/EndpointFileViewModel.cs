using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebView.Models
{
    public class EndpointFileViewModel
    {
        public EndpointFileViewModel()
        {
            Files = new List<IFormFile>();
            FileFormat = "Fiddler";
        }
        public IEnumerable<IFormFile> Files { get; set; }
        public string FileFormat { get; set; }

        public static List<SelectListItem> SupportedFormats = new List<SelectListItem>
        {
            new SelectListItem { Value = "Fiddler", Text = "Fiddler Plain-Text" },
            new SelectListItem { Value = "Burp", Text = "Burp Intruder XML" },
        };

    }
}
