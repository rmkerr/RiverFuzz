using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptureParse;
using CaptureParse.Parsers;
using Database;
using Database.Entities;
using Database.Repositories;
using HttpTokenize;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebView.Models;

namespace WebView.Controllers
{
    public class EndpointsController : Controller
    {
        private readonly ILogger<EndpointsController> _logger;
        private readonly IFuzzerRepository _endpointRepository;
        private readonly IParserFactory _parserFactory;

        public EndpointsController(ILogger<EndpointsController> logger, IFuzzerRepository endpointRepo, IParserFactory parserFactory)
        {
            _logger = logger;
            _endpointRepository = endpointRepo;
            _parserFactory = parserFactory;
        }

        [HttpGet]
        [Route("Endpoints/API/ById/{id}")]
        public async Task<ActionResult<KnownEndpointEntity>> GetByID(int id)
        {
            return await _endpointRepository.GetEndpointById(id);
        }

        [HttpGet]
        [Route("Endpoints/API/All")]
        public async Task<ActionResult<List<KnownEndpointEntity>>> GetAll()
        {
            return await _endpointRepository.GetAllEndpoints();
        }

        [HttpGet]
        [Route("Endpoints/All")]
        public async Task<IActionResult> GetAllVisual()
        {
            List<KnownEndpointEntity> endpoints = await _endpointRepository.GetAllEndpoints();
            List<KnownEndpointViewModel> viewModels = new List<KnownEndpointViewModel>();
            foreach(KnownEndpointEntity entity in endpoints)
            {
                viewModels.Add(new KnownEndpointViewModel(entity));
            }
            ViewBag.Endpoints = viewModels;
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
            KnownEndpointEntity entity = new KnownEndpointEntity();

            entity.content = request.content;
            entity.headers = request.headers;
            entity.method = request.method;
            entity.url = request.url;
            _endpointRepository.AddEndpoint(entity);

            return RedirectToAction("GetAllVisual");
        }

        [HttpGet]
        [Route("Endpoints/AddFromFile")]
        public IActionResult AddFromFile()
        {
            return View();
        }

        [HttpPost]
        [Route("Endpoints/API/AddFromFile")]
        public IActionResult APIAddEndpointFromFile(EndpointFileViewModel model)
        {
            ICaptureParse parser = _parserFactory.GetParser(model.FileFormat);

            foreach (IFormFile file in model.Files)
            {
                StreamReader sr = new StreamReader(file.OpenReadStream());
                string content = sr.ReadToEnd();

                // TODO: Replace the hardcoded URL here.
                Request request = parser.ParseSingleRequestFile(content, "http://localhost").Request;
                KnownEndpointEntity entity = KnownEndpointEntity.FromRequest(request);

                // Default friendly name is filename without extension.
                entity.friendly_name = file.FileName.Split(".")[0];

                _endpointRepository.AddEndpoint(entity);
            }
            return RedirectToAction("GetAllVisual");
        }

    }
}
