#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

namespace Reko.Arch.SuperH
{
    public class SuperHInstruction : MachineInstruction
    {


        public Opcode Opcode { get; set; }
        public MachineOperand op1 { get; set; }
        public MachineOperand op2 { get; set; }
        public MachineOperand op3 { get; set; }

        public override int OpcodeAsInteger => (int) Opcode;

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
            { Opcode.fmov_d, "fmov.d" },
            { Opcode.fmov_s, "fmov.s" },
            { Opcode.ldc_l, "ldc.l" },
            { Opcode.lds_l, "lds.l" },
            { Opcode.mac_l, "mac.l" },
            { Opcode.mac_w, "mac.w" },
            { Opcode.mov_b, "mov.b" },
            { Opcode.mov_l, "mov.l" },
            { Opcode.mov_w, "mov.w" },
            { Opcode.movca_l, "movca.l" },
            { Opcode.movco_l, "movco.l" },
            { Opcode.movmu_l, "movmu.l" },
            { Opcode.mul_l, "mul.l" },
            { Opcode.stc_l, "stc.l" },
            { Opcode.sts_l, "sts.l" },
            { Opcode.swap_w, "swap.w" },
            { Opcode.tas_b, "tas.b" },
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
            if (op2 == null)
                return;
            writer.WriteChar(',');
            Render(op2, writer, options);
            if (op3 == null)
                return;
            writer.WriteChar(',');
            Render(op3, writer, options);
        }

        private void Render(MachineOperand op, MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            switch (op)
            {
            case ImmediateOperand immOp:
                writer.WriteChar('#');
                immOp.Write(writer, options);
                return;
            case MemoryOperand memOp:
                if (memOp.mode == AddressingMode.PcRelativeDisplacement)
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
                        writer.WriteChar('(');
                        writer.WriteAddress(addr.ToString(), addr);
                        writer.WriteChar(')');
                        writer.AddAnnotation(op.ToString());
                    }
                    else
                    {
                        op.Write(writer, options);
                        writer.AddAnnotation(addr.ToString());
                    }
                    return;
                }
                goto default;
            default:
                op.Write(writer, options);
                break;
            }
        }
    }
}