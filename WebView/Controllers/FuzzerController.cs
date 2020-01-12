using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Database.Entities;
using Database.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WebView.Models;

namespace WebView.Controllers
{
    public class FuzzerController : Controller
    {
        private readonly ILogger<FuzzerController> _logger;
        private readonly IFuzzerRepository _endpointRepository;
        private HttpClient _client;

        public FuzzerController(ILogger<FuzzerController> logger, IFuzzerRepository endpointRepo)
        {
            _logger = logger;
            _endpointRepository = endpointRepo;
            _client = new HttpClient();
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

        public async Task<FuzzerParametersViewModel> Start(FuzzerParametersViewModel model)
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            await _client.PostAsync("https://localhost:44393/Fuzz/Start", content);
            return model;
        }
    }
}