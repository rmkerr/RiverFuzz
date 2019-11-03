using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebView.Models
{
    // Literally copied and pasted from the RequestEntity file. This seems like a bad approach,
    // but the consensus online seems to be that entities and viewmodels should be totally seperate.
    public class RequestViewModel
    {
        public int? id { get; set; }
        public string url { get; set; }
        public string method { get; set; }
        public string? headers { get; set; }
        public string? content { get; set; }
        public int? sequence_id { get; set; }
        public int? sequence_position { get; set; }

    }
}
