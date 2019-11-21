using Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebView.Models
{
    public class SubstitutionViewModel
    {
        public SubstitutionViewModel(SubstitutionEntity entity)
        {
            id = entity.id;
            type = entity.type;
            summary = entity.summary;
            sequence_id = entity.sequence_id;
            sequence_position = entity.sequence_position;
        }

        // From entity.
        public int? id { get; set; }
        public string type { get; set; }
        public string summary { get; set; }
        public int? sequence_id { get; set; }
        public int? sequence_position { get; set; }

        // Not from entity.
        public RequestSequenceViewModel Sequence;
    }
}
