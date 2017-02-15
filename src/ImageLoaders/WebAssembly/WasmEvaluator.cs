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
using System.Collections.Generic;
using System;

namespace Reko.ImageLoaders.WebAssembly
{
    public class WasmEvaluator
    {
        private WasmImageReader rdr;
        private Stack<object> stack;

        public WasmEvaluator(WasmImageReader rdr)
        {
            this.rdr = rdr;
            this.stack = new Stack<object>();
        }

        public object Run()
        {
            for (;;)
            {
                byte b;
                uint u;
                if (!rdr.TryReadByte(out b))
                    throw new BadImageFormatException();
                switch ((Opcode)b)
                {
                case Opcode.unreachable:
                case Opcode.nop:
                case Opcode.block:
                case Opcode.loop:
                case Opcode.@if:
                case Opcode.@else:
                case Opcode.end:
                    return stack.Pop();
                case Opcode.br:
                case Opcode.br_if:
                case Opcode.br_table:
                case Opcode.@return:

                case Opcode.call:
                case Opcode.call_indirect:

                case Opcode.drop:
                case Opcode.select:
                case Opcode.get_local:
                case Opcode.set_local:
                case Opcode.tee_local:
                case Opcode.get_global:
                case Opcode.set_global:
                case Opcode.i32_load:
                case Opcode.i64_load:
                case Opcode.f32_load:
                case Opcode.f64_load:
                case Opcode.i32_load8_s:
                case Opcode.i32_load8_u:
                case Opcode.i32_load16_s:
                case Opcode.i32_load16_u:
                case Opcode.i64_load8_s:
                case Opcode.i64_load8_u:
                case Opcode.i64_load16_s:
                case Opcode.i64_load16_u:
                case Opcode.i64_load32_s:
                case Opcode.i64_load32_u:
                case Opcode.i32_store:
                case Opcode.i64_store:
                case Opcode.f32_store:
                case Opcode.f64_store:
                case Opcode.i32_store8:
                case Opcode.i32_store16:
                case Opcode.i64_store8:
                case Opcode.i64_store16:
                case Opcode.i64_store32:
                case Opcode.current_memory:
                case Opcode.grow_memory:
                    goto default;
                case Opcode.i32_const:
                    if (!rdr.TryReadVarUInt32(out u))
                        throw new InvalidOperationException();
                    stack.Push(u);
                    break;
                case Opcode.i64_const:
                case Opcode.f32_const:
                case Opcode.f64_const:
                case Opcode.i32_eqz:
                case Opcode.i32_eq:
                case Opcode.i32_ne:
                case Opcode.i32_lt_s:
                case Opcode.i32_lt_u:
                case Opcode.i32_gt_s:
                case Opcode.i32_gt_u:
                case Opcode.i32_le_s:
                case Opcode.i32_le_u:
                case Opcode.i32_ge_s:
                case Opcode.i32_ge_u:
                case Opcode.i64_eqz:
                case Opcode.i64_eq:
                case Opcode.i64_ne:
                case Opcode.i64_lt_s:
                case Opcode.i64_lt_u:
                case Opcode.i64_gt_s:
                case Opcode.i64_gt_u:
                case Opcode.i64_le_s:
                case Opcode.i64_le_u:
                case Opcode.i64_ge_s:
                case Opcode.i64_ge_u:
                case Opcode.f32_eq:
                case Opcode.f32_ne:
                case Opcode.f32_lt:
                case Opcode.f32_gt:
                case Opcode.f32_le:
                case Opcode.f32_ge:
                case Opcode.f64_eq:
                case Opcode.f64_ne:
                case Opcode.f64_lt:
                case Opcode.f64_gt:
                case Opcode.f64_le:
                case Opcode.f64_ge:

                case Opcode.i32_clz:
                case Opcode.i32_ctz:
                case Opcode.i32_popcnt:
                case Opcode.i32_add:
                case Opcode.i32_sub:
                case Opcode.i32_mul:
                case Opcode.i32_div_s:
                case Opcode.i32_div_u:
                case Opcode.i32_rem_s:
                case Opcode.i32_rem_u:
                case Opcode.i32_and:
                case Opcode.i32_or:
                case Opcode.i32_xor:
                case Opcode.i32_shl:
                case Opcode.i32_shr_s:
                case Opcode.i32_shr_u:
                case Opcode.i32_rotl:
                case Opcode.i32_rotr:
                case Opcode.i64_clz:
                case Opcode.i64_ctz:
                case Opcode.i64_popcnt:
                case Opcode.i64_add:
                case Opcode.i64_sub:
                case Opcode.i64_mul:
                case Opcode.i64_div_s:
                case Opcode.i64_div_u:
                case Opcode.i64_rem_s:
                case Opcode.i64_rem_u:
                case Opcode.i64_and:
                case Opcode.i64_or:
                case Opcode.i64_xor:
                case Opcode.i64_shl:
                case Opcode.i64_shr_s:
                case Opcode.i64_shr_u:
                case Opcode.i64_rotl:
                case Opcode.i64_rotr:
                case Opcode.f32_abs:
                case Opcode.f32_neg:
                case Opcode.f32_ceil:
                case Opcode.f32_floor:
                case Opcode.f32_trunc:
                case Opcode.f32_nearest:
                case Opcode.f32_sqrt:
                case Opcode.f32_add:
                case Opcode.f32_sub:
                case Opcode.f32_mul:
                case Opcode.f32_div:
                case Opcode.f32_min:
                case Opcode.f32_max:
                case Opcode.f32_copysign:
                case Opcode.f64_abs:
                case Opcode.f64_neg:
                case Opcode.f64_ceil:
                case Opcode.f64_floor:
                case Opcode.f64_trunc:
                case Opcode.f64_nearest:
                case Opcode.f64_sqrt:
                case Opcode.f64_add:
                case Opcode.f64_sub:
                case Opcode.f64_mul:
                case Opcode.f64_div:
                case Opcode.f64_min:
                case Opcode.f64_max:
                case Opcode.f64_copysign:

                case Opcode.i32_wrap_i64:
                case Opcode.i32_trunc_s_f32:
                case Opcode.i32_trunc_u_f32:
                case Opcode.i32_trunc_s_f64:
                case Opcode.i32_trunc_u_f64:
                case Opcode.i64_extend_s_i32:
                case Opcode.i64_extend_u_i32:
                case Opcode.i64_trunc_s_f32:
                case Opcode.i64_trunc_u_f32:
                case Opcode.i64_trunc_s_f64:
                case Opcode.i64_trunc_u_f64:
                case Opcode.f32_convert_s_i32:
                case Opcode.f32_convert_u_i32:
                case Opcode.f32_convert_s_i64:
                case Opcode.f32_convert_u_i64:
                case Opcode.f32_demote_f64:
                case Opcode.f64_convert_s_i32:
                case Opcode.f64_convert_u_i32:
                case Opcode.f64_convert_s_i64:
                case Opcode.f64_convert_u_i64:
                case Opcode.f64_promote_f32:

                case Opcode.i32_reinterpret_f32:
                case Opcode.i64_reinterpret_f64:
                case Opcode.f32_reinterpret_i32:
                case Opcode.f64_reinterpret_i64:
                default:
                    string str;
                    if (!WasmInstruction.mpoptostring.TryGetValue((Opcode)b, out str))
                        str = string.Format("0x{0:X2}", (int)b);
                    throw new NotImplementedException(string.Format(
                        "WASM evaluator doesn't know how to evaluate {0} ({1:X2}).",
                        str,
                        (int)b));
                        
                }
            }
        }

        public object Pop()
        {
            throw new NotImplementedException();
        }
    }
}