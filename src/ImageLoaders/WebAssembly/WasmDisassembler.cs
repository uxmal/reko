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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.WebAssembly
{
    public class WasmDisassembler : DisassemblerBase<WasmInstruction>
    {
        private WasmImageReader rdr;

        public WasmDisassembler(LeImageReader rdr)
        {
            this.rdr = new WasmImageReader(rdr.Bytes);
            this.rdr.Offset = rdr.Offset;
        }

        public override WasmInstruction DisassembleInstruction()
        {
            byte b;
            if (!rdr.TryReadByte(out b))
            {
                return null;
            }
            string fmt;
            var opcode = (Opcode)b;
            if (!opRecs.TryGetValue(opcode, out fmt))
            {
                opcode = Opcode.unreachable;
                fmt = "";
            }
            var ops = new List<MachineOperand>();
            int i = 0;
            uint u;
            while (i < fmt.Length)
            {
                switch (fmt[i++])
                {
                case ',': continue;
                case 'v':
                    if (!rdr.TryReadVarUInt32(out u))
                    {
                        opcode = Opcode.unreachable;
                        break;
                    }
                    ops.Add(ImmediateOperand.Word32(u));
                    break;
                case 'r':
                    if (!rdr.TryReadUInt32(out u))
                    {
                        opcode = Opcode.unreachable;
                        break;
                    }
                    ops.Add(new ImmediateOperand(Constant.FloatFromBitpattern(u)));
                    break;
                default:
                    throw new NotImplementedException(string.Format("Format '{0}'.", fmt[i - 1]));
                }
            }

            var instr = new WasmInstruction(opcode);
            instr.Operands = ops.ToArray();
            return instr;
        }

        private static Dictionary<Opcode, string> opRecs = new Dictionary<Opcode, string>
        {
            { Opcode.unreachable, "" },
            { Opcode.nop, "" },
            { Opcode.block, "b" },
            { Opcode.loop, "b" },
            { Opcode.@if, "b" },
            { Opcode.@else, "" },
            { Opcode.end, "" },
            { Opcode.br, "v" },
            { Opcode.br_if, "v" },
            { Opcode.br_table, "t" },
            { Opcode.@return, "" },

            { Opcode.call, "v" },
            { Opcode.call_indirect, "v,v" },

            { Opcode.drop, "" },
            { Opcode.select, "" },
            { Opcode.get_local, "v" },
            { Opcode.set_local, "v" },
            { Opcode.tee_local, "v" },
            { Opcode.get_global, "v" },
            { Opcode.set_global, "v" },
            { Opcode.i32_load, "m" },
            { Opcode.i64_load, "m" },
            { Opcode.f32_load, "m" },
            { Opcode.f64_load, "m" },
            { Opcode.i32_load8_s, "m" },
            { Opcode.i32_load8_u, "m" },
            { Opcode.i32_load16_s, "m" },
            { Opcode.i32_load16_u, "m" },
            { Opcode.i64_load8_s, "m" },
            { Opcode.i64_load8_u, "m" },
            { Opcode.i64_load16_s, "m" },
            { Opcode.i64_load16_u, "m" },
            { Opcode.i64_load32_s, "m" },
            { Opcode.i64_load32_u, "m" },
            { Opcode.i32_store, "m" },
            { Opcode.i64_store, "m" },
            { Opcode.f32_store, "m" },
            { Opcode.f64_store, "m" },
            { Opcode.i32_store8, "m" },
            { Opcode.i32_store16, "m" },
            { Opcode.i64_store8, "m" },
            { Opcode.i64_store16, "m" },
            { Opcode.i64_store32, "m" },
            { Opcode.current_memory, "v" },
            { Opcode.grow_memory, "v" },

            { Opcode.i32_const, "v" },
            { Opcode.i64_const, "V" },
            { Opcode.f32_const, "r" },
            { Opcode.f64_const, "R" },
            { Opcode.i32_eqz, "" },
            { Opcode.i32_eq, "" },
            { Opcode.i32_ne, "" },
            { Opcode.i32_lt_s, "" },
            { Opcode.i32_lt_u, "" },
            { Opcode.i32_gt_s, "" },
            { Opcode.i32_gt_u, "" },
            { Opcode.i32_le_s, "" },
            { Opcode.i32_le_u, "" },
            { Opcode.i32_ge_s, "" },
            { Opcode.i32_ge_u, "" },
            { Opcode.i64_eqz, "" },
            { Opcode.i64_eq, "" },
            { Opcode.i64_ne, "" },
            { Opcode.i64_lt_s, "" },
            { Opcode.i64_lt_u, "" },
            { Opcode.i64_gt_s, "" },
            { Opcode.i64_gt_u, "" },
            { Opcode.i64_le_s, "" },
            { Opcode.i64_le_u, "" },
            { Opcode.i64_ge_s, "" },
            { Opcode.i64_ge_u, "" },
            { Opcode.f32_eq, "" },
            { Opcode.f32_ne, "" },
            { Opcode.f32_lt, "" },
            { Opcode.f32_gt, "" },
            { Opcode.f32_le, "" },
            { Opcode.f32_ge, "" },
            { Opcode.f64_eq, "" },
            { Opcode.f64_ne, "" },
            { Opcode.f64_lt, "" },
            { Opcode.f64_gt, "" },
            { Opcode.f64_le, "" },
            { Opcode.f64_ge, "" },

            { Opcode.i32_clz, "" },
            { Opcode.i32_ctz, "" },
            { Opcode.i32_popcnt, "" },
            { Opcode.i32_add, "" },
            { Opcode.i32_sub, "" },
            { Opcode.i32_mul, "" },
            { Opcode.i32_div_s, "" },
            { Opcode.i32_div_u, "" },
            { Opcode.i32_rem_s, "" },
            { Opcode.i32_rem_u, "" },
            { Opcode.i32_and, "" },
            { Opcode.i32_or, "" },
            { Opcode.i32_xor, "" },
            { Opcode.i32_shl, "" },
            { Opcode.i32_shr_s, "" },
            { Opcode.i32_shr_u, "" },
            { Opcode.i32_rotl, "" },
            { Opcode.i32_rotr, "" },
            { Opcode.i64_clz, "" },
            { Opcode.i64_ctz, "" },
            { Opcode.i64_popcnt, "" },
            { Opcode.i64_add, "" },
            { Opcode.i64_sub, "" },
            { Opcode.i64_mul, "" },
            { Opcode.i64_div_s, "" },
            { Opcode.i64_div_u, "" },
            { Opcode.i64_rem_s, "" },
            { Opcode.i64_rem_u, "" },
            { Opcode.i64_and, "" },
            { Opcode.i64_or, "" },
            { Opcode.i64_xor, "" },
            { Opcode.i64_shl, "" },
            { Opcode.i64_shr_s, "" },
            { Opcode.i64_shr_u, "" },
            { Opcode.i64_rotl, "" },
            { Opcode.i64_rotr, "" },
            { Opcode.f32_abs, "" },
            { Opcode.f32_neg, "" },
            { Opcode.f32_ceil, "" },
            { Opcode.f32_floor, "" },
            { Opcode.f32_trunc, "" },
            { Opcode.f32_nearest, "" },
            { Opcode.f32_sqrt, "" },
            { Opcode.f32_add, "" },
            { Opcode.f32_sub, "" },
            { Opcode.f32_mul, "" },
            { Opcode.f32_div, "" },
            { Opcode.f32_min, "" },
            { Opcode.f32_max, "" },
            { Opcode.f32_copysign, "" },
            { Opcode.f64_abs, "" },
            { Opcode.f64_neg, "" },
            { Opcode.f64_ceil, "" },
            { Opcode.f64_floor, "" },
            { Opcode.f64_trunc, "" },
            { Opcode.f64_nearest, "" },
            { Opcode.f64_sqrt, "" },
            { Opcode.f64_add, "" },
            { Opcode.f64_sub, "" },
            { Opcode.f64_mul, "" },
            { Opcode.f64_div, "" },
            { Opcode.f64_min, "" },
            { Opcode.f64_max, "" },
            { Opcode.f64_copysign, "" },

            { Opcode.i32_wrap_i64, "" },
            { Opcode.i32_trunc_s_f32, "" },
            { Opcode.i32_trunc_u_f32, "" },
            { Opcode.i32_trunc_s_f64, "" },
            { Opcode.i32_trunc_u_f64, "" },
            { Opcode.i64_extend_s_i32, "" },
            { Opcode.i64_extend_u_i32, "" },
            { Opcode.i64_trunc_s_f32, "" },
            { Opcode.i64_trunc_u_f32, "" },
            { Opcode.i64_trunc_s_f64, "" },
            { Opcode.i64_trunc_u_f64, "" },
            { Opcode.f32_convert_s_i32, "" },
            { Opcode.f32_convert_u_i32, "" },
            { Opcode.f32_convert_s_i64, "" },
            { Opcode.f32_convert_u_i64, "" },
            { Opcode.f32_demote_f64, "" },
            { Opcode.f64_convert_s_i32, "" },
            { Opcode.f64_convert_u_i32, "" },
            { Opcode.f64_convert_s_i64, "" },
            { Opcode.f64_convert_u_i64, "" },
            { Opcode.f64_promote_f32, "" },

            { Opcode.i32_reinterpret_f32, "" },
            { Opcode.i64_reinterpret_f64, "" },
            { Opcode.f32_reinterpret_i32, "" },
            { Opcode.f64_reinterpret_i64, "" },
        };

    }
}
