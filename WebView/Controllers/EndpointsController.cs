using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database;
using Database.Entities;
using Database.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebView.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EndpointsController : Controller
    {
        private readonly ILogger<EndpointsController> _logger;
        private readonly IEndpointRepository _endpointRepository;

        public EndpointsController(ILogger<EndpointsController> logger, IEndpointRepository endpointRepo)
        {
            _logger = logger;
            _endpointRepository = endpointRepo;
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

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<RequestEntity>> GetByID(int id)
        {
            return await _endpointRepository.GetByID(id);
        }

        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<List<RequestEntity>>> GetAll()
        {
            return await _endpointRepository.GetAll();
        }
    }
}
