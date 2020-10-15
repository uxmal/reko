using Reko.Core;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.MilStd1750
{
    public static class Registers
    {
        public static RegisterStorage[] GpRegs { get; }
        public static RegisterStorage pir { get; }    /* 16 */
        public static RegisterStorage mk { get; }     /* 17 */
        public static RegisterStorage ft { get; }     /* 18 */
        public static RegisterStorage ic { get; }     /* 19 */
        public static RegisterStorage sw { get; }     /* 20 */
        public static RegisterStorage ta { get; }     /* 21 */
        public static RegisterStorage tb { get; }     /* 22 */
        public static RegisterStorage go { get; }     /* not a real register but handled like TA/TB */
        public static RegisterStorage sys { get; }    /* system configuration register */

        static Registers()
        {
            var factory = new StorageFactory();
            GpRegs = factory.RangeOfReg(16, n => $"gp{n}", PrimitiveType.Word16);
            pir = factory.Reg16("pir");
            mk = factory.Reg16("mk");
            ft = factory.Reg16("ft");
            ic = factory.Reg16("ic");
            sw = factory.Reg16("sw");
            ta = factory.Reg16("ta");
            tb = factory.Reg16("tb");
            go = factory.Reg16("go");
            sys = factory.Reg16("sys");
        }
    }
}
