using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Entities
{
    public class RequestSequenceEntity
    {
        public int? id { get; set; }
        public int request_count { get; set; }
        public int substitution_count { get; set; }
        public int generation_id { get; set; }
    }
}
