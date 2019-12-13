using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Entities
{

    // Used to store metadata about a fuzzer generation or run.
    public class FuzzerRunEntity
    {
        public int? id { get; set; }
        public string name { get; set; }
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
    }
}
