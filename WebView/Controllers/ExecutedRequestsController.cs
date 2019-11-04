using Database;
using Database.Entities;
using Database.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebView.Controllers
{
    public class ExecutedRequestsController : Controller
    {
        private readonly ILogger<EndpointsController> _logger;
        private readonly IFuzzerRepository _endpointRepository;

        public ExecutedRequestsController(ILogger<EndpointsController> logger, IFuzzerRepository endpointRepo)
        {
            _logger = logger;
            _endpointRepository = endpointRepo;
        }

        [HttpGet]
        [Route("ExecutedRequests/API/ById/{id}")]
        public async Task<ActionResult<RequestEntity>> GetExecutedRequestById(int id)
        {
            return await _endpointRepository.GetExecutedRequestById(id);
        }

        [HttpGet]
        [Route("ExecutedRequests/API/All")]
        public async Task<ActionResult<List<RequestEntity>>> GetAll()
        {
            return await _endpointRepository.GetAllExecutedRequest();
        }

        [HttpGet]
        [Route("ExecutedRequests/All")]
        public async Task<IActionResult> GetAllVisual()
        {
            List<RequestEntity> endpoints = await _endpointRepository.GetAllExecutedRequest();
            ViewBag.Endpoints = endpoints;
            return View();
        }
    }
}
