using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Entities
{
    // Allows you to add arbitrary debug metadata to a sequence.
    public class SequenceMetadataEntity
    {
        public int? id { get; set; }
        public int? sequence_id { get; set; }
        public string type { get; set; }
        public string content { get; set; }
    }
}
