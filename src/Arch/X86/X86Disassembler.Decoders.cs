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
        /// Opcode Records are used to pick apart the somewhat complex x86 instructions, which have many optional
        /// prefixes, segment overrides, and two classes of instructions, single-byte and two-byte (that is,
        /// prefixed with 0F)
        /// </summary>
		public abstract class OpRec
        {
            public abstract X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat);
        }



        /// <summary>
        /// Single byte opcode record.
        /// </summary>
        public class SingleByteOpRec : OpRec
        {
            public Opcode opcode;
            public string format;
            public InstrClass iclass;

            public SingleByteOpRec(Opcode op) : this(op, InstrClass.Linear, "")
            {
            }

            public SingleByteOpRec(Opcode op, string fmt) : this(op, InstrClass.Linear, fmt)
            {
            }

            public SingleByteOpRec(Opcode op, InstrClass icl, string fmt)
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
        /// Use this OpRec when an instruction encoding is dependent on whether the processor
        /// is in 64-bit mode or not.
        /// </summary>
        public class Alternative64OpRec : OpRec
        {
            private OpRec oprec32;
            private OpRec oprec64;

            public Alternative64OpRec(OpRec oprec32, OpRec oprec64)
            {
                this.oprec32 = oprec32;
                this.oprec64 = oprec64;
            }

            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                if (disasm.defaultAddressWidth.BitSize == 64)
                    return oprec64.Decode(disasm, op, opFormat);
                else
                    return oprec32.Decode(disasm, op, opFormat);
            }
        }

        public class Rex_SingleByteOpRec : SingleByteOpRec
        {
            public Rex_SingleByteOpRec(Opcode op, string fmt)
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
                    op = disasm.rdr.ReadByte();
                    return s_aOpRec[op].Decode(disasm, op, opFormat);
                }
                else
                    return base.Decode(disasm, op, opFormat);
            }
        }

        public class SegmentOverrideOprec : OpRec
        {
            private readonly int seg;

            public SegmentOverrideOprec(int seg)
            {
                this.seg = seg;
            }

            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                disasm.currentDecodingContext.SegmentOverride = SegFromBits(seg);
                op = disasm.rdr.ReadByte();
                return s_aOpRec[op].Decode(disasm, op, opFormat);
            }
        }

        public class GroupOpRec : OpRec
        {
            public int Group;
            public string format;

            public GroupOpRec(int group, string format)
            {
                this.Group = group;
                this.format = format;
            }

            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                int grp = Group - 1;
                if (!disasm.TryEnsureModRM(out byte modRm))
                    return null;
                OpRec opRec = s_aOpRecGrp[grp * 8 + ((modRm >> 3) & 0x07)];
                return opRec.Decode(disasm, op, opFormat + format);
            }
        }

        // Uses the 2 high bits of the ModRM word for further discrimination
        public class Group7OpRec : OpRec
        {
            private OpRec memInstr;
            private OpRec[] regInstrs;

            public Group7OpRec(
                OpRec memInstr,
                params OpRec[] regInstrs)
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

        public class NyiDecoder : OpRec
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

        public class FpuOpRec : OpRec
        {
            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                if (!disasm.TryEnsureModRM(out byte modRM))
                    return null;
                OpRec opRec;
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

        public class TwoByteOpRec : OpRec
        {
            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                if (!disasm.rdr.TryReadByte(out op))
                    return null;
                return s_aOpRec0F[op].Decode(disasm, op, "");
            }
        }

        public class ThreeByteOpRec : OpRec
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

        public class VexDecoder2 : OpRec
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

        public class VexDecoder3 : OpRec
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

                OpRec[] decoders;
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

        public class F2ByteOpRec : OpRec
        {
            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                disasm.currentDecodingContext.F2Prefix = true;
                if (!disasm.rdr.TryReadByte(out op))
                    return null;
                return s_aOpRec[op].Decode(disasm, op, opFormat);
            }
        }

        public class F3ByteOpRec : OpRec
        {
            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                byte b = disasm.rdr.PeekByte(0);
                if (b == 0xC3)
                {
                    op = disasm.rdr.ReadByte();
                    return s_aOpRec[b].Decode(disasm, op, opFormat);
                }
                disasm.currentDecodingContext.F3Prefix = true;
                if (!disasm.rdr.TryReadByte(out op))
                    return null;
                return s_aOpRec[op].Decode(disasm, op, opFormat);
            }
        }

        public class ChangeDataWidth : OpRec
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

        public class ChangeAddressWidth : OpRec
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

        public class PrefixedOpRec : OpRec
        {
            private readonly OpRec op;
            private readonly OpRec opWide;
            private readonly OpRec op66;
            private readonly OpRec op66Wide;
            private readonly OpRec opF3;
            private readonly OpRec opF2;

            public PrefixedOpRec(
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
                OpRec MakeDecoder(Opcode opc, string format)
                {
                    return opc != Opcode.illegal
                        ? new SingleByteOpRec(opc, iclass, format)
                        : s_invalid;
                }

                this.op = this.opWide = MakeDecoder(op, opFmt);
                this.op66 = this.op66Wide = MakeDecoder(op66, op66Fmt);
                this.opF3 = MakeDecoder(opF3, opF3Fmt);
                this.opF2 = MakeDecoder(opF2, opF2Fmt);
            }

            public PrefixedOpRec(
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
                OpRec MakeDecoder(Opcode opc, string format)
                {
                    return opc != Opcode.illegal
                        ? new SingleByteOpRec(opc, iclass, format)
                        : s_nyi;
                }

                this.op = MakeDecoder(op, opFmt);
                this.opWide = MakeDecoder(opWide, opFmt);
                this.op66 = MakeDecoder(op66, op66Fmt);
                this.op66Wide = MakeDecoder(op66Wide, op66Fmt);
                this.opF3 = MakeDecoder(opF3, opF3Fmt);
                this.opF2 = s_nyi;
            }

            public PrefixedOpRec(
                OpRec dec = null,
                OpRec decWide = null,
                OpRec dec66 = null,
                OpRec dec66Wide = null,
                OpRec decF3 = null,
                OpRec decF2 = null)
            {
                this.op = dec ?? s_nyi;
                this.opWide = decWide ?? s_nyi;
                this.op66 = dec66 ?? s_nyi;
                this.op66Wide = dec66Wide ?? s_nyi;
                this.opF3 = decF3 ?? s_nyi;
                this.opF2 = decF2 ?? s_nyi;
            }

            public override X86Instruction Decode(X86Disassembler disasm, byte op, string opFormat)
            {
                if (disasm.currentDecodingContext.F2Prefix)
                {
                    var instr = opF2.Decode(disasm, op, opFormat);
                    instr.repPrefix = 0;
                    return instr;
                }
                else if (disasm.currentDecodingContext.F3Prefix)
                {
                    var instr = opF3.Decode(disasm, op, opFormat);
                    instr.repPrefix = 0;
                    return instr;
                }
                else if (disasm.currentDecodingContext.SizeOverridePrefix)
                {
                    if (disasm.isRegisterExtensionEnabled && disasm.currentDecodingContext.RegisterExtension.FlagWideValue)
                        return op66Wide.Decode(disasm, op, opFormat);
                    else
                        return op66.Decode(disasm, op, opFormat);
                }
                else
                {
                    if (disasm.isRegisterExtensionEnabled && disasm.currentDecodingContext.RegisterExtension.FlagWideValue)
                        return opWide.Decode(disasm, op, opFormat);
                    else
                        return this.op.Decode(disasm, op, opFormat);
                }
            }
        }

        public class InterruptOpRec : SingleByteOpRec
        {
            public InterruptOpRec(Opcode op, string fmt) : base(op, fmt)
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
