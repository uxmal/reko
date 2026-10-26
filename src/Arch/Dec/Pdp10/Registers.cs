using Reko.Arch.Dec.Memory;
using Reko.Core;
using Reko.Core.Machine;
using System;

namespace Reko.Arch.Dec.Pdp10
{
    public static class Registers
    {
        public static RegisterBank All { get; }

        public static RegisterStorage[] Accumulators { get; }

        // "Virtual" processor status word. It appears the PDP-10 didn't have any,
        // but the status bits have to live somewhere.
        public static RegisterStorage Psw { get; }

        public static FlagGroupStorage C0 { get; }
        public static FlagGroupStorage C1 { get; }
        public static FlagGroupStorage V  { get; }
        public static FlagGroupStorage T { get; }
        public static FlagGroupStorage ND { get; }

        static Registers()
        {
            var factory = new StorageFactory();
            Accumulators = factory.RangeOfReg(16, u => $"r{u}", PdpTypes.Word36);
            Psw = factory.Reg("Psw", PdpTypes.Word36);
            All = new RegisterBank(factory.NamesToRegisters.Values);

            C0 = new FlagGroupStorage(Psw, (ulong) FlagM.C0, nameof(C0));
            C1 = new FlagGroupStorage(Psw, (ulong) FlagM.C1, nameof(C1));
            V = new FlagGroupStorage(Psw, (ulong) FlagM.V, nameof(V));
            T = new FlagGroupStorage(Psw, (ulong) FlagM.T, nameof(T));
            ND = new FlagGroupStorage(Psw, (ulong) FlagM.T, nameof(ND));
        }
    }

    [Flags]
    public enum FlagM
    {
        C0 = 1,
        C1 = 2,
        V = 4,
        T = 8,
        ND = 16 // No divide
    }
}
