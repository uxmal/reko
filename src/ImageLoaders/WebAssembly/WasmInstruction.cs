#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
        internal static Dictionary<Mnemonic, string> mpoptostring = new Dictionary<Mnemonic, string>
        {
            { Mnemonic.unreachable, "unreachable" },
            { Mnemonic.nop, "nop" },
            { Mnemonic.block, "block" },
            { Mnemonic.loop, "loop" },
            { Mnemonic.@if, "if" },
            { Mnemonic.@else, "else" },
            { Mnemonic.end, "end" },
            { Mnemonic.br, "br" },
            { Mnemonic.br_if, "br_if" },
            { Mnemonic.br_table, "br_table" },
            { Mnemonic.@return, "return" },

            { Mnemonic.call, "call" },
            { Mnemonic.call_indirect, "call_indirect" },

            { Mnemonic.drop, "drop" },
            { Mnemonic.select, "select" },
            { Mnemonic.get_local, "get_local" },
            { Mnemonic.set_local, "set_local" },
            { Mnemonic.tee_local, "tee_local" },
            { Mnemonic.get_global, "get_global" },
            { Mnemonic.set_global, "set_global" },

            { Mnemonic.table_get, "table.get" },
            { Mnemonic.table_set, "table.set" },

            { Mnemonic.i32_load, "i32.load" },
            { Mnemonic.i64_load, "i64.load" },
            { Mnemonic.f32_load, "f32.load" },
            { Mnemonic.f64_load, "f64.load" },
            { Mnemonic.i32_load8_s, "i32.load8_s" },
            { Mnemonic.i32_load8_u, "i32.load8_u" },
            { Mnemonic.i32_load16_s, "i32.load16_s" },
            { Mnemonic.i32_load16_u, "i32.load16_u" },
            { Mnemonic.i64_load8_s, "i64.load8_s" },
            { Mnemonic.i64_load8_u, "i64.load8_u" },
            { Mnemonic.i64_load16_s, "i64.load16_s" },
            { Mnemonic.i64_load16_u, "i64.load16_u" },
            { Mnemonic.i64_load32_s, "i64.load32_s" },
            { Mnemonic.i64_load32_u, "i64.load32_u" },
            { Mnemonic.i32_store, "i32.store" },
            { Mnemonic.i64_store, "i64.store" },
            { Mnemonic.f32_store, "f32.store" },
            { Mnemonic.f64_store, "f64.store" },
            { Mnemonic.i32_store8, "i32.store8" },
            { Mnemonic.i32_store16, "i32.store16" },
            { Mnemonic.i64_store8, "i64.store8" },
            { Mnemonic.i64_store16, "i64.store16" },
            { Mnemonic.i64_store32, "i64.store32" },
            { Mnemonic.current_memory, "current_memory" },
            { Mnemonic.grow_memory, "grow_memory" },

            { Mnemonic.i32_const, "i32.const" },
            { Mnemonic.i64_const, "i64.const" },
            { Mnemonic.f32_const, "f32.const" },
            { Mnemonic.f64_const, "f64.const" },
            { Mnemonic.i32_eqz, "i32.eqz" },
            { Mnemonic.i32_eq, "i32.eq" },
            { Mnemonic.i32_ne, "i32.ne" },
            { Mnemonic.i32_lt_s, "i32.lt_s" },
            { Mnemonic.i32_lt_u, "i32.lt_u" },
            { Mnemonic.i32_gt_s, "i32.gt_s" },
            { Mnemonic.i32_gt_u, "i32.gt_u" },
            { Mnemonic.i32_le_s, "i32.le_s" },
            { Mnemonic.i32_le_u, "i32.le_u" },
            { Mnemonic.i32_ge_s, "i32.ge_s" },
            { Mnemonic.i32_ge_u, "i32.ge_u" },
            { Mnemonic.i64_eqz, "i64.eqz" },
            { Mnemonic.i64_eq, "i64.eq" },
            { Mnemonic.i64_ne, "i64.ne" },
            { Mnemonic.i64_lt_s, "i64.lt_s" },
            { Mnemonic.i64_lt_u, "i64.lt_u" },
            { Mnemonic.i64_gt_s, "i64.gt_s" },
            { Mnemonic.i64_gt_u, "i64.gt_u" },
            { Mnemonic.i64_le_s, "i64.le_s" },
            { Mnemonic.i64_le_u, "i64.le_u" },
            { Mnemonic.i64_ge_s, "i64.ge_s" },
            { Mnemonic.i64_ge_u, "i64.ge_u" },
            { Mnemonic.f32_eq, "f32.eq" },
            { Mnemonic.f32_ne, "f32.ne" },
            { Mnemonic.f32_lt, "f32.lt" },
            { Mnemonic.f32_gt, "f32.gt" },
            { Mnemonic.f32_le, "f32.le" },
            { Mnemonic.f32_ge, "f32.ge" },
            { Mnemonic.f64_eq, "f64.eq" },
            { Mnemonic.f64_ne, "f64.ne" },
            { Mnemonic.f64_lt, "f64.lt" },
            { Mnemonic.f64_gt, "f64.gt" },
            { Mnemonic.f64_le, "f64.le" },
            { Mnemonic.f64_ge, "f64.ge" },

            { Mnemonic.i32_clz, "i32.clz" },
            { Mnemonic.i32_ctz, "i32.ctz" },
            { Mnemonic.i32_popcnt, "i32.popcnt" },
            { Mnemonic.i32_add, "i32.add" },
            { Mnemonic.i32_sub, "i32.sub" },
            { Mnemonic.i32_mul, "i32.mul" },
            { Mnemonic.i32_div_s, "i32.div_s" },
            { Mnemonic.i32_div_u, "i32.div_u" },
            { Mnemonic.i32_rem_s, "i32.rem_s" },
            { Mnemonic.i32_rem_u, "i32.rem_u" },
            { Mnemonic.i32_and, "i32.and" },
            { Mnemonic.i32_or, "i32.or" },
            { Mnemonic.i32_xor, "i32.xor" },
            { Mnemonic.i32_shl, "i32.shl" },
            { Mnemonic.i32_shr_s, "i32.shr_s" },
            { Mnemonic.i32_shr_u, "i32.shr_u" },
            { Mnemonic.i32_rotl, "i32.rotl" },
            { Mnemonic.i32_rotr, "i32.rotr" },
            { Mnemonic.i64_clz, "i64.clz" },
            { Mnemonic.i64_ctz, "i64.ctz" },
            { Mnemonic.i64_popcnt, "i64.popcnt" },
            { Mnemonic.i64_add, "i64.add" },
            { Mnemonic.i64_sub, "i64.sub" },
            { Mnemonic.i64_mul, "i64.mul" },
            { Mnemonic.i64_div_s, "i64.div_s" },
            { Mnemonic.i64_div_u, "i64.div_u" },
            { Mnemonic.i64_rem_s, "i64.rem_s" },
            { Mnemonic.i64_rem_u, "i64.rem_u" },
            { Mnemonic.i64_and, "i64.and" },
            { Mnemonic.i64_or, "i64.or" },
            { Mnemonic.i64_xor, "i64.xor" },
            { Mnemonic.i64_shl, "i64.shl" },
            { Mnemonic.i64_shr_s, "i64.shr_s" },
            { Mnemonic.i64_shr_u, "i64.shr_u" },
            { Mnemonic.i64_rotl, "i64.rotl" },
            { Mnemonic.i64_rotr, "i64.rotr" },
            { Mnemonic.f32_abs, "f32.abs" },
            { Mnemonic.f32_neg, "f32.neg" },
            { Mnemonic.f32_ceil, "f32.ceil" },
            { Mnemonic.f32_floor, "f32.floor" },
            { Mnemonic.f32_trunc, "f32.trunc" },
            { Mnemonic.f32_nearest, "f32.nearest" },
            { Mnemonic.f32_sqrt, "f32.sqrt" },
            { Mnemonic.f32_add, "f32.add" },
            { Mnemonic.f32_sub, "f32.sub" },
            { Mnemonic.f32_mul, "f32.mul" },
            { Mnemonic.f32_div, "f32.div" },
            { Mnemonic.f32_min, "f32.min" },
            { Mnemonic.f32_max, "f32.max" },
            { Mnemonic.f32_copysign, "f32.copysign" },
            { Mnemonic.f64_abs, "f64.abs" },
            { Mnemonic.f64_neg, "f64.neg" },
            { Mnemonic.f64_ceil, "f64.ceil" },
            { Mnemonic.f64_floor, "f64.floor" },
            { Mnemonic.f64_trunc, "f64.trunc" },
            { Mnemonic.f64_nearest, "f64.nearest" },
            { Mnemonic.f64_sqrt, "f64.sqrt" },
            { Mnemonic.f64_add, "f64.add" },
            { Mnemonic.f64_sub, "f64.sub" },
            { Mnemonic.f64_mul, "f64.mul" },
            { Mnemonic.f64_div, "f64.div" },
            { Mnemonic.f64_min, "f64.min" },
            { Mnemonic.f64_max, "f64.max" },
            { Mnemonic.f64_copysign, "f64.copysign" },

            { Mnemonic.i32_wrap_i64, "i32.wrap/i64" },
            { Mnemonic.i32_trunc_s_f32, "i32.trunc_s/f32" },
            { Mnemonic.i32_trunc_u_f32, "i32.trunc_u/f32" },
            { Mnemonic.i32_trunc_s_f64, "i32.trunc_s/f64" },
            { Mnemonic.i32_trunc_u_f64, "i32.trunc_u/f64" },
            { Mnemonic.i64_extend_s_i32, "i64.extend_s/i32" },
            { Mnemonic.i64_extend_u_i32, "i64.extend_u/i32" },
            { Mnemonic.i64_trunc_s_f32, "i64.trunc_s/f32" },
            { Mnemonic.i64_trunc_u_f32, "i64.trunc_u/f32" },
            { Mnemonic.i64_trunc_s_f64, "i64.trunc_s/f64" },
            { Mnemonic.i64_trunc_u_f64, "i64.trunc_u/f64" },
            { Mnemonic.f32_convert_s_i32, "f32.convert_s/i32" },
            { Mnemonic.f32_convert_u_i32, "f32.convert_u/i32" },
            { Mnemonic.f32_convert_s_i64, "f32.convert_s/i64" },
            { Mnemonic.f32_convert_u_i64, "f32.convert_u/i64" },
            { Mnemonic.f32_demote_f64, "f32.demote/f64" },
            { Mnemonic.f64_convert_s_i32, "f64.convert_s/i32" },
            { Mnemonic.f64_convert_u_i32, "f64.convert_u/i32" },
            { Mnemonic.f64_convert_s_i64, "f64.convert_s/i64" },
            { Mnemonic.f64_convert_u_i64, "f64.convert_u/i64" },
            { Mnemonic.f64_promote_f32, "f64.promote/f32" },

            { Mnemonic.i32_reinterpret_f32, "i32.reinterpret/f32" },
            { Mnemonic.i64_reinterpret_f64, "i64.reinterpret/f64" },
            { Mnemonic.f32_reinterpret_i32, "f32.reinterpret/i32" },
            { Mnemonic.f64_reinterpret_i64, "f64.reinterpret/i64" },

            { Mnemonic.i32_extend8_s, "i32.extends_s" },
        };

        public WasmInstruction(Mnemonic mnemonic)
        {
            this.Mnemonic = mnemonic;
        }

        public Mnemonic Mnemonic { get; set; }
        public override int MnemonicAsInteger => (int) Mnemonic;

        public override string MnemonicAsString => mpoptostring[this.Mnemonic];

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteMnemonic(MnemonicAsString);
            base.RenderOperands(renderer, options);
        }

        protected override void RenderOperand(MachineOperand operand, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            switch (operand)
            {
            case ImmediateOperand imm:
                if (imm.Width.IsIntegral || imm.Width.IsWord)
                    renderer.WriteFormat("0x{0:X}", imm.Value.ToUInt64());
                else if (imm.Width.IsReal)
                    renderer.WriteFormat("{0}", imm.Value.ToReal64());
                return;
            }
            base.RenderOperand(operand, renderer, options);
        }
    }
}
