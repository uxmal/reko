using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ScannerV2
{
    public abstract class Worker
    {
        public abstract void Run();
    }

    public sealed class NullWorker : Worker
    {
        private NullWorker() { }

        public static NullWorker Instance { get; } = new NullWorker();

        public sealed override void Run()
        {
        }
    }
}
