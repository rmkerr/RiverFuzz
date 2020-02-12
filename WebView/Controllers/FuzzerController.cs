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
        private IConfiguration _configuration;
        private HttpClient _client;

        public FuzzerController(ILogger<FuzzerController> logger, IFuzzerRepository endpointRepo, IConfiguration configuration)
        {
            _logger = logger;
            _endpointRepository = endpointRepo;
            _client = new HttpClient();
            _configuration = configuration;
        }

        public async Task<IActionResult> IndexAsync()
        {
            List<RequestEntity> endpoints = await _endpointRepository.GetAllEndpoints();

            FuzzerParametersViewModel model = new FuzzerParametersViewModel();
            model.ExecutionTime = 2;
            model.Target = @"https://webview20200210104037.azurewebsites.net";

            foreach (RequestEntity requestEntity in endpoints)
            {
                model.Endpoints.Add(new RequestViewModel(requestEntity));
            }

            return View(model);
        }

        public async Task<IActionResult> Start(FuzzerParametersViewModel model)
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            await _client.PostAsync(_configuration.GetValue<string>("FuzzerHost") + "/Fuzz/Start", content);
            return RedirectToAction("Summary", "FuzzerRun");
        }
    }
}