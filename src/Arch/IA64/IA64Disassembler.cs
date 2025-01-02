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
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Security.Cryptography;

namespace Reko.Arch.IA64
{
#pragma warning disable IDE1006

    using WideDecoder = WideDecoder<IA64Disassembler, Mnemonic, IA64Instruction>;
    using WideMutator = WideMutator<IA64Disassembler>;

    public class IA64Disassembler : DisassemblerBase<IA64Instruction, Mnemonic>,
        IEnumerable<IA64Instruction>
    {
        private const ulong slot0Mask = (1ul << 41) - 1u;
        private const ulong slot1MaskLo = (1ul << 18) - 1u;
        private const ulong slot1MaskHi = (1ul << 23) - 1u;
        private const ulong slot2Mask = (1ul << 41) - 1u;

        private static readonly TemplateDecoders[] templates;
        private static WideDecoder Bdecoders;
        private static WideDecoder Idecoders;
        private static WideDecoder Fdecoders;
        private static WideDecoder Ldecoders;
        private static WideDecoder Mdecoders;
        private static WideDecoder Xdecoders;

        private readonly IA64Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;
        private RegisterStorage qpReg;

        public IA64Disassembler(IA64Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
            this.qpReg = Registers.PredicateRegisters[0];
            this.addr = null!;
        }

        public new IEnumerator<IA64Instruction> GetEnumerator()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadUInt64(out ulong uBundleLo) ||
                !rdr.TryReadUInt64(out ulong uBundleHi))
            {
                yield break;
            }
            var template = uBundleLo & 0x1F;
            var uSlot0 = (uBundleLo >> 5) & slot0Mask;
            var uSlot1 = ((uBundleLo >> 46) & slot1MaskLo) | ((uBundleHi & slot1MaskHi) << 18);
            var uSlot2 = (uBundleHi >> 23) & slot2Mask;

            var slot0 = templates[template].Slot0.Decode(uSlot0, this);
            slot0.Address = addr;
            slot0.Length = 6;
            Clear();
            var slot1 = templates[template].Slot1.Decode(uSlot1, this);
            slot1.Address = addr + 6;
            slot1.Length = 6;
            Clear();
            var slot2 = templates[template].Slot2.Decode(uSlot2, this);
            slot2.Address = addr + 6;
            slot2.Length = 6;
            Clear();
            yield return slot0;
            yield return slot1;
            yield return slot2;
        }

        private void Clear()
        {
            ops.Clear();
            qpReg = Registers.PredicateRegisters[0];
        }

        public override IA64Instruction DisassembleInstruction()
        {
            throw new InvalidOperationException("Call GetEnumerator method instead.");
        }

        public override IA64Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new IA64Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray(),
                QualifyingPredicate = qpReg,
            };
        }

        public override IA64Instruction CreateInvalidInstruction()
        {
            return new IA64Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
                Operands = Array.Empty<MachineOperand>()
            };
        }

        #region Mutators

        private static WideMutator GpReg(int bitpos, int length)
        {
            var field = new Bitfield(bitpos, length);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                var reg = Registers.GpRegisters[iReg];
                d.ops.Add(reg);
                return true;
            };
        }
        private static readonly WideMutator r1 = GpReg(6, 7);
        private static readonly WideMutator r2 = GpReg(13, 7);
        private static readonly WideMutator r3 = GpReg(20, 7);
        private static readonly WideMutator r3_2 = GpReg(20, 2);

        private static WideMutator Imm(PrimitiveType dt, Bitfield[] fields)
        {
            return (u, d) =>
            {
                var imm = Bitfield.ReadFields(fields, u);
                var op = new ImmediateOperand(Constant.Create(dt, imm));
                d.ops.Add(op);
                return true;
            };
        }
        private static readonly WideMutator I32_1_20 = Imm(PrimitiveType.Word32, Bf((36,1),(6, 20)));

        private static WideMutator ImmS(PrimitiveType dt, Bitfield[] fields)
        {
            return (u, d) =>
            {
                var imm = Bitfield.ReadSignedFields(fields, u);
                var op = new ImmediateOperand(Constant.Create(dt, imm));
                d.ops.Add(op);
                return true;
            };
        }

        private static readonly WideMutator IsImm14 = ImmS(PrimitiveType.Int64, Bf((36, 1), (27, 6), (13, 7)));
        private static readonly WideMutator IsImm22 = ImmS(PrimitiveType.Int64, Bf((36, 1), (22, 5), (27, 9), (13, 7)));

        /// <summary>
        /// Qualifying predicate.
        /// </summary>
        private static bool qp(ulong uInstr, IA64Disassembler dasm)
        {
            dasm.qpReg = Registers.PredicateRegisters[(int) uInstr & 0x3F];
            return true;
        }

        #endregion

        #region Decoders 

        public override IA64Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("Ia64Dis", this.addr, rdr, message);
            return CreateInvalidInstruction();
        }

        private struct TemplateDecoders
        {
            public readonly WideDecoder Slot0;
            public readonly WideDecoder Slot1;
            public readonly WideDecoder Slot2;
            public readonly int StopMask;

            public TemplateDecoders(
                WideDecoder slot0Decoder,
                WideDecoder slot1Decoder,
                WideDecoder slot2Decoder,
                int stopMask)
            {
                this.Slot0 = slot0Decoder;
                this.Slot1 = slot1Decoder;
                this.Slot2 = slot2Decoder;
                this.StopMask = stopMask;
            }
        }

        private class UnitDecoder : WideDecoder
        {
            private readonly char unit;
            private readonly WideDecoder innerDecoder;

            public UnitDecoder(char unit, WideDecoder innerDecoder)
            {
                this.unit = unit;
                this.innerDecoder = innerDecoder;
            }

            public override IA64Instruction Decode(ulong wInstr, IA64Disassembler dasm)
            {
                var instr = innerDecoder.Decode(wInstr, dasm);
                instr.Unit = unit;
                return instr;
            }
        }

        protected static WideDecoder<IA64Disassembler, Mnemonic, IA64Instruction> Instr(Mnemonic mnemonic, params WideMutator<IA64Disassembler>[] mutators)
        {
            return new WideInstrDecoder<IA64Disassembler, Mnemonic, IA64Instruction>(InstrClass.Linear, mnemonic, mutators);
        }

        protected static WideDecoder<IA64Disassembler, Mnemonic, IA64Instruction> Instr(Mnemonic mnemonic, InstrClass iclass, params WideMutator<IA64Disassembler>[] mutators)
        {
            return new WideInstrDecoder<IA64Disassembler, Mnemonic, IA64Instruction>(iclass, mnemonic, mutators);
        }


        private static WideDecoder<IA64Disassembler, Mnemonic, IA64Instruction> Nyi(string message)
        {
            return new WideNyiDecoder<IA64Disassembler, Mnemonic, IA64Instruction>(message);
        }

        protected static WideMaskDecoder<IA64Disassembler, Mnemonic, IA64Instruction> WideMask(int bitPos, int bitLength, string tag, params WideDecoder[] decoders)
        {
            return new WideMaskDecoder<IA64Disassembler, Mnemonic, IA64Instruction>(new Bitfield(bitPos, bitLength), tag, decoders);
        }



        #endregion

