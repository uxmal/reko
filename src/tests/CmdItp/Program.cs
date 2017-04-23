using Reko.Arch.Arm;
using Reko.Core;
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
            byte[] ldr = // Ldr
            {
                0x08, 0x0, 0x94, 0xE5, // 0xE5940008
            };
            var x = new ArmRewriterNew(ldr, 0, Address.Ptr32(0x00123400));
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
