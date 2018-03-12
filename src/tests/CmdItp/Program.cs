using Reko.Arch.Arm;
using Reko.Core;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmdItp
{
    class Program
    {
        static void Main(string[] args)
        {
            var mem = BuildTest(
                0xE8BD000C
                );
            var rdr = new LeImageReader(mem, mem.BaseAddress);
            var frame = new Frame(PrimitiveType.Pointer32);
            var x = new ArmRewriterNew(null, rdr, null, frame, null);
            Debug.Print("************************");
            foreach (var c in x)
            {
                Debug.Print("{0}:", c.Address);
                foreach (var i in c.Instructions)
                {
                    Debug.Print("    {0}", i);
                }
            }
            Debug.Print("************************");
        }

        private static MemoryArea BuildTest(params uint[] words)
        {
            var bytes = words
                .SelectMany(u => new byte[] { (byte)u, (byte)(u >> 8), (byte)(u >> 16), (byte)(u >> 24) })
                .ToArray();
            return new MemoryArea(Address.Ptr32(0x00100000), bytes);
        }

    }
}
