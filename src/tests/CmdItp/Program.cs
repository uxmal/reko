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
            byte[] bytes = // Ldr
            {
                0x00, 0x00, 0x00, 0x00,
                0x08, 0x00, 0x94, 0xE5, // 0xE5940008
            };
            var mem = new MemoryArea(Address.Ptr32(0x00123400), bytes);
            var rdr = new LeImageReader(mem, mem.BaseAddress + 4);
            var frame = new Frame(PrimitiveType.Pointer32);
            var x = new ArmRewriterNew(null, rdr, null, frame, null);

            var e = x.GetEnumerator();
            var m = e.MoveNext();
            if (m)
            {
                var c = e.Current;
                Debug.Print("{0}", c);
            }
        }
    }
}
