#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.PaRisc
{
    public class PaRiscDisassembler : DisassemblerBase<PaRiscInstruction>
    {
        private static readonly MaskDecoder rootDecoder;
        private static readonly InstrDecoder invalid;

        private readonly PaRiscArchitecture arch;
        private readonly EndianImageReader rdr;

        private Address addr;
        private readonly List<MachineOperand> ops;

        public PaRiscDisassembler(PaRiscArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override PaRiscInstruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            this.ops.Clear();
            if (!rdr.TryReadBeUInt32(out uint uInstr))
                return null;
            var instr = rootDecoder.Decode(uInstr, this);
            instr.Address = this.addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        private static Mutator<PaRiscDisassembler> r(int bitPos, int bitLength)
        {
            var field = new Bitfield(32 - (bitPos + bitLength), bitLength);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                d.ops.Add(new RegisterOperand(Registers.GpRegs[iReg]));
                return true;
            };
        }
        private static readonly Mutator<PaRiscDisassembler> r1 = r(11, 5);
        private static readonly Mutator<PaRiscDisassembler> r2 = r(6, 5);
        private static readonly Mutator<PaRiscDisassembler> rt = r(27, 5);

        private static Mutator<PaRiscDisassembler> cf(int bitPos, ConditionOperand[] conds)
        {
            Debug.Assert(conds.Length == 16);
            var field = new Bitfield(32 - (bitPos + 4), 4);
            return (u, d) =>
            {
                var iCond = field.Read(u);
                var cond = conds[iCond];
                if (cond == null)
                    return false;
                if (cond.Type != ConditionType.Never)
                {
                    d.ops.Add(cond);
                }
                return true;
            };
        }
        private static Mutator<PaRiscDisassembler> cf16_add = cf(16, new[]
        {
            new ConditionOperand(ConditionType.Never, ""),
            new ConditionOperand(ConditionType.Eq, "="),
            new ConditionOperand(ConditionType.Lt, "<"),
            new ConditionOperand(ConditionType.Le, "<="),

            new ConditionOperand(ConditionType.Nuv, "nuv"),
            new ConditionOperand(ConditionType.Znv, "znv"),
            new ConditionOperand(ConditionType.Sv, "sv"),
            new ConditionOperand(ConditionType.Odd, "od"),

            new ConditionOperand(ConditionType.Tr, "tr"),
            new ConditionOperand(ConditionType.Ne, "<>"),
            new ConditionOperand(ConditionType.Ge, ">="),
            new ConditionOperand(ConditionType.Gt, ">"),

            new ConditionOperand(ConditionType.Uv, "uv"),
            new ConditionOperand(ConditionType.Vnz, "vnz"),
            new ConditionOperand(ConditionType.Nsv, "nsv"),
            new ConditionOperand(ConditionType.Even, "ev"),
        });

        private abstract class Decoder
        {
            public abstract PaRiscInstruction Decode(uint uInstr, PaRiscDisassembler dasm);
        }

        private class InstrDecoder : Decoder
        {
            private readonly InstrClass iclass;
            private readonly Opcode opcode;
            private readonly Mutator<PaRiscDisassembler>[] mutators;

            public InstrDecoder(InstrClass iclass, Opcode opcode, params Mutator<PaRiscDisassembler> [] mutators)
            {
                this.iclass = iclass;
                this.opcode = opcode;
                this.mutators = mutators;
            }

            public override PaRiscInstruction Decode(uint uInstr, PaRiscDisassembler dasm)
            {
                foreach (var m in mutators)
                {
                    if (!m(uInstr, dasm))
                        return invalid.Decode(uInstr, dasm);
                }
                return new PaRiscInstruction
                {
                    IClass = iclass,
                    Opcode = opcode,
                    Operands = dasm.ops.ToArray()
                };
            }
        }

        private class MaskDecoder : Decoder
        {
            private readonly Bitfield bitfield;
            private readonly Decoder[] decoders;

            public MaskDecoder(Bitfield bitfield, Decoder [] decoders)
            {
                this.bitfield = bitfield;
                this.decoders = decoders;
            }

            public override PaRiscInstruction Decode(uint uInstr, PaRiscDisassembler dasm)
            {
                uint code = bitfield.Read(uInstr);
                return decoders[code].Decode(uInstr, dasm);
            }
        }

        private class NyiDecoder : Decoder
        {
            private readonly string message;

            public NyiDecoder(string message)
            {
                this.message = message;
            }

            public override PaRiscInstruction Decode(uint uInstr, PaRiscDisassembler dasm)
            {
                EmitUnitTest(uInstr);
                return invalid.Decode(uInstr, dasm);
            }

            [Conditional("DEBUG")]
            private void EmitUnitTest(uint wInstr)
            {
                Console.WriteLine("    // {0}", message);
                Console.WriteLine("    [Test]");
                Console.WriteLine("    public void PaRiscDis_{0:X8}()", wInstr);
                Console.WriteLine("    {");
                Console.WriteLine("        AssertCode(\"@@@\", \"{0:X8}\");", wInstr);
                Console.WriteLine("    }");
                Console.WriteLine("");
            }
        }


        private static InstrDecoder Instr(Opcode opcode, params Mutator<PaRiscDisassembler> [] mutators)
        {
            return new InstrDecoder(InstrClass.Linear, opcode, mutators);
        }
        private static InstrDecoder Instr(Opcode opcode, InstrClass iclass, params Mutator<PaRiscDisassembler>[] mutators)
        {
            return new InstrDecoder(iclass, opcode, mutators);
        }

        // Note: the bit positions are big-endian to follow the PA-RISC manual.
        private static MaskDecoder Mask(
            int bitPos,
            int bitLength,
            params Decoder[] decoders)
        {
            Debug.Assert(1 << bitLength == decoders.Length, $"Expected {1 << bitLength} decoders but saw {decoders.Length}");
            return new MaskDecoder(new Bitfield(32 - (bitPos + bitLength), bitLength), decoders);
        }

        private static MaskDecoder Mask(
            int bitPos,
            int bitLength,
            Decoder defaultDecoder,
            params (int, Decoder)[] sparseDecoders)
        {
            var decoders = Enumerable.Range(0, 1 << bitLength).Select(n => defaultDecoder).ToArray();
            foreach (var (i, d) in sparseDecoders)
            {
                decoders[i] = d;
            }
            return new MaskDecoder(new Bitfield(32 - (bitPos + bitLength), bitLength), decoders);
        }

        private static NyiDecoder Nyi(string message)
        {
            return new NyiDecoder(message);
        }

        static PaRiscDisassembler()
        {
            invalid = Instr(Opcode.invalid, InstrClass.Invalid);

            var systemOp = Nyi("systemOp");
            var memMgmt = Nyi("memMgmt");
            var arithLog = Mask(20, 6, invalid,
                (0x18, Instr(Opcode.add, r1,r2,rt,cf16_add)),
                (0x38, Nyi("addo")),
                (0x1C, Nyi("addc")),
                (0x3C, Nyi("addco")),
                (0x19, Nyi("sh1add")),
                (0x39, Nyi("sh1addo")),
                (0x1A, Nyi("sh2add")),
                (0x3A, Nyi("sh2addo")),
                (0x1B, Nyi("sh3add")),
                (0x3B, Nyi("sh3addo")),
                (0x10, Nyi("sub")),
                (0x30, Nyi("subo")),
                (0x13, Nyi("subt")),
                (0x33, Nyi("subto")),
                (0x14, Nyi("subb")),
                (0x34, Nyi("subbo")),
                (0x11, Nyi("ds")),
                (0x00, Nyi("andcm")),
                (0x08, Nyi("and")),
                (0x09, Nyi("or")),
                (0x0A, Nyi("xor")),
                (0x0E, Nyi("uxor")),
                (0x22, Nyi("comclr")),
                (0x26, Nyi("uaddcm")),
                (0x27, Nyi("uaddcmt")),
                (0x28, Nyi("addl")),
                (0x29, Nyi("sh1addl")),
                (0x2A, Nyi("sh2addl")),
                (0x2B, Nyi("sh3addl")),
                (0x2E, Nyi("dcor")),
                (0x2F, Nyi("idcor")));

            var indexMem = Nyi("indexMem");
            var spopN = Nyi("spopN");
            var coprW = Nyi("coprW");
            var coprDw = Nyi("coprDw");
            var floatDecoder = Nyi("floatDecoder");
            var productSpecific = Nyi("productSpecific");
            var subi = Nyi("subi");
            var addit = Nyi("addit");
            var addi = Nyi("addi");
            var extract = Nyi("extract");
            var deposit = Nyi("deposit");
            var branch = Nyi("branch");


            rootDecoder = Mask(0, 6,
                systemOp,
                memMgmt,
                arithLog,
                indexMem,

                spopN,
                Nyi("diag"),
                Nyi("fmpyadd"),
                invalid,

                Nyi("ldil"),
                coprW,
                Nyi("addil"),
                coprDw,

                Nyi("copr"),
                Nyi("ldo"),
                floatDecoder,
                productSpecific,

                Nyi("ldb"),
                Nyi("ldh"),
                Nyi("ldw"),
                Nyi("ldwm"),

                invalid,
                invalid,
                invalid,
                invalid,

                Nyi("stb"),
                Nyi("sth"),
                Nyi("stw"),
                Nyi("stwm"),

                invalid,
                invalid,
                invalid,
                invalid,

                // 20
                Nyi("combt"),
                Nyi("comibt"),
                Nyi("combf"),
                Nyi("comibf"),

                Nyi("comiclr"),
                subi,
                Nyi("fmpysub"),
                invalid,

                Nyi("addbt"),
                Nyi("addibt"),
                Nyi("addbf"),
                Nyi("addibf"),

                addit,
                addi,
                invalid,
                invalid,

                // 30
                Nyi("bvb"),
                Nyi("bb"),
                Nyi("movb"),
                Nyi("movib"),

                extract,
                deposit,
                invalid,
                invalid,

                Nyi("be"),
                Nyi("ble"),
                branch,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid);
        }
    }
}
