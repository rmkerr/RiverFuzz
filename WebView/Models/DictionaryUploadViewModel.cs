using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebView.Models
{
    public class DictionaryUploadViewModel
    {
        public DictionaryUploadViewModel()
        {
            Files = new List<IFormFile>();
            Name = String.Empty;
        }

        public IEnumerable<IFormFile> Files { get; set; }
        public string Name { get; set; }
    }
}
