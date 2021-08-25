using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Reko.Arch.Avr.Avr32
{
    public class RegisterRange : AbstractMachineOperand
    {
        public RegisterStorage[] Registers { get; }
        public int RegisterIndex { get; }
        public int Count { get; } 

        public RegisterRange(RegisterStorage[] gpRegisters, int iRegStart, int cRegs)
            : base(PrimitiveType.Word32)    // Don't care.
        {
            this.Registers = gpRegisters;
            this.RegisterIndex = iRegStart;
            this.Count = cRegs;
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteString(Registers[RegisterIndex].Name);
            if (Count <= 1)
                return;
            if (Count <= 2)
            {
                renderer.WriteString(",");
            }
            else
            {
                renderer.WriteString("-");
            }
            renderer.WriteString(Registers[RegisterIndex + Count - 1].Name);
        }

        public IEnumerable<RegisterStorage> Enumerate()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return Registers[RegisterIndex + i];
            }
        }
    }
}