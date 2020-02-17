using Database.Entities;
using System;

namespace WebView.Models
{
    public class FuzzerGenerationViewModel
    {
        public FuzzerGenerationViewModel()
        {
            id = null;
            run_id = 0;
            run_position = 0;
            population_size = 0;
            executed_requests = 0;
            execution_time = new TimeSpan();
        }

        public FuzzerGenerationViewModel(FuzzerGenerationEntity entity)
        {
            id = entity.id;
            run_id = entity.run_id;
            run_position = entity.run_position;
            population_size = entity.population_size;
            execution_time = entity.execution_time;
            executed_requests = entity.executed_requests;
        }

        public int? id { get; set; }
        public int run_id { get; set; }
        public int run_position { get; set; }
        public int population_size { get; set; }
        public int executed_requests { get; set; }
        public TimeSpan execution_time { get; set; }
    }
}
