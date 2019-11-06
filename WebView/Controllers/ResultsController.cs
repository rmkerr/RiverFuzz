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
    public class ResultsController : Controller
    {
        private readonly ILogger<EndpointsController> _logger;
        private readonly IFuzzerRepository _endpointRepository;

        public ResultsController(ILogger<EndpointsController> logger, IFuzzerRepository endpointRepo)
        {
            _logger = logger;
            _endpointRepository = endpointRepo;
        }

        [HttpGet]
        [Route("Results/API/Requests/ById/{id}")]
        public async Task<ActionResult<RequestEntity>> GetExecutedRequestById(int id)
        {
            return await _endpointRepository.GetExecutedRequestById(id);
        }

        [HttpGet]
        [Route("Results/API/Requests/All")]
        public async Task<ActionResult<List<RequestEntity>>> GetAllRequests()
        {
            return await _endpointRepository.GetAllExecutedRequest();
        }

        [HttpGet]
        [Route("Results/API/Sequences/All")]
        public async Task<ActionResult<List<RequestSequenceEntity>>> GetAllRequestSequences()
        {
            return await _endpointRepository.GetAllRequestSequences();
        }

        [HttpGet]
        [Route("Results/API/Sequences/ById/{id}")]
        public async Task<ActionResult<RequestSequenceEntity>> GetRequestSequenceById(int id)
        {
            return await _endpointRepository.GetRequestSequenceById(id);
        }

        [HttpGet]
        [Route("Results/All")]
        public async Task<IActionResult> GetAllVisual()
        {
            List<RequestEntity> endpoints = await _endpointRepository.GetAllExecutedRequest();
            ViewBag.Endpoints = endpoints;
            return View();
        }
    }
}
