using Reko.Core;
using System.Linq;

namespace Reko.Arch.MSP430
{
    public static class Registers
    {
        public static RegisterStorage[] GpRegisters =
            Enumerable.Range(0, 16)
                .Select(i => new RegisterStorage(
                    string.Format("r{0}", i), i, 0, Msp430Architecture.Word20))
                .ToArray();
    }
}