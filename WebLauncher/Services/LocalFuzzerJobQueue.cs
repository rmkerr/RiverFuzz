using Database.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebLauncher.Services
{
    public class LocalFuzzerJobQueue : IFuzzerJobQueue, IDisposable
    {
        // Fuzzer jobs are represented as JSON strings
        private ConcurrentQueue<string> jobQueue;
        private CancellationTokenSource cancelFuzzer;
        public LocalFuzzerJobQueue(IConfiguration configuration)
        {
            jobQueue = new ConcurrentQueue<string>();
            cancelFuzzer = new CancellationTokenSource();

            Task.Run(async () => {
                while (!cancelFuzzer.Token.IsCancellationRequested)
                {
                    string nextConfig = "";
                    if (jobQueue.TryDequeue(out nextConfig))
                    {
                        //JObject config = JObject.Parse(nextConfig);
                        await Fuzz.Program.Fuzz(configuration);
                    }
                    else
                    {
                        // TODO: Figure out how to remove this 5 second delay / if it makes sense to keep it.
                        await Task.Delay(5000);
                    }
                }
            });
        }

        public void AddFuzzerJob(string config)
        {
            jobQueue.Enqueue(config);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    cancelFuzzer.Cancel();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

    }
}
