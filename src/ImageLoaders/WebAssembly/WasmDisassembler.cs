#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Types;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.ImageLoaders.WebAssembly
{
    public class WasmDisassembler : DisassemblerBase<WasmInstruction, Mnemonic>
    {
        private static readonly Decoder[] decoders;

        private readonly WasmArchitecture arch;
        private readonly WasmImageReader rdr;
        private Address addr;

        public WasmDisassembler(WasmArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.addr = rdr.Address;
            var bytes = ((ByteImageReader) rdr).Bytes;
            this.rdr = new WasmImageReader(new ByteMemoryArea(rdr.Address - rdr.Offset, bytes))
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
            var instr = decoders[b].Decode(b, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        private WasmInstruction DecodeInstruction(InstrDecoder decoder)
        {
            var ops = new List<MachineOperand>();
            int i = 0;
            uint u;
            ulong ul;
            var fmt = decoder.OperandFormat;
            while (i < fmt.Length)
            {
                switch (fmt[i++])
                {
                case ',': continue;
                case 'v':
                    if (!rdr.TryReadVarUInt32(out u))
                    {
                        return CreateInvalidInstruction();
                    }
                    ops.Add(ImmediateOperand.Word32(u));
                    break;
                case 'V':
                    if (!rdr.TryReadVarUInt64(out ul))
                    {
                        return CreateInvalidInstruction();
                    }
                    ops.Add(ImmediateOperand.Word64(ul));
                    break;
                case 'b':
                    if (!rdr.TryReadVarInt64(out long blockType))
                    {
                        return CreateInvalidInstruction();
                    }
                    if (blockType < 0)
                    {
                        if (blockType != -0x40L)
                        {
                            // byte: signals a valtype
                            //$TODO: add TypeOperand
                            ops.Add(ImmediateOperand.Byte((byte) (blockType & 0x7F)));
                        }
                    }
                    else
                    {
                        // uint32: signals type index.
                        ops.Add(ImmediateOperand.UInt32((uint) blockType));
                    }
                    break;
                case 'r':
                    if (!rdr.TryReadUInt32(out u))
                    {
                        return CreateInvalidInstruction();
                    }
                    ops.Add(new ImmediateOperand(Constant.FloatFromBitpattern(u)));
                    break;
                case 'R':
                    if (!rdr.TryReadUInt64(out ul))
                    {
                        return CreateInvalidInstruction();
                    }
                    ops.Add(new ImmediateOperand(Constant.DoubleFromBitpattern((long)ul)));
                    break;
                case 'm':
                    if (!rdr.TryReadVarUInt32(out uint alignment) ||
                        !rdr.TryReadVarUInt32(out uint offset))
                    {
                        return CreateInvalidInstruction();
                    }
                    Debug.Assert(decoder.DataType is not null, $"Missing data type for {decoder.Mnemonic}");
                    ops.Add(new MemoryOperand(decoder.DataType, alignment, offset));
                    break;
                case 't':
                    if (!rdr.TryReadVarUInt32(out uint tableidx))
                    {
                        return CreateInvalidInstruction();
                    }
                    ops.Add(ImmediateOperand.UInt32(tableidx));
                    break;
                case 'T':
                    if (!rdr.TryReadVarUInt32(out uint cLabels))
                    {
                        return CreateInvalidInstruction();
                    }
                    var labels = new uint[cLabels];
                    for (i = 0; i < (int)cLabels; ++i)
                    {
                        if (!rdr.TryReadVarUInt32(out labels[i]))
                            return CreateInvalidInstruction();
                    }
                    if (!rdr.TryReadVarUInt32(out uint defaultLabel))
                    {
                        return CreateInvalidInstruction();
                    }
                    ops.Add(new BranchTableOperand(labels, defaultLabel));
                    break;
                default:
                    return this.NotYetImplemented($"Wasm format '{fmt[i - 1]}'.");
                }
            }

            var instr = new WasmInstruction(decoder.Mnemonic)
            {
                Address = addr,
                Operands = ops.ToArray(),
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

        private abstract class Decoder
        {
            public abstract WasmInstruction Decode(uint uInstr, WasmDisassembler dasm);
        }

        private class InstrDecoder : Decoder
        {
            public readonly Mnemonic Mnemonic;
            public readonly InstrClass InstrClass;
            public readonly PrimitiveType? DataType;
            public readonly string OperandFormat;

            public InstrDecoder(Mnemonic mnemonic, InstrClass iclass, PrimitiveType? dataType, string operandFormat)
            {
                this.Mnemonic = mnemonic;
                this.InstrClass = iclass;
                this.DataType = dataType;
                this.OperandFormat = operandFormat;
            }

            public override WasmInstruction Decode(uint uInstr, WasmDisassembler dasm)
            {
                return dasm.DecodeInstruction(this);
            }
        }

        private class SparseDecoder : Decoder
        {
            private readonly Dictionary<uint, InstrDecoder> decoders;
            private readonly InstrDecoder defaultDecoder;

            public SparseDecoder(InstrDecoder defaultDecoder, params (uint, InstrDecoder)[] sparseDecoders)
            {
                this.decoders = new Dictionary<uint, InstrDecoder>();
                this.defaultDecoder = defaultDecoder;
                foreach (var (u, decoder) in sparseDecoders)
                {
                    decoders.Add(u, decoder);
                }
            }

            public override WasmInstruction Decode(uint uInstr, WasmDisassembler dasm)
            {
                if (dasm.rdr.TryReadVarUInt32(out uint u) &&
                    decoders.TryGetValue(u, out var decoder))
                {
                    return decoder.Decode(u, dasm);
                }
                else
                {
                    return dasm.CreateInvalidInstruction();
                }
            }
        }

        private static InstrDecoder Instr(Mnemonic mnemonic)
        {
            return new InstrDecoder(mnemonic, InstrClass.Linear, null, "");
        }

        private static InstrDecoder Instr(Mnemonic mnemonic, string operandFormat)
        {
            return new InstrDecoder(mnemonic, InstrClass.Linear, null, operandFormat);
        }

        private static InstrDecoder Instr(Mnemonic mnemonic, PrimitiveType dt, string operandFormat)
        {
            return new InstrDecoder(mnemonic, InstrClass.Linear, dt, operandFormat);
        }

        private static InstrDecoder Instr(Mnemonic mnemonic, InstrClass iclass)
        {
            return new InstrDecoder(mnemonic, iclass, null, "");
        }


        static WasmDisassembler()
        {
            var invalid = Instr(Mnemonic.unreachable, InstrClass.Invalid);

            var fc_decoders = new SparseDecoder(invalid,
                (0, Instr(Mnemonic.i32_trunc_sat_f32_s)),
                (1, Instr(Mnemonic.i32_trunc_sat_f32_u)),
                (2, Instr(Mnemonic.i32_trunc_sat_f64_s)),
                (3, Instr(Mnemonic.i32_trunc_sat_f64_u)),
                (4, Instr(Mnemonic.i64_trunc_sat_f32_s)),
                (5, Instr(Mnemonic.i64_trunc_sat_f32_u)),
                (6, Instr(Mnemonic.i64_trunc_sat_f64_s)),
                (7, Instr(Mnemonic.i64_trunc_sat_f64_u)),

                (8, Instr(Mnemonic.memory_init, "d")), // 0x00 
                (9, Instr(Mnemonic.data_drop, "d")),
                (10, Instr(Mnemonic.memory_copy, "")), // 0x00 0x00
                (11, Instr(Mnemonic.memory_fill, "")), // 0x00 ⇒ m


                (12, Instr(Mnemonic.table_init, "e,t,S")),
                (13, Instr(Mnemonic.elem_drop, "e")),
                (14, Instr(Mnemonic.table_copy, "t,t")),
                (15, Instr(Mnemonic.table_grow, "t")),
                (16, Instr(Mnemonic.table_size, "t")),
                (17, Instr(Mnemonic.table_fill, "t")));

            var fd_decoders = new SparseDecoder(invalid,
                (0, Instr(Mnemonic.v128_load, PrimitiveType.Word128, "m")),
                (1, Instr(Mnemonic.v128_load8x8_s, "m")),
                (2, Instr(Mnemonic.v128_load8x8_u, "m")),
                (3, Instr(Mnemonic.v128_load16x4_s, "m")),
                (4, Instr(Mnemonic.v128_load16x4_u, "m")),
                (5, Instr(Mnemonic.v128_load32x2_s, "m")),
                (6, Instr(Mnemonic.v128_load32x2_u, "m")),
                (7, Instr(Mnemonic.v128_load8_splat, "m")),
                (8, Instr(Mnemonic.v128_load16_splat, "m")),
                (9, Instr(Mnemonic.v128_load32_splat, "m")),
                (10, Instr(Mnemonic.v128_load64_splat, "m")),
                (92, Instr(Mnemonic.v128_load32_zero, "m")),
                (93, Instr(Mnemonic.v128_load64_zero, "m")),
                (11, Instr(Mnemonic.v128_store, "m")),
                (84, Instr(Mnemonic.v128_load8_lane, "m,l")),
                (85, Instr(Mnemonic.v128_load16_lane, "m,l")),
                (86, Instr(Mnemonic.v128_load32_lane, "m,l")),
                (87, Instr(Mnemonic.v128_load64_lane, "m,l")),
                (88, Instr(Mnemonic.v128_store8_lane, "m,l")),
                (89, Instr(Mnemonic.v128_store16_lane, "m,l")),
                (90, Instr(Mnemonic.v128_store32_lane, "m,l")),
                (91, Instr(Mnemonic.v128_store64_lane, "m,l")));

            decoders = new Decoder[256] {
                // 00
                Instr(Mnemonic.unreachable, ""),
                Instr(Mnemonic.nop, ""),
                Instr(Mnemonic.block, "b"),
                Instr(Mnemonic.loop, "b"),

                Instr(Mnemonic.@if, "b"),
                Instr(Mnemonic.@else, ""),
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                Instr(Mnemonic.end, ""),

                Instr(Mnemonic.br, "v"),
                Instr(Mnemonic.br_if, "v"),
                Instr(Mnemonic.br_table, "T"),
                Instr(Mnemonic.@return, ""),

                // 10
                Instr(Mnemonic.call, "v"),
                Instr(Mnemonic.call_indirect, "v,v"),
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                Instr(Mnemonic.drop, ""),
                Instr(Mnemonic.select, ""),

                Instr(Mnemonic.select, "t*"),
                invalid,
                invalid,
                invalid,

                // 20
                Instr(Mnemonic.get_local, "v"),
                Instr(Mnemonic.set_local, "v"),
                Instr(Mnemonic.tee_local, "v"),
                Instr(Mnemonic.get_global, "v"),

                Instr(Mnemonic.set_global, "v"),
                Instr(Mnemonic.table_get, "t"),
                Instr(Mnemonic.table_set, "t"),
                invalid,

                Instr(Mnemonic.i32_load, PrimitiveType.Word32, "m"),
                Instr(Mnemonic.i64_load, PrimitiveType.Word64, "m"),
                Instr(Mnemonic.f32_load, PrimitiveType.Real32, "m"),
                Instr(Mnemonic.f64_load, PrimitiveType.Real64, "m"),

                Instr(Mnemonic.i32_load8_s, PrimitiveType.Int8, "m"),
                Instr(Mnemonic.i32_load8_u, PrimitiveType.Byte, "m"),
                Instr(Mnemonic.i32_load16_s, PrimitiveType.Int16, "m"),
                Instr(Mnemonic.i32_load16_u, PrimitiveType.Word16, "m"),

                // 30
                Instr(Mnemonic.i64_load8_s, PrimitiveType.Int8, "m"),
                Instr(Mnemonic.i64_load8_u, PrimitiveType.Byte, "m"),
                Instr(Mnemonic.i64_load16_s, PrimitiveType.Int16, "m"),
                Instr(Mnemonic.i64_load16_u, PrimitiveType.Word16, "m"),

                Instr(Mnemonic.i64_load32_s, PrimitiveType.Int32, "m"),
                Instr(Mnemonic.i64_load32_u, PrimitiveType.Word32,"m"),
                Instr(Mnemonic.i32_store, PrimitiveType.Word32, "m"),
                Instr(Mnemonic.i64_store, PrimitiveType.Word64, "m"),

                Instr(Mnemonic.f32_store, PrimitiveType.Word32, "m"),
                Instr(Mnemonic.f64_store, PrimitiveType.Word64, "m"),
                Instr(Mnemonic.i32_store8, PrimitiveType.Byte, "m"),
                Instr(Mnemonic.i32_store16, PrimitiveType.Word16, "m"),

                Instr(Mnemonic.i64_store8, PrimitiveType.Byte, "m"),
                Instr(Mnemonic.i64_store16, PrimitiveType.Word16, "m"),
                Instr(Mnemonic.i64_store32, PrimitiveType.Word32, "m"),
                Instr(Mnemonic.current_memory, "v"),    //$ expect 0 in byte stream

                // 40
                Instr(Mnemonic.grow_memory, "v"),       //$ expect 0 in byte stream
                Instr(Mnemonic.i32_const, "v"),
                Instr(Mnemonic.i64_const, "V"),
                Instr(Mnemonic.f32_const, "r"),

                Instr(Mnemonic.f64_const, "R"),
                Instr(Mnemonic.i32_eqz, ""),
                Instr(Mnemonic.i32_eq, ""),
                Instr(Mnemonic.i32_ne, ""),

                Instr(Mnemonic.i32_lt_s, ""),
                Instr(Mnemonic.i32_lt_u, ""),
                Instr(Mnemonic.i32_gt_s, ""),
                Instr(Mnemonic.i32_gt_u, ""),

                Instr(Mnemonic.i32_le_s, ""),
                Instr(Mnemonic.i32_le_u, ""),
                Instr(Mnemonic.i32_ge_s, ""),
                Instr(Mnemonic.i32_ge_u, ""),

                // 50
                Instr(Mnemonic.i64_eqz, ""),
                Instr(Mnemonic.i64_eq, ""),
                Instr(Mnemonic.i64_ne, ""),
                Instr(Mnemonic.i64_lt_s, ""),

                Instr(Mnemonic.i64_lt_u, ""),
                Instr(Mnemonic.i64_gt_s, ""),
                Instr(Mnemonic.i64_gt_u, ""),
                Instr(Mnemonic.i64_le_s, ""),

                Instr(Mnemonic.i64_le_u, ""),
                Instr(Mnemonic.i64_ge_s, ""),
                Instr(Mnemonic.i64_ge_u, ""),
                Instr(Mnemonic.f32_eq, ""),

                Instr(Mnemonic.f32_ne, ""),
                Instr(Mnemonic.f32_lt, ""),
                Instr(Mnemonic.f32_gt, ""),
                Instr(Mnemonic.f32_le, ""),

                // 60
                Instr(Mnemonic.f32_ge, ""),
                Instr(Mnemonic.f64_eq, ""),
                Instr(Mnemonic.f64_ne, ""),
                Instr(Mnemonic.f64_lt, ""),

                Instr(Mnemonic.f64_gt, ""),
                Instr(Mnemonic.f64_le, ""),
                Instr(Mnemonic.f64_ge, ""),
                Instr(Mnemonic.i32_clz, ""),

                Instr(Mnemonic.i32_ctz, ""),
                Instr(Mnemonic.i32_popcnt, ""),
                Instr(Mnemonic.i32_add, ""),
                Instr(Mnemonic.i32_sub, ""),

                Instr(Mnemonic.i32_mul, ""),
                Instr(Mnemonic.i32_div_s, ""),
                Instr(Mnemonic.i32_div_u, ""),
                Instr(Mnemonic.i32_rem_s, ""),

                // 70
                Instr(Mnemonic.i32_rem_u, ""),
                Instr(Mnemonic.i32_and, ""),
                Instr(Mnemonic.i32_or, ""),
                Instr(Mnemonic.i32_xor, ""),

                Instr(Mnemonic.i32_shl, ""),
                Instr(Mnemonic.i32_shr_s, ""),
                Instr(Mnemonic.i32_shr_u, ""),
                Instr(Mnemonic.i32_rotl, ""),

                Instr(Mnemonic.i32_rotr, ""),
                Instr(Mnemonic.i64_clz, ""),
                Instr(Mnemonic.i64_ctz, ""),
                Instr(Mnemonic.i64_popcnt, ""),

                Instr(Mnemonic.i64_add, ""),
                Instr(Mnemonic.i64_sub, ""),
                Instr(Mnemonic.i64_mul, ""),
                Instr(Mnemonic.i64_div_s, ""),

                // 80
                Instr(Mnemonic.i64_div_u, ""),
                Instr(Mnemonic.i64_rem_s, ""),
                Instr(Mnemonic.i64_rem_u, ""),
                Instr(Mnemonic.i64_and, ""),

                Instr(Mnemonic.i64_or, ""),
                Instr(Mnemonic.i64_xor, ""),
                Instr(Mnemonic.i64_shl, ""),
                Instr(Mnemonic.i64_shr_s, ""),

                Instr(Mnemonic.i64_shr_u, ""),
                Instr(Mnemonic.i64_rotl, ""),
                Instr(Mnemonic.i64_rotr, ""),
                Instr(Mnemonic.f32_abs, ""),

                Instr(Mnemonic.f32_neg, ""),
                Instr(Mnemonic.f32_ceil, ""),
                Instr(Mnemonic.f32_floor, ""),
                Instr(Mnemonic.f32_trunc, ""),

                // 90
                Instr(Mnemonic.f32_nearest, ""),
                Instr(Mnemonic.f32_sqrt, ""),
                Instr(Mnemonic.f32_add, ""),
                Instr(Mnemonic.f32_sub, ""),

                Instr(Mnemonic.f32_mul, ""),
                Instr(Mnemonic.f32_div, ""),
                Instr(Mnemonic.f32_min, ""),
                Instr(Mnemonic.f32_max, ""),

                Instr(Mnemonic.f32_copysign, ""),
                Instr(Mnemonic.f64_abs, ""),
                Instr(Mnemonic.f64_neg, ""),
                Instr(Mnemonic.f64_ceil, ""),

                Instr(Mnemonic.f64_floor, ""),
                Instr(Mnemonic.f64_trunc, ""),
                Instr(Mnemonic.f64_nearest, ""),
                Instr(Mnemonic.f64_sqrt, ""),

                // A0
                Instr(Mnemonic.f64_add, ""),
                Instr(Mnemonic.f64_sub, ""),
                Instr(Mnemonic.f64_mul, ""),
                Instr(Mnemonic.f64_div, ""),

                Instr(Mnemonic.f64_min, ""),
                Instr(Mnemonic.f64_max, ""),
                Instr(Mnemonic.f64_copysign, ""),
                Instr(Mnemonic.i32_wrap_i64, ""),

                Instr(Mnemonic.i32_trunc_s_f32, ""),
                Instr(Mnemonic.i32_trunc_u_f32, ""),
                Instr(Mnemonic.i32_trunc_s_f64, ""),
                Instr(Mnemonic.i32_trunc_u_f64, ""),

                Instr(Mnemonic.i64_extend_s_i32, ""),
                Instr(Mnemonic.i64_extend_u_i32, ""),
                Instr(Mnemonic.i64_trunc_s_f32, ""),
                Instr(Mnemonic.i64_trunc_u_f32, ""),

                // B0
                Instr(Mnemonic.i64_trunc_s_f64, ""),
                Instr(Mnemonic.i64_trunc_u_f64, ""),
                Instr(Mnemonic.f32_convert_s_i32, ""),
                Instr(Mnemonic.f32_convert_u_i32, ""),

                Instr(Mnemonic.f32_convert_s_i64, ""),
                Instr(Mnemonic.f32_convert_u_i64, ""),
                Instr(Mnemonic.f32_demote_f64, ""),
                Instr(Mnemonic.f64_convert_s_i32, ""),

                Instr(Mnemonic.f64_convert_u_i32, ""),
                Instr(Mnemonic.f64_convert_s_i64, ""),
                Instr(Mnemonic.f64_convert_u_i64, ""),
                Instr(Mnemonic.f64_promote_f32, ""),

                Instr(Mnemonic.i32_reinterpret_f32, ""),
                Instr(Mnemonic.i64_reinterpret_f64, ""),
                Instr(Mnemonic.f32_reinterpret_i32, ""),
                Instr(Mnemonic.f64_reinterpret_i64, ""),

                // C0
                Instr(Mnemonic.i32_extend8_s  , ""),
                Instr(Mnemonic.i32_extend16_s , ""),
                Instr(Mnemonic.i64_extend8_s  , ""),
                Instr(Mnemonic.i64_extend16_s , ""),

                Instr(Mnemonic.i64_extend32_s , ""),
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                // D0
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                // E0
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                // F0
                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                fc_decoders,
                fd_decoders,
                invalid,
                invalid,
        };
    }
    }
}
