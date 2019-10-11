using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebView.Models
{
    public class EndpointModel
    {
        public int EndpointModelID { get; set; }
        public string Name { get; set; }

        [InverseProperty("Endpoint")]
        public ICollection<RequestModel> Requests { get; set; }
    }
}
