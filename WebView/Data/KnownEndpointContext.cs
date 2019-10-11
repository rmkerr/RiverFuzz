using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebView.Models;

namespace WebView.Data
{
    public class KnownEndpointContext : DbContext
    {
        public KnownEndpointContext(DbContextOptions<KnownEndpointContext> options) : base(options)
        {

        }

        public DbSet<RequestModel> KnownRequests { get; set; }
        public DbSet<EndpointModel> KnownEndpoints { get; set; }
    }
}
