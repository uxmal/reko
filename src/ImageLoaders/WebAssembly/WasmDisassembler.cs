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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

        private static bool i32(uint uInstr, WasmDisassembler dasm)
        {
            if (!dasm.rdr.TryReadVarInt64(out var i32))
                return false;
            dasm.ops.Add(ImmediateOperand.Word32((uint) i32));
            return true;
        }

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

        private static bool i64(uint uInstr, WasmDisassembler dasm)
        {
            if (!dasm.rdr.TryReadVarInt64(out var i64))
                return false;
            dasm.ops.Add(ImmediateOperand.Word64(i64));
            return true;
        }

        private static bool u64(uint uInstr, WasmDisassembler dasm)
        {
            if (!dasm.rdr.TryReadVarUInt64(out var u64))
                return false;
            dasm.ops.Add(ImmediateOperand.Word64(u64));
            return true;
        }

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

        private class NextDecoder : Decoder<WasmDisassembler, Mnemonic, WasmInstruction>
        {
            private readonly Decoder<WasmDisassembler, Mnemonic, WasmInstruction> next;

            public NextDecoder(Decoder<WasmDisassembler, Mnemonic, WasmInstruction> next)
            {
                this.next = next;
            }

            public override WasmInstruction Decode(uint wInstr, WasmDisassembler dasm)
            {
                if (!dasm.rdr.TryReadByte(out byte nextByte))
                    return dasm.CreateInvalidInstruction();
                return next.Decode(nextByte, dasm);
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

                (12, Instr(Mnemonic.table_init, u32,u32)),
                (13, Instr(Mnemonic.elem_drop, u32)),
                (14, Instr(Mnemonic.table_copy, u32, u32)),
                (15, Instr(Mnemonic.table_grow, u32)),
                (16, Instr(Mnemonic.table_size, u32)),
                (17, Instr(Mnemonic.table_fill, u32)));

            var fd_decoders = Sparse(0, 8, "  0xFD decoders", invalid,
                (0, Instr(Mnemonic.v128_load, m128)),
                (1, Instr(Mnemonic.v128_load8x8_s, m128)),
                (2, Instr(Mnemonic.v128_load8x8_u, m128)),
                (3, Instr(Mnemonic.v128_load16x4_s, m128)),
                (4, Instr(Mnemonic.v128_load16x4_u, m128)),
                (5, Instr(Mnemonic.v128_load32x2_s, m128)),
                (6, Instr(Mnemonic.v128_load32x2_u, m128)),
                (7, Instr(Mnemonic.v128_load8_splat, m128)),
                (8, Instr(Mnemonic.v128_load16_splat, m128)),
                (9, Instr(Mnemonic.v128_load32_splat, m128)),
                (10, Instr(Mnemonic.v128_load64_splat, m128)),
                (92, Instr(Mnemonic.v128_load32_zero, m128)),
                (93, Instr(Mnemonic.v128_load64_zero, m128)),
                (11, Instr(Mnemonic.v128_store, m128)),
                (84, Instr(Mnemonic.v128_load8_lane, m128,u32)),
                (85, Instr(Mnemonic.v128_load16_lane, m128,u32)),
                (86, Instr(Mnemonic.v128_load32_lane, m128,u32)),
                (87, Instr(Mnemonic.v128_load64_lane, m128,u32)),
                (88, Instr(Mnemonic.v128_store8_lane, m128,u32)),
                (89, Instr(Mnemonic.v128_store16_lane, m128,u32)),
                (90, Instr(Mnemonic.v128_store32_lane, m128,u32)),
                (91, Instr(Mnemonic.v128_store64_lane, m128,u32)));

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

                Instr(Mnemonic.br, u32),
                Instr(Mnemonic.br_if, u32),
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
                invalid,

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

                Next(fc_decoders),
                Next(fd_decoders),
                invalid,
                invalid,
        };
    }
    }
}
