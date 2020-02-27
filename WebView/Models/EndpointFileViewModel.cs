using CaptureParse.Parsers;
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
            FileFormat = ParserConstants.FiddlerValue;
        }
        public IEnumerable<IFormFile> Files { get; set; }
        public string FileFormat { get; set; }

        public List<SelectListItem> SupportedFormats = new List<SelectListItem>
        {
            new SelectListItem { Value = ParserConstants.FiddlerValue, Text = ParserConstants.FiddlerDiplayName },
            new SelectListItem { Value = ParserConstants.BurpValue, Text = ParserConstants.BurpDisplayName },
        };

    }
}
