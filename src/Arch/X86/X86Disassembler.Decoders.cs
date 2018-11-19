#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
            public abstract X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat);
        }



        /// <summary>
        /// Decodes a single instructions by interpreting a format string.
        /// </summary>
        public class InstructionDecoder : Decoder
        {
            public Opcode opcode;       // mnemonic for the decoded instruction
            public string format;       // mini language for decoding operands to this instruction
            public InstrClass iclass;

            public InstructionDecoder(Opcode op) : this(op, InstrClass.Linear, "")
            {
            }

            public InstructionDecoder(Opcode op, string fmt) : this(op, InstrClass.Linear, fmt)
            {
            }

            public InstructionDecoder(Opcode op, InstrClass icl, string fmt)
            {
                opcode = op;
                format = fmt;
                iclass = icl;
            }

            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                return disasm.DecodeOperands(opcode, op, opFormat + format, iclass);
            }
        }

        /// <summary>
        /// Use this decoder when an instruction encoding is dependent on whether the processor
        /// is in 64-bit mode or not.
        /// </summary>
        public class Alternative64Decoder : Decoder
        {
            private Decoder decoder32;
            private Decoder decoder64;

            public Alternative64Decoder(Decoder decoder32, Decoder decoder64)
            {
                this.decoder32 = decoder32;
                this.decoder64 = decoder64;
            }

            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                if (disasm.defaultAddressWidth.BitSize == 64)
                    return decoder64.Decode(disasm, op, opFormat);
                else
                    return decoder32.Decode(disasm, op, opFormat);
            }
        }

        /// <summary>
        /// Decodes instructions whose meaning depends on whether REX prefixes
        /// are to be interpretd or not.
        /// </summary>
        public class Rex_or_InstructionDecoder : InstructionDecoder
        {
            public Rex_or_InstructionDecoder(Opcode op, string fmt)
                : base(op, fmt)
            {
            }

            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                if (disasm.isRegisterExtensionEnabled)
                {
                    disasm.currentDecodingContext.RegisterExtensionPrefixByte = op;
                    if (disasm.currentDecodingContext.RegisterExtension.FlagWideValue)
                    {
                        disasm.dataWidth = PrimitiveType.Word64;
                    }
                    if (!disasm.rdr.TryReadByte(out var op2))
                        return null;
                    return s_aOpRec[op2].Decode(disasm, op2, opFormat);
                }
                else
                    return base.Decode(disasm, op, opFormat);
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

            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                disasm.currentDecodingContext.SegmentOverride = SegFromBits(seg);
                if (!disasm.rdr.TryReadByte(out var op2))
                    return null;
                return s_aOpRec[op2].Decode(disasm, op2, opFormat);
            }
        }

        public class GroupDecoder : Decoder
        {
            public readonly int Group;
            public readonly string format;

            public GroupDecoder(int group, string format)
            {
                this.Group = group;
                this.format = format;
            }

            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                int grp = Group - 1;
                if (!disasm.TryEnsureModRM(out byte modRm))
                    return null;
                Decoder opRec = s_aOpRecGrp[grp * 8 + ((modRm >> 3) & 0x07)];
                return opRec.Decode(disasm, op, opFormat + format);
            }
        }

        // Uses the 2 high bits of the ModRM word for further discrimination
        public class Group7Decoder : Decoder
        {
            private Decoder memInstr;
            private Decoder[] regInstrs;

            public Group7Decoder(
                Decoder memInstr,
                params Decoder[] regInstrs)
            {
                this.memInstr = memInstr;
                this.regInstrs = regInstrs;
            }

            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                if (!disasm.TryEnsureModRM(out byte modRm))
                    return null;
                if ((modRm & 0xC0) == 0xC0)
                {
                    var i = modRm & 0x07;
                    if (i < regInstrs.Length)
                    {
                        return regInstrs[i].Decode(disasm, op, opFormat);
                    }
                    else
                    {
                        return disasm.Illegal();
                    }
                }
                else
                {
                    return memInstr.Decode(disasm, op, opFormat);
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
            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                return disasm.NotYetImplemented(message, 0);
            }
        }

        /// <summary>
        /// Decodes X86 FPU instructions, which are encoded in their own
        /// special way.
        /// </summary>
        public class X87Decoder : Decoder
        {
            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                if (!disasm.TryEnsureModRM(out byte modRM))
                    return null;
                Decoder opRec;
                int iOpRec = (op & 0x07) * 0x48;
                if (modRM < 0xC0)
                {
                    opRec = s_aFpOpRec[iOpRec + ((modRM >> 3) & 0x07)];
                }
                else
                {
                    opRec = s_aFpOpRec[iOpRec + modRM - 0xB8];
                }
                return opRec.Decode(disasm, op, opFormat);
            }
        }

        /// <summary>
        /// Decodes an instruction with a 0x0F prefix.
        /// </summary>
        public class AdditionalByteDecoder : Decoder
        {
            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                if (!disasm.rdr.TryReadByte(out op))
                    return null;
                return s_aOpRec0F[op].Decode(disasm, op, "");
            }
        }

        public class ThreeByteOpRec : Decoder
        {
            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                switch (op)
                {
                case 0x38:
                    if (!disasm.rdr.TryReadByte(out op))
                        return null;
                    return s_aOpRec0F38[op].Decode(disasm, op, "");
                case 0x3A:
                    if (!disasm.rdr.TryReadByte(out op))
                        return null;
                    return s_aOpRec0F3A[op].Decode(disasm, op, "");
                default: return null;
                }
            }
        }

        /// <summary>
        /// Decodes 2-byte VEX encoded instructions.
        /// </summary>
        public class VexDecoder2 : Decoder
        {
            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                if (!disasm.rdr.TryReadByte(out op))
                    return null;
                var r = (~op >> 2) & 4;
                var vvvv = (~op >> 3) & 0xF;
                var pp = op & 3;
                var ctx = disasm.currentDecodingContext;
                ctx.IsVex = true;
                ctx.VexRegister = (byte) vvvv;
                ctx.RegisterExtensionPrefixByte = (byte) r;
                ctx.VexLong = (op & 4) != 0;
                ctx.F2Prefix = pp == 3;
                ctx.F3Prefix = pp == 2;
                ctx.SizeOverridePrefix = pp == 1;
                if (!disasm.rdr.TryReadByte(out op))
                    return null;
                var instr = s_aOpRec0F[op].Decode(disasm, op, opFormat);
                if (instr == null)
                    return instr;
                if (!s_mpVex.TryGetValue(instr.code, out Opcode vexCode))
                {
                    Debug.Print("X86Disassembler: {0} Failed to map {1} to VEX counterpart", instr.Address, instr.code);
                    return null;
                }
                instr.code = vexCode;
                return instr;
            }
        }

        /// <summary>
        /// Decodes 3-byte VEX encoded instructions.
        /// </summary>
        public class VexDecoder3 : Decoder
        {
            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                if (!disasm.rdr.TryReadByte(out op))
                    return null;
                var rxb = op >> 5;
                var mmmmm = op & 0x1F;

                if (!disasm.rdr.TryReadByte(out op))
                    return null;
                var w = op >> 7;
                var vvvv = (~op >> 3) & 0xF;
                var pp = op & 0x3;

                var ctx = disasm.currentDecodingContext;
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
                case 1: decoders = s_aOpRec0F; break;
                case 2: decoders = s_aOpRec0F38; break;
                case 3: decoders = s_aOpRec0F3A; break;
                default: return null;
                }
                if (!disasm.rdr.TryReadByte(out op))
                    return null;
                var instr = decoders[op].Decode(disasm, op, opFormat);
                if (instr == null)
                    return instr;
                if (!s_mpVex.TryGetValue(instr.code, out instr.code))
                    return null;
                return instr;
            }
        }

        public class F2PrefixDecoder : Decoder
        {
            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                disasm.currentDecodingContext.F2Prefix = true;
                if (!disasm.rdr.TryReadByte(out op))
                    return null;
                return s_aOpRec[op].Decode(disasm, op, opFormat);
            }
        }

        public class F3PrefixDecoder : Decoder
        {
            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                if (!disasm.rdr.TryPeekByte(0, out byte b))
                    return null;
                if (b == 0xC3)
                {
                    // rep ret idiom.
                    op = disasm.rdr.ReadByte();
                    return s_aOpRec[b].Decode(disasm, op, opFormat);
                }
                disasm.currentDecodingContext.F3Prefix = true;
                if (!disasm.rdr.TryReadByte(out op))
                    return null;
                return s_aOpRec[op].Decode(disasm, op, opFormat);
            }
        }

        public class ChangeDataWidth : Decoder
        {
            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                disasm.currentDecodingContext.SizeOverridePrefix = true;
                disasm.dataWidth = (disasm.dataWidth == PrimitiveType.Word16)
                    ? PrimitiveType.Word32
                    : PrimitiveType.Word16;
                op = disasm.rdr.ReadByte();
                return s_aOpRec[op].Decode(disasm, op, opFormat);
            }
        }

        public class ChangeAddressWidth : Decoder
        {
            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                disasm.addressWidth = (disasm.addressWidth == PrimitiveType.Word16)
                    ? PrimitiveType.Word32
                    : PrimitiveType.Word16;
                op = disasm.rdr.ReadByte();
                return s_aOpRec[op].Decode(disasm, op, opFormat);
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

            [Obsolete("Use other constructor")]
            public PrefixedDecoder(
                Opcode op,
                string opFmt,
                Opcode op66 = Opcode.illegal,
                string op66Fmt = null,
                Opcode opF3 = Opcode.illegal,
                string opF3Fmt = null,
                Opcode opF2 = Opcode.illegal,
                string opF2Fmt = null,
                InstrClass iclass = InstrClass.Linear)
            {
                Decoder MakeDecoder(Opcode opc, string format)
                {
                    return opc != Opcode.illegal
                        ? new InstructionDecoder(opc, iclass, format)
                        : s_invalid;
                }

                this.decoderBase = this.decoderWide = MakeDecoder(op, opFmt);
                this.decoder66 = this.decoder66Wide = MakeDecoder(op66, op66Fmt);
                this.decoderF3 = MakeDecoder(opF3, opF3Fmt);
                this.decoderF2 = MakeDecoder(opF2, opF2Fmt);
            }

            [Obsolete("Use other constructor")]
            public PrefixedDecoder(
                Opcode op,
                Opcode opWide,
                string opFmt,
                Opcode op66,
                Opcode op66Wide,
                string op66Fmt,
                Opcode opF3 = Opcode.illegal,
                string opF3Fmt = null,
                InstrClass iclass = InstrClass.Linear)
            {
                Decoder MakeDecoder(Opcode opc, string format)
                {
                    return opc != Opcode.illegal
                        ? new InstructionDecoder(opc, iclass, format)
                        : s_nyi;
                }

                this.decoderBase = MakeDecoder(op, opFmt);
                this.decoderWide = MakeDecoder(opWide, opFmt);
                this.decoder66 = MakeDecoder(op66, op66Fmt);
                this.decoder66Wide = MakeDecoder(op66Wide, op66Fmt);
                this.decoderF3 = MakeDecoder(opF3, opF3Fmt);
                this.decoderF2 = s_nyi;
            }

            public PrefixedDecoder(
                Decoder dec = null,
                Decoder decWide = null,
                Decoder dec66 = null,
                Decoder dec66Wide = null,
                Decoder decF3 = null,
                Decoder decF2 = null)
            {
                this.decoderBase = dec ?? s_invalid;
                this.decoderWide = decWide ?? s_invalid;
                this.decoder66 = dec66 ?? s_invalid;
                this.decoder66Wide = dec66Wide ?? s_invalid;
                this.decoderF3 = decF3 ?? s_invalid;
                this.decoderF2 = decF2 ?? s_invalid;
            }

            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                if (disasm.currentDecodingContext.F2Prefix)
                {
                    var instr = decoderF2.Decode(disasm, op, opFormat);
                    instr.repPrefix = 0;
                    return instr;
                }
                else if (disasm.currentDecodingContext.F3Prefix)
                {
                    var instr = decoderF3.Decode(disasm, op, opFormat);
                    instr.repPrefix = 0;
                    return instr;
                }
                else if (disasm.currentDecodingContext.SizeOverridePrefix)
                {
                    if (disasm.isRegisterExtensionEnabled && disasm.currentDecodingContext.RegisterExtension.FlagWideValue)
                        return decoder66Wide.Decode(disasm, op, opFormat);
                    else
                        return decoder66.Decode(disasm, op, opFormat);
                }
                else
                {
                    if (disasm.isRegisterExtensionEnabled && disasm.currentDecodingContext.RegisterExtension.FlagWideValue)
                        return decoderWide.Decode(disasm, op, opFormat);
                    else
                        return this.decoderBase.Decode(disasm, op, opFormat);
                }
            }
        }

        /// <summary>
        /// This hacky decoder will interpret old-school software emulated
        /// X87 instructions implemented as interrupts.
        /// </summary>
        public class InterruptDecoder : InstructionDecoder
        {
            public InterruptDecoder(Opcode op, string fmt) : base(op, fmt)
            {
            }

            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                var instr = base.Decode(disasm, op, opFormat);
                if (disasm.Emulate8087)
                {
                    var imm = (ImmediateOperand) instr.op1;
                    var vector = imm.Value.ToByte();
                    if (disasm.IsEmulated8087Vector(vector))
                    {
                        return disasm.RewriteEmulated8087Instruction(vector);
                    }
                }
                return instr;
            }
        }
    }
}
