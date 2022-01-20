using Reko.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Pdp10.Disassembler
{
    public static class Registers
    {
        public static RegisterStorage[] Accumulators { get; }

        static Registers()
        {
            var factory = new StorageFactory();
            Accumulators = factory.RangeOfReg(16, u => $"r{u}", Pdp10Architecture.Word36);
        }

    }
}
