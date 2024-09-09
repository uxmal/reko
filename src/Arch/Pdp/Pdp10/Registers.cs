using Reko.Arch.Pdp.Memory;
using Reko.Core;
using Reko.Core.Types;
using System;

namespace Reko.Arch.Pdp.Pdp10
{
    public static class Registers
    {
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

            C0 = new FlagGroupStorage(Psw, (uint) FlagM.C0, nameof(C0));
            C1 = new FlagGroupStorage(Psw, (uint) FlagM.C1, nameof(C1));
            V = new FlagGroupStorage(Psw, (uint) FlagM.V, nameof(V));
            T = new FlagGroupStorage(Psw, (uint) FlagM.T, nameof(T));
            ND = new FlagGroupStorage(Psw, (uint) FlagM.T, nameof(ND));
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
