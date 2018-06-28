#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Vax
{
    public class VaxInstruction : MachineInstruction
    {
        internal MachineOperand[] Operands;
        private static Dictionary<Opcode, InstrClass> classOf;

        public override InstrClass InstructionClass
        {
            get
            {
                InstrClass c;
                if (!classOf.TryGetValue(Opcode, out c))
                {
                    c = InstrClass.Linear;
                }
                return c;
            }
        }

        public Opcode Opcode { get; internal set; }

        public override int OpcodeAsInteger
        {
            get { return (int)Opcode; }
        }

        public override MachineOperand GetOperand(int i)
        {
            if (i >= Operands.Length)
                return null;
            return Operands[i];
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(this.Opcode.ToString());
            writer.Tab();
            bool sep = false; 
            foreach (var op in Operands)
            {
                if (sep)
                    writer.WriteChar(',');
                sep = true;
                if (op is ImmediateOperand)
                {
                    writer.WriteChar('#');
                    op.Write(writer, options);
                }
                else if (op is MemoryOperand && ((MemoryOperand)op).Base == Registers.pc)
                {
                    var addr = this.Address + (this.Length + ((MemoryOperand)op).Offset.ToInt32());
                    if ((options & MachineInstructionWriterOptions.ResolvePcRelativeAddress) != 0)
                    {
                        writer.WriteAddress(addr.ToString(), addr);
                        writer.AddAnnotation(op.ToString());
                    }
                    else
                    {
                        op.Write(writer, options);
                        writer.AddAnnotation(addr.ToString());
                    }
                }
                else
                {
                    op.Write(writer, options);
                }
            }
        }

        static VaxInstruction()
        {
            classOf = new Dictionary<Opcode, InstrClass>
            {
                { Opcode.Invalid, InstrClass.Invalid },
                { Opcode.aobleq,  InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.aoblss,  InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.beql ,  InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.bgeq ,  InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.bgequ,  InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.bgtr ,  InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.bgtru,  InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.bleq ,  InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.blequ,  InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.blss ,  InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.blssu,  InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.bneq ,  InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.bvc,    InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.bvs,    InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.blbc,   InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.blbs,   InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.brb,    InstrClass.Transfer },
                { Opcode.brw,    InstrClass.Transfer },
                { Opcode.callg,  InstrClass.Transfer|InstrClass.Call },
                { Opcode.calls,  InstrClass.Transfer|InstrClass.Call },
                { Opcode.jmp,    InstrClass.Transfer },
                { Opcode.jsb,    InstrClass.Transfer|InstrClass.Call },
                { Opcode.ret,    InstrClass.Transfer },
                { Opcode.rsb,    InstrClass.Transfer },
                { Opcode.sobgeq, InstrClass.Transfer|InstrClass.Conditional },
                { Opcode.sobgtr, InstrClass.Transfer|InstrClass.Conditional }
            };
        }
    }
}
