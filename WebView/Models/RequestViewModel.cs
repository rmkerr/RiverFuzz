using Database.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebView.Models
{
    public class RequestViewModel
    {
        public RequestViewModel(RequestEntity entity)
        {
            id = entity.id;
            url = entity.url;
            method = entity.method;
            headers = entity.headers;
            content = entity.content;
            sequence_id = entity.sequence_id;
            sequence_position = entity.sequence_position;
        }

        public RequestViewModel()
        {
            id = null;
            url = "";
            method = "";
            headers = null;
            content = null;
            sequence_id = null;
            sequence_position = null;
        }

        // From entity.
        public int? id { get; set; }
        public string url { get; set; }
        public string method { get; set; }
        public string? headers { get; set; }
        public string? content { get; set; }
        public int? sequence_id { get; set; }
        public int? sequence_position { get; set; }

        // Not from entity
        public RequestSequenceViewModel? Sequence { get; set; }

        // Selection menu items for method
        public static List<SelectListItem> Methods = new List<SelectListItem>
        {
            new SelectListItem { Value = "GET", Text = "GET" },
            new SelectListItem { Value = "HEAD", Text = "HEAD" },
            new SelectListItem { Value = "POST", Text = "POST"  },
            new SelectListItem { Value = "PUT", Text = "PUT"  },
            new SelectListItem { Value = "DELETE", Text = "DELETE"  },
            new SelectListItem { Value = "CONNECT", Text = "CONNECT"  },
            new SelectListItem { Value = "OPTIONS", Text = "OPTIONS"  },
            new SelectListItem { Value = "TRACE", Text = "TRACE"  },
            new SelectListItem { Value = "PATCH", Text = "PATCH"  }
        };
    }
}
