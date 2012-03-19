using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.PowerPC
{
    public static class Registers
    {
        public static MachineRegister r0 = new MachineRegister("r0", 0, PrimitiveType.Word32);
        public static MachineRegister r1 = new MachineRegister("r1", 1, PrimitiveType.Word32);
        public static MachineRegister r2 = new MachineRegister("r2", 2, PrimitiveType.Word32);
        public static MachineRegister r3 = new MachineRegister("r3", 3, PrimitiveType.Word32);
        public static MachineRegister r4 = new MachineRegister("r4", 4, PrimitiveType.Word32);
    }
}
