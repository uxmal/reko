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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Arch.X86
{
    using Decoder = Decoder<X86Disassembler, Mnemonic, X86Instruction>;

    public partial class X86Disassembler
    {
        /// <summary>
        /// Decoders are used to pick apart the complex x86 instructions, which have many optional
        /// prefixes, segment overrides, and other warts accumulated over the decades.
        /// </summary>

        /// <summary>
        /// Decodes an instruction that has a different mnemonic when using a VEX prefix.
        /// </summary>
        public class VexInstructionDecoder : Decoder
        {
            private readonly Decoder legacy;
            private readonly Decoder vex;

            public VexInstructionDecoder(Decoder legacy, Decoder vex)
            {
                this.legacy = legacy;
                this.vex = vex;
            }

            public override X86Instruction Decode(uint op, X86Disassembler disasm)
            {
                var decoder = disasm.decodingContext.IsVex
                    ? this.vex
                    : this.legacy;
                return decoder.Decode(op, disasm);
            }
        }

        /// <summary>
        /// Decodes an instruction that has a different mnemonic when using an EVEX or VEX prefix.
        /// </summary>
        public class EvexInstructionDecoder : Decoder
        {
            private readonly Decoder legacy;
            private readonly Decoder vex;
            private readonly Decoder evex;

            public EvexInstructionDecoder(Decoder legacy, Decoder vex, Decoder evex)
            {
                this.legacy = legacy;
                this.vex = vex;
                this.evex = evex;
            }

            public override X86Instruction Decode(uint op, X86Disassembler disasm)
            {
                Decoder decoder;
                if (!disasm.decodingContext.IsVex)
                    decoder = legacy;
                else if (!disasm.decodingContext.IsEvex)
                    decoder = vex;
                else
                    decoder = evex;
                return decoder.Decode(op, disasm);
            }
        }

        public class VexLongDecoder : Decoder
        {
            private readonly Decoder[] decoders;

            public VexLongDecoder(params Decoder[] decoders)
            {
                this.decoders = decoders;
            }

            public override X86Instruction Decode(uint op, X86Disassembler disasm)
            {
                var decoder = decoders[disasm.decodingContext.VexLongCode];
                return decoder.Decode(op, disasm);
            }
        }

        /// <summary>
        /// Use this decoder when an instruction encoding is dependent on whether the processor
        /// is in 64-bit mode or not.
        /// </summary>
        public class Alternative64Decoder : Decoder
        {
            private readonly Decoder decoder32;
            private readonly Decoder decoder64;

            public Alternative64Decoder(Decoder decoder32, Decoder decoder64)
            {
                this.decoder32 = decoder32;
                this.decoder64 = decoder64;
            }

            public override X86Instruction Decode(uint op, X86Disassembler disasm)
            {
                var decoder = (disasm.defaultAddressWidth.BitSize == 64)
                    ? decoder64
                    : decoder32;
                return decoder.Decode(op, disasm);
            }
        }

        /// <summary>
        /// Decodes instructions whose meaning depends on whether REX prefixes
        /// are to be interpreted or not.
        /// </summary>
        public class Rex_or_InstructionDecoder : Decoder
        {
            private readonly Decoder[] decoders;

            public Rex_or_InstructionDecoder(Decoder[] decoders)
            {
                this.decoders = decoders;
            }

            public override X86Instruction Decode(uint op, X86Disassembler disasm)
            {
                bool isWide = (op & 0x8) != 0;
                disasm.decodingContext.RexPrefix = true;
                disasm.decodingContext.IsWide = isWide;
                disasm.decodingContext.RExtension = (int)(op & 4) << 1;
                disasm.decodingContext.XExtension = (int)(op & 2) << 2;
                disasm.decodingContext.BExtension = (int)(op & 1) << 3;
                if (isWide)
                {
                    var w64 = PrimitiveType.Word64;
                    disasm.decodingContext.dataWidth = w64;
                    disasm.decodingContext.iWidth = w64;
                }
                if (!disasm.TryReadByte(out var op2))
                    return disasm.CreateInvalidInstruction();
                return decoders[op2].Decode(op2, disasm);
            }
        }

        public class Rex2Decoder : Decoder
        {
            private static readonly Bitfield[] bfR = Bf((6, 1), (2, 1));
            private static readonly Bitfield[] bfX = Bf((5, 1), (1, 1));
            private static readonly Bitfield[] bfB = Bf((4, 1), (0, 1));

            private readonly Decoder[] map0decoders;
            private readonly Decoder[] map1decoders;

            public Rex2Decoder(Decoder[] map0decoders, Decoder[] map1decoders)
            {
                this.map0decoders = map0decoders;
                this.map1decoders = map1decoders;
            }

            public override X86Instruction Decode(uint rex2Prefix, X86Disassembler disasm)
            {
                if (!disasm.TryReadByte(out var rex2))
                    return disasm.CreateInvalidInstruction();
                if (!disasm.TryReadByte(out var op))
                    return disasm.CreateInvalidInstruction();

                var map = Bits.IsBitSet(rex2, 7)
                    ? map1decoders
                    : map0decoders;
                disasm.decodingContext.RexPrefix = true;
                disasm.decodingContext.RExtension = (int) Bitfield.ReadFields(bfR, rex2) << 3;
                disasm.decodingContext.XExtension = (int) Bitfield.ReadFields(bfX, rex2) << 3;
                disasm.decodingContext.BExtension = (int) Bitfield.ReadFields(bfB, rex2) << 3;

                if (Bits.IsBitSet(rex2, 3))
                {
                    // REX2.W
                    disasm.decodingContext.IsWide = true;
                    disasm.decodingContext.dataWidth = PrimitiveType.Word64;
                    disasm.decodingContext.iWidth = PrimitiveType.Word64;
                }
                return map[op].Decode(op, disasm);
            }
        }

        /// <summary>
        /// Decodes segment override prefixes.
        /// </summary>
        public class SegmentOverrideDecoder : Decoder
        {
            private readonly int seg;

            public SegmentOverrideDecoder(int seg)
            {
                this.seg = seg;
            }

            public override X86Instruction Decode(uint op, X86Disassembler disasm)
            {
                disasm.decodingContext.SegmentOverride = SegFromBits(seg);
                if (!disasm.TryReadByte(out var op2))
                    return disasm.CreateInvalidInstruction();
                return disasm.rootDecoders[op2].Decode(op2, disasm);
            }
        }

        /// <summary>
        /// Uses bits 3-5 of the ModRM byte to decode further.
        /// </summary>
        public class GroupDecoder : Decoder
        {
            private readonly Decoder[] group;
            private readonly Mutator<X86Disassembler>[] mutators;

            public GroupDecoder(Decoder[] groupDecoders, params Mutator<X86Disassembler>[] mutators)
            {
                this.group = groupDecoders;
                this.mutators = mutators;
            }

            public override X86Instruction Decode(uint op, X86Disassembler disasm)
            {
                if (!disasm.TryEnsureModRM(out byte modRm))
                    return disasm.CreateInvalidInstruction();
                if (mutators is not null)
                {
                    foreach (var m in this.mutators)
                    {
                        if (!m(op, disasm))
                            return disasm.CreateInvalidInstruction();
                    }
                }

                Decoder decoder = group[((modRm >> 3) & 0x07)];
                return decoder.Decode(op, disasm);
            }
        }

        /// <summary>
        /// Different decoding depending on whether the ModRM byte is a mem or a reg access.
        /// </summary>
        public class MemRegDecoder : Decoder
        {
            private readonly Decoder memDecoder;
            private readonly Decoder regDecoder;

            public MemRegDecoder(
                Decoder memDecoder,
                Decoder regDecoder)
            {
                this.memDecoder = memDecoder;
                this.regDecoder = regDecoder;
            }

            public override X86Instruction Decode(uint op, X86Disassembler disasm)
            {
                if (!disasm.TryEnsureModRM(out byte modRm))
                    return disasm.CreateInvalidInstruction();

                var decoder = ((modRm & 0xC0) == 0xC0)
                    ? regDecoder
                    : memDecoder;
                return decoder.Decode(op, disasm);
            }
        }

        /// <summary>
        /// Different decoding depending on whether the next byte has its high two bits set
        /// (corresponding to a ModRM byte reg access) or not (corresponding to a memory
        /// access).
        /// </summary>
        /// <remarks>
        /// This looks identical to <see cref="MemRegDecoder"/> but there is a subtle 
        /// difference. In this class we cannot assume the bits we are testing are 
        /// part of the modRm byte.
        /// </remarks>
        public class C0Decoder : Decoder
        {
            private readonly Decoder memDecoder;
            private readonly Decoder regDecoder;

            public C0Decoder(
                Decoder memDecoder,
                Decoder regDecoder)
            {
                this.memDecoder = memDecoder;
                this.regDecoder = regDecoder;
            }

            public override X86Instruction Decode(uint op, X86Disassembler disasm)
            {
                if (!disasm.rdr.TryPeekByte(0, out byte bNext))
                    return disasm.CreateInvalidInstruction();

                var decoder = ((bNext & 0xC0) == 0xC0)
                    ? regDecoder
                    : memDecoder;
                return decoder.Decode(op, disasm);
            }
        }

        /// <summary>
        /// If the the high 2 bits of the ModRM byte are set,
        /// use the array of register decoders for further discrimination
        /// using bits 0-2 of ModRM byte.
        /// </summary>
        public class Group7Decoder : Decoder
        {
            private readonly Decoder memDecoder;
            private readonly Decoder[] regDecoders;

            public Group7Decoder(
                Decoder memDecoder,
                params Decoder[] regDecoder)
            {
                this.memDecoder = memDecoder;
                this.regDecoders = regDecoder;
            }

            public override X86Instruction Decode(uint op, X86Disassembler disasm)
            {
                if (!disasm.TryEnsureModRM(out byte modRm))
                    return disasm.CreateInvalidInstruction();
                if ((modRm & 0xC0) == 0xC0)
                {
                    var i = modRm & 0x07;
                    if (i < regDecoders.Length)
                    {
                        return regDecoders[i].Decode(op, disasm);
                    }
                    else
                    {
                        return disasm.CreateInvalidInstruction();
                    }
                }
                else
                {
                    return memDecoder.Decode(op, disasm);
                }
            }
        }



        /// <summary>
        /// Decodes X86 FPU instructions, which are encoded in their own
        /// special way.
        /// </summary>
        public class X87Decoder : Decoder
        {
            private readonly Decoder[] fpuDecoders;

            public X87Decoder(Decoder[] fpuDecoders)
            {
                this.fpuDecoders = fpuDecoders;
            }

            public override X86Instruction Decode(uint op, X86Disassembler disasm)
            {
                if (!disasm.TryEnsureModRM(out byte modRM))
                    return disasm.CreateInvalidInstruction();
                Decoder decoder;
                uint iDecoder = (op & 0x07) * 0x48;
                if (modRM < 0xC0)
                {
                    decoder = fpuDecoders[iDecoder + ((modRM >> 3) & 0x07)];
                }
                else
                {
                    decoder = fpuDecoders[iDecoder + modRM - 0xB8];
                }
                return decoder.Decode(op, disasm);
            }
        }

        /// <summary>
        /// Reads an additional opcode byte and decodes it.
        /// </summary>
        public class AdditionalByteDecoder : Decoder
        {
            private readonly Decoder[] decoders;

            public AdditionalByteDecoder(Decoder[] decoders)
            {
                this.decoders = decoders;
            }

            public override X86Instruction Decode(uint op, X86Disassembler disasm)
            {
                if (!disasm.TryReadByte(out byte op2))
                    return disasm.CreateInvalidInstruction();
                return decoders[op2].Decode(op2, disasm);
            }
        }

        /// <summary>
        /// Decodes 2-byte VEX encoded instructions.
        /// </summary>
        public class VexDecoder2 : Decoder
        {
            private readonly Decoder[] decoders0F;

            public VexDecoder2(Decoder[] decoders0F)
            {
                this.decoders0F = decoders0F;
            }

            public override X86Instruction Decode(uint op, X86Disassembler disasm)
            {
                var ctx = disasm.decodingContext;
                if ((ctx.SizeOverridePrefix | ctx.F2Prefix | ctx.F3Prefix) ||
                    !disasm.TryReadByte(out byte op2))
                {
                    return disasm.CreateInvalidInstruction();
                }
                var r = (~op2 >>> 4) & 0x8;
                var vvvv = (~op2 >> 3) & 0xF;
                var pp = op2 & 3;
                ctx.RexPrefix = true;
                ctx.IsVex = true;
                ctx.VexRegister = (byte) vvvv;
                ctx.RExtension = r;
                ctx.VexLongCode = (byte)((op2 & 4) >> 2);
                ctx.F2Prefix = pp == 3;
                ctx.F3Prefix = pp == 2;
                ctx.SizeOverridePrefix = pp == 1;
                if (!disasm.TryReadByte(out op2) ||
                    op2 == 0x38 || op2 == 0x3A)
                {
                    return disasm.CreateInvalidInstruction();
                }
                TraceEvex(ctx, 1, op2);
                return decoders0F[op2].Decode(op2, disasm);
            }
        }

        /// <summary>
        /// Decodes 3-byte VEX encoded instructions.
        /// </summary>
        public class VexDecoder3 : Decoder
        {
            private readonly Decoder[] decoders0F;
            private readonly Decoder[] decoders0F38;
            private readonly Decoder[] decoders0F3A;

            public VexDecoder3(Decoder[] decoders0F, Decoder[] decoders0F38, Decoder[] decoders0F3A)
            {
                this.decoders0F = decoders0F;
                this.decoders0F38 = decoders0F38;
                this.decoders0F3A = decoders0F3A;
            }

            public override X86Instruction Decode(uint op, X86Disassembler disasm)
            {
                var ctx = disasm.decodingContext;
                if (ctx.RexPrefix || ctx.SizeOverridePrefix || ctx.F2Prefix || ctx.F3Prefix)
                    return disasm.CreateInvalidInstruction();
                if (!disasm.TryReadByte(out byte vex1))
                    return disasm.CreateInvalidInstruction();
                if (!disasm.TryReadByte(out byte vex2))
                    return disasm.CreateInvalidInstruction();
                var rxb = ~vex1 >> 5;
                var mmmmm = vex1 & 0x1F;
                var w = vex2 >> 7;
                var vvvv = (~vex2 >> 3) & 0xF;
                var pp = vex2 & 0x3;

                ctx.RexPrefix = true;
                ctx.IsVex = true;
                ctx.VexRegister = (byte) vvvv;
                ctx.VexLongCode = (byte)((vex2 & 4) >> 2);
                ctx.IsWide = w != 0;
                ctx.RExtension = (rxb & 4) << 1;
                ctx.XExtension = (rxb & 2) << 2;
                ctx.BExtension = (rxb & 1) << 3;
                ctx.F2Prefix = pp == 3;
                ctx.F3Prefix = pp == 2;
                ctx.SizeOverridePrefix = pp == 1;

                Decoder[] decoders;
                switch (mmmmm)
                {
                case 1: decoders = this.decoders0F; break;
                case 2: decoders = this.decoders0F38; break;
                case 3: decoders = this.decoders0F3A; break;
                default: return disasm.CreateInvalidInstruction();
                }
                if (!disasm.TryReadByte(out byte vex3))
                    return disasm.CreateInvalidInstruction();
                TraceEvex(ctx, mmmmm, vex3);
                return decoders[vex3].Decode(vex3, disasm);
            }
        }

        /// <summary>
        /// Decoder for EVEX-prefixed instructions.
        /// </summary>
        public class EvexDecoder : Decoder
        {
            private readonly Decoder[] decoders0F;
            private readonly Decoder[] decoders0F38;
            private readonly Decoder[] decoders0F3A;
            private readonly Decoder[] decodersLegacy;

            private static readonly Bitfield p0Reserved = new Bitfield(2, 2);
            private static readonly Bitfield p1Reserved = new Bitfield(2, 1);
            private static readonly Bitfield p1Vvvv = new Bitfield(3, 4);

            public EvexDecoder(Decoder[] legacy, Decoder[] decoders0F, Decoder[] decoders0F38, Decoder[] decoders0F3A)
            {
                this.decoders0F = decoders0F;
                this.decoders0F38 = decoders0F38;
                this.decoders0F3A = decoders0F3A;
                this.decodersLegacy = legacy;
            }

            public override X86Instruction Decode(uint op, X86Disassembler disasm)
            {
                // 62 must be the first byte. The presence of previous
                // prefixes is an error (according to Intel manual 2.6, vol 2A).
                var ctx = disasm.decodingContext;
                if (ctx.F2Prefix |
                    ctx.F3Prefix |
                    ctx.RexPrefix |
                    ctx.SizeOverridePrefix)
                    return disasm.CreateInvalidInstruction();
                // The EVEX prefix consists of a leading 0x62 byte, and three
                // packed payload bytes P0, P1, and P2.
                //$TODO: this is incomplete: there are many missing flags.
                if (!disasm.TryReadByte(out byte p0) ||
                    !disasm.TryReadByte(out byte p1) ||
                    !disasm.TryReadByte(out byte p2) ||
                    p0Reserved.Read(p0) != 0 ||
                    p1Reserved.Read(p1) != 1)
                {
                    return disasm.CreateInvalidInstruction();
                }
                var mm = p0 & 3;
                var rxb = ~p0 >> 5;
                var pp = p1 & 3;
                var w = p1 >> 7;
                var vvvv = p1Vvvv.Read(~(uint)p1);
                if (!Bits.IsBitSet(p2, 3))
                    vvvv |= 0x10;
                ctx.RexPrefix = true;
                ctx.IsVex = true;
                ctx.IsEvex = true;
                ctx.VexRegister = (byte) vvvv;
                ctx.VexLongCode = (byte)((p2 >> 5) & 3);
                ctx.OpMask = (byte)(p2 & 7);
                ctx.IsWide = w != 0;
                ctx.RExtension = (rxb & 4) << 1;
                ctx.XExtension = (rxb & 2) << 2;
                ctx.BExtension = (rxb & 1) << 3;
                ctx.EvexR = !Bits.IsBitSet(p0, 4);
                ctx.EvexX = !Bits.IsBitSet(p0, 6);
                ctx.EvexMergeMode = p2 >> 7;
                ctx.EvexBroadcast = (p2 & 0x10) != 0;
                ctx.F2Prefix = pp == 3;
                ctx.F3Prefix = pp == 2;
                ctx.SizeOverridePrefix = pp == 1;

                Decoder[] decoders;
                switch (mm)
                {
                case 2: decoders = decoders0F38; break;
                case 3: decoders = decoders0F3A; break;
                case 4: decoders = decodersLegacy; break;
                default: decoders = decoders0F; break;
                }
                if (!disasm.TryReadByte(out byte op2))
                    return disasm.CreateInvalidInstruction();
                TraceEvex(ctx, mm, op2);
                return decoders[op2].Decode(op2, disasm);
            }
        }

        public class F2PrefixDecoder : Decoder
        {
            private readonly Decoder[] rootDecoders;

            public F2PrefixDecoder(Decoder[] rootDecoders)
            {
                this.rootDecoders = rootDecoders;
            }

            public override X86Instruction Decode(uint op, X86Disassembler disasm)
            {
                disasm.decodingContext.F2Prefix = true;
                if (!disasm.TryReadByte(out byte op2))
                    return disasm.CreateInvalidInstruction();
                return rootDecoders[op2].Decode(op2, disasm);
            }
        }

        public class F3PrefixDecoder : Decoder
        {
            private readonly Decoder[] rootDecoders;

            public F3PrefixDecoder(Decoder[] rootDecoders)
            {
                this.rootDecoders = rootDecoders;
            }

            public override X86Instruction Decode(uint op, X86Disassembler disasm)
            {
                if (!disasm.rdr.TryPeekByte(0, out byte b))
                    return disasm.CreateInvalidInstruction();
                if (b == 0xC3)
                {
                    // rep ret idiom.
                    if (!disasm.TryReadByte(out byte opC3))
                        return disasm.CreateInvalidInstruction();
                    return rootDecoders[opC3].Decode(opC3, disasm);
                }
                disasm.decodingContext.F3Prefix = true;
                if (!disasm.TryReadByte(out byte op2))
                    return disasm.CreateInvalidInstruction();
                return rootDecoders[op2].Decode(op2, disasm);
            }
        }

        public class ChangeDataWidth : Decoder
        {
            private readonly Decoder[] rootDecoders;

            public ChangeDataWidth(Decoder[] rootDecoders)
            {
                this.rootDecoders = rootDecoders;
            }
            public override X86Instruction Decode(uint op, X86Disassembler disasm)
            {
                disasm.decodingContext.SizeOverridePrefix = true;
                disasm.decodingContext.dataWidth = (disasm.defaultDataWidth == PrimitiveType.Word16)
                    ? PrimitiveType.Word32
                    : PrimitiveType.Word16;
                disasm.decodingContext.iWidth = disasm.decodingContext.dataWidth;
                if (!disasm.TryReadByte(out byte op2))
                    return disasm.CreateInvalidInstruction();
                return rootDecoders[op2].Decode(op2, disasm);
            }
        }

        public class ChangeAddressWidth : Decoder
        {
            private readonly Decoder[] rootDecoders;

            public ChangeAddressWidth(Decoder[] rootDecoders)
            {
                this.rootDecoders = rootDecoders;
            }

            public override X86Instruction Decode(uint op, X86Disassembler disasm)
            {
                PrimitiveType addrWidth;
                if (disasm.decodingContext.addressWidth != disasm.defaultAddressWidth)
                {
                    addrWidth = disasm.defaultAddressWidth;
                }
                else
                {
                    if (disasm.defaultAddressWidth.BitSize == 16)
                        addrWidth = PrimitiveType.Word32;
                    else if (disasm.defaultAddressWidth.BitSize == 32)
                        addrWidth = PrimitiveType.Word16;
                    else
                        addrWidth = PrimitiveType.Word32;
                }
                disasm.decodingContext.addressWidth = addrWidth;
                if (!disasm.TryReadByte(out byte op2))
                    return disasm.CreateInvalidInstruction();
                return rootDecoders[op2].Decode(op2, disasm);
            }
        }

        /// <summary>
        /// Handles the complex 66, F2, and F3 prefixes for instructions in 
        /// the x86-64 instruction set.
        /// </summary>
        public class PrefixedDecoder : Decoder
        {
            private readonly Decoder decoderBase;
            private readonly Decoder decoderWide;
            private readonly Decoder decoder66;
            private readonly Decoder decoder66Wide;
            private readonly Decoder decoderF3;
            private readonly Decoder decoderF3Wide;
            private readonly Decoder decoderF2;
            private readonly Decoder decoderF2Wide;

            public PrefixedDecoder(
                Decoder? dec = null,
                Decoder? dec66 = null,
                Decoder? decF3 = null,
                Decoder? decF2 = null,
                Decoder? decWide = null,
                Decoder? dec66Wide = null,
                Decoder? decF3Wide = null,
                Decoder? decF2Wide = null)
            {
                this.decoderBase = dec ?? InstructionSet.s_invalid;
                this.decoderWide = decWide ?? decoderBase;
                this.decoder66 = dec66 ?? InstructionSet.s_invalid;
                this.decoder66Wide = dec66Wide ?? decoder66;
                this.decoderF3 = decF3 ?? InstructionSet.s_invalid;
                this.decoderF3Wide = decF3Wide ?? decoderF3;
                this.decoderF2 = decF2 ?? InstructionSet.s_invalid;
                this.decoderF2Wide = decF2Wide ?? decoderF2;
            }

            public override X86Instruction Decode(uint op, X86Disassembler dasm)
            {
                bool isWide = dasm.decodingContext.IsWide;

                if (dasm.decodingContext.F2Prefix)
                {
                    dasm.decodingContext.F2Prefix = false;
                    dasm.decodingContext.F3Prefix = false;
                    var instr = (isWide ? decoderF2Wide : decoderF2).Decode(op, dasm);
                    return instr;
                }
                else if (dasm.decodingContext.F3Prefix)
                {
                    dasm.decodingContext.F2Prefix = false;
                    dasm.decodingContext.F3Prefix = false;
                    var instr = (isWide ? decoderF3Wide : decoderF3).Decode(op, dasm);
                    return instr;
                }
                else if (dasm.decodingContext.SizeOverridePrefix)
                {
                    if (isWide)
                    {
                        dasm.decodingContext.dataWidth = PrimitiveType.Word64;
                        return decoder66Wide.Decode(op, dasm);
                    }
                    else
                    {
                        dasm.decodingContext.dataWidth = dasm.defaultDataWidth;
                        return decoder66.Decode(op, dasm);
                    }
                }
                else
                {
                    var instr = (isWide ? decoderWide : decoderBase).Decode(op, dasm);
                    return instr;
                }
            }
        }


        /// <summary>
        /// This decoder will use a different sub-decoder depending on the current
        /// address width.
        /// </summary>
        public class AddrWidthDecoder : Decoder
        {
            private readonly Decoder bit16;
            private readonly Decoder bit32;
            private readonly Decoder bit64;

            public AddrWidthDecoder(
                Decoder bit16,
                Decoder bit32,
                Decoder bit64)
            {
                this.bit16 = bit16;
                this.bit32 = bit32;
                this.bit64 = bit64;
            }

            public override X86Instruction Decode(uint op, X86Disassembler disasm)
            {
                return disasm.decodingContext.addressWidth.Size switch
                {
                    2 => bit16.Decode(op, disasm),
                    4 => bit32.Decode(op, disasm),
                    8 => bit64.Decode(op, disasm),
                    _ => disasm.CreateInvalidInstruction(),
                };
            }
        }

        /// <summary>
        /// This decoder will use a different sub-decoder depending on the current
        /// data width.
        /// </summary>
        public class DataWidthDecoder : Decoder
        {
            private readonly Decoder bit16;
            private readonly Decoder bit32;
            private readonly Decoder bit64;

            public DataWidthDecoder(
                Decoder bit16,
                Decoder bit32,
                Decoder bit64)
            {
                this.bit16 = bit16;
                this.bit32 = bit32;
                this.bit64 = bit64;
            }

            public override X86Instruction Decode(uint op, X86Disassembler disasm)
            {
                return disasm.decodingContext.dataWidth.Size switch
                {
                    2 => bit16.Decode(op, disasm),
                    4 => bit32.Decode(op, disasm),
                    8 => bit64.Decode(op, disasm),
                    _ => disasm.CreateInvalidInstruction(),
                };
            }
        }


        /// <summary>
        /// This hacky decoder will interpret old-school software emulated
        /// X87 instructions implemented as interrupts.
        /// </summary>
        public class InterruptDecoder : InstrDecoder<X86Disassembler, Mnemonic, X86Instruction>
        {
            public InterruptDecoder(Mnemonic mnemonic, Mutator<X86Disassembler> mutator) : base(InstrClass.Linear, mnemonic, mutator)
            {
            }

            public override X86Instruction Decode(uint op, X86Disassembler disasm)
            {
                var instr = base.Decode(op, disasm);
                if (!instr.IsValid)
                    return instr;
                if (disasm.Emulate8087)
                {
                    var imm = (Constant) disasm.decodingContext.ops[0];
                    var vector = imm.ToByte();
                    if (disasm.IsEmulated8087Vector(vector))
                    {
                        instr = disasm.RewriteEmulated8087Instruction(vector);
                    }
                }
                return instr ?? disasm.CreateInvalidInstruction();
            }
        }

        /// <summary>
        /// Decodes an instruction encoded in the ModRM byte.
        /// </summary>
        public class ModRmOpcodeDecoder : Decoder
        {
            private readonly Decoder[] decoders;

            public ModRmOpcodeDecoder(
                Decoder defaultDecoder,
                params (byte, Decoder) [] decoders)
            {
                this.decoders = new Decoder[256];
                foreach (var (b, decoder) in decoders)
                {
                    Debug.Assert(this.decoders[b] is null, $"Decoder {b:X2} has already a value!");
                    this.decoders[b] = decoder;
                }
                for (int i = 0; i < this.decoders.Length; ++i)
                {
                    if (this.decoders[i] is null)
                    {
                        this.decoders[i] = defaultDecoder;
                    }
                }
            }

            public override X86Instruction Decode(uint op, X86Disassembler disasm)
            {
                if (!disasm.TryEnsureModRM(out byte modRm))
                    return disasm.CreateInvalidInstruction();
                return this.decoders[modRm].Decode(op, disasm);
            }
        }

        /// <summary>
        /// Predicated decoder uses the last operand (an immediate)
        /// to further refine the opcode.
        /// </summary>
        public class PredicatedDecoder : Decoder
        {
            private readonly Mnemonic mnemonic;
            private readonly Mutator<X86Disassembler>[] mutators;
            private readonly Dictionary<byte, Mnemonic> predicateMnemonics;

            public PredicatedDecoder(
                Mnemonic mnemonic,
                Dictionary<byte, Mnemonic> predicateMnemonics,
                params Mutator<X86Disassembler>[] mutators)
            {
                this.mnemonic = mnemonic;
                this.predicateMnemonics = predicateMnemonics;
                this.mutators = mutators;
            }

            public override X86Instruction Decode(uint wInstr, X86Disassembler dasm)
            {
                DumpMaskedInstruction(wInstr, 0, this.mnemonic);
                foreach (var m in mutators)
                {
                    if (!m(wInstr, dasm))
                        return dasm.CreateInvalidInstruction();
                }
                var ops = dasm.decodingContext.ops;
                var predicateOpcode = ((Constant) ops[^1]).ToByte();
                if (predicateMnemonics.TryGetValue(predicateOpcode, out var mnemonic))
                {
                    ops.RemoveAt(ops.Count - 1);
                    return dasm.MakeInstruction(InstrClass.Linear, mnemonic);
                }
                else
                {
                    // Other decodings are reserved.
                    return dasm.CreateInvalidInstruction();
                }
            }
        }
    }
}
