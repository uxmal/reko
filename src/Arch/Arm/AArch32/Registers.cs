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
        public static readonly RegisterStorage r0 = RegisterStorage.Reg32("r0", 0);
        public static readonly RegisterStorage r1 = RegisterStorage.Reg32("r1", 1);
        public static readonly RegisterStorage r2 = RegisterStorage.Reg32("r2", 2);
        public static readonly RegisterStorage r3 = RegisterStorage.Reg32("r3", 3);
        public static readonly RegisterStorage r4 = RegisterStorage.Reg32("r4", 4);
        public static readonly RegisterStorage r5 = RegisterStorage.Reg32("r5", 5);
        public static readonly RegisterStorage r6 = RegisterStorage.Reg32("r6", 6);
        public static readonly RegisterStorage r7 = RegisterStorage.Reg32("r7", 7);
        public static readonly RegisterStorage r8 = RegisterStorage.Reg32("r8", 8);
        public static readonly RegisterStorage r9 = RegisterStorage.Reg32("r9", 9);
        public static readonly RegisterStorage r10 = RegisterStorage.Reg32("r10", 10);
        public static readonly RegisterStorage fp = RegisterStorage.Reg32("fp", 11);
        public static readonly RegisterStorage ip = RegisterStorage.Reg32("ip", 12);
        public static readonly RegisterStorage sp = RegisterStorage.Reg32("sp", 13);
        public static readonly RegisterStorage lr = RegisterStorage.Reg32("lr", 14);
        public static readonly RegisterStorage pc = RegisterStorage.Reg32("pc", 15);

        public static readonly RegisterStorage[] GpRegs = new[]
        {
                Registers.r0, Registers.r1, Registers.r2, Registers.r3,
                Registers.r4, Registers.r5, Registers.r6, Registers.r7,
                Registers.r8, Registers.r9, Registers.r10, Registers.fp,
                Registers.ip, Registers.sp, Registers.lr, Registers.pc,
        };

        public static readonly RegisterStorage cpsr = RegisterStorage.Reg32("cpsr", 16);
        public static readonly RegisterStorage fpscr = RegisterStorage.Reg32("fpscr", 17);
        public static readonly RegisterStorage spsr = RegisterStorage.Reg32("spsr", 18);
        public static readonly RegisterStorage fpsid = RegisterStorage.Reg32("fpsid", 19);
        public static readonly RegisterStorage mvfr2 = RegisterStorage.Reg32("mvfr2", 20);
        public static readonly RegisterStorage mvfr1 = RegisterStorage.Reg32("mvfr1", 21);
        public static readonly RegisterStorage mvfr0 = RegisterStorage.Reg32("mvfr0", 22);
        public static readonly RegisterStorage fpexc = RegisterStorage.Reg32("fpexc", 23);

        public static readonly RegisterStorage r8_usr = RegisterStorage.Reg32("r8_usr", 40);
        public static readonly RegisterStorage r9_usr = RegisterStorage.Reg32("r9_usr", 41);
        public static readonly RegisterStorage r10_usr = RegisterStorage.Reg32("r10_usr", 42);
        public static readonly RegisterStorage r11_usr = RegisterStorage.Reg32("r11_usr", 43);
        public static readonly RegisterStorage r12_usr = RegisterStorage.Reg32("r12_usr", 44);
        public static readonly RegisterStorage sp_usr = RegisterStorage.Reg32("sp_usr", 45);
        public static readonly RegisterStorage lr_usr = RegisterStorage.Reg32("lr_usr", 46);

        public static readonly RegisterStorage r8_fiq = RegisterStorage.Reg32("r8_fiq", 48);
        public static readonly RegisterStorage r9_fiq = RegisterStorage.Reg32("r9_fiq", 49);
        public static readonly RegisterStorage r10_fiq = RegisterStorage.Reg32("r10_fiq", 50);
        public static readonly RegisterStorage r11_fiq = RegisterStorage.Reg32("r11_fiq", 51);
        public static readonly RegisterStorage r12_fiq = RegisterStorage.Reg32("r12_fiq", 52);
        public static readonly RegisterStorage sp_fiq = RegisterStorage.Reg32("sp_fiq", 53);
        public static readonly RegisterStorage lr_fiq = RegisterStorage.Reg32("lr_fiq", 54);

        public static readonly RegisterStorage lr_irq = RegisterStorage.Reg32("lr_irq", 55);
        public static readonly RegisterStorage sp_irq = RegisterStorage.Reg32("sp_irq", 56);

        public static readonly RegisterStorage lr_svc = RegisterStorage.Reg32("lr_svc", 57);
        public static readonly RegisterStorage sp_svc = RegisterStorage.Reg32("sp_svc", 58);
        public static readonly RegisterStorage lr_abt = RegisterStorage.Reg32("lr_abt", 59);
        public static readonly RegisterStorage sp_abt = RegisterStorage.Reg32("sp_abt", 60);
        public static readonly RegisterStorage lr_und = RegisterStorage.Reg32("lr_und", 61);
        public static readonly RegisterStorage sp_und = RegisterStorage.Reg32("sp_und", 62);
        public static readonly RegisterStorage lr_mon = RegisterStorage.Reg32("lr_mon", 63);
        public static readonly RegisterStorage sp_mon = RegisterStorage.Reg32("sp_mon", 64);
        public static readonly RegisterStorage elr_hyp = RegisterStorage.Reg32("elr_hyp", 65);
        public static readonly RegisterStorage sp_hyp = RegisterStorage.Reg32("sp_hyp", 66);
        public static readonly RegisterStorage spsr_fiq = RegisterStorage.Reg32("spsr_fiq", 67);
        public static readonly RegisterStorage spsr_irq = RegisterStorage.Reg32("spsr_irq", 68);
        public static readonly RegisterStorage spsr_svc = RegisterStorage.Reg32("spsr_svc", 69);
        public static readonly RegisterStorage spsr_abt = RegisterStorage.Reg32("spsr_abt", 70);
        public static readonly RegisterStorage spsr_und = RegisterStorage.Reg32("spsr_und", 71);
        public static readonly RegisterStorage spsr_mon = RegisterStorage.Reg32("spsr_mon", 72);
        public static readonly RegisterStorage spsr_hyp = RegisterStorage.Reg32("spsr_hyp", 73);

        // We need to provide the coprocessors as named register storages even though
        // they more appropriately should be treated as symbols.
        public static readonly RegisterStorage[] Coprocessors = Enumerable.Range(0, 16)
            .Select(n => RegisterStorage.Reg8($"p{n}", 128 + n, 0))
            .ToArray();
        public static readonly RegisterStorage[] CoprocessorRegisters = Enumerable.Range(0, 16)
            .Select(n => RegisterStorage.Reg32($"cr{n}", 144 + n))
            .ToArray();

        // The 'S..' floating point registers alias the 'D..' double floating point registers
        // which in turn alias the  128-bit 'Q..' registers 
        public static readonly RegisterStorage[] QRegs = Enumerable.Range(0, 16)
            .Select(n => new RegisterStorage($"q{n}", 160 + n, 0, PrimitiveType.Word128))
            .ToArray();
        public static readonly RegisterStorage[] DRegs = Enumerable.Range(0, 32)
            .Select(n => RegisterStorage.Reg64($"d{n}", QRegs[n / 2].Number, (uint)(n & 1) * 64))
            .ToArray();
        public static readonly RegisterStorage[] SRegs = Enumerable.Range(0, 32)
            .Select(n => RegisterStorage.Reg32($"s{n}", QRegs[n / 4].Number, (uint)(n & 3) * 32))
            .ToArray();

        public static readonly Dictionary<string, RegisterStorage> ByName;
        public static readonly Dictionary<StorageDomain, RegisterStorage> ByDomain;

        public static readonly HashSet<RegisterStorage> SIMDRegisters;

        public static readonly FlagGroupStorage C =  FlagGroup(FlagM.CF, "C", PrimitiveType.Bool);
        public static readonly FlagGroupStorage N = FlagGroup(FlagM.NF, "N", PrimitiveType.Bool);
        public static readonly FlagGroupStorage NV = FlagGroup(FlagM.NF | FlagM.VF, "NV", PrimitiveType.Byte);
        public static readonly FlagGroupStorage NZCV = FlagGroup(FlagM.NF| FlagM.ZF | FlagM.CF | FlagM.VF, "NZCV", PrimitiveType.Byte);
        public static readonly FlagGroupStorage NZC = FlagGroup(FlagM.NF | FlagM.ZF | FlagM.CF, "NZC", PrimitiveType.Byte);
        public static readonly FlagGroupStorage NZV = FlagGroup(FlagM.NF | FlagM.ZF | FlagM.VF, "NZV", PrimitiveType.Byte);
        public static readonly FlagGroupStorage Q = FlagGroup(FlagM.QF, "Q", PrimitiveType.Bool);
        public static readonly FlagGroupStorage V = FlagGroup(FlagM.VF, "V", PrimitiveType.Bool);
        public static readonly FlagGroupStorage Z = FlagGroup(FlagM.ZF, "Z", PrimitiveType.Bool);
        public static readonly FlagGroupStorage ZC = FlagGroup(FlagM.ZF | FlagM.CF, "ZC", PrimitiveType.Byte);

        static Registers()
        {
            ByName = GpRegs
                .Concat(new[] { cpsr, fpscr, spsr })
                .Concat(CoprocessorRegisters)
                .Concat(QRegs)
                .Concat(DRegs)
                .Concat(SRegs)
                .ToDictionary(r => r.Name);

            SIMDRegisters = QRegs.Concat(DRegs).Concat(SRegs).ToHashSet();
            ByDomain = GpRegs
                .Concat(new[] { cpsr, fpscr, spsr })
                .Concat(CoprocessorRegisters)
                .Concat(QRegs)
                .ToDictionary(r => r.Domain);
        }

        private static FlagGroupStorage FlagGroup(FlagM grf, string name, PrimitiveType type)
        {
            return new FlagGroupStorage(Registers.cpsr, (uint) grf, name, type);
        }
    }
}
