using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace WebLauncher.Controllers
{
    public class FuzzController : Controller
    {
        private readonly ILogger<FuzzController> _logger;

        public FuzzController(ILogger<FuzzController> logger)
        {
            _logger = logger;
        }

        public string Index()
        {
            return "FuzzController loads.";
        }

        [HttpPost]
        [Route("Fuzz/Start")]
        public async Task<string> Start()
        {
            _logger.LogDebug("Starting Fuzzer...");
            string jsonString;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                jsonString = await reader.ReadToEndAsync();
            }

            _logger.LogDebug($"Fuzzer Config: {jsonString}");

            JObject config = JObject.Parse(jsonString);
            
            await Fuzz.Program.Fuzz(config);

            return config.ToString();
        }
    }
}
