using Reko.Core;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Arm
{
    public static class Registers
    {
        public static RegisterStorage r0 = new RegisterStorage("r0", 0, 0, PrimitiveType.Word32);
        public static RegisterStorage r1 = new RegisterStorage("r1", 1, 0, PrimitiveType.Word32);
        public static RegisterStorage r2 = new RegisterStorage("r2", 2, 0, PrimitiveType.Word32);
        public static RegisterStorage r3 = new RegisterStorage("r3", 3, 0, PrimitiveType.Word32);
        public static RegisterStorage r4 = new RegisterStorage("r4", 4, 0, PrimitiveType.Word32);
        public static RegisterStorage r5 = new RegisterStorage("r5", 5, 0, PrimitiveType.Word32);
        public static RegisterStorage r6 = new RegisterStorage("r6", 6, 0, PrimitiveType.Word32);
        public static RegisterStorage r7 = new RegisterStorage("r7", 7, 0, PrimitiveType.Word32);
        public static RegisterStorage r8 = new RegisterStorage("r8", 8, 0, PrimitiveType.Word32);
        public static RegisterStorage r9 = new RegisterStorage("r9", 9, 0, PrimitiveType.Word32);
        public static RegisterStorage r10 = new RegisterStorage("r10", 10, 0, PrimitiveType.Word32);
        public static RegisterStorage r11 = new RegisterStorage("r11", 11, 0, PrimitiveType.Word32);
        public static RegisterStorage r12 = new RegisterStorage("r12", 12, 0, PrimitiveType.Word32);
        public static RegisterStorage sp = new RegisterStorage("sp", 13, 0, PrimitiveType.Word32);
        public static RegisterStorage lr = new RegisterStorage("lr", 14, 0, PrimitiveType.Word32);
        public static RegisterStorage pc = new RegisterStorage("pc", 15, 0, PrimitiveType.Word32);

        public static RegisterStorage cpsr = new RegisterStorage("cpsr", 16, 0, PrimitiveType.Word32);
        public static RegisterStorage fpscr = new RegisterStorage("fpscr", 17, 0, PrimitiveType.Word32);
    }
}
