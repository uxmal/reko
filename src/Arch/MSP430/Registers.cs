using Reko.Core;
using System.Linq;

namespace Reko.Arch.MSP430
{
    public static class Registers
    {
        public static RegisterStorage[] GpRegisters;

        public static RegisterStorage pc = new RegisterStorage("pc", 0, 0, Msp430Architecture.Word20);

        static Registers()
        {
            GpRegisters = new RegisterStorage[]
            {
                pc,
                new RegisterStorage("sp", 1, 0, Msp430Architecture.Word20),
                new RegisterStorage("sr", 2, 0, Msp430Architecture.Word20),
                new RegisterStorage("cg2", 3, 0, Msp430Architecture.Word20),
            }.Concat(
                Enumerable.Range(4, 12)
                .Select(i => new RegisterStorage(
                    string.Format("r{0}", i), i, 0, Msp430Architecture.Word20)))

                .ToArray();
        }
    }
}