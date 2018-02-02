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

using System;
using Reko.Core.Machine;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.SuperH
{
    public class SuperHInstruction : MachineInstruction
    {
        public override InstructionClass InstructionClass
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsValid
        {
            get { return Opcode == Opcode.invalid; }
        }

        public Opcode Opcode { get; set; }

        public MachineOperand op1 { get; set; }
        public MachineOperand op2 { get; set; }

        public override int OpcodeAsInteger
        {
            get { throw new NotImplementedException(); }
        }

        public override MachineOperand GetOperand(int i)
        {
            throw new NotImplementedException();
        }

        private static Dictionary<Opcode, string> opcodes = new Dictionary<Opcode, string>
        {
            { Opcode.and_b, "and.b" },
            { Opcode.bf_s, "bf/s" },
            { Opcode.bt_s, "bt/s" },
            { Opcode.cmp_eq, "cmp/eq" },
            { Opcode.cmp_ge, "cmp/ge" },
            { Opcode.cmp_gt, "cmp/gt" },
            { Opcode.cmp_hi, "cmp/hi" },
            { Opcode.cmp_hs, "cmp/hs" },
            { Opcode.cmp_pl, "cmp/pl" },
            { Opcode.cmp_pz, "cmp/pz" },
            { Opcode.cmp_str, "cmp/str" },
            { Opcode.dmuls_l, "dmuls.l" },
            { Opcode.exts_b, "exts.b" },
            { Opcode.exts_w, "exts.w" },
            { Opcode.extu_b, "extu.b" },
            { Opcode.extu_w, "extu.w" },
            { Opcode.fcmp_eq, "fcmp/eq" },
            { Opcode.fcmp_gt, "fcmp/gt" },
            { Opcode.lds_l, "lds.l" },
            { Opcode.mov_b, "mov.b" },
            { Opcode.mov_l, "mov.l" },
            { Opcode.mov_w, "mov.w" },
            { Opcode.mul_l, "mul.l" },
            { Opcode.sts_l, "sts.l" },
            { Opcode.swap_w, "swap.w" },
        };

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (!opcodes.TryGetValue(Opcode, out var sOpcode))
                sOpcode = Opcode.ToString();
            writer.WriteOpcode(sOpcode);
            if (op1 == null)
                return;
            writer.Tab();
            Render(op1, writer, options);
            if (op2 != null)
            {
                writer.Write(',');
                Render(op2, writer, options);
            }
        }

        private void Render(MachineOperand op, MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var immOp = op as ImmediateOperand;
            if (immOp != null)
            {
                writer.Write('#');
                immOp.Write(writer, options);
                return;
            }
            var memOp = op as MemoryOperand;
            if (memOp != null && memOp.mode == AddressingMode.PcRelativeDisplacement)
            {
                uint uAddr = this.Address.ToUInt32();
                if (memOp.Width.Size == 4)
                {
                    uAddr &= ~3u;
                }
                uAddr += (uint)(memOp.disp + 4);
                var addr = Core.Address.Ptr32(uAddr);
                if ((options & MachineInstructionWriterOptions.ResolvePcRelativeAddress) != 0)
                {
                    writer.Write('(');
                    writer.WriteAddress(addr.ToString(), addr);
                    writer.Write(')');
                    writer.AddAnnotation(op.ToString());
                }
                else
                {
                    op.Write(writer, options);
                    writer.AddAnnotation(addr.ToString());
                }
                return;
            }
            op.Write(writer, options);
        }
    }
}