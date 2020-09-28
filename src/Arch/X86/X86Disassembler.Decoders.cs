#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Decodes a single instructions by interpreting a format string.
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
        public class Rex_or_InstructionDecoder : InstructionDecoder
        {
            public Rex_or_InstructionDecoder(Mnemonic op, Mutator<X86Disassembler> mutator)
                : base(op, InstrClass.Linear, mutator)
            {
            }

            public override bool Decode(X86Disassembler disasm, byte op)
            {
                if (disasm.isRegisterExtensionEnabled)
                {
                    disasm.decodingContext.RegisterExtensionPrefixByte = op;
                    if (disasm.decodingContext.RegisterExtension.FlagWideValue)
                    {
                        var w64 = PrimitiveType.Word64;
                        disasm.decodingContext.dataWidth = w64;
                        disasm.decodingContext.iWidth = w64;
                    }
                    if (!disasm.rdr.TryReadByte(out var op2))
                        return false;
                    return s_rootDecoders[op2].Decode(disasm, op2);
                }
                else
                    return base.Decode(disasm, op);
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
                if (!disasm.rdr.TryReadByte(out var op2))
                    return false;
                return s_rootDecoders[op2].Decode(disasm, op2);
            }
        }

        /// <summary>
        /// Uses bits 3-5 of the ModRM byte to decode further.
        /// </summary>
        public class GroupDecoder : Decoder
        {
            public readonly int Group;
            public readonly Mutator<X86Disassembler>[] mutators;

            public GroupDecoder(int group, params Mutator<X86Disassembler>[] mutators)
            {
                this.Group = group;
                this.mutators = mutators;
            }

            public override bool Decode(X86Disassembler disasm, byte op)
            {
                int grp = Group - 1;
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

                Decoder decoder = s_groupDecoders[grp * 8 + ((modRm >> 3) & 0x07)];
                return decoder.Decode(disasm, op);
            }
        }

        public class Group6Decoder : Decoder
        {
            private readonly Decoder memDecoder;
            private readonly Decoder regDecoder;

            public Group6Decoder(
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
                disasm.NotYetImplemented(op, message);
                return false;
            }
        }

        /// <summary>
        /// Decodes X86 FPU instructions, which are encoded in their own
        /// special way.
        /// </summary>
        public class X87Decoder : Decoder
        {
            public override bool Decode(X86Disassembler disasm, byte op)
            {
                if (!disasm.TryEnsureModRM(out byte modRM))
                    return false;
                Decoder decoder;
                int iDecoder = (op & 0x07) * 0x48;
                if (modRM < 0xC0)
                {
                    decoder = s_fpuDecoders[iDecoder + ((modRM >> 3) & 0x07)];
                }
                else
                {
                    decoder = s_fpuDecoders[iDecoder + modRM - 0xB8];
                }
                return decoder.Decode(disasm, op);
            }
        }

        /// <summary>
        /// Decodes an instruction with a 0x0F prefix.
        /// </summary>
        public class AdditionalByteDecoder : Decoder
        {
            public override bool Decode(X86Disassembler disasm, byte op)
            {
                if (!disasm.rdr.TryReadByte(out op))
                    return false;
                return s_decoders0F[op].Decode(disasm, op);
            }
        }

        public class ThreeByteDecoder : Decoder
        {
            public override bool Decode(X86Disassembler disasm, byte op)
            {
                byte op2;
                switch (op)
                {
                case 0x38:
                    if (!disasm.rdr.TryReadByte(out op2))
                        return false;
                    return s_decoders0F38[op2].Decode(disasm, op2);
                case 0x3A:
                    if (!disasm.rdr.TryReadByte(out op2))
                        return false;
                    return s_decoders0F3A[op2].Decode(disasm, op2);
                default: return false;
                }
            }
        }

        /// <summary>
        /// Decodes 2-byte VEX encoded instructions.
        /// </summary>
        public class VexDecoder2 : Decoder
        {
            public override bool Decode(X86Disassembler disasm, byte op)
            {
                var ctx = disasm.decodingContext;
                if (ctx.SizeOverridePrefix | ctx.F2Prefix | ctx.F3Prefix)
                    return false;
                if (!disasm.rdr.TryReadByte(out op))
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
                if (!disasm.rdr.TryReadByte(out op))
                    return false;
                if (op == 0x38 || op == 0x3A)
                    return false;
                var instr = s_decoders0F[op].Decode(disasm, op);
                if (!instr)
                    return false;
                if (!s_mpVex.TryGetValue(disasm.decodingContext.mnemonic, out Mnemonic vexCode))
                {
                    Debug.Print("X86Disassembler: {0} Failed to map {1} to VEX counterpart", disasm.addr, disasm.decodingContext.mnemonic);
                    return false;
                }
                disasm.decodingContext.mnemonic = vexCode;
                return true;
            }
        }

        /// <summary>
        /// Decodes 3-byte VEX encoded instructions.
        /// </summary>
        public class VexDecoder3 : Decoder
        {
            public override bool Decode(X86Disassembler disasm, byte op)
            {
                if (!disasm.rdr.TryReadByte(out op))
                    return false;
                var rxb = op >> 5;
                var mmmmm = op & 0x1F;

                if (!disasm.rdr.TryReadByte(out op))
                    return false;
                var w = op >> 7;
                var vvvv = (~op >> 3) & 0xF;
                var pp = op & 0x3;

                var ctx = disasm.decodingContext;
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
                switch (mmmmm)
                {
                case 1: decoders = s_decoders0F; break;
                case 2: decoders = s_decoders0F38; break;
                case 3: decoders = s_decoders0F3A; break;
                default: return false;
                }
                if (!disasm.rdr.TryReadByte(out op))
                    return false;
                if (!decoders[op].Decode(disasm, op))
                    return false;
                if (s_mpVex.TryGetValue(disasm.decodingContext.mnemonic, out var vCode))
                {
                    disasm.decodingContext.mnemonic = vCode;
                }
                return true;
            }
        }

        public class F2PrefixDecoder : Decoder
        {
            public override bool Decode(X86Disassembler disasm, byte op)
            {
                disasm.decodingContext.F2Prefix = true;
                if (!disasm.rdr.TryReadByte(out op))
                    return false;
                return s_rootDecoders[op].Decode(disasm, op);
            }
        }

        public class F3PrefixDecoder : Decoder
        {
            public override bool Decode(X86Disassembler disasm, byte op)
            {
                if (!disasm.rdr.TryPeekByte(0, out byte b))
                    return false;
                if (b == 0xC3)
                {
                    // rep ret idiom.
                    op = disasm.rdr.ReadByte();
                    return s_rootDecoders[b].Decode(disasm, op);
                }
                disasm.decodingContext.F3Prefix = true;
                if (!disasm.rdr.TryReadByte(out op))
                    return false;
                return s_rootDecoders[op].Decode(disasm, op);
            }
        }

        public class ChangeDataWidth : Decoder
        {
            public override bool Decode(X86Disassembler disasm, byte op)
            {
                disasm.decodingContext.SizeOverridePrefix = true;
                disasm.decodingContext.dataWidth = (disasm.decodingContext.dataWidth == PrimitiveType.Word16)
                    ? PrimitiveType.Word32
                    : PrimitiveType.Word16;
                disasm.decodingContext.iWidth = disasm.decodingContext.dataWidth;
                if (!disasm.rdr.TryReadByte(out byte op2))
                    return false;
                return s_rootDecoders[op2].Decode(disasm, op2);
            }
        }

        public class ChangeAddressWidth : Decoder
        {
            public override bool Decode(X86Disassembler disasm, byte op)
            {
                disasm.decodingContext.addressWidth = (disasm.decodingContext.addressWidth == PrimitiveType.Word16)
                    ? PrimitiveType.Word32
                    : PrimitiveType.Word16;
                op = disasm.rdr.ReadByte();
                return s_rootDecoders[op].Decode(disasm, op);
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
                Decoder dec = null,
                Decoder dec66 = null,
                Decoder decF3 = null,
                Decoder decF2 = null,
                Decoder decWide = null,
                Decoder dec66Wide = null,
                InstrClass iclass = InstrClass.Linear)
            {
                this.decoderBase = dec ?? s_invalid;
                this.decoderWide = decWide ?? decoderBase;
                this.decoder66 = dec66 ?? s_invalid;
                this.decoder66Wide = dec66Wide ?? decoder66;
                this.decoderF3 = decF3 ?? s_invalid;
                this.decoderF2 = decF2 ?? s_invalid;
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
                        return decoder66Wide.Decode(disasm, op);
                    else
                        return decoder66.Decode(disasm, op);
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
    }
}
