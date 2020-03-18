using Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebView.Models.Results;

namespace WebView.Models
{
    public class RequestSequenceViewModel
    {
        public RequestSequenceViewModel(RequestSequenceEntity entity)
        {
            id = entity.id;
            request_count = entity.request_count;
            substitution_count = entity.substitution_count;
            run_id = entity.run_id;

            Requests = new List<RequestViewModel>();
            Responses = new List<ResponseViewModel>();
            Substitutions = new List<List<SubstitutionViewModel>>();
            Labels = new List<RequestSequenceLabelViewModel>();
            Metadata = new List<SequenceMetadataViewModel>();
        }

        // From entity
        public int? id { get; set; }
        public int request_count { get; set; }
        public int substitution_count { get; set; }
        public int run_id { get; set; }

        // Not from entity
        public List<RequestViewModel> Requests { get; set; }
        public List<ResponseViewModel> Responses { get; set; }
        public List<List<SubstitutionViewModel>> Substitutions { get; set; }
        public List<RequestSequenceLabelViewModel> Labels { get; set; }
        public List<SequenceMetadataViewModel> Metadata { get; set; }
    }
}
