using Database.Entities;
using System;

namespace WebView.Models
{
    public class FuzzerRunViewModel
    {
        public FuzzerRunViewModel()
        {
            id = null;
            name = "Unknown Run";
            start_time = DateTime.MinValue;
            end_time = DateTime.MinValue;
        }

        public FuzzerRunViewModel(FuzzerRunEntity entity)
        {
            id = entity.id;
            name = entity.name;
            start_time = entity.start_time;
            end_time = entity.end_time;
        }

        public int? id { get; set; }
        public string name { get; set; }
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
    }
}
