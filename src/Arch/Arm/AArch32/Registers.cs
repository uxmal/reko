using Reko.Core;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Arm.AArch32
{
    public static class Registers
    {
        public static readonly RegisterStorage r0 = new RegisterStorage("r0", 0, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage r1 = new RegisterStorage("r1", 1, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage r2 = new RegisterStorage("r2", 2, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage r3 = new RegisterStorage("r3", 3, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage r4 = new RegisterStorage("r4", 4, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage r5 = new RegisterStorage("r5", 5, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage r6 = new RegisterStorage("r6", 6, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage r7 = new RegisterStorage("r7", 7, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage r8 = new RegisterStorage("r8", 8, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage r9 = new RegisterStorage("r9", 9, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage r10 = new RegisterStorage("r10", 10, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage fp = new RegisterStorage("fp", 11, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage ip = new RegisterStorage("ip", 12, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage sp = new RegisterStorage("sp", 13, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage lr = new RegisterStorage("lr", 14, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage pc = new RegisterStorage("pc", 15, 0, PrimitiveType.Word32);

        public static readonly RegisterStorage[] GpRegs = new[]
        {
                Registers.r0, Registers.r1, Registers.r2, Registers.r3,
                Registers.r4, Registers.r5, Registers.r6, Registers.r7,
                Registers.r8, Registers.r9, Registers.r10, Registers.fp,
                Registers.ip, Registers.sp, Registers.lr, Registers.pc,
        };

        public static readonly RegisterStorage cpsr = new RegisterStorage("cpsr", 16, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage fpscr = new RegisterStorage("fpscr", 17, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage spsr = new RegisterStorage("spsr", 18, 0, PrimitiveType.Word32);

        // We need to provide the coprocessors as named register storages even though
        // they more appropriately should be treated as symbols.
        public static readonly RegisterStorage[] Coprocessors = Enumerable.Range(0, 16)
            .Select(n => new RegisterStorage($"p{n}", 128 + n, 0, PrimitiveType.Byte))
            .ToArray();
        public static readonly RegisterStorage[] CoprocessorRegisters = Enumerable.Range(0, 16)
            .Select(n => new RegisterStorage($"c{n}", 144 + n, 0, PrimitiveType.Word32))
            .ToArray();

        // The 'S..' floating point registers alias the 'D..' double floating point registers
        // which in turn alias the  128-bit 'Q..' registers 
        public static readonly RegisterStorage[] QRegs = Enumerable.Range(0, 16)
            .Select(n => new RegisterStorage($"q{n}", 160, 0, PrimitiveType.Word128))
            .ToArray();
        public static readonly RegisterStorage[] DRegs = Enumerable.Range(0, 32)
            .Select(n => new RegisterStorage($"d{n}", QRegs[n / 2].Number, (uint)(n & 1) * 64, PrimitiveType.Word64))
            .ToArray();
        public static readonly RegisterStorage[] SRegs = Enumerable.Range(0, 32)
            .Select(n => new RegisterStorage($"s{n}", QRegs[n / 4].Number, (uint)(n & 3) * 32, PrimitiveType.Word32))
            .ToArray();

        public static readonly Dictionary<string, RegisterStorage> RegistersByName;

        static Registers()
        {
            RegistersByName = GpRegs
                .Concat(new[] { cpsr, fpscr, spsr })
                .Concat(CoprocessorRegisters)
                .Concat(QRegs)
                .Concat(DRegs)
                .Concat(SRegs)
                .ToDictionary(r => r.Name);
        }
    }
}
