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
        public static readonly RegisterStorage fpsid = new RegisterStorage("fpsid", 19, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage mvfr2 = new RegisterStorage("mvfr2", 20, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage mvfr1 = new RegisterStorage("mvfr1", 21, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage mvfr0 = new RegisterStorage("mvfr0", 22, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage fpexc = new RegisterStorage("fpexc", 23, 0, PrimitiveType.Word32);

        public static readonly RegisterStorage r8_usr = new RegisterStorage("r8_usr", 40, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage r9_usr = new RegisterStorage("r9_usr", 41, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage r10_usr = new RegisterStorage("r10_usr", 42, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage r11_usr = new RegisterStorage("r11_usr", 43, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage r12_usr = new RegisterStorage("r12_usr", 44, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage sp_usr = new RegisterStorage("sp_usr", 45, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage lr_usr = new RegisterStorage("lr_usr", 46, 0, PrimitiveType.Word32);

        public static readonly RegisterStorage r8_fiq = new RegisterStorage("r8_fiq", 48, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage r9_fiq = new RegisterStorage("r9_fiq", 49, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage r10_fiq = new RegisterStorage("r10_fiq", 50, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage r11_fiq = new RegisterStorage("r11_fiq", 51, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage r12_fiq = new RegisterStorage("r12_fiq", 52, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage sp_fiq = new RegisterStorage("sp_fiq", 53, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage lr_fiq = new RegisterStorage("lr_fiq", 54, 0, PrimitiveType.Word32);

        public static readonly RegisterStorage lr_irq = new RegisterStorage("lr_irq", 55, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage sp_irq = new RegisterStorage("sp_irq", 56, 0, PrimitiveType.Word32);

        public static readonly RegisterStorage lr_svc = new RegisterStorage("lr_svc", 57, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage sp_svc = new RegisterStorage("sp_svc", 58, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage lr_abt = new RegisterStorage("lr_abt", 59, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage sp_abt = new RegisterStorage("sp_abt", 60, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage lr_und = new RegisterStorage("lr_und", 61, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage sp_und = new RegisterStorage("sp_und", 62, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage lr_mon = new RegisterStorage("lr_mon", 63, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage sp_mon = new RegisterStorage("sp_mon", 64, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage elr_hyp = new RegisterStorage("elr_hyp", 65, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage sp_hyp = new RegisterStorage("sp_hyp", 66, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage spsr_fiq = new RegisterStorage("spsr_fiq", 67, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage spsr_irq = new RegisterStorage("spsr_irq", 68, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage spsr_svc = new RegisterStorage("spsr_svc", 69, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage spsr_abt = new RegisterStorage("spsr_abt", 70, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage spsr_und = new RegisterStorage("spsr_und", 71, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage spsr_mon = new RegisterStorage("spsr_mon", 72, 0, PrimitiveType.Word32);
        public static readonly RegisterStorage spsr_hyp = new RegisterStorage("spsr_hyp", 73, 0, PrimitiveType.Word32);

        // We need to provide the coprocessors as named register storages even though
        // they more appropriately should be treated as symbols.
        public static readonly RegisterStorage[] Coprocessors = Enumerable.Range(0, 16)
            .Select(n => new RegisterStorage($"p{n}", 128 + n, 0, PrimitiveType.Byte))
            .ToArray();
        public static readonly RegisterStorage[] CoprocessorRegisters = Enumerable.Range(0, 16)
            .Select(n => new RegisterStorage($"cr{n}", 144 + n, 0, PrimitiveType.Word32))
            .ToArray();

        // The 'S..' floating point registers alias the 'D..' double floating point registers
        // which in turn alias the  128-bit 'Q..' registers 
        public static readonly RegisterStorage[] QRegs = Enumerable.Range(0, 16)
            .Select(n => new RegisterStorage($"q{n}", 160 + n, 0, PrimitiveType.Word128))
            .ToArray();
        public static readonly RegisterStorage[] DRegs = Enumerable.Range(0, 32)
            .Select(n => new RegisterStorage($"d{n}", QRegs[n / 2].Number, (uint)(n & 1) * 64, PrimitiveType.Word64))
            .ToArray();
        public static readonly RegisterStorage[] SRegs = Enumerable.Range(0, 32)
            .Select(n => new RegisterStorage($"s{n}", QRegs[n / 4].Number, (uint)(n & 3) * 32, PrimitiveType.Word32))
            .ToArray();

        public static readonly Dictionary<string, RegisterStorage> RegistersByName;
        public static readonly Dictionary<StorageDomain, RegisterStorage> RegistersByDomain;

        public static readonly HashSet<RegisterStorage> SIMDRegisters;

        static Registers()
        {
            RegistersByName = GpRegs
                .Concat(new[] { cpsr, fpscr, spsr })
                .Concat(CoprocessorRegisters)
                .Concat(QRegs)
                .Concat(DRegs)
                .Concat(SRegs)
                .ToDictionary(r => r.Name);

            SIMDRegisters = QRegs.Concat(DRegs).Concat(SRegs).ToHashSet();
            RegistersByDomain = GpRegs
                .Concat(new[] { cpsr, fpscr, spsr })
                .Concat(CoprocessorRegisters)
                .Concat(QRegs)
                .ToDictionary(r => r.Domain);
        }
    }
}
