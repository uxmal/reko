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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.WebAssembly
{
    public class WasmDisassembler : DisassemblerBase<WasmInstruction, Mnemonic>
    {
        private readonly WasmArchitecture arch;
        private readonly WasmImageReader rdr;
        private Address addr;

        public WasmDisassembler(WasmArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.addr = rdr.Address;
            var bytes = ((ByteImageReader) rdr).Bytes;
            this.rdr = new WasmImageReader(new ByteMemoryArea(rdr.Address, bytes))
            {
                Offset = rdr.Offset
            };
        }

        public override WasmInstruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadByte(out byte b))
            {
                return null;
            }
            var opcode = (Mnemonic)b;
            if (!decoders.TryGetValue(opcode, out string? fmt))
            {
                opcode = Mnemonic.unreachable;
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
                        opcode = Mnemonic.unreachable;
                        break;
                    }
                    ops.Add(ImmediateOperand.Word32(u));
                    break;
                case 'r':
                    if (!rdr.TryReadUInt32(out u))
                    {
                        opcode = Mnemonic.unreachable;
                        break;
                    }
                    ops.Add(new ImmediateOperand(Constant.FloatFromBitpattern(u)));
                    break;
                default:
                    throw new NotImplementedException(string.Format("Format '{0}'.", fmt[i - 1]));
                }
            }

            var instr = new WasmInstruction(opcode)
            {
                Address = addr,
                Operands = ops.ToArray(),
                Length = (int) (rdr.Address - addr)
            };
            return instr;
        }

        public override WasmInstruction CreateInvalidInstruction()
        {
            var instr = new WasmInstruction(Mnemonic.unreachable)
            {
                InstructionClass = InstrClass.Invalid,
                Operands = MachineInstruction.NoOperands,
            };
            return instr;
        }

        public override WasmInstruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("WasmDis", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        private static readonly Dictionary<Mnemonic, string> decoders = new Dictionary<Mnemonic, string>
        {
            { Mnemonic.unreachable, "" },
            { Mnemonic.nop, "" },
            { Mnemonic.block, "b" },
            { Mnemonic.loop, "b" },
            { Mnemonic.@if, "b" },
            { Mnemonic.@else, "" },
            { Mnemonic.end, "" },
            { Mnemonic.br, "v" },
            { Mnemonic.br_if, "v" },
            { Mnemonic.br_table, "t" },
            { Mnemonic.@return, "" },

            { Mnemonic.call, "v" },
            { Mnemonic.call_indirect, "v,v" },

            { Mnemonic.drop, "" },
            { Mnemonic.select, "" },
            { Mnemonic.get_local, "v" },
            { Mnemonic.set_local, "v" },
            { Mnemonic.tee_local, "v" },
            { Mnemonic.get_global, "v" },
            { Mnemonic.set_global, "v" },
            { Mnemonic.i32_load, "m" },
            { Mnemonic.i64_load, "m" },
            { Mnemonic.f32_load, "m" },
            { Mnemonic.f64_load, "m" },
            { Mnemonic.i32_load8_s, "m" },
            { Mnemonic.i32_load8_u, "m" },
            { Mnemonic.i32_load16_s, "m" },
            { Mnemonic.i32_load16_u, "m" },
            { Mnemonic.i64_load8_s, "m" },
            { Mnemonic.i64_load8_u, "m" },
            { Mnemonic.i64_load16_s, "m" },
            { Mnemonic.i64_load16_u, "m" },
            { Mnemonic.i64_load32_s, "m" },
            { Mnemonic.i64_load32_u, "m" },
            { Mnemonic.i32_store, "m" },
            { Mnemonic.i64_store, "m" },
            { Mnemonic.f32_store, "m" },
            { Mnemonic.f64_store, "m" },
            { Mnemonic.i32_store8, "m" },
            { Mnemonic.i32_store16, "m" },
            { Mnemonic.i64_store8, "m" },
            { Mnemonic.i64_store16, "m" },
            { Mnemonic.i64_store32, "m" },
            { Mnemonic.current_memory, "v" },
            { Mnemonic.grow_memory, "v" },

            { Mnemonic.i32_const, "v" },
            { Mnemonic.i64_const, "V" },
            { Mnemonic.f32_const, "r" },
            { Mnemonic.f64_const, "R" },
            { Mnemonic.i32_eqz, "" },
            { Mnemonic.i32_eq, "" },
            { Mnemonic.i32_ne, "" },
            { Mnemonic.i32_lt_s, "" },
            { Mnemonic.i32_lt_u, "" },
            { Mnemonic.i32_gt_s, "" },
            { Mnemonic.i32_gt_u, "" },
            { Mnemonic.i32_le_s, "" },
            { Mnemonic.i32_le_u, "" },
            { Mnemonic.i32_ge_s, "" },
            { Mnemonic.i32_ge_u, "" },
            { Mnemonic.i64_eqz, "" },
            { Mnemonic.i64_eq, "" },
            { Mnemonic.i64_ne, "" },
            { Mnemonic.i64_lt_s, "" },
            { Mnemonic.i64_lt_u, "" },
            { Mnemonic.i64_gt_s, "" },
            { Mnemonic.i64_gt_u, "" },
            { Mnemonic.i64_le_s, "" },
            { Mnemonic.i64_le_u, "" },
            { Mnemonic.i64_ge_s, "" },
            { Mnemonic.i64_ge_u, "" },
            { Mnemonic.f32_eq, "" },
            { Mnemonic.f32_ne, "" },
            { Mnemonic.f32_lt, "" },
            { Mnemonic.f32_gt, "" },
            { Mnemonic.f32_le, "" },
            { Mnemonic.f32_ge, "" },
            { Mnemonic.f64_eq, "" },
            { Mnemonic.f64_ne, "" },
            { Mnemonic.f64_lt, "" },
            { Mnemonic.f64_gt, "" },
            { Mnemonic.f64_le, "" },
            { Mnemonic.f64_ge, "" },

            { Mnemonic.i32_clz, "" },
            { Mnemonic.i32_ctz, "" },
            { Mnemonic.i32_popcnt, "" },
            { Mnemonic.i32_add, "" },
            { Mnemonic.i32_sub, "" },
            { Mnemonic.i32_mul, "" },
            { Mnemonic.i32_div_s, "" },
            { Mnemonic.i32_div_u, "" },
            { Mnemonic.i32_rem_s, "" },
            { Mnemonic.i32_rem_u, "" },
            { Mnemonic.i32_and, "" },
            { Mnemonic.i32_or, "" },
            { Mnemonic.i32_xor, "" },
            { Mnemonic.i32_shl, "" },
            { Mnemonic.i32_shr_s, "" },
            { Mnemonic.i32_shr_u, "" },
            { Mnemonic.i32_rotl, "" },
            { Mnemonic.i32_rotr, "" },
            { Mnemonic.i64_clz, "" },
            { Mnemonic.i64_ctz, "" },
            { Mnemonic.i64_popcnt, "" },
            { Mnemonic.i64_add, "" },
            { Mnemonic.i64_sub, "" },
            { Mnemonic.i64_mul, "" },
            { Mnemonic.i64_div_s, "" },
            { Mnemonic.i64_div_u, "" },
            { Mnemonic.i64_rem_s, "" },
            { Mnemonic.i64_rem_u, "" },
            { Mnemonic.i64_and, "" },
            { Mnemonic.i64_or, "" },
            { Mnemonic.i64_xor, "" },
            { Mnemonic.i64_shl, "" },
            { Mnemonic.i64_shr_s, "" },
            { Mnemonic.i64_shr_u, "" },
            { Mnemonic.i64_rotl, "" },
            { Mnemonic.i64_rotr, "" },
            { Mnemonic.f32_abs, "" },
            { Mnemonic.f32_neg, "" },
            { Mnemonic.f32_ceil, "" },
            { Mnemonic.f32_floor, "" },
            { Mnemonic.f32_trunc, "" },
            { Mnemonic.f32_nearest, "" },
            { Mnemonic.f32_sqrt, "" },
            { Mnemonic.f32_add, "" },
            { Mnemonic.f32_sub, "" },
            { Mnemonic.f32_mul, "" },
            { Mnemonic.f32_div, "" },
            { Mnemonic.f32_min, "" },
            { Mnemonic.f32_max, "" },
            { Mnemonic.f32_copysign, "" },
            { Mnemonic.f64_abs, "" },
            { Mnemonic.f64_neg, "" },
            { Mnemonic.f64_ceil, "" },
            { Mnemonic.f64_floor, "" },
            { Mnemonic.f64_trunc, "" },
            { Mnemonic.f64_nearest, "" },
            { Mnemonic.f64_sqrt, "" },
            { Mnemonic.f64_add, "" },
            { Mnemonic.f64_sub, "" },
            { Mnemonic.f64_mul, "" },
            { Mnemonic.f64_div, "" },
            { Mnemonic.f64_min, "" },
            { Mnemonic.f64_max, "" },
            { Mnemonic.f64_copysign, "" },

            { Mnemonic.i32_wrap_i64, "" },
            { Mnemonic.i32_trunc_s_f32, "" },
            { Mnemonic.i32_trunc_u_f32, "" },
            { Mnemonic.i32_trunc_s_f64, "" },
            { Mnemonic.i32_trunc_u_f64, "" },
            { Mnemonic.i64_extend_s_i32, "" },
            { Mnemonic.i64_extend_u_i32, "" },
            { Mnemonic.i64_trunc_s_f32, "" },
            { Mnemonic.i64_trunc_u_f32, "" },
            { Mnemonic.i64_trunc_s_f64, "" },
            { Mnemonic.i64_trunc_u_f64, "" },
            { Mnemonic.f32_convert_s_i32, "" },
            { Mnemonic.f32_convert_u_i32, "" },
            { Mnemonic.f32_convert_s_i64, "" },
            { Mnemonic.f32_convert_u_i64, "" },
            { Mnemonic.f32_demote_f64, "" },
            { Mnemonic.f64_convert_s_i32, "" },
            { Mnemonic.f64_convert_u_i32, "" },
            { Mnemonic.f64_convert_s_i64, "" },
            { Mnemonic.f64_convert_u_i64, "" },
            { Mnemonic.f64_promote_f32, "" },

            { Mnemonic.i32_reinterpret_f32, "" },
            { Mnemonic.i64_reinterpret_f64, "" },
            { Mnemonic.f32_reinterpret_i32, "" },
            { Mnemonic.f64_reinterpret_i64, "" },
        };
    }
}
