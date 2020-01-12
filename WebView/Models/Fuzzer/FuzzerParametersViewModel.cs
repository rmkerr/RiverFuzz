using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebView.Models
{
    public class FuzzerParametersViewModel
    {
        [Display(Name = "Execution Time (minutes)")]
        [JsonProperty("ExecutionTime")]
        public int? ExecutionTime { get; set; }

        [Url]
        [Display(Name = "Fuzzer Target URL")]
        public string Target { get; set; }

        [Display(Name = "Target Endpoints")]
        public IEnumerable<int> TargetEndpoints { get; set; }

        [Display(Name = "Run Name")]
        public string RunName { get; set; }

        public List<RequestViewModel> Endpoints { get; } = new List<RequestViewModel>();
    }
}
