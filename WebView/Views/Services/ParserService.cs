using CaptureParse.Parsers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebView.Models.Services
{
    public class ParserService
    {
        public List<SelectListItem> SupportedFormats = new List<SelectListItem>();

        public ParserService(IEnumerable<ICaptureParse> parsers)
        {
            foreach (var parser in parsers)
            {
                this.SupportedFormats.Add(new SelectListItem { Value = parser.Value, Text = parser.DisplayName });
            }
        }
    }
}
