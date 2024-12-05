#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Reko.ImageLoaders.WebAssembly
{
    using Decoder = Decoder<WasmDisassembler, Mnemonic, WasmInstruction>;
    using Mutator = Mutator<WasmDisassembler>;

    public class WasmDisassembler : DisassemblerBase<WasmInstruction, Mnemonic>
    {
        private static readonly Decoder[] decoders;

        private readonly WasmArchitecture arch;
        private readonly WasmImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;

        public WasmDisassembler(WasmArchitecture arch, WasmImageReader rdr)
        {
            this.arch = arch;
            this.addr = rdr.Address;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override WasmInstruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadByte(out byte b))
            {
                return null;
            }
            var instr = decoders[b].Decode(b, this);
            ops.Clear();
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override WasmInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new WasmInstruction(mnemonic)
            {
                Operands = ops.ToArray()
            };
        }

        public override WasmInstruction CreateInvalidInstruction()
        {
            var instr = new WasmInstruction(Mnemonic.unreachable)
            {
                InstructionClass = InstrClass.Invalid,
                Operands = Array.Empty<MachineOperand>()
            };
            return instr;
        }

        public override WasmInstruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("WasmDis", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        /// <summary>
        /// Variable length i32
        /// </summary>
        private static bool i32(uint uInstr, WasmDisassembler dasm)
        {
            if (!dasm.rdr.TryReadVarInt64(out var i32))
                return false;
            dasm.ops.Add(ImmediateOperand.Word32((uint) i32));
            return true;
        }
        

        /// <summary>
        /// Variable length u32
        /// </summary>
        private static bool u32(uint uInstr, WasmDisassembler dasm)
        {
            if (!dasm.rdr.TryReadVarUInt64(out var u32))
                return false;
            if (uint.MinValue <= u32 && u32 <= uint.MaxValue)
            {
                dasm.ops.Add(ImmediateOperand.Word32((uint) u32));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Variable length signed 64-bit number
        /// </summary>
        private static bool i64(uint uInstr, WasmDisassembler dasm)
        {
            if (!dasm.rdr.TryReadVarInt64(out var i64))
                return false;
            dasm.ops.Add(ImmediateOperand.Word64(i64));
            return true;
        }

        /// <summary>
        /// Variable length unsigned 64-bit number.
        /// </summary>
        private static bool u64(uint uInstr, WasmDisassembler dasm)
        {
            if (!dasm.rdr.TryReadVarUInt64(out var u64))
                return false;
            dasm.ops.Add(ImmediateOperand.Word64(u64));
            return true;
        }

        /// <summary>
        /// 128-bit constant.
        /// </summary>
        private static bool i128(uint uInstr, WasmDisassembler dasm)
        {
            var data = dasm.rdr.ReadBytes(16);
            if (data.Length != 16)
                return false;
            var number = new BigInteger(data);
            var imm = new ImmediateOperand(new BigConstant(PrimitiveType.Word128, number));
            dasm.ops.Add(imm);
            return true;
        }

        private static readonly Mutator<WasmDisassembler> laneidx_16 = i128;

        private static readonly Mutator<WasmDisassembler> dataidx = u32;
        private static readonly Mutator<WasmDisassembler> elemidx = u32;
        private static readonly Mutator<WasmDisassembler> fieldidx = u32;
        private static readonly Mutator<WasmDisassembler> funcidx = u32;
        private static readonly Mutator<WasmDisassembler> labelidx = u32;
        private static readonly Mutator<WasmDisassembler> laneidx = u32;
        private static readonly Mutator<WasmDisassembler> typeidx = u32;
        private static readonly Mutator<WasmDisassembler> heaptype = i32;

        private static bool b(uint uInstr, WasmDisassembler dasm)
        {
            if (!dasm.rdr.TryReadVarInt64(out long blockType))
                return false;

            if (blockType < 0)
            {
                if (blockType != -0x40L)
                {
                    // byte: signals a valtype
                    //$TODO: add TypeOperand
                    dasm.ops.Add(ImmediateOperand.Byte((byte) (blockType & 0x7F)));
                }
            }
            else
            {
                // uint32: signals type index.
                dasm.ops.Add(ImmediateOperand.UInt32((uint) blockType));
            }
            return true;
        }

        private static bool f32(uint uInstr, WasmDisassembler dasm)
        {
            if (!dasm.rdr.TryReadUInt32(out uint u))
            {
                return false;
            }
            dasm.ops.Add(new ImmediateOperand(Constant.FloatFromBitpattern(u)));
            return true;
        }

        private static bool f64(uint uInstr, WasmDisassembler dasm)
        {
            if (!dasm.rdr.TryReadUInt64(out ulong u))
            {
                return false;
            }
            dasm.ops.Add(new ImmediateOperand(Constant.DoubleFromBitpattern((long) u)));
            return true;
        }

        public static Mutator m(PrimitiveType dt)
        {
            return (uint uInstr, WasmDisassembler dasm) =>
            {
                if (!dasm.rdr.TryReadVarUInt32(out uint alignment) ||
                    !dasm.rdr.TryReadVarUInt32(out uint offset))
                {
                    return false;
                }
                dasm.ops.Add(new MemoryOperand(dt, alignment, offset));
                return true;
            };
        }
        private static readonly Mutator m128 = m(PrimitiveType.Word128);

        private static bool T(uint uInstr, WasmDisassembler dasm)
        {
            if (!dasm.rdr.TryReadVarUInt32(out uint cLabels))
            {
                return false;
            }
            var labels = new uint[cLabels];
            for (int i = 0; i < (int) cLabels; ++i)
            {
                if (!dasm.rdr.TryReadVarUInt32(out labels[i]))
                    return false;
            }
            if (!dasm.rdr.TryReadVarUInt32(out uint defaultLabel))
            {
                return false;
            }
            dasm.ops.Add(new BranchTableOperand(labels, defaultLabel));
            return true;

        }

        private static bool vtypes(uint uInstr, WasmDisassembler dasm)
        {
            return false; //$TODO
        }


        private static bool catch_vec(uint uInstr, WasmDisassembler dasm)
        {
            return false; //$TODO
        }

        private static bool castflags(uint uInstr, WasmDisassembler dasm)
        {
            return false; //$TODO
        }

        private class NextDecoder : Decoder<WasmDisassembler, Mnemonic, WasmInstruction>
        {
            private readonly Decoder<WasmDisassembler, Mnemonic, WasmInstruction> next;

            public NextDecoder(Decoder<WasmDisassembler, Mnemonic, WasmInstruction> next)
            {
                this.next = next;
            }

            public override WasmInstruction Decode(uint wInstr, WasmDisassembler dasm)
            {
                if (!dasm.rdr.TryReadVarUInt32(out uint opcode))
                    return dasm.CreateInvalidInstruction();
                return next.Decode(opcode, dasm);
            }
        }

        public static InstrDecoder<WasmDisassembler, Mnemonic, WasmInstruction> Instr(Mnemonic mnemonic, params Mutator<WasmDisassembler>[] mutators)
        {
            return new InstrDecoder<WasmDisassembler, Mnemonic, WasmInstruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static InstrDecoder<WasmDisassembler, Mnemonic, WasmInstruction> Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator[] mutators)
        {
            return new InstrDecoder<WasmDisassembler, Mnemonic, WasmInstruction>(iclass, mnemonic, mutators);
        }


        private static NextDecoder Next(Decoder<WasmDisassembler, Mnemonic, WasmInstruction> next)
        {
            return new NextDecoder(next);
        }

        static WasmDisassembler()
        {
            var invalid = Instr(Mnemonic.unreachable, InstrClass.Invalid);

            var fb_decoders = Sparse(0, 8, "  0xFB decoders", invalid,
                (0, Instr(Mnemonic.struct_new, typeidx)),
                (1, Instr(Mnemonic.struct_new_default, typeidx)),
                (2, Instr(Mnemonic.struct_get, typeidx, fieldidx)),
                (3, Instr(Mnemonic.struct_get_s, typeidx, fieldidx)),
                (4, Instr(Mnemonic.struct_get_u, typeidx, fieldidx)),
                (5, Instr(Mnemonic.struct_set, typeidx, fieldidx)),
                (6, Instr(Mnemonic.array_new, typeidx)),
                (7, Instr(Mnemonic.array_new_default, typeidx)),
                (8, Instr(Mnemonic.array_new_fixed, typeidx, i32)),
                (9, Instr(Mnemonic.array_new_data, typeidx, dataidx)),
                (10, Instr(Mnemonic.array_new_elem, typeidx, elemidx)),
                (11, Instr(Mnemonic.array_get, typeidx)),
                (12, Instr(Mnemonic.array_get_s, typeidx)),
                (13, Instr(Mnemonic.array_get_u, typeidx)),
                (14, Instr(Mnemonic.array_set, typeidx)),
                (15, Instr(Mnemonic.array_len)),
                (16, Instr(Mnemonic.array_fill, typeidx)),
                (17, Instr(Mnemonic.array_copy, typeidx, typeidx)),
                (18, Instr(Mnemonic.array_init_data, typeidx, dataidx)),
                (19, Instr(Mnemonic.array_init_elem, typeidx, elemidx)),
                (20, Instr(Mnemonic.ref_test, heaptype)),
                (21, Instr(Mnemonic.ref_test_null, heaptype)),
                (22, Instr(Mnemonic.ref_cast, heaptype)),
                (23, Instr(Mnemonic.ref_cast_null, heaptype)),
                (24, Instr(Mnemonic.br_on_cast, castflags, labelidx, heaptype, heaptype)),
                (25, Instr(Mnemonic.br_on_cast_fail, castflags, labelidx, heaptype, heaptype)),
                (26, Instr(Mnemonic.any_convert_extern  )),
                (27, Instr(Mnemonic.extern_convert_any)),
                (28, Instr(Mnemonic.ref_i31)),
                (29, Instr(Mnemonic.i31_get_s)),
                (30, Instr(Mnemonic.i31_get_u)));

            var fc_decoders = Sparse(0, 8, "  0xFC decoders", invalid,
                (0, Instr(Mnemonic.i32_trunc_sat_f32_s)),
                (1, Instr(Mnemonic.i32_trunc_sat_f32_u)),
                (2, Instr(Mnemonic.i32_trunc_sat_f64_s)),
                (3, Instr(Mnemonic.i32_trunc_sat_f64_u)),
                (4, Instr(Mnemonic.i64_trunc_sat_f32_s)),
                (5, Instr(Mnemonic.i64_trunc_sat_f32_u)),
                (6, Instr(Mnemonic.i64_trunc_sat_f64_s)),
                (7, Instr(Mnemonic.i64_trunc_sat_f64_u)),

                (8, Instr(Mnemonic.memory_init, u32, u32)), // 0x00 
                (9, Instr(Mnemonic.data_drop, u32)),
                (10, Instr(Mnemonic.memory_copy, u32, u32)), // 0x00 0x00
                (11, Instr(Mnemonic.memory_fill, u32)), // 0x00 ⇒ m

                (12, Instr(Mnemonic.table_init, u32, u32)),
                (13, Instr(Mnemonic.elem_drop, u32)),
                (14, Instr(Mnemonic.table_copy, u32, u32)),
                (15, Instr(Mnemonic.table_grow, u32)),
                (16, Instr(Mnemonic.table_size, u32)),
                (17, Instr(Mnemonic.table_fill, u32)));

            var fd_decoders = Sparse(0, 9, "  0xFD decoders", invalid,
                (0, Instr(Mnemonic.v128_load, m128)),           // v128.load 𝑚
                (1, Instr(Mnemonic.v128_load8x8_s, m128)),      // v128.load8x8_s 𝑚
                (2, Instr(Mnemonic.v128_load8x8_u, m128)),      // v128.load8x8_u 𝑚
                (3, Instr(Mnemonic.v128_load16x4_s, m128)),     // v128.load16x4_s 𝑚
                (4, Instr(Mnemonic.v128_load16x4_u, m128)),     // v128.load16x4_u 𝑚
                (5, Instr(Mnemonic.v128_load32x2_s, m128)),     // v128.load32x2_s 𝑚
                (6, Instr(Mnemonic.v128_load32x2_u, m128)),     // v128.load32x2_u 𝑚
                (7, Instr(Mnemonic.v128_load8_splat, m128)),    // v128.load8_splat 𝑚
                (8, Instr(Mnemonic.v128_load16_splat, m128)),   // v128.load16_splat 𝑚
                (9, Instr(Mnemonic.v128_load32_splat, m128)),   // v128.load32_splat 𝑚
                (10, Instr(Mnemonic.v128_load64_splat, m128)),  // v128.load64_splat 𝑚
                (92, Instr(Mnemonic.v128_load32_zero, m128)),   // v128.load32_zero 𝑚
                (93, Instr(Mnemonic.v128_load64_zero, m128)),   // v128.load64_zero 𝑚
                (11, Instr(Mnemonic.v128_store, m128)),         // v128.store 𝑚
                (84, Instr(Mnemonic.v128_load8_lane, m128, laneidx)), // v128.load8_lane 𝑚 𝑙
                (85, Instr(Mnemonic.v128_load16_lane, m128, laneidx)), // v128.load16_lane 𝑚 𝑙
                (86, Instr(Mnemonic.v128_load32_lane, m128, laneidx)), // v128.load32_lane 𝑚 𝑙
                (87, Instr(Mnemonic.v128_load64_lane, m128, laneidx)), // v128.load64_lane 𝑚 𝑙
                (88, Instr(Mnemonic.v128_store8_lane, m128, laneidx)), // v128.store8_lane 𝑚 𝑙
                (89, Instr(Mnemonic.v128_store16_lane, m128, laneidx)), // v128.store16_lane 𝑚 𝑙
                (90, Instr(Mnemonic.v128_store32_lane, m128, laneidx)), // v128.store32_lane 𝑚 𝑙
                (91, Instr(Mnemonic.v128_store64_lane, m128, laneidx)), // v128.store64_lane 𝑚 𝑙
                (12, Instr(Mnemonic.v128_const, i128)),                 // v128.const b
                (13, Instr(Mnemonic.i8x16_shuffle, laneidx_16)),        // i8x16.shuffle laneidx_16
                (21, Instr(Mnemonic.i8x16_extract_lane_s, laneidx)), // i8x16.extract_lane_s 𝑙
                (22, Instr(Mnemonic.i8x16_extract_lane_u, laneidx)), // i8x16.extract_lane_u 𝑙
                (23, Instr(Mnemonic.i8x16_replace_lane, laneidx)), // i8x16.replace_lane 𝑙
                (24, Instr(Mnemonic.i16x8_extract_lane_s, laneidx)), // i16x8.extract_lane_s 𝑙
                (25, Instr(Mnemonic.i16x8_extract_lane_u, laneidx)), // i16x8.extract_lane_u 𝑙
                (26, Instr(Mnemonic.i16x8_replace_lane, laneidx)), // i16x8.replace_lane 𝑙
                (27, Instr(Mnemonic.i32x4_extract_lane, laneidx)), // i32x4.extract_lane 𝑙
                (28, Instr(Mnemonic.i32x4_replace_lane, laneidx)), // i32x4.replace_lane 𝑙
                (29, Instr(Mnemonic.i64x2_extract_lane, laneidx)), // i64x2.extract_lane 𝑙
                (30, Instr(Mnemonic.i64x2_replace_lane, laneidx)), // i64x2.replace_lane 𝑙
                (31, Instr(Mnemonic.f32x4_extract_lane, laneidx)), // f32x4.extract_lane 𝑙
                (32, Instr(Mnemonic.f32x4_replace_lane, laneidx)), // f32x4.replace_lane 𝑙
                (33, Instr(Mnemonic.f64x2_extract_lane, laneidx)), // f64x2.extract_lane 𝑙
                (34, Instr(Mnemonic.f64x2_replace_lane, laneidx)), // f64x2.replace_lane 𝑙
                (14, Instr(Mnemonic.i8x16_swizzle)), // i8x16.swizzle
                (15, Instr(Mnemonic.i8x16_splat)), // i8x16.splat
                (16, Instr(Mnemonic.i16x8_splat)), // i16x8.splat
                (17, Instr(Mnemonic.i32x4_splat)), // i32x4.splat
                (18, Instr(Mnemonic.i64x2_splat)), // i64x2.splat
                (19, Instr(Mnemonic.f32x4_splat)), // f32x4.splat
                (20, Instr(Mnemonic.f64x2_splat)), // f64x2.splat
                (35, Instr(Mnemonic.i8x16_eq)), // i8x16.eq
                (36, Instr(Mnemonic.i8x16_ne)), // i8x16.ne
                (37, Instr(Mnemonic.i8x16_lt_s)), // i8x16.lt_s
                (38, Instr(Mnemonic.i8x16_lt_u)), // i8x16.lt_u
                (39, Instr(Mnemonic.i8x16_gt_s)), // i8x16.gt_s
                (40, Instr(Mnemonic.i8x16_gt_u)), // i8x16.gt_u
                (41, Instr(Mnemonic.i8x16_le_s)), // i8x16.le_s
                (42, Instr(Mnemonic.i8x16_le_u)), // i8x16.le_u
                (43, Instr(Mnemonic.i8x16_ge_s)), // i8x16.ge_s
                (44, Instr(Mnemonic.i8x16_ge_u)), // i8x16.ge_u
                (45, Instr(Mnemonic.i16x8_eq)), // i16x8.eq
                (46, Instr(Mnemonic.i16x8_ne)), // i16x8.ne
                (47, Instr(Mnemonic.i16x8_lt_s)), // i16x8.lt_s
                (48, Instr(Mnemonic.i16x8_lt_u)), // i16x8.lt_u
                (49, Instr(Mnemonic.i16x8_gt_s)), // i16x8.gt_s
                (50, Instr(Mnemonic.i16x8_gt_u)), // i16x8.gt_u
                (51, Instr(Mnemonic.i16x8_le_s)), // i16x8.le_s
                (52, Instr(Mnemonic.i16x8_le_u)), // i16x8.le_u
                (53, Instr(Mnemonic.i16x8_ge_s)), // i16x8.ge_s
                (54, Instr(Mnemonic.i16x8_ge_u)), // i16x8.ge_u
                (55, Instr(Mnemonic.i32x4_eq)),   // i32x4.eq
                (56, Instr(Mnemonic.i32x4_ne)),   // i32x4.ne
                (57, Instr(Mnemonic.i32x4_lt_s)), // i32x4.lt_s
                (58, Instr(Mnemonic.i32x4_lt_u)), // i32x4.lt_u
                (59, Instr(Mnemonic.i32x4_gt_s)), // i32x4.gt_s
                (60, Instr(Mnemonic.i32x4_gt_u)), // i32x4.gt_u
                (61, Instr(Mnemonic.i32x4_le_s)), // i32x4.le_s
                (62, Instr(Mnemonic.i32x4_le_u)), // i32x4.le_u
                (63, Instr(Mnemonic.i32x4_ge_s)), // i32x4.ge_s
                (64, Instr(Mnemonic.i32x4_ge_u)), // i32x4.ge_u
                (214, Instr(Mnemonic.i64x2_eq)), // i64x2.eq
                (215, Instr(Mnemonic.i64x2_ne)), // i64x2.ne
                (216, Instr(Mnemonic.i64x2_lt_s)), // i64x2.lt_s
                (217, Instr(Mnemonic.i64x2_gt_s)), // i64x2.gt_s
                (218, Instr(Mnemonic.i64x2_le_s)), // i64x2.le_s
                (219, Instr(Mnemonic.i64x2_ge_s)), // i64x2.ge_s
                (65, Instr(Mnemonic.f32x4_eq)), // f32x4.eq
                (66, Instr(Mnemonic.f32x4_ne)), // f32x4.ne
                (67, Instr(Mnemonic.f32x4_lt)), // f32x4.lt
                (68, Instr(Mnemonic.f32x4_gt)), // f32x4.gt
                (69, Instr(Mnemonic.f32x4_le)), // f32x4.le
                (70, Instr(Mnemonic.f32x4_ge)), // f32x4.ge
                (71, Instr(Mnemonic.f64x2_eq)), // f64x2.eq
                (72, Instr(Mnemonic.f64x2_ne)), // f64x2.ne
                (73, Instr(Mnemonic.f64x2_lt)), // f64x2.lt
                (74, Instr(Mnemonic.f64x2_gt)), // f64x2.gt
                (75, Instr(Mnemonic.f64x2_le)), // f64x2.le
                (76, Instr(Mnemonic.f64x2_ge)), // f64x2.ge
                (77, Instr(Mnemonic.v128_not)), // v128.not
                (78, Instr(Mnemonic.v128_and)), // v128.and
                (79, Instr(Mnemonic.v128_andnot)), // v128.andnot
                (80, Instr(Mnemonic.v128_or)), // v128.or
                (81, Instr(Mnemonic.v128_xor)), // v128.xor
                (82, Instr(Mnemonic.v128_bitselect)), // v128.bitselect
                (83, Instr(Mnemonic.v128_any_true)), // v128.any_true
                (96, Instr(Mnemonic.i8x16_abs)), // i8x16.abs
                (97, Instr(Mnemonic.i8x16_neg)), // i8x16.neg
                (98, Instr(Mnemonic.i8x16_popcnt)), // i8x16.popcnt
                (99, Instr(Mnemonic.i8x16_all_true)), // i8x16.all_true
                (100, Instr(Mnemonic.i8x16_bitmask)), // i8x16.bitmask
                (101, Instr(Mnemonic.i8x16_narrow_i16x8_s)), // i8x16.narrow_i16x8_s
                (102, Instr(Mnemonic.i8x16_narrow_i16x8_u)), // i8x16.narrow_i16x8_u
                (107, Instr(Mnemonic.i8x16_shl)), // i8x16.shl
                (108, Instr(Mnemonic.i8x16_shr_s)), // i8x16.shr_s
                (109, Instr(Mnemonic.i8x16_shr_u)), // i8x16.shr_u
                (110, Instr(Mnemonic.i8x16_add)), // i8x16.add
                (111, Instr(Mnemonic.i8x16_add_sat_s)), // i8x16.add_sat_s
                (112, Instr(Mnemonic.i8x16_add_sat_u)), // i8x16.add_sat_u
                (113, Instr(Mnemonic.i8x16_sub)), // i8x16.sub
                (114, Instr(Mnemonic.i8x16_sub_sat_s)), // i8x16.sub_sat_s
                (115, Instr(Mnemonic.i8x16_sub_sat_u)), // i8x16.sub_sat_u
                (118, Instr(Mnemonic.i8x16_min_s)), // i8x16.min_s
                (119, Instr(Mnemonic.i8x16_min_u)), // i8x16.min_u
                (120, Instr(Mnemonic.i8x16_max_s)), // i8x16.max_s
                (121, Instr(Mnemonic.i8x16_max_u)), // i8x16.max_u
                (123, Instr(Mnemonic.i8x16_avgr_u)), // i8x16.avgr_u
                (124, Instr(Mnemonic.i16x8_extadd_pairwise_i8x16_s)), // i16x8.extadd_pairwise_i8x16_s
                (125, Instr(Mnemonic.i16x8_extadd_pairwise_i8x16_u)), // i16x8.extadd_pairwise_i8x16_u
                (128, Instr(Mnemonic.i16x8_abs)), // i16x8.abs
                (129, Instr(Mnemonic.i16x8_neg)), // i16x8.neg
                (130, Instr(Mnemonic.i16x8_q15mulr_sat_s)), // i16x8.q15mulr_sat_s
                (131, Instr(Mnemonic.i16x8_all_true)), // i16x8.all_true
                (132, Instr(Mnemonic.i16x8_bitmask)), // i16x8.bitmask
                (133, Instr(Mnemonic.i16x8_narrow_i32x4_s)), // i16x8.narrow_i32x4_s
                (134, Instr(Mnemonic.i16x8_narrow_i32x4_u)), // i16x8.narrow_i32x4_u
                (135, Instr(Mnemonic.i16x8_extend_low_i8x16_s)), // i16x8.extend_low_i8x16_s
                (136, Instr(Mnemonic.i16x8_extend_high_i8x16_s)), // i16x8.extend_high_i8x16_s
                (137, Instr(Mnemonic.i16x8_extend_low_i8x16_u)), // i16x8.extend_low_i8x16_u
                (138, Instr(Mnemonic.i16x8_extend_high_i8x16_u)), // i16x8.extend_high_i8x16_u
                (139, Instr(Mnemonic.i16x8_shl)), // i16x8.shl
                (140, Instr(Mnemonic.i16x8_shr_s)), // i16x8.shr_s
                (141, Instr(Mnemonic.i16x8_shr_u)), // i16x8.shr_u
                (142, Instr(Mnemonic.i16x8_add)), // i16x8.add
                (143, Instr(Mnemonic.i16x8_add_sat_s)), // i16x8.add_sat_s
                (144, Instr(Mnemonic.i16x8_add_sat_u)), // i16x8.add_sat_u
                (145, Instr(Mnemonic.i16x8_sub)), // i16x8.sub
                (146, Instr(Mnemonic.i16x8_sub_sat_s)), // i16x8.sub_sat_s
                (147, Instr(Mnemonic.i16x8_sub_sat_u)), // i16x8.sub_sat_u
                (149, Instr(Mnemonic.i16x8_mul)), // i16x8.mul
                (150, Instr(Mnemonic.i16x8_min_s)), // i16x8.min_s
                (151, Instr(Mnemonic.i16x8_min_u)), // i16x8.min_u
                (152, Instr(Mnemonic.i16x8_max_s)), // i16x8.max_s
                (153, Instr(Mnemonic.i16x8_max_u)), // i16x8.max_u
                (155, Instr(Mnemonic.i16x8_avgr_u)), // i16x8.avgr_u
                (156, Instr(Mnemonic.i16x8_extmul_low_i8x16_s)), // i16x8.extmul_low_i8x16_s
                (157, Instr(Mnemonic.i16x8_extmul_high_i8x16_s)), // i16x8.extmul_high_i8x16_s
                (158, Instr(Mnemonic.i16x8_extmul_low_i8x16_u)), // i16x8.extmul_low_i8x16_u
                (159, Instr(Mnemonic.i16x8_extmul_high_i8x16_u)), // i16x8.extmul_high_i8x16_u
                (126, Instr(Mnemonic.i32x4_extadd_pairwise_i16x8_s)), // i32x4.extadd_pairwise_i16x8_s
                (127, Instr(Mnemonic.i32x4_extadd_pairwise_i16x8_u)), // i32x4.extadd_pairwise_i16x8_u
                (160, Instr(Mnemonic.i32x4_abs)), // i32x4.abs
                (161, Instr(Mnemonic.i32x4_neg)), // i32x4.neg
                (163, Instr(Mnemonic.i32x4_all_true)), // i32x4.all_true
                (164, Instr(Mnemonic.i32x4_bitmask)), // i32x4.bitmask
                (167, Instr(Mnemonic.i32x4_extend_low_i16x8_s)), // i32x4.extend_low_i16x8_s
                (168, Instr(Mnemonic.i32x4_extend_high_i16x8_s)), // i32x4.extend_high_i16x8_s
                (169, Instr(Mnemonic.i32x4_extend_low_i16x8_u)), // i32x4.extend_low_i16x8_u
                (170, Instr(Mnemonic.i32x4_extend_high_i16x8_u)), // i32x4.extend_high_i16x8_u
                (171, Instr(Mnemonic.i32x4_shl)), // i32x4.shl
                (172, Instr(Mnemonic.i32x4_shr_s)), // i32x4.shr_s
                (173, Instr(Mnemonic.i32x4_shr_u)), // i32x4.shr_u
                (174, Instr(Mnemonic.i32x4_add)), // i32x4.add
                (177, Instr(Mnemonic.i32x4_sub)), // i32x4.sub
                (181, Instr(Mnemonic.i32x4_mul)), // i32x4.mul
                (182, Instr(Mnemonic.i32x4_min_s)), // i32x4.min_s
                (183, Instr(Mnemonic.i32x4_min_u)), // i32x4.min_u
                (184, Instr(Mnemonic.i32x4_max_s)), // i32x4.max_s
                (185, Instr(Mnemonic.i32x4_max_u)), // i32x4.max_u
                (186, Instr(Mnemonic.i32x4_dot_i16x8_s)), // i32x4.dot_i16x8_s
                (188, Instr(Mnemonic.i32x4_extmul_low_i16x8_s)), // i32x4.extmul_low_i16x8_s
                (189, Instr(Mnemonic.i32x4_extmul_high_i16x8_s)), // i32x4.extmul_high_i16x8_s
                (190, Instr(Mnemonic.i32x4_extmul_low_i16x8_u)), // i32x4.extmul_low_i16x8_u
                (191, Instr(Mnemonic.i32x4_extmul_high_i16x8_u)), // i32x4.extmul_high_i16x8_u
                (192, Instr(Mnemonic.i64x2_abs)), // i64x2.abs
                (193, Instr(Mnemonic.i64x2_neg)), // i64x2.neg
                (195, Instr(Mnemonic.i64x2_all_true)), // i64x2.all_true
                (196, Instr(Mnemonic.i64x2_bitmask)), // i64x2.bitmask
                (199, Instr(Mnemonic.i64x2_extend_low_i32x4_s)), // i64x2.extend_low_i32x4_s
                (200, Instr(Mnemonic.i64x2_extend_high_i32x4_s)), // i64x2.extend_high_i32x4_s
                (201, Instr(Mnemonic.i64x2_extend_low_i32x4_u)), // i64x2.extend_low_i32x4_u
                (202, Instr(Mnemonic.i64x2_extend_high_i32x4_u)), // i64x2.extend_high_i32x4_u
                (203, Instr(Mnemonic.i64x2_shl)), // i64x2.shl
                (204, Instr(Mnemonic.i64x2_shr_s)), // i64x2.shr_s
                (205, Instr(Mnemonic.i64x2_shr_u)), // i64x2.shr_u
                (206, Instr(Mnemonic.i64x2_add)), // i64x2.add
                (209, Instr(Mnemonic.i64x2_sub)), // i64x2.sub
                (213, Instr(Mnemonic.i64x2_mul)), // i64x2.mul
                (220, Instr(Mnemonic.i64x2_extmul_low_i32x4_s)), // i64x2.extmul_low_i32x4_s
                (221, Instr(Mnemonic.i64x2_extmul_high_i32x4_s)), // i64x2.extmul_high_i32x4_s
                (222, Instr(Mnemonic.i64x2_extmul_low_i32x4_u)), // i64x2.extmul_low_i32x4_u
                (223, Instr(Mnemonic.i64x2_extmul_high_i32x4_u)), // i64x2.extmul_high_i32x4_u
                (103, Instr(Mnemonic.f32x4_ceil)), // f32x4.ceil
                (104, Instr(Mnemonic.f32x4_floor)), // f32x4.floor
                (105, Instr(Mnemonic.f32x4_trunc)), // f32x4.trunc
                (106, Instr(Mnemonic.f32x4_nearest)), // f32x4.nearest
                (224, Instr(Mnemonic.f32x4_abs)), // f32x4.abs
                (225, Instr(Mnemonic.f32x4_neg)), // f32x4.neg
                (227, Instr(Mnemonic.f32x4_sqrt)), // f32x4.sqrt
                (228, Instr(Mnemonic.f32x4_add)), // f32x4.add
                (229, Instr(Mnemonic.f32x4_sub)), // f32x4.sub
                (230, Instr(Mnemonic.f32x4_mul)), // f32x4.mul
                (231, Instr(Mnemonic.f32x4_div)), // f32x4.div
                (232, Instr(Mnemonic.f32x4_min)), // f32x4.min
                (233, Instr(Mnemonic.f32x4_max)), // f32x4.max
                (234, Instr(Mnemonic.f32x4_pmin)), // f32x4.pmin
                (235, Instr(Mnemonic.f32x4_pmax)), // f32x4.pmax
                (116, Instr(Mnemonic.f64x2_ceil)), // f64x2.ceil
                (117, Instr(Mnemonic.f64x2_floor)), // f64x2.floor
                (122, Instr(Mnemonic.f64x2_trunc)), // f64x2.trunc
                (148, Instr(Mnemonic.f64x2_nearest)), // f64x2.nearest
                (236, Instr(Mnemonic.f64x2_abs)), // f64x2.abs
                (237, Instr(Mnemonic.f64x2_neg)), // f64x2.neg
                (239, Instr(Mnemonic.f64x2_sqrt)), // f64x2.sqrt
                (240, Instr(Mnemonic.f64x2_add)), // f64x2.add
                (241, Instr(Mnemonic.f64x2_sub)), // f64x2.sub
                (242, Instr(Mnemonic.f64x2_mul)), // f64x2.mul
                (243, Instr(Mnemonic.f64x2_div)), // f64x2.div
                (244, Instr(Mnemonic.f64x2_min)), // f64x2.min
                (245, Instr(Mnemonic.f64x2_max)), // f64x2.max
                (246, Instr(Mnemonic.f64x2_pmin)), // f64x2.pmin
                (247, Instr(Mnemonic.f64x2_pmax)), // f64x2.pmax
                (248, Instr(Mnemonic.i32x4_trunc_sat_f32x4_s)), // i32x4.trunc_sat_f32x4_s
                (249, Instr(Mnemonic.i32x4_trunc_sat_f32x4_u)), // i32x4.trunc_sat_f32x4_u
                (250, Instr(Mnemonic.f32x4_convert_i32x4_s)), // f32x4.convert_i32x4_s
                (251, Instr(Mnemonic.f32x4_convert_i32x4_u)), // f32x4.convert_i32x4_u
                (252, Instr(Mnemonic.i32x4_trunc_sat_f64x2_s_zero)), // i32x4.trunc_sat_f64x2_s_zero
                (253, Instr(Mnemonic.i32x4_trunc_sat_f64x2_u_zero)), // i32x4.trunc_sat_f64x2_u_zero
                (254, Instr(Mnemonic.f64x2_convert_low_i32x4_s)), // f64x2.convert_low_i32x4_s
                (255, Instr(Mnemonic.f64x2_convert_low_i32x4_u)), // f64x2.convert_low_i32x4_u
                (94, Instr(Mnemonic.f32x4_demote_f64x2_zero)), // f32x4.demote_f64x2_zero
                (95, Instr(Mnemonic.f64x2_promote_low_f32x4)), // f64x2.promote_low_f32x4
                (256, Instr(Mnemonic.i16x8_relaxed_swizzle)), // i16x8.relaxed_swizzle
                (257, Instr(Mnemonic.i32x4_relaxed_trunc_f32x4_s)), // i32x4.relaxed_trunc_f32x4_s
                (258, Instr(Mnemonic.i32x4_relaxed_trunc_f32x4_u)), // i32x4.relaxed_trunc_f32x4_u
                (259, Instr(Mnemonic.i32x4_relaxed_trunc_f32x4_s_zero)), // i32x4.relaxed_trunc_f32x4_s_zero
                (260, Instr(Mnemonic.i32x4_relaxed_trunc_f32x4_u_zero)), // i32x4.relaxed_trunc_f32x4_u_zero
                (261, Instr(Mnemonic.f32x4_relaxed_madd)), // f32x4.relaxed_madd
                (262, Instr(Mnemonic.f32x4_relaxed_nmadd)), // f32x4.relaxed_nmadd
                (263, Instr(Mnemonic.f64x2_relaxed_madd)), // f64x2.relaxed_madd
                (264, Instr(Mnemonic.f64x2_relaxed_nmadd)), // f64x2.relaxed_nmadd
                (265, Instr(Mnemonic.i8x16_relaxed_laneselect)), // i8x16.relaxed_laneselect
                (266, Instr(Mnemonic.i16x8_relaxed_laneselect)), // i16x8.relaxed_laneselect
                (267, Instr(Mnemonic.i32x4_relaxed_laneselect)), // i32x4.relaxed_laneselect
                (268, Instr(Mnemonic.i64x2_relaxed_laneselect)), // i64x2.relaxed_laneselect
                (269, Instr(Mnemonic.f32x4_relaxed_min)), // f32x4.relaxed_min
                (270, Instr(Mnemonic.f32x4_relaxed_max)), // f32x4.relaxed_max
                (271, Instr(Mnemonic.f64x2_relaxed_min)), // f64x2.relaxed_min
                (272, Instr(Mnemonic.f64x2_relaxed_max)), // f64x2.relaxed_max
                (273, Instr(Mnemonic.i16x8_relaxed_q15mulr_s)), // i16x8.relaxed_q15mulr_s
                (274, Instr(Mnemonic.i16x8_relaxed_dot_i8x16_i7x16_s)), // i16x8.relaxed_dot_i8x16_i7x16_s
                (275, Instr(Mnemonic.i16x8_relaxed_dot_i8x16_i7x16_add_s))); // i16x8.relaxed_dot_i8x16_i7x16_add_s

            decoders = new Decoder[256] {
                // 00
                Instr(Mnemonic.unreachable),
                Instr(Mnemonic.nop),
                Instr(Mnemonic.block, b),
                Instr(Mnemonic.loop, b),

                Instr(Mnemonic.@if, b),
                Instr(Mnemonic.@else),
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                Instr(Mnemonic.end),

                Instr(Mnemonic.br, labelidx),
                Instr(Mnemonic.br_if, labelidx),
                Instr(Mnemonic.br_table, T),
                Instr(Mnemonic.@return),

                // 10
                Instr(Mnemonic.call, u32),
                Instr(Mnemonic.call_indirect, u32, u32),
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                Instr(Mnemonic.drop),
                Instr(Mnemonic.select),

                Instr(Mnemonic.select, vtypes),
                invalid,
                invalid,
                Instr(Mnemonic.try_table, b, catch_vec),

                // 20
                Instr(Mnemonic.get_local, u32),
                Instr(Mnemonic.set_local, u32),
                Instr(Mnemonic.tee_local, u32),
                Instr(Mnemonic.get_global, u32),

                Instr(Mnemonic.set_global, u32),
                Instr(Mnemonic.table_get, u32),
                Instr(Mnemonic.table_set, u32),
                invalid,

                Instr(Mnemonic.i32_load, m(PrimitiveType.Word32)),
                Instr(Mnemonic.i64_load, m(PrimitiveType.Word64)),
                Instr(Mnemonic.f32_load, m(PrimitiveType.Real32)),
                Instr(Mnemonic.f64_load, m(PrimitiveType.Real64)),

                Instr(Mnemonic.i32_load8_s, m(PrimitiveType.Int8)),
                Instr(Mnemonic.i32_load8_u, m(PrimitiveType.Byte)),
                Instr(Mnemonic.i32_load16_s, m(PrimitiveType.Int16)),
                Instr(Mnemonic.i32_load16_u, m(PrimitiveType.Word16)),

                // 30
                Instr(Mnemonic.i64_load8_s, m(PrimitiveType.Int8)),
                Instr(Mnemonic.i64_load8_u, m(PrimitiveType.Byte)),
                Instr(Mnemonic.i64_load16_s, m(PrimitiveType.Int16)),
                Instr(Mnemonic.i64_load16_u, m(PrimitiveType.Word16)),

                Instr(Mnemonic.i64_load32_s, m(PrimitiveType.Int32)),
                Instr(Mnemonic.i64_load32_u, m(PrimitiveType.Word32)),
                Instr(Mnemonic.i32_store, m(PrimitiveType.Word32)),
                Instr(Mnemonic.i64_store, m(PrimitiveType.Word64)),

                Instr(Mnemonic.f32_store, m(PrimitiveType.Word32)),
                Instr(Mnemonic.f64_store, m(PrimitiveType.Word64)),
                Instr(Mnemonic.i32_store8, m(PrimitiveType.Byte)),
                Instr(Mnemonic.i32_store16, m(PrimitiveType.Word16)),

                Instr(Mnemonic.i64_store8, m(PrimitiveType.Byte)),
                Instr(Mnemonic.i64_store16, m(PrimitiveType.Word16)),
                Instr(Mnemonic.i64_store32, m(PrimitiveType.Word32)),
                Instr(Mnemonic.current_memory, i32),    //$ expect 0 in byte stream

                // 40
                Instr(Mnemonic.grow_memory, i32),       //$ expect 0 in byte stream
                Instr(Mnemonic.i32_const, i32),
                Instr(Mnemonic.i64_const, i64),
                Instr(Mnemonic.f32_const, f32),

                Instr(Mnemonic.f64_const, f64),
                Instr(Mnemonic.i32_eqz),
                Instr(Mnemonic.i32_eq),
                Instr(Mnemonic.i32_ne),

                Instr(Mnemonic.i32_lt_s),
                Instr(Mnemonic.i32_lt_u),
                Instr(Mnemonic.i32_gt_s),
                Instr(Mnemonic.i32_gt_u),

                Instr(Mnemonic.i32_le_s),
                Instr(Mnemonic.i32_le_u),
                Instr(Mnemonic.i32_ge_s),
                Instr(Mnemonic.i32_ge_u),

                // 50
                Instr(Mnemonic.i64_eqz),
                Instr(Mnemonic.i64_eq),
                Instr(Mnemonic.i64_ne),
                Instr(Mnemonic.i64_lt_s),

                Instr(Mnemonic.i64_lt_u),
                Instr(Mnemonic.i64_gt_s),
                Instr(Mnemonic.i64_gt_u),
                Instr(Mnemonic.i64_le_s),

                Instr(Mnemonic.i64_le_u),
                Instr(Mnemonic.i64_ge_s),
                Instr(Mnemonic.i64_ge_u),
                Instr(Mnemonic.f32_eq),

                Instr(Mnemonic.f32_ne),
                Instr(Mnemonic.f32_lt),
                Instr(Mnemonic.f32_gt),
                Instr(Mnemonic.f32_le),

                // 60
                Instr(Mnemonic.f32_ge),
                Instr(Mnemonic.f64_eq),
                Instr(Mnemonic.f64_ne),
                Instr(Mnemonic.f64_lt),

                Instr(Mnemonic.f64_gt),
                Instr(Mnemonic.f64_le),
                Instr(Mnemonic.f64_ge),
                Instr(Mnemonic.i32_clz),

                Instr(Mnemonic.i32_ctz),
                Instr(Mnemonic.i32_popcnt),
                Instr(Mnemonic.i32_add),
                Instr(Mnemonic.i32_sub),

                Instr(Mnemonic.i32_mul),
                Instr(Mnemonic.i32_div_s),
                Instr(Mnemonic.i32_div_u),
                Instr(Mnemonic.i32_rem_s),

                // 70
                Instr(Mnemonic.i32_rem_u),
                Instr(Mnemonic.i32_and),
                Instr(Mnemonic.i32_or),
                Instr(Mnemonic.i32_xor),

                Instr(Mnemonic.i32_shl),
                Instr(Mnemonic.i32_shr_s),
                Instr(Mnemonic.i32_shr_u),
                Instr(Mnemonic.i32_rotl),

                Instr(Mnemonic.i32_rotr),
                Instr(Mnemonic.i64_clz),
                Instr(Mnemonic.i64_ctz),
                Instr(Mnemonic.i64_popcnt),

                Instr(Mnemonic.i64_add),
                Instr(Mnemonic.i64_sub),
                Instr(Mnemonic.i64_mul),
                Instr(Mnemonic.i64_div_s),

                // 80
                Instr(Mnemonic.i64_div_u),
                Instr(Mnemonic.i64_rem_s),
                Instr(Mnemonic.i64_rem_u),
                Instr(Mnemonic.i64_and),

                Instr(Mnemonic.i64_or),
                Instr(Mnemonic.i64_xor),
                Instr(Mnemonic.i64_shl),
                Instr(Mnemonic.i64_shr_s),

                Instr(Mnemonic.i64_shr_u),
                Instr(Mnemonic.i64_rotl),
                Instr(Mnemonic.i64_rotr),
                Instr(Mnemonic.f32_abs),

                Instr(Mnemonic.f32_neg),
                Instr(Mnemonic.f32_ceil),
                Instr(Mnemonic.f32_floor),
                Instr(Mnemonic.f32_trunc),

                // 90
                Instr(Mnemonic.f32_nearest),
                Instr(Mnemonic.f32_sqrt),
                Instr(Mnemonic.f32_add),
                Instr(Mnemonic.f32_sub),

                Instr(Mnemonic.f32_mul),
                Instr(Mnemonic.f32_div),
                Instr(Mnemonic.f32_min),
                Instr(Mnemonic.f32_max),

                Instr(Mnemonic.f32_copysign),
                Instr(Mnemonic.f64_abs),
                Instr(Mnemonic.f64_neg),
                Instr(Mnemonic.f64_ceil),

                Instr(Mnemonic.f64_floor),
                Instr(Mnemonic.f64_trunc),
                Instr(Mnemonic.f64_nearest),
                Instr(Mnemonic.f64_sqrt),

                // A0
                Instr(Mnemonic.f64_add),
                Instr(Mnemonic.f64_sub),
                Instr(Mnemonic.f64_mul),
                Instr(Mnemonic.f64_div),

                Instr(Mnemonic.f64_min),
                Instr(Mnemonic.f64_max),
                Instr(Mnemonic.f64_copysign),
                Instr(Mnemonic.i32_wrap_i64),

                Instr(Mnemonic.i32_trunc_s_f32),
                Instr(Mnemonic.i32_trunc_u_f32),
                Instr(Mnemonic.i32_trunc_s_f64),
                Instr(Mnemonic.i32_trunc_u_f64),

                Instr(Mnemonic.i64_extend_s_i32),
                Instr(Mnemonic.i64_extend_u_i32),
                Instr(Mnemonic.i64_trunc_s_f32),
                Instr(Mnemonic.i64_trunc_u_f32),

                // B0
                Instr(Mnemonic.i64_trunc_s_f64),
                Instr(Mnemonic.i64_trunc_u_f64),
                Instr(Mnemonic.f32_convert_s_i32),
                Instr(Mnemonic.f32_convert_u_i32),

                Instr(Mnemonic.f32_convert_s_i64),
                Instr(Mnemonic.f32_convert_u_i64),
                Instr(Mnemonic.f32_demote_f64),
                Instr(Mnemonic.f64_convert_s_i32),

                Instr(Mnemonic.f64_convert_u_i32),
                Instr(Mnemonic.f64_convert_s_i64),
                Instr(Mnemonic.f64_convert_u_i64),
                Instr(Mnemonic.f64_promote_f32),

                Instr(Mnemonic.i32_reinterpret_f32),
                Instr(Mnemonic.i64_reinterpret_f64),
                Instr(Mnemonic.f32_reinterpret_i32),
                Instr(Mnemonic.f64_reinterpret_i64),

                // C0
                Instr(Mnemonic.i32_extend8_s  ),
                Instr(Mnemonic.i32_extend16_s ),
                Instr(Mnemonic.i64_extend8_s  ),
                Instr(Mnemonic.i64_extend16_s ),

                Instr(Mnemonic.i64_extend32_s ),
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
                Instr(Mnemonic.ref_null, heaptype), // 0xD0 𝑡:heaptype ⇒ ref.null 𝑡
                Instr(Mnemonic.ref_is_null),        // 0xD1 ⇒ ref.is_null
                Instr(Mnemonic.ref_func, funcidx),  // 0xD2 𝑥:funcidx ⇒ ref.func 𝑥
                Instr(Mnemonic.ref_eq),             // 0xD3 ⇒ ref.eq

                Instr(Mnemonic.ref_as_non_null),    // 0xD4 ⇒ ref.as_non_null
                Instr(Mnemonic.br_on_null, labelidx),
                Instr(Mnemonic.br_on_non_null, labelidx),
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
                Next(fb_decoders),

                Next(fc_decoders),
                Next(fd_decoders),
                invalid,
                invalid,
        };
    }
    }
}
