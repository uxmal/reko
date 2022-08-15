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

using Reko.Core;
using System.Collections.Generic;
using System;
using Reko.Core.Expressions;

namespace Reko.ImageLoaders.WebAssembly
{
    public class WasmEvaluator
    {
        private readonly WasmImageReader rdr;
        private readonly Stack<object> stack;

        public WasmEvaluator(WasmImageReader rdr)
        {
            this.rdr = rdr;
            this.stack = new Stack<object>();
        }

        public object Run()
        {
            for (;;)
            {
                uint u;
                long l;
                int i;
                if (!rdr.TryReadByte(out byte b))
                    throw new BadImageFormatException();
                switch ((Mnemonic)b)
                {
                case Mnemonic.unreachable:
                case Mnemonic.nop:
                case Mnemonic.block:
                case Mnemonic.loop:
                case Mnemonic.@if:
                case Mnemonic.@else:
                case Mnemonic.end:
                    return stack.Pop();
                case Mnemonic.br:
                case Mnemonic.br_if:
                case Mnemonic.br_table:
                case Mnemonic.@return:

                case Mnemonic.call:
                case Mnemonic.call_indirect:

                case Mnemonic.drop:
                case Mnemonic.select:
                case Mnemonic.get_local:
                case Mnemonic.set_local:
                case Mnemonic.tee_local:
                case Mnemonic.get_global:
                    if (!rdr.TryReadVarUInt32(out u))
                        throw new InvalidOperationException();
                    //$TODO: real impl.
                    stack.Push(0);
                    break;
                case Mnemonic.set_global:
                case Mnemonic.i32_load:
                case Mnemonic.i64_load:
                case Mnemonic.f32_load:
                case Mnemonic.f64_load:
                case Mnemonic.i32_load8_s:
                case Mnemonic.i32_load8_u:
                case Mnemonic.i32_load16_s:
                case Mnemonic.i32_load16_u:
                case Mnemonic.i64_load8_s:
                case Mnemonic.i64_load8_u:
                case Mnemonic.i64_load16_s:
                case Mnemonic.i64_load16_u:
                case Mnemonic.i64_load32_s:
                case Mnemonic.i64_load32_u:
                case Mnemonic.i32_store:
                case Mnemonic.i64_store:
                case Mnemonic.f32_store:
                case Mnemonic.f64_store:
                case Mnemonic.i32_store8:
                case Mnemonic.i32_store16:
                case Mnemonic.i64_store8:
                case Mnemonic.i64_store16:
                case Mnemonic.i64_store32:
                case Mnemonic.current_memory:
                case Mnemonic.grow_memory:
                    goto default;
                case Mnemonic.i32_const:
                    if (!rdr.TryReadVarUInt32(out u))
                        throw new InvalidOperationException();
                    stack.Push(u);
                    break;
                case Mnemonic.i64_const:
                    if (!rdr.TryReadLeInt64(out l))
                        throw new InvalidOperationException();
                    stack.Push(l);
                    break;
                case Mnemonic.f32_const:
                    if (!rdr.TryReadLeInt32(out i))
                        throw new InvalidOperationException();
                    stack.Push(BitConverter.Int32BitsToSingle(i));
                    break;
                case Mnemonic.f64_const:
                    if (!rdr.TryReadLeInt64(out l))
                        throw new InvalidOperationException();
                    stack.Push(BitConverter.Int64BitsToDouble(l));
                    break;
                case Mnemonic.i32_eqz:
                case Mnemonic.i32_eq:
                case Mnemonic.i32_ne:
                case Mnemonic.i32_lt_s:
                case Mnemonic.i32_lt_u:
                case Mnemonic.i32_gt_s:
                case Mnemonic.i32_gt_u:
                case Mnemonic.i32_le_s:
                case Mnemonic.i32_le_u:
                case Mnemonic.i32_ge_s:
                case Mnemonic.i32_ge_u:
                case Mnemonic.i64_eqz:
                case Mnemonic.i64_eq:
                case Mnemonic.i64_ne:
                case Mnemonic.i64_lt_s:
                case Mnemonic.i64_lt_u:
                case Mnemonic.i64_gt_s:
                case Mnemonic.i64_gt_u:
                case Mnemonic.i64_le_s:
                case Mnemonic.i64_le_u:
                case Mnemonic.i64_ge_s:
                case Mnemonic.i64_ge_u:
                case Mnemonic.f32_eq:
                case Mnemonic.f32_ne:
                case Mnemonic.f32_lt:
                case Mnemonic.f32_gt:
                case Mnemonic.f32_le:
                case Mnemonic.f32_ge:
                case Mnemonic.f64_eq:
                case Mnemonic.f64_ne:
                case Mnemonic.f64_lt:
                case Mnemonic.f64_gt:
                case Mnemonic.f64_le:
                case Mnemonic.f64_ge:

                case Mnemonic.i32_clz:
                case Mnemonic.i32_ctz:
                case Mnemonic.i32_popcnt:
                case Mnemonic.i32_add:
                case Mnemonic.i32_sub:
                case Mnemonic.i32_mul:
                case Mnemonic.i32_div_s:
                case Mnemonic.i32_div_u:
                case Mnemonic.i32_rem_s:
                case Mnemonic.i32_rem_u:
                case Mnemonic.i32_and:
                case Mnemonic.i32_or:
                case Mnemonic.i32_xor:
                case Mnemonic.i32_shl:
                case Mnemonic.i32_shr_s:
                case Mnemonic.i32_shr_u:
                case Mnemonic.i32_rotl:
                case Mnemonic.i32_rotr:
                case Mnemonic.i64_clz:
                case Mnemonic.i64_ctz:
                case Mnemonic.i64_popcnt:
                case Mnemonic.i64_add:
                case Mnemonic.i64_sub:
                case Mnemonic.i64_mul:
                case Mnemonic.i64_div_s:
                case Mnemonic.i64_div_u:
                case Mnemonic.i64_rem_s:
                case Mnemonic.i64_rem_u:
                case Mnemonic.i64_and:
                case Mnemonic.i64_or:
                case Mnemonic.i64_xor:
                case Mnemonic.i64_shl:
                case Mnemonic.i64_shr_s:
                case Mnemonic.i64_shr_u:
                case Mnemonic.i64_rotl:
                case Mnemonic.i64_rotr:
                case Mnemonic.f32_abs:
                case Mnemonic.f32_neg:
                case Mnemonic.f32_ceil:
                case Mnemonic.f32_floor:
                case Mnemonic.f32_trunc:
                case Mnemonic.f32_nearest:
                case Mnemonic.f32_sqrt:
                case Mnemonic.f32_add:
                case Mnemonic.f32_sub:
                case Mnemonic.f32_mul:
                case Mnemonic.f32_div:
                case Mnemonic.f32_min:
                case Mnemonic.f32_max:
                case Mnemonic.f32_copysign:
                case Mnemonic.f64_abs:
                case Mnemonic.f64_neg:
                case Mnemonic.f64_ceil:
                case Mnemonic.f64_floor:
                case Mnemonic.f64_trunc:
                case Mnemonic.f64_nearest:
                case Mnemonic.f64_sqrt:
                case Mnemonic.f64_add:
                case Mnemonic.f64_sub:
                case Mnemonic.f64_mul:
                case Mnemonic.f64_div:
                case Mnemonic.f64_min:
                case Mnemonic.f64_max:
                case Mnemonic.f64_copysign:

                case Mnemonic.i32_wrap_i64:
                case Mnemonic.i32_trunc_s_f32:
                case Mnemonic.i32_trunc_u_f32:
                case Mnemonic.i32_trunc_s_f64:
                case Mnemonic.i32_trunc_u_f64:
                case Mnemonic.i64_extend_s_i32:
                case Mnemonic.i64_extend_u_i32:
                case Mnemonic.i64_trunc_s_f32:
                case Mnemonic.i64_trunc_u_f32:
                case Mnemonic.i64_trunc_s_f64:
                case Mnemonic.i64_trunc_u_f64:
                case Mnemonic.f32_convert_s_i32:
                case Mnemonic.f32_convert_u_i32:
                case Mnemonic.f32_convert_s_i64:
                case Mnemonic.f32_convert_u_i64:
                case Mnemonic.f32_demote_f64:
                case Mnemonic.f64_convert_s_i32:
                case Mnemonic.f64_convert_u_i32:
                case Mnemonic.f64_convert_s_i64:
                case Mnemonic.f64_convert_u_i64:
                case Mnemonic.f64_promote_f32:

                case Mnemonic.i32_reinterpret_f32:
                case Mnemonic.i64_reinterpret_f64:
                case Mnemonic.f32_reinterpret_i32:
                case Mnemonic.f64_reinterpret_i64:
                default:
                    if (!WasmInstruction.mpoptostring.TryGetValue((Mnemonic)b, out string? str))
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