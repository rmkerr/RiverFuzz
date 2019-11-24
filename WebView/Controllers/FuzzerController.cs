using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Entities;
using Database.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebView.Models;

namespace WebView.Controllers
{
    public class FuzzerController : Controller
    {
        private readonly ILogger<FuzzerController> _logger;
        private readonly IFuzzerRepository _endpointRepository;

        public FuzzerController(ILogger<FuzzerController> logger, IFuzzerRepository endpointRepo)
        {
            _logger = logger;
            _endpointRepository = endpointRepo;
        }

        public async Task<IActionResult> IndexAsync()
        {
            List<RequestEntity> endpoints = await _endpointRepository.GetAllEndpoints();

            FuzzerParametersViewModel model = new FuzzerParametersViewModel();
            model.GenerationCount = 10;
            model.TargetUrl = @"http://localhost/";

            foreach (RequestEntity requestEntity in endpoints)
            {
                model.Endpoints.Add(new RequestViewModel(requestEntity));
            }

            return View(model);
        }

        public FuzzerParametersViewModel Start(FuzzerParametersViewModel model)
        {
            return model;
        }
    }
}