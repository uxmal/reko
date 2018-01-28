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

using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Z80
{
    public class Z80Instruction : MachineInstruction
    {
        private static Dictionary<Opcode, InstructionClass> classOf;

        public Opcode Code;
        public MachineOperand Op1;
        public MachineOperand Op2;

        public override bool IsValid { get { return Code != Opcode.illegal; } }

        public override int OpcodeAsInteger { get { return (int)Code; } }

        public override InstructionClass InstructionClass
        {
            get
            {
                InstructionClass ct;
                if (!classOf.TryGetValue(Code, out ct))
                {
                    ct = InstructionClass.Linear;
                }
                else if ((Op1 as ConditionOperand) != null)
                {
                    ct |= InstructionClass.Conditional;
                }
                return ct;
            }
        }

        public override MachineOperand GetOperand(int i)
        {
            if (i == 0)
                return Op1;
            else if (i == 1)
                return Op2;
            else
                return null;
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (Code == Opcode.ex_af)
            {
                writer.WriteOpcode("ex");
                writer.Tab();
                writer.WriteString("af,af'");
                return;
            }
            writer.WriteOpcode(Code.ToString());
            if (Op1 != null)
            {
                writer.Tab();
                Op1.Write(writer, options);
                if (Op2 != null)
                {
                    writer.WriteString(",");
                    Op2.Write(writer, options);
                }
            }
        }

        static Z80Instruction()
        {
            classOf = new Dictionary<Opcode, InstructionClass>
            {
                { Opcode.illegal, InstructionClass.Transfer },

                { Opcode.jc,      InstructionClass.Transfer | InstructionClass.Conditional },
                { Opcode.jm,      InstructionClass.Transfer | InstructionClass.Conditional },
                { Opcode.jmp,     InstructionClass.Transfer },
                { Opcode.jnc,     InstructionClass.Transfer | InstructionClass.Conditional },
                { Opcode.jnz,     InstructionClass.Transfer | InstructionClass.Conditional },
                { Opcode.jpe,     InstructionClass.Transfer | InstructionClass.Conditional },
                { Opcode.jpo,     InstructionClass.Transfer | InstructionClass.Conditional },
                { Opcode.jz,      InstructionClass.Transfer | InstructionClass.Conditional },

                { Opcode.call,    InstructionClass.Transfer | InstructionClass.Call},
                { Opcode.djnz,    InstructionClass.Transfer | InstructionClass.Conditional},
                { Opcode.jr,      InstructionClass.Transfer },
                { Opcode.ret,     InstructionClass.Transfer },
                { Opcode.reti,    InstructionClass.Transfer },
                { Opcode.retn,    InstructionClass.Transfer },

                { Opcode.hlt,     InstructionClass.Transfer },
                { Opcode.jp,      InstructionClass.Transfer }
            };
        }
    }
}
