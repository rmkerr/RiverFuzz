using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebView.Models
{
    public class FuzzerParametersViewModel
    {
        [Display(Name = "Number of Generations")]
        public int? GenerationCount { get; set; }

        [Url]
        [Display(Name = "Fuzzer Target URL")]
        public string TargetUrl { get; set; }

        [Display(Name = "Target Endpoints")]
        public IEnumerable<int> TargetEndpoints { get; set; }

        public List<RequestViewModel> Endpoints { get; } = new List<RequestViewModel>();
    }
}
