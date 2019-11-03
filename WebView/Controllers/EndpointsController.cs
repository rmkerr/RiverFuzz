using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebView.Models;

namespace WebView.Controllers
{
    public class EndpointsController : Controller
    {
        private readonly ILogger<EndpointsController> _logger;

        public EndpointsController(ILogger<EndpointsController> logger)
        {
            _logger = logger;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<string> Index()
        {
            DatabaseHelper databaseHelper = new DatabaseHelper(@"C:\Users\Richa\Documents\RiverFuzzResources\Database\results.sqlite");

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("FROM DATABASE:");
            List<Database.Entities.RequestEntity> endpoints = await databaseHelper.AllEndpoints();
            foreach (Database.Entities.RequestEntity endpoint in endpoints)
            {
                sb.AppendLine($"\tEndpoint ID: {endpoint.id}, URL: {endpoint.url}");
            }

            return sb.ToString();
        }
    }
}
