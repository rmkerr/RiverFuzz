using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Database.Entities;
using Database.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WebView.Models;

namespace WebView.Controllers
{
    public class FuzzerController : Controller
    {
        private readonly ILogger<FuzzerController> _logger;
        private readonly IFuzzerRepository _endpointRepository;
        private readonly IHttpClientFactory _clientFactory;

        private IConfiguration _configuration;

        public FuzzerController(ILogger<FuzzerController> logger, IFuzzerRepository endpointRepo, IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _endpointRepository = endpointRepo;
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> IndexAsync()
        {
            List<RequestEntity> endpoints = await _endpointRepository.GetAllEndpoints();

            FuzzerParametersViewModel model = new FuzzerParametersViewModel();
            model.ExecutionTime = 2;
            model.Target = @"http://localhost";

            foreach (RequestEntity requestEntity in endpoints)
            {
                model.Endpoints.Add(new RequestViewModel(requestEntity));
            }

            return View(model);
        }

        public async Task<IActionResult> Start(FuzzerParametersViewModel model)
        {
            HttpClient client = _clientFactory.CreateClient();

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            await client.PostAsync(_configuration.GetValue<string>("FuzzerHost") + "/Fuzz/Start", content);

            return RedirectToAction("Summary", "FuzzerRun");
        }
    }
}