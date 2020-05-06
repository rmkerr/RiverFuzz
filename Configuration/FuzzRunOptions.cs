using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Configuration
{
    public class FuzzRunOptions
    {
        public string Target { get; set; }
        public List<int> TargetEndpointIds { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public string RunName { get; set; }

        private const string DefaultFuzzerRunName = "Untitled Fuzzer Run";

        public FuzzRunOptions(IConfiguration configuration)
        {
            this.Target = configuration.GetSection("Target").Get<string>();
            this.TargetEndpointIds = configuration.GetSection("TargetEndpoints").Get<int[]>().ToList();
            this.ExecutionTime = TimeSpan.FromMinutes(configuration.GetSection("ExecutionTime").Get<int>());
            this.RunName = configuration.GetSection("RunName").Get<string>();

            if (string.IsNullOrWhiteSpace(this.RunName))
            {
                this.RunName = DefaultFuzzerRunName;
            }
        }

        public FuzzRunOptions(string json)
        {
            var config = JObject.Parse(json);
            this.Target = config.Value<string>("Target");
            this.TargetEndpointIds = config["TargetEndpoints"].Select(x => (int)x).ToList();
            this.RunName = config.Value<string?>("RunName") ?? DefaultFuzzerRunName;
            this.ExecutionTime = TimeSpan.FromMinutes(config.Value<int>("ExecutionTime"));
        }
    }
}
