using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Database.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebView.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebView.Controllers
{
    public class DictionaryController : Controller
    {
        private readonly ILogger<EndpointsController> _logger;
        private readonly IFuzzerRepository _endpointRepository;

        public DictionaryController(ILogger<EndpointsController> logger, IFuzzerRepository endpointRepo)
        {
            _logger = logger;
            _endpointRepository = endpointRepo;
        }

        public IActionResult AddFromFile()
        {
            return View();
        }

        [HttpPost]
        [Route("Dictionary/API/AddFromFile")]
        public IActionResult APIAddEndpointFromFile(DictionaryUploadViewModel model)
        {
            foreach (IFormFile file in model.Files)
            {
                StreamReader sr = new StreamReader(file.OpenReadStream());

                List<string> entries = new List<string>();
                string? content = sr.ReadLine();
                while (content != null)
                {
                    entries.Add(content);
                    content = sr.ReadLine();
                }

                _endpointRepository.AddDictionary(model.Name, entries);
            }
            return RedirectToAction("AddFromFile");
        }
    }
}
