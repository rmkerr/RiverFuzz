using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseModels.Models
{
    public class RequestSequenceModel
    {
        public int? id { get; set; }
        public int request_count { get; set; }
        public int substitution_count { get; set; }
    }
}
