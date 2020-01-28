using Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebView.Models
{
    public class SequenceSummaryViewModel
    {
        public SequenceSummaryViewModel(SequenceSummaryEntity entity)
        {
            sequence_id = entity.sequence_id;
            url = entity.url;
            method = entity.method;
            status = entity.status;

            Labels = new List<RequestSequenceLabelViewModel>();
        }

        // Direct from database entity
        public int? sequence_id { get; set; }
        public string url { get; set; }
        public string method { get; set; }
        public string status { get; set; }

        // Loaded from another table
        public List<RequestSequenceLabelViewModel> Labels { get; }
    }
}
