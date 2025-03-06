#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;

namespace Reko.Arch.SuperH
{
    public class SuperHInstruction : MachineInstruction
    {
        public Mnemonic Mnemonic { get; set; }

        public override int MnemonicAsInteger => (int) Mnemonic;

        public override string MnemonicAsString => Mnemonic.ToString();

        private static readonly Dictionary<Mnemonic, string> mnemonics = new Dictionary<Mnemonic, string>
        {
            { Mnemonic.and_b, "and.b" },
            { Mnemonic.bf_s, "bf/s" },
            { Mnemonic.bt_s, "bt/s" },
            { Mnemonic.cmp_eq, "cmp/eq" },
            { Mnemonic.cmp_ge, "cmp/ge" },
            { Mnemonic.cmp_gt, "cmp/gt" },
            { Mnemonic.cmp_hi, "cmp/hi" },
            { Mnemonic.cmp_hs, "cmp/hs" },
            { Mnemonic.cmp_pl, "cmp/pl" },
            { Mnemonic.cmp_pz, "cmp/pz" },
            { Mnemonic.cmp_str, "cmp/str" },
            { Mnemonic.dmuls_l, "dmuls.l" },
            { Mnemonic.exts_b, "exts.b" },
            { Mnemonic.exts_w, "exts.w" },
            { Mnemonic.extu_b, "extu.b" },
            { Mnemonic.extu_w, "extu.w" },
            { Mnemonic.fcmp_eq, "fcmp/eq" },
            { Mnemonic.fcmp_gt, "fcmp/gt" },
            { Mnemonic.fmov_d, "fmov.d" },
            { Mnemonic.fmov_s, "fmov.s" },
            { Mnemonic.jsr_n, "jsr/n" },
            { Mnemonic.ldc_l, "ldc.l" },
            { Mnemonic.lds_l, "lds.l" },
            { Mnemonic.mac_l, "mac.l" },
            { Mnemonic.mac_w, "mac.w" },
            { Mnemonic.mov_b, "mov.b" },
            { Mnemonic.mov_l, "mov.l" },
            { Mnemonic.mov_w, "mov.w" },
            { Mnemonic.movca_l, "movca.l" },
            { Mnemonic.movco_l, "movco.l" },
            { Mnemonic.movmu_l, "movmu.l" },
            { Mnemonic.mul_l, "mul.l" },
            { Mnemonic.or_b, "or.b" },
            { Mnemonic.rts_n, "rts/n" },
            { Mnemonic.rtv_n, "rtv/n" },
            { Mnemonic.stc_l, "stc.l" },
            { Mnemonic.sts_l, "sts.l" },
            { Mnemonic.swap_w, "swap.w" },
            { Mnemonic.tas_b, "tas.b" },
        };

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            if (!mnemonics.TryGetValue(Mnemonic, out var sMnemonic))
                sMnemonic = Mnemonic.ToString();
            renderer.WriteMnemonic(sMnemonic);
            RenderOperands(renderer, options);
        }

        protected override void RenderOperand(MachineOperand op, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            switch (op)
            {
            case Constant:
                renderer.WriteChar('#');
                op.Render(renderer, options);
                return;
            case MemoryOperand memOp:
                if (memOp.mode == AddressingMode.PcRelativeDisplacement)
                {
                    uint uAddr = this.Address.ToUInt32();
                    if (memOp.DataType.Size == 4)
                    {
                        uAddr &= ~3u;
                    }
                    uAddr += (uint)(memOp.disp + 4);
                    var addr = Core.Address.Ptr32(uAddr);
                    if ((options.Flags & MachineInstructionRendererFlags.ResolvePcRelativeAddress) != 0)
                    {
                        renderer.WriteChar('(');
                        renderer.WriteAddress(addr.ToString(), addr);
                        renderer.WriteChar(')');
                        renderer.AddAnnotation(op.ToString());
                    }
                    else
                    {
                        op.Render(renderer, options);
                        renderer.AddAnnotation(addr.ToString());
                    }
                    return;
                }
                goto default;
            default:
                op.Render(renderer, options);
                break;
            }
        }
    }
}