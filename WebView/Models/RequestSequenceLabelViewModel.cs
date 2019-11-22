using Database.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebView.Models
{
    public class RequestSequenceLabelViewModel
    {
        public RequestSequenceLabelViewModel()
        {
            id = null;
            sequence_id = 0;
            name = "Unknown Tag";
        }
        public RequestSequenceLabelViewModel(RequestSequenceLabelEntity entity)
        {
            id = entity.id;
            sequence_id = entity.sequence_id;
            name = entity.name;
        }
        public int? id { get; set; }
        public int sequence_id { get; set; }
        public string name { get; set; }

        // All labels mapped to their colors.
        public static readonly Dictionary<string, string> ColorMappings = new Dictionary<string, string>
        {
            { "Informational", "badge-info"},
            { "Success", "badge-info"},
            { "Redirection", "badge-info"},
            { "Client Error", "badge-warning"},
            { "Server Error", "badge-warning"},
            { "Unknown Status", "badge-warning"},
            { "Possible Vulnerability", "badge-danger"},
            { "Duplicate", "badge-secondary"},
            { "Expected Result", "badge-success"},
        };

        // User assignable labels.
        public static List<SelectListItem> Labels = new List<SelectListItem>
        {
            new SelectListItem { Value = "Possible Vulnerability", Text = "Possible Vulnerability" },
            new SelectListItem { Value = "Duplicate", Text = "Duplicate" },
            new SelectListItem { Value = "Expected Result", Text = "Expected Result" }
        };
    }
}
