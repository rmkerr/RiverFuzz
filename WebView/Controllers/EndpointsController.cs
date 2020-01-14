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
using WebView.Models;

namespace WebView.Controllers
{
    public class EndpointsController : Controller
    {
        private readonly ILogger<EndpointsController> _logger;
        private readonly IFuzzerRepository _endpointRepository;

        public EndpointsController(ILogger<EndpointsController> logger, IFuzzerRepository endpointRepo)
        {
            _logger = logger;
            _endpointRepository = endpointRepo;
        }

        [HttpGet]
        [Route("Endpoints/API/ById/{id}")]
        public async Task<ActionResult<RequestEntity>> GetByID(int id)
        {
            return await _endpointRepository.GetEndpointById(id);
        }

        [HttpGet]
        [Route("Endpoints/API/All")]
        public async Task<ActionResult<List<RequestEntity>>> GetAll()
        {
            return await _endpointRepository.GetAllEndpoints();
        }

        [HttpGet]
        [Route("Endpoints/All")]
        public async Task<IActionResult> GetAllVisual()
        {
            List<RequestEntity> endpoints = await _endpointRepository.GetAllEndpoints();
            ViewBag.Endpoints = endpoints;
            return View();
        }

        [HttpGet]
        [Route("Endpoints/Add")]
        public IActionResult AddEndpoint()
        {
            return View();
        }

        [HttpPost]
        [Route("Endpoints/API/Add")]
        public IActionResult APIAddEndpoint(RequestViewModel request)
        {
            RequestEntity entity = new RequestEntity();

            entity.content = request.content;
            entity.headers = request.headers;
            entity.method = request.method;
            entity.url = request.url;
            _endpointRepository.AddEndpoint(entity);

            return RedirectToAction("GetAllVisual");
        }

    }
}
