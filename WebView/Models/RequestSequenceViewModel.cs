using Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebView.Models
{
    public class RequestSequenceViewModel
    {
        public RequestSequenceViewModel(RequestSequenceEntity entity)
        {
            id = entity.id;
            request_count = entity.request_count;
            substitution_count = entity.substitution_count;

            Requests = new List<RequestViewModel>();
            Responses = new List<ResponseViewModel>();
            Substitutions = new List<SubstitutionViewModel>();
        }

        // From entity
        public int? id { get; set; }
        public int request_count { get; set; }
        public int substitution_count { get; set; }

        // Not from entity
        public List<RequestViewModel> Requests { get; set; }
        public List<ResponseViewModel> Responses { get; set; }
        public List<SubstitutionViewModel> Substitutions { get; set; }
        // TODO: Add substitutions.
        // TODO: Add tags.
    }
}
