using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Entities
{
    [Table("sequences")]
    public class RequestSequenceEntity
    {
        [Key]
        public int? id { get; set; }
        public int request_count { get; set; }
        public int substitution_count { get; set; }
        public int run_id { get; set; }

        // Generated. Not part of the database.
        public int? next_id { get; set; }
        public int? prev_id { get; set; }
    }
}
