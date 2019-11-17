using Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebView.Models
{
    // TODO: Note updated or used
    public class ResponseViewModel
    {
        public ResponseViewModel(ResponseEntity entity)
        {
            id = entity.id;
            status = entity.status;
            headers = entity.headers;
            content = entity.content;
            sequence_id = entity.sequence_id;
            sequence_position = entity.sequence_position;
        }

        public int? id { get; set; }
        public string status { get; set; }
        public string? headers { get; set; }
        public string? content { get; set; }
        public int? sequence_id { get; set; }
        public int? sequence_position { get; set; }

        // Not from entity
        public RequestSequenceViewModel? Sequence { get; set; }
    }
}
