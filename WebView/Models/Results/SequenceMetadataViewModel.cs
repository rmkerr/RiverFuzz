using Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebView.Models.Results
{
    public class SequenceMetadataViewModel
    {
        public string Type { get; set; }
        public string Content { get; set; }

        public SequenceMetadataViewModel(SequenceMetadataEntity entity)
        {
            Type = entity.type;
            Content = entity.content;
        }
    }
}
