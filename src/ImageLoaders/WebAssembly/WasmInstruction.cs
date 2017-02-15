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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Machine;

namespace Reko.ImageLoaders.WebAssembly
{
    public class WasmInstruction : MachineInstruction
    {
        internal static Dictionary<Opcode, string> mpoptostring = new Dictionary<Opcode, string>
        {
            { Opcode.unreachable, "unreachable" },
            { Opcode.nop, "nop" },
            { Opcode.block, "block" },
            { Opcode.loop, "loop" },
            { Opcode.@if, "if" },
            { Opcode.@else, "else" },
            { Opcode.end, "end" },
            { Opcode.br, "br" },
            { Opcode.br_if, "br_if" },
            { Opcode.br_table, "br_table" },
            { Opcode.@return, "return" },

            { Opcode.call, "call" },
            { Opcode.call_indirect, "call_indirect" },

            { Opcode.drop, "drop" },
            { Opcode.select, "select" },
            { Opcode.get_local, "get_local" },
            { Opcode.set_local, "set_local" },
            { Opcode.tee_local, "tee_local" },
            { Opcode.get_global, "get_global" },
            { Opcode.set_global, "set_global" },
            { Opcode.i32_load, "i32.load" },
            { Opcode.i64_load, "i64.load" },
            { Opcode.f32_load, "f32.load" },
            { Opcode.f64_load, "f64.load" },
            { Opcode.i32_load8_s, "i32.load8_s" },
            { Opcode.i32_load8_u, "i32.load8_u" },
            { Opcode.i32_load16_s, "i32.load16_s" },
            { Opcode.i32_load16_u, "i32.load16_u" },
            { Opcode.i64_load8_s, "i64.load8_s" },
            { Opcode.i64_load8_u, "i64.load8_u" },
            { Opcode.i64_load16_s, "i64.load16_s" },
            { Opcode.i64_load16_u, "i64.load16_u" },
            { Opcode.i64_load32_s, "i64.load32_s" },
            { Opcode.i64_load32_u, "i64.load32_u" },
            { Opcode.i32_store, "i32.store" },
            { Opcode.i64_store, "i64.store" },
            { Opcode.f32_store, "f32.store" },
            { Opcode.f64_store, "f64.store" },
            { Opcode.i32_store8, "i32.store8" },
            { Opcode.i32_store16, "i32.store16" },
            { Opcode.i64_store8, "i64.store8" },
            { Opcode.i64_store16, "i64.store16" },
            { Opcode.i64_store32, "i64.store32" },
            { Opcode.current_memory, "current_memory" },
            { Opcode.grow_memory, "grow_memory" },

            { Opcode.i32_const, "i32.const" },
            { Opcode.i64_const, "i64.const" },
            { Opcode.f32_const, "f32.const" },
            { Opcode.f64_const, "f64.const" },
            { Opcode.i32_eqz, "i32.eqz" },
            { Opcode.i32_eq, "i32.eq" },
            { Opcode.i32_ne, "i32.ne" },
            { Opcode.i32_lt_s, "i32.lt_s" },
            { Opcode.i32_lt_u, "i32.lt_u" },
            { Opcode.i32_gt_s, "i32.gt_s" },
            { Opcode.i32_gt_u, "i32.gt_u" },
            { Opcode.i32_le_s, "i32.le_s" },
            { Opcode.i32_le_u, "i32.le_u" },
            { Opcode.i32_ge_s, "i32.ge_s" },
            { Opcode.i32_ge_u, "i32.ge_u" },
            { Opcode.i64_eqz, "i64.eqz" },
            { Opcode.i64_eq, "i64.eq" },
            { Opcode.i64_ne, "i64.ne" },
            { Opcode.i64_lt_s, "i64.lt_s" },
            { Opcode.i64_lt_u, "i64.lt_u" },
            { Opcode.i64_gt_s, "i64.gt_s" },
            { Opcode.i64_gt_u, "i64.gt_u" },
            { Opcode.i64_le_s, "i64.le_s" },
            { Opcode.i64_le_u, "i64.le_u" },
            { Opcode.i64_ge_s, "i64.ge_s" },
            { Opcode.i64_ge_u, "i64.ge_u" },
            { Opcode.f32_eq, "f32.eq" },
            { Opcode.f32_ne, "f32.ne" },
            { Opcode.f32_lt, "f32.lt" },
            { Opcode.f32_gt, "f32.gt" },
            { Opcode.f32_le, "f32.le" },
            { Opcode.f32_ge, "f32.ge" },
            { Opcode.f64_eq, "f64.eq" },
            { Opcode.f64_ne, "f64.ne" },
            { Opcode.f64_lt, "f64.lt" },
            { Opcode.f64_gt, "f64.gt" },
            { Opcode.f64_le, "f64.le" },
            { Opcode.f64_ge, "f64.ge" },

            { Opcode.i32_clz, "i32.clz" },
            { Opcode.i32_ctz, "i32.ctz" },
            { Opcode.i32_popcnt, "i32.popcnt" },
            { Opcode.i32_add, "i32.add" },
            { Opcode.i32_sub, "i32.sub" },
            { Opcode.i32_mul, "i32.mul" },
            { Opcode.i32_div_s, "i32.div_s" },
            { Opcode.i32_div_u, "i32.div_u" },
            { Opcode.i32_rem_s, "i32.rem_s" },
            { Opcode.i32_rem_u, "i32.rem_u" },
            { Opcode.i32_and, "i32.and" },
            { Opcode.i32_or, "i32.or" },
            { Opcode.i32_xor, "i32.xor" },
            { Opcode.i32_shl, "i32.shl" },
            { Opcode.i32_shr_s, "i32.shr_s" },
            { Opcode.i32_shr_u, "i32.shr_u" },
            { Opcode.i32_rotl, "i32.rotl" },
            { Opcode.i32_rotr, "i32.rotr" },
            { Opcode.i64_clz, "i64.clz" },
            { Opcode.i64_ctz, "i64.ctz" },
            { Opcode.i64_popcnt, "i64.popcnt" },
            { Opcode.i64_add, "i64.add" },
            { Opcode.i64_sub, "i64.sub" },
            { Opcode.i64_mul, "i64.mul" },
            { Opcode.i64_div_s, "i64.div_s" },
            { Opcode.i64_div_u, "i64.div_u" },
            { Opcode.i64_rem_s, "i64.rem_s" },
            { Opcode.i64_rem_u, "i64.rem_u" },
            { Opcode.i64_and, "i64.and" },
            { Opcode.i64_or, "i64.or" },
            { Opcode.i64_xor, "i64.xor" },
            { Opcode.i64_shl, "i64.shl" },
            { Opcode.i64_shr_s, "i64.shr_s" },
            { Opcode.i64_shr_u, "i64.shr_u" },
            { Opcode.i64_rotl, "i64.rotl" },
            { Opcode.i64_rotr, "i64.rotr" },
            { Opcode.f32_abs, "f32.abs" },
            { Opcode.f32_neg, "f32.neg" },
            { Opcode.f32_ceil, "f32.ceil" },
            { Opcode.f32_floor, "f32.floor" },
            { Opcode.f32_trunc, "f32.trunc" },
            { Opcode.f32_nearest, "f32.nearest" },
            { Opcode.f32_sqrt, "f32.sqrt" },
            { Opcode.f32_add, "f32.add" },
            { Opcode.f32_sub, "f32.sub" },
            { Opcode.f32_mul, "f32.mul" },
            { Opcode.f32_div, "f32.div" },
            { Opcode.f32_min, "f32.min" },
            { Opcode.f32_max, "f32.max" },
            { Opcode.f32_copysign, "f32.copysign" },
            { Opcode.f64_abs, "f64.abs" },
            { Opcode.f64_neg, "f64.neg" },
            { Opcode.f64_ceil, "f64.ceil" },
            { Opcode.f64_floor, "f64.floor" },
            { Opcode.f64_trunc, "f64.trunc" },
            { Opcode.f64_nearest, "f64.nearest" },
            { Opcode.f64_sqrt, "f64.sqrt" },
            { Opcode.f64_add, "f64.add" },
            { Opcode.f64_sub, "f64.sub" },
            { Opcode.f64_mul, "f64.mul" },
            { Opcode.f64_div, "f64.div" },
            { Opcode.f64_min, "f64.min" },
            { Opcode.f64_max, "f64.max" },
            { Opcode.f64_copysign, "f64.copysign" },

            { Opcode.i32_wrap_i64, "i32.wrap/i64" },
            { Opcode.i32_trunc_s_f32, "i32.trunc_s/f32" },
            { Opcode.i32_trunc_u_f32, "i32.trunc_u/f32" },
            { Opcode.i32_trunc_s_f64, "i32.trunc_s/f64" },
            { Opcode.i32_trunc_u_f64, "i32.trunc_u/f64" },
            { Opcode.i64_extend_s_i32, "i64.extend_s/i32" },
            { Opcode.i64_extend_u_i32, "i64.extend_u/i32" },
            { Opcode.i64_trunc_s_f32, "i64.trunc_s/f32" },
            { Opcode.i64_trunc_u_f32, "i64.trunc_u/f32" },
            { Opcode.i64_trunc_s_f64, "i64.trunc_s/f64" },
            { Opcode.i64_trunc_u_f64, "i64.trunc_u/f64" },
            { Opcode.f32_convert_s_i32, "f32.convert_s/i32" },
            { Opcode.f32_convert_u_i32, "f32.convert_u/i32" },
            { Opcode.f32_convert_s_i64, "f32.convert_s/i64" },
            { Opcode.f32_convert_u_i64, "f32.convert_u/i64" },
            { Opcode.f32_demote_f64, "f32.demote/f64" },
            { Opcode.f64_convert_s_i32, "f64.convert_s/i32" },
            { Opcode.f64_convert_u_i32, "f64.convert_u/i32" },
            { Opcode.f64_convert_s_i64, "f64.convert_s/i64" },
            { Opcode.f64_convert_u_i64, "f64.convert_u/i64" },
            { Opcode.f64_promote_f32, "f64.promote/f32" },

            { Opcode.i32_reinterpret_f32, "i32.reinterpret/f32" },
            { Opcode.i64_reinterpret_f64, "i64.reinterpret/f64" },
            { Opcode.f32_reinterpret_i32, "f32.reinterpret/i32" },
            { Opcode.f64_reinterpret_i64, "f64.reinterpret/i64" },
        };

        public WasmInstruction(Opcode code)
        {
            this.Opcode = code;
        }

        public override InstructionClass InstructionClass
        {
            get
            {
                return InstructionClass.Linear;
            }
        } 

        public override bool IsValid {  get { return this.Opcode != Opcode.unreachable; } }

        public Opcode Opcode { get; internal set; }

        public override int OpcodeAsInteger { get { return (int)Opcode; } }

        public MachineOperand[] Operands { get; internal set; }

        public override MachineOperand GetOperand(int i)
        {
            if (0 <= i && i < Operands.Length)
                return Operands[i];
            else
                return null;
        }
    }
}
