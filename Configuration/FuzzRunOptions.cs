using Microsoft.Extensions.Configuration;
using System;

namespace Configuration
{
    public class FuzzRunOptions
    {
        public string Target { get; set; }
        public int[] TargetEndpoints { get; set; }
        public int ExecutionTime { get; set; }
        public string RunName { get; set; }

        public FuzzRunOptions(IConfiguration configuration)
        {
            this.Target = configuration.GetSection("Target").Get<string>();
            this.TargetEndpoints = configuration.GetSection("TargetEndpoints").Get<int[]>();
            this.ExecutionTime = configuration.GetSection("ExecutionTime").Get<int>();
            this.RunName = configuration.GetSection("RunName").Get<string>();

            if(string.IsNullOrWhiteSpace(this.RunName))
            {
                this.RunName = "Untitled Fuzzer Run";
            }
        }
    }
}
