#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Reko.Arch.X86
{
    public partial class X86Disassembler
    {
        /// <summary>
        /// Decoders are used to pick apart the complex x86 instructions, which have many optional
        /// prefixes, segment overrides, and other warts accumulated over the decades.
        /// </summary>
		public abstract class Decoder
        {
            public abstract bool Decode(X86Disassembler disasm, byte op);
        }

        /// <summary>
        /// Decodes a single instructions by executing an array of mutators.
        /// </summary>
        public class InstructionDecoder : Decoder
        {
            public readonly InstrClass iclass;
            public readonly Mnemonic mnemonic;       // mnemonic for the decoded instruction
            public readonly Mutator<X86Disassembler>[] mutators;  // mutators for decoding operands to this instruction

            public InstructionDecoder(Mnemonic mnemonic, InstrClass icl, params Mutator<X86Disassembler> [] mutators)
            {
                this.iclass = icl;
                this.mnemonic = mnemonic;
                this.mutators = mutators;
            }

            public override bool Decode(X86Disassembler disasm, byte op)
            {
                disasm.decodingContext.iclass = this.iclass;
                disasm.decodingContext.mnemonic = this.mnemonic;
                foreach (var m in mutators)
                {
                    if (!m(op, disasm))
                        return false;
                }
                return true;
            }

            public override string ToString()
            {
                return $"{iclass}:{mnemonic}";
            }
        }

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

            public override bool Decode(X86Disassembler disasm, byte op)
            {
                var decoder = disasm.decodingContext.IsVex
                    ? this.vex
                    : this.legacy;
                return decoder.Decode(disasm, op);
            }
        }

        public class VexLongDecoder : Decoder
        {
            private readonly Decoder notLongDecoder;
            private readonly Decoder longDecoder;

            public VexLongDecoder(Decoder notLongDecoder, Decoder longDecoder)
            {
                this.notLongDecoder = notLongDecoder;
                this.longDecoder = longDecoder;
            }

            public override bool Decode(X86Disassembler disasm, byte op)
            {
                var decoder = disasm.decodingContext.VexLong
                    ? longDecoder
                    : notLongDecoder;
                return decoder.Decode(disasm, op);
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

            public override bool Decode(X86Disassembler disasm, byte op)
            {
                var decoder = (disasm.defaultAddressWidth.BitSize == 64)
                    ? decoder64
                    : decoder32;
                return decoder.Decode(disasm, op);
            }
        }

        /// <summary>
        /// Decodes instructions whose meaning depends on whether REX prefixes
        /// are to be interpretd or not.
        /// </summary>
        public class Rex_or_InstructionDecoder : Decoder
        {
            private readonly Decoder[] decoders;

            public Rex_or_InstructionDecoder(Decoder[] decoders)
            {
                this.decoders = decoders;
            }

            public override bool Decode(X86Disassembler disasm, byte op)
            {
                disasm.decodingContext.RegisterExtensionPrefixByte = op;
                if (disasm.decodingContext.RegisterExtension.FlagWideValue)
                {
                    var w64 = PrimitiveType.Word64;
                    disasm.decodingContext.dataWidth = w64;
                    disasm.decodingContext.iWidth = w64;
                }
                if (!disasm.TryReadByte(out var op2))
                    return false;
                return decoders[op2].Decode(disasm, op2);
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

            public override bool Decode(X86Disassembler disasm, byte op)
            {
                disasm.decodingContext.SegmentOverride = SegFromBits(seg);
                if (!disasm.TryReadByte(out var op2))
                    return false;
                return disasm.rootDecoders[op2].Decode(disasm, op2);
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

            public override bool Decode(X86Disassembler disasm, byte op)
            {
                if (!disasm.TryEnsureModRM(out byte modRm))
                    return false;
                if (mutators != null)
                {
                    foreach (var m in this.mutators)
                    {
                        if (!m(op, disasm))
                            return false;
                    }
                }

                Decoder decoder = group[((modRm >> 3) & 0x07)];
                return decoder.Decode(disasm, op);
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

            public override bool Decode(X86Disassembler disasm, byte op)
            {
                if (!disasm.TryEnsureModRM(out byte modRm))
                    return false;
                var decoder = ((modRm & 0xC0) == 0xC0)
                    ? regDecoder
                    : memDecoder;
                return decoder.Decode(disasm, op);
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

            public override bool Decode(X86Disassembler disasm, byte op)
            {
                if (!disasm.TryEnsureModRM(out byte modRm))
                    return false;
                if ((modRm & 0xC0) == 0xC0)
                {
                    var i = modRm & 0x07;
                    if (i < regDecoders.Length)
                    {
                        return regDecoders[i].Decode(disasm, op);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return memDecoder.Decode(disasm, op);
                }
            }
        }

        /// <summary>
        /// Use this decoder to mark instructions for which no decoder has 
        /// been written yet.
        /// </summary>
        /// <remarks>
        /// The x86 instruction set is large and keeps growing....
        /// </remarks>
        public class NyiDecoder : Decoder
        {
            private readonly string message;

            public NyiDecoder(string message)
            {
                this.message = message;
            }
            public override bool Decode(X86Disassembler disasm, byte op)
            {
                disasm.NotYetImplemented(message);
                return false;
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

            public override bool Decode(X86Disassembler disasm, byte op)
            {
                if (!disasm.TryEnsureModRM(out byte modRM))
                    return false;
                Decoder decoder;
                int iDecoder = (op & 0x07) * 0x48;
                if (modRM < 0xC0)
                {
                    decoder = fpuDecoders[iDecoder + ((modRM >> 3) & 0x07)];
                }
                else
                {
                    decoder = fpuDecoders[iDecoder + modRM - 0xB8];
                }
                return decoder.Decode(disasm, op);
            }
        }

        /// <summary>
        /// Reads an additional opcode byte and decodes it.
        /// </summary>
        public class AdditionalByteDecoder : Decoder
        {
            private readonly Decoder[] decoders;

            public  AdditionalByteDecoder(Decoder[] decoders)
            {
                this.decoders = decoders;
            }

            public override bool Decode(X86Disassembler disasm, byte op)
            {
                if (!disasm.TryReadByte(out op))
                    return false;
                return decoders[op].Decode(disasm, op);
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

            public override bool Decode(X86Disassembler disasm, byte op)
            {
                var ctx = disasm.decodingContext;
                if (ctx.SizeOverridePrefix | ctx.F2Prefix | ctx.F3Prefix)
                    return false;
                if (!disasm.TryReadByte(out op))
                    return false;
                var r = (~op >> 5) & 4;
                var vvvv = (~op >> 3) & 0xF;
                var pp = op & 3;
                ctx.IsVex = true;
                ctx.VexRegister = (byte) vvvv;
                ctx.RegisterExtensionPrefixByte = (byte) r;
                ctx.VexLong = (op & 4) != 0;
                ctx.F2Prefix = pp == 3;
                ctx.F3Prefix = pp == 2;
                ctx.SizeOverridePrefix = pp == 1;
                if (!disasm.TryReadByte(out op))
                    return false;
                if (op == 0x38 || op == 0x3A)
                    return false;
                return decoders0F[op].Decode(disasm, op);
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

            public override bool Decode(X86Disassembler disasm, byte op)
            {
                var ctx = disasm.decodingContext;
                if (ctx.RegisterExtension.ByteValue != 0 || ctx.SizeOverridePrefix || ctx.F2Prefix || ctx.F3Prefix)
                    return false;
                if (!disasm.TryReadByte(out byte evex1))
                    return false;
                var rxb = evex1 >> 5;
                var mmmmm = evex1 & 0x1F;

                if (!disasm.TryReadByte(out byte evex2))
                    return false;
                var w = evex2 >> 7;
                var vvvv = (~evex2 >> 3) & 0xF;
                var pp = evex2 & 0x3;

                ctx.IsVex = true;
                ctx.VexRegister = (byte) vvvv;
                ctx.VexLong = (evex2 & 4) != 0;
                ctx.RegisterExtension.FlagWideValue = w != 0;
                ctx.RegisterExtension.FlagTargetModrmRegister = (rxb & 4) == 0;
                ctx.RegisterExtension.FlagTargetSIBIndex = (rxb & 2) == 0;
                ctx.RegisterExtension.FlagTargetModrmRegOrMem = (rxb & 1) == 0;

                ctx.F2Prefix = pp == 3;
                ctx.F3Prefix = pp == 2;
                ctx.SizeOverridePrefix = pp == 1;

                Decoder[] decoders;
                switch (mmmmm)
                {
                case 1: decoders = decoders0F; break;
                case 2: decoders = decoders0F38;  break;
                case 3: decoders = decoders0F3A; break;
                default: return false;
                }
                if (!disasm.TryReadByte(out op))
                    return false;
                return decoders[op].Decode(disasm, op);
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

            private static readonly Bitfield p0Reserved = new Bitfield(2, 2);
            private static readonly Bitfield p1Reserved = new Bitfield(2, 1);
            private static readonly Bitfield p1Vvvv = new Bitfield(3, 4);

            public EvexDecoder(Decoder[] decoders0F, Decoder[] decoders0F38, Decoder[] decoders0F3A)
            {
                this.decoders0F = decoders0F;
                this.decoders0F38 = decoders0F38;
                this.decoders0F3A = decoders0F3A;
            }

            public override bool Decode(X86Disassembler disasm, byte op)
            {
                // 62 must be the first byte. The presence of previous
                // prefixes is an error (according to Intel manual 2.6, vol 2A.
                var ctx = disasm.decodingContext;
                if (ctx.F2Prefix |
                    ctx.F3Prefix |
                    ctx.IsRegisterExtensionActive() |
                    ctx.SizeOverridePrefix)
                        return false;
                // The EVEX prefix consists of a leading 0x62 byte, and three
                // packed payload bytes P0, P1, and P2.
                //$TODO: this is incomplete: there are many missing flags.
                if (!disasm.TryReadByte(out byte p0)) return false;
                if (!disasm.TryReadByte(out byte p1)) return false;
                if (!disasm.TryReadByte(out byte p2)) return false;
                if (p0Reserved.Read(p0) != 0)
                    return false;
                if (p1Reserved.Read(p1) != 1)
                    return false;
                var pp = p1 & 3;
                var rxb = p0 >> 5;
                var w = p1 >> 7;
                var vvvv = p1Vvvv.Read(p1);
                ctx.IsVex = true;
                ctx.VexRegister = (byte) vvvv;
                ctx.VexLong = (op & 4) != 0;
                ctx.RegisterExtension.FlagWideValue = w != 0;
                ctx.RegisterExtension.FlagTargetModrmRegister = (rxb & 4) == 0;
                ctx.RegisterExtension.FlagTargetSIBIndex = (rxb & 2) == 0;
                ctx.RegisterExtension.FlagTargetModrmRegOrMem = (rxb & 1) == 0;
                ctx.F2Prefix = pp == 3;
                ctx.F3Prefix = pp == 2;
                ctx.SizeOverridePrefix = pp == 1;

                Decoder[] decoders;
                switch (pp)
                {
                case 2: decoders = decoders0F38; break;
                case 3: decoders = decoders0F3A; break;
                default: decoders = decoders0F; break;
                }
                if (!disasm.TryReadByte(out op))
                    return false;
                return decoders[op].Decode(disasm, op);
            }
        }

        public class F2PrefixDecoder : Decoder
        {
            private readonly Decoder[] rootDecoders;

            public F2PrefixDecoder(Decoder[] rootDecoders)
            {
                this.rootDecoders = rootDecoders;
            }

            public override bool Decode(X86Disassembler disasm, byte op)
            {
                disasm.decodingContext.F2Prefix = true;
                if (!disasm.TryReadByte(out op))
                    return false;
                return rootDecoders[op].Decode(disasm, op);
            }
        }

        public class F3PrefixDecoder : Decoder
        {
            private readonly Decoder[] rootDecoders;

            public F3PrefixDecoder(Decoder[] rootDecoders)
            {
                this.rootDecoders = rootDecoders;
            }

            public override bool Decode(X86Disassembler disasm, byte op)
            {
                if (!disasm.rdr.TryPeekByte(0, out byte b))
                    return false;
                if (b == 0xC3)
                {
                    // rep ret idiom.
                    if (!disasm.TryReadByte(out op))
                        return false;
                    return rootDecoders[b].Decode(disasm, op);
                }
                disasm.decodingContext.F3Prefix = true;
                if (!disasm.TryReadByte(out op))
                    return false;
                return rootDecoders[op].Decode(disasm, op);
            }
        }

        public class ChangeDataWidth : Decoder
        {
            private readonly Decoder[] rootDecoders;

            public ChangeDataWidth(Decoder[] rootDecoders)
            {
                this.rootDecoders = rootDecoders;
            }
            public override bool Decode(X86Disassembler disasm, byte op)
            {
                disasm.decodingContext.SizeOverridePrefix = true;
                disasm.decodingContext.dataWidth = (disasm.decodingContext.dataWidth == PrimitiveType.Word16)
                    ? PrimitiveType.Word32
                    : PrimitiveType.Word16;
                disasm.decodingContext.iWidth = disasm.decodingContext.dataWidth;
                if (!disasm.TryReadByte(out byte op2))
                    return false;
                return rootDecoders[op2].Decode(disasm, op2);
            }
        }

        public class ChangeAddressWidth : Decoder
        {
            private readonly Decoder[] rootDecoders;

            public ChangeAddressWidth(Decoder[] rootDecoders)
            {
                this.rootDecoders = rootDecoders;
            }

            public override bool Decode(X86Disassembler disasm, byte op)
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
                if (!disasm.TryReadByte(out op))
                    return false;
                return rootDecoders[op].Decode(disasm, op);
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
            private readonly Decoder decoderF2;

            public PrefixedDecoder(
                Decoder? dec = null,
                Decoder? dec66 = null,
                Decoder? decF3 = null,
                Decoder? decF2 = null,
                Decoder? decWide = null,
                Decoder? dec66Wide = null,
                InstrClass iclass = InstrClass.Linear)
            {
                this.decoderBase = dec ?? InstructionSet.s_invalid;
                this.decoderWide = decWide ?? decoderBase;
                this.decoder66 = dec66 ?? InstructionSet.s_invalid;
                this.decoder66Wide = dec66Wide ?? decoder66;
                this.decoderF3 = decF3 ?? InstructionSet.s_invalid;
                this.decoderF2 = decF2 ?? InstructionSet.s_invalid;
            }

            public override bool Decode(X86Disassembler disasm, byte op)
            {
                if (disasm.decodingContext.F2Prefix)
                {
                    var instr = decoderF2.Decode(disasm, op);
                    disasm.decodingContext.F2Prefix = false;
                    disasm.decodingContext.F3Prefix = false;
                    return instr;
                }
                else if (disasm.decodingContext.F3Prefix)
                {
                    var instr = decoderF3.Decode(disasm, op);
                    disasm.decodingContext.F2Prefix = false;
                    disasm.decodingContext.F3Prefix = false;
                    return instr;
                }
                else if (disasm.decodingContext.SizeOverridePrefix)
                {
                    if (disasm.isRegisterExtensionEnabled && disasm.decodingContext.RegisterExtension.FlagWideValue)
                    {
                        disasm.decodingContext.dataWidth = PrimitiveType.Word64;
                        return decoder66Wide.Decode(disasm, op);
                    }
                    else
                    {
                        disasm.decodingContext.dataWidth = disasm.defaultDataWidth;
                        return decoder66.Decode(disasm, op);
                    }
                }
                else
                {
                    if (disasm.isRegisterExtensionEnabled && disasm.decodingContext.RegisterExtension.FlagWideValue)
                        return decoderWide.Decode(disasm, op);
                    else
                        return this.decoderBase.Decode(disasm, op);
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

            public override bool Decode(X86Disassembler disasm, byte op)
            {
                switch (disasm.decodingContext.addressWidth.Size)
                {
                case 2: return bit16.Decode(disasm, op);
                case 4: return bit32.Decode(disasm, op);
                case 8: return bit64.Decode(disasm, op);
                default: return false;
                }
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

            public override bool Decode(X86Disassembler disasm, byte op)
            {
                switch (disasm.decodingContext.dataWidth.Size)
                {
                case 2: return bit16.Decode(disasm, op);
                case 4: return bit32.Decode(disasm, op);
                case 8: return bit64.Decode(disasm, op);
                default: return false;
                }
            }
        }


        /// <summary>
        /// This hacky decoder will interpret old-school software emulated
        /// X87 instructions implemented as interrupts.
        /// </summary>
        public class InterruptDecoder : InstructionDecoder
        {
            public InterruptDecoder(Mnemonic op, Mutator<X86Disassembler> mutator) : base(op, InstrClass.Linear, mutator)
            {
            }

            public override bool Decode(X86Disassembler disasm, byte op)
            {
                if (!base.Decode(disasm, op))
                    return false;
                if (disasm.Emulate8087)
                {
                    var imm = (ImmediateOperand) disasm.decodingContext.ops[0];
                    var vector = imm.Value.ToByte();
                    if (disasm.IsEmulated8087Vector(vector))
                    {
                        disasm.RewriteEmulated8087Instruction(vector);
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Decodes an instruction encoded in the ModRM byte.
        /// </summary>
        public class ModRmOpcodeDecoder : Decoder
        {
            private readonly InstructionDecoder[] decoders;

            public ModRmOpcodeDecoder(
                InstructionDecoder defaultDecoder,
                params (byte, InstructionDecoder) [] decoders)
            {
                this.decoders = new InstructionDecoder[256];
                foreach (var (b, decoder) in decoders)
                {
                    Debug.Assert(this.decoders[b] == null, $"Decoder {b:X2} has already a value!");
                    this.decoders[b] = decoder;
                }
                for (int i = 0; i < this.decoders.Length; ++i)
                {
                    if (this.decoders[i] == null)
                    {
                        this.decoders[i] = defaultDecoder;
                    }
                }
            }

            public override bool Decode(X86Disassembler disasm, byte op)
            {
                if (!disasm.TryEnsureModRM(out byte modRm))
                    return false;
                return this.decoders[modRm].Decode(disasm, op);
            }
        }
    }
}