#nullable disable
        static IA64Disassembler()
        {
            BuildDecoders();
            templates = BuildTemplateDecoders();
        }
#nullable enable

        private static void BuildDecoders()
        {
            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);
            var reserved = Instr(Mnemonic.Invalid, InstrClass.Invalid);
            var nop = Instr(Mnemonic.nop, InstrClass.Invalid);


            // Major opcodes.

            var sysMemMgmt4_2bit = WideSparse(27, 4, "  Sys/Mem Mgmt 4+2-bit opcode extensions", Nyi(""),
                (0, WideMask(31, 2, "  2-bits",
                    Instr(Mnemonic.break_m, InstrClass.Terminates, I32_1_20, qp),
                    Nyi("invala"),
                    Nyi("fwb"),
                    Nyi("srlz.d"))));

            var sysMemMgmt3bit = WideMask(33, 3, "  Sys/Mem Mgmt 3-bit opcode extensions",
                    sysMemMgmt4_2bit,
                    invalid,
                    invalid,
                    invalid,

                    Nyi("chk.a.nc - int"),
                    Nyi("chk.a.clr - int"),
                    Nyi("chk.a.nc - fp"),
                    Nyi("chk.a.clr - fp"));

            var misc6bitExt = WideMask(31, 2, "  Misc I-Unit 6-bit Opcode Extensions",
                WideSparse(27, 4, "  0", Nyi("0"),
                    (0, Instr(Mnemonic.break_i, InstrClass.Terminates, I32_1_20, qp)),
                    (1, Instr(Mnemonic.nop_i, InstrClass.Padding, I32_1_20, qp))
                    ),
                Nyi("1"),
                Nyi("2"),
                Nyi("3"));

            var misc = WideSparse(33, 3, "  misc", Nyi("misc"),
                (0, misc6bitExt));
            var deposit = Nyi("deposit");
            var shiftTestBit = Nyi("shift/test bit");
            var mmMpyShift = Nyi("mm/mpy shift");

            var multimediaAlu1_2ext = Nyi("Multimedia ALU 1-bit+2-bit Ext");

            var aluMmAlu = WideMask(33, 1, "  ALU/mm ALU",
                WideMask(34, 2, "  0",
                    Nyi("Integer ALU 4+2 ext"),
                    multimediaAlu1_2ext,
                    Instr(Mnemonic.adds, r1, IsImm14, r3, qp),
                    Nyi("addp4 - imm14")),
                WideMask(34, 2, "  1",
                    invalid,
                    multimediaAlu1_2ext,
                    invalid,
                    invalid));

            var addImm22 = Instr(Mnemonic.addl, r1, IsImm22, r3_2, qp);
            var compare = Nyi("compare");

            var sysMemMgmt = Nyi("Sys/Mem Mgmt");
            var intLdRegGetf = Nyi("Int Ld + Reg/getf");
            var intLdStImm = Nyi("Int Ld/St +Imm");
            var fpLdStRegSetf = Nyi("FP Ld/St +Reg/setf");
            var fpLdStImm = Nyi("FP Ld/St +Imm");
            var fpMisc = Nyi("FP Misc");
            var fpCompare = Nyi("FP Compare");
            var fpClass = Nyi("FP Class");
            var fma = Nyi("fma");
            var fms = Nyi("fms"); 
            var fnma = Nyi("fnma");
            var fselextXma = Nyi("fselext/xma");

            var miscIndirectBranch = Nyi("Misc/IndirectBranch");
            var indirectCall = Nyi("IndirectCall");
            var indirectPredictNop = Nyi("Indirect Predict/Nop");
            var ipRelativeBranch = Nyi("IP-relative Branch");
            var ipRelativeCall = Nyi("IP-relative Call");
            var ipRelativePredict = Nyi("IP-relative Predict");
            var movl = Nyi("movl");

            var longBranch = Nyi("Long Branch");
            var longCall = Nyi("Long Call");

            Idecoders = new UnitDecoder('i', WideMask(37, 4, "  I",
                misc,
                invalid,
                invalid,
                invalid,

                deposit,
                shiftTestBit,
                invalid,
                mmMpyShift,

                aluMmAlu,
                addImm22,
                invalid,
                invalid,

                compare,
                compare,
                compare,
                invalid));

            Mdecoders = new UnitDecoder('m', WideMask(37, 4, "  M",
                sysMemMgmt3bit,
                sysMemMgmt,
                invalid,
                invalid,

                intLdRegGetf,
                intLdStImm,
                fpLdStRegSetf,
                fpLdStImm,

                aluMmAlu,
                addImm22,
                invalid,
                invalid,

                compare,
                compare,
                compare,
                invalid));

            Fdecoders = new UnitDecoder('f', WideMask(37, 4, "  F",
                fpMisc,
                fpMisc,
                invalid,
                invalid,

                fpCompare,
                fpClass,
                invalid,
                invalid,

                fma,
                fma,
                fms,
                fms,

                fnma,
                fnma,
                fselextXma,
                invalid));

            Bdecoders = new UnitDecoder('b', WideMask(37, 4, "  B",
                miscIndirectBranch,
                indirectCall,
                indirectPredictNop,
                nop,

                ipRelativeBranch,
                ipRelativeCall,
                nop,
                ipRelativePredict,

                reserved,
                reserved,
                reserved,
                reserved,

                reserved,
                reserved,
                reserved,
                reserved));

            Ldecoders = new UnitDecoder('l', WideMask(37, 4, "  L",
                misc,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                movl,
                invalid,

                Nyi("br?"),
                Nyi("br?"),
                Nyi("br?"),
                Nyi("br?"),

                longBranch,
                longCall,
                Nyi("br?"),
                Nyi("br?")));

            Xdecoders = new UnitDecoder('x', WideMask(37, 4, "  X",
                misc,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                movl,
                invalid,

                Nyi("br?"),
                Nyi("br?"),
                Nyi("br?"),
                Nyi("br?"),

                longBranch,
                longCall,
                Nyi("br?"),
                Nyi("br?")));
        }

        private static TemplateDecoders[] BuildTemplateDecoders()
        {
            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);

            var invalidBundle = new TemplateDecoders(invalid, invalid, invalid, 0);
            WideDecoder b = Bdecoders;
            WideDecoder i = Idecoders;
            WideDecoder f = Fdecoders;
            WideDecoder l = Ldecoders;
            WideDecoder m = Mdecoders;
            WideDecoder x = Xdecoders;
            return new TemplateDecoders[32] 
            {
                new TemplateDecoders(m,i,i,0b000),
                new TemplateDecoders(m,i,i,0b001),
                new TemplateDecoders(m,i,i,0b010),
                new TemplateDecoders(m,i,i,0b011),

                new TemplateDecoders(m,l,x,0b000),
                new TemplateDecoders(m,l,x,0b001),
                invalidBundle,
                invalidBundle,

                new TemplateDecoders(m,m,i,0b000),
                new TemplateDecoders(m,m,i,0b001),
                new TemplateDecoders(m,m,i,0b100),
                new TemplateDecoders(m,m,i,0b101),

                new TemplateDecoders(m,f,i,0b000),
                new TemplateDecoders(m,f,i,0b001),
                new TemplateDecoders(m,m,f,0b000),
                new TemplateDecoders(m,m,f,0b001),

                // 10
                new TemplateDecoders(m,i,b,0b000),
                new TemplateDecoders(m,i,b,0b001),
                new TemplateDecoders(m,b,b,0b000),
                new TemplateDecoders(m,b,b,0b001),

                invalidBundle,
                invalidBundle,
                new TemplateDecoders(b,b,b,0b000),
                new TemplateDecoders(b,b,b,0b001),
                
                new TemplateDecoders(m,m,b,0b000),
                new TemplateDecoders(m,m,b,0b001),
                invalidBundle,
                invalidBundle,

                new TemplateDecoders(m,f,b,0b000),
                new TemplateDecoders(m,f,b,0b001),
                invalidBundle,
                invalidBundle,
            };
        }
    }
}