using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Entities
{
    public class RequestSequenceLabelEntity
    {
        public int? id { get; set; }
        public int sequence_id { get; set; }
        public string name { get; set; }
    }
}
