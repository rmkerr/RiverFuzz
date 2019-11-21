using Database.Entities;
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

        public static readonly Dictionary<string, string> ColorMappings = new Dictionary<string, string>
        {
            { "Informational", "badge-info"},
            { "Success", "badge-success"},
            { "Redirection", "badge-info"},
            { "Client Error", "badge-warning"},
            { "Server Error", "badge-danger"},
            { "Unknown Status", "badge-warning"},
        };
    }
}
