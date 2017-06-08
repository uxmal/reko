#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
        private static Dictionary<Opcode, InstructionClass> classOf;

        public override InstructionClass InstructionClass
        {
            get
            {
                InstructionClass c;
                if (!classOf.TryGetValue(Opcode, out c))
                {
                    c = InstructionClass.Linear;
                }
                return c;
            }
        }

        public override bool IsValid
        {
            get
            {
                throw new NotImplementedException();
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
                    writer.Write(',');
                sep = true;
                if (op is ImmediateOperand)
                {
                    writer.Write('#');
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
            classOf = new Dictionary<Opcode, InstructionClass>
            {
                { Opcode.Invalid, InstructionClass.Invalid },
                { Opcode.aobleq,  InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.aoblss,  InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.beql ,  InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.bgeq ,  InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.bgequ,  InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.bgtr ,  InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.bgtru,  InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.bleq ,  InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.blequ,  InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.blss ,  InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.blssu,  InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.bneq ,  InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.bvc,    InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.bvs,    InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.blbc,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.blbs,   InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.brb,    InstructionClass.Transfer },
                { Opcode.brw,    InstructionClass.Transfer },
                { Opcode.callg,  InstructionClass.Transfer|InstructionClass.Call },
                { Opcode.calls,  InstructionClass.Transfer|InstructionClass.Call },
                { Opcode.jmp,    InstructionClass.Transfer },
                { Opcode.jsb,    InstructionClass.Transfer|InstructionClass.Call },
                { Opcode.ret,    InstructionClass.Transfer },
                { Opcode.rsb,    InstructionClass.Transfer },
                { Opcode.sobgeq, InstructionClass.Transfer|InstructionClass.Conditional },
                { Opcode.sobgtr, InstructionClass.Transfer|InstructionClass.Conditional }
            };
        }
    }
}
