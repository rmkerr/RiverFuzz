using Database.Entities;
using Database.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebView.Models;

namespace WebView.Controllers
{
    public class FuzzerRunController : Controller
    {
        private readonly ILogger<FuzzerRunController> _logger;
        private readonly IFuzzerRepository _endpointRepository;

        public FuzzerRunController(ILogger<FuzzerRunController> logger, IFuzzerRepository endpointRepo)
        {
            _logger = logger;
            _endpointRepository = endpointRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Summary()
        {
            // Get info from database.
            List<FuzzerRunEntity> runEntities = await _endpointRepository.GetAllFuzzerRuns();
            
            // Convert database entities to view models.
            List<FuzzerRunViewModel> runViewModels = new List<FuzzerRunViewModel>();
            foreach (FuzzerRunEntity entity in runEntities)
            {
                runViewModels.Add(new FuzzerRunViewModel(entity));
            }

            // Render view.
            return View(runViewModels);
        }
    }
}
