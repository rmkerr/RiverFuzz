using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebLauncher.Services
{
    public interface IFuzzerJobQueue
    {
        public void AddFuzzerJob(string config);
    }
}
