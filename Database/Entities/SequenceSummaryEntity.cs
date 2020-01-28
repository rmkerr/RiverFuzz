using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Entities
{
    public class SequenceSummaryEntity
    {
        public int? sequence_id { get; set; }
        public string url { get; set; }
        public string method { get; set; }
        public string status { get; set; }
    }
}
