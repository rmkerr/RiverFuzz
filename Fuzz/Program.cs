using System;
using HttpTokenize;
using HttpTokenize.Tokens;
using HttpTokenize.Tokenizers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Generators;
using CaptureParse;
using Population.Bucketers;
using Population;
using Database;
using System.Diagnostics;
using Database.Entities;
using ProjectSpecific;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;

namespace Fuzz
{
    public class Program
    {
        static bool production = false;

        static async Task Main(string[] args)
        {
            string fuzzerConfig = System.IO.File.ReadAllText(@"fuzz.json");
            JObject config = JObject.Parse(fuzzerConfig);
            
            await Fuzz(config);
        }

        public static async Task Fuzz(JObject config)
        {
            // Set up a database connection to store the results.
            DatabaseHelper databaseHelper = new DatabaseHelper("riverfuzz", production);
            if (config.Value<bool?>("ResetDatabase") ?? false)
            {
                databaseHelper.DeleteDatabase();
                databaseHelper.CreateDatabase();
            }

            // Parse the list of endpoints we should include in this run, then load them.
            List<KnownEndpoint> endpoints = InitializeEndpoints(REPLACE_THIS_WITH_PATH, "http://localhost");

            // Add the endpoints to the population and set up bucketers.
            PopulationManager population = new PopulationManager();
            foreach (KnownEndpoint endpoint in endpoints)
            {
                databaseHelper.AddEndpoint(RequestEntity.FromRequest(endpoint.Request));
            }
        }

        public static List<KnownEndpoint> InitializeEndpoints(string endpoint_path, string host)
        {
            return BurpSavedParse.LoadRequestsFromDirectory(endpoint_path, host);
            //return BurpSavedParse.LoadRequestsFromDirectory(@"C:\Users\Richa\Documents\RiverFuzzResources\Wordpress\wp-json", @"http://192.168.43.232");
            //return BurpSavedParse.LoadRequestsFromDirectory(@"C:\Users\Richa\Documents\RiverFuzzResources\Moodle", @"http://10.0.0.197");
        }
    }
}
