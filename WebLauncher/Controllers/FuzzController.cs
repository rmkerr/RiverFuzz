using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using WebLauncher.Services;

namespace WebLauncher.Controllers
{
    public class FuzzController : Controller
    {
        private readonly ILogger<FuzzController> _logger;
        private readonly IFuzzerJobQueue _fuzzerJobQueue;

        public FuzzController(ILogger<FuzzController> logger, IFuzzerJobQueue jobQueue)
        {
            _logger = logger;
            _fuzzerJobQueue = jobQueue;
        }

        [HttpPost]
        [Route("Fuzz/Start")]
        public async Task<IActionResult> Start()
        {
            _logger.LogDebug("Starting Fuzzer...");
            string jsonString;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                jsonString = await reader.ReadToEndAsync();
            }

            _logger.LogDebug($"Fuzzer Config: {jsonString}");

            _fuzzerJobQueue.AddFuzzerJob(jsonString);

            return new EmptyResult();
        }
    }
}
