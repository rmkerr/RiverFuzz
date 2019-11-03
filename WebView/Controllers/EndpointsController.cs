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
        [Route("Endpoints/API/ById/{id}")]
        public async Task<ActionResult<RequestEntity>> GetByID(int id)
        {
            return await _endpointRepository.GetByID(id);
        }

        [HttpGet]
        [Route("Endpoints/API/All")]
        public async Task<ActionResult<List<RequestEntity>>> GetAll()
        {
            return await _endpointRepository.GetAll();
        }

        [HttpGet]
        [Route("Endpoints/All")]
        public async Task<IActionResult> GetAllVisual()
        {
            List<RequestEntity> endpoints = await _endpointRepository.GetAll();
            ViewBag.Endpoints = endpoints;
            return View();
        }
    }
}
