using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Entities
{
    public class FuzzerGenerationEntity
    {
        public int? id { get; set; }

        // Identifies the run this generation is associated with.
        public int run_id { get; set; }

        // Indicates which generation this was within the run.
        public int run_position { get; set; }

        // The population size at the end of this generation.
        public int population_size { get; set; }

        // The total number of requests executed this generation.
        public int executed_requests { get; set; }

        // The time it took to execute this generation.
        public TimeSpan execution_time { get; set;}
    }
}
