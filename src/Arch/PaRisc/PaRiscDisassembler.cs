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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
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
        private bool annul;

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
            this.annul = false;

            if (!rdr.TryReadBeUInt32(out uint uInstr))
                return null;
            var instr = rootDecoder.Decode(uInstr, this);
            instr.Address = this.addr;
            instr.Length = (int) (rdr.Address - addr);
            instr.IClass |= uInstr == 0 ? InstrClass.Zero : 0;
            return instr;
        }

        /// <summary>
        /// PA Risc instruction bits are numbers from the MSB to LSB.
        /// </summary>
        private static Bitfield PaRiscBitField(int bitPos, int bitLength)
        {
            return new Bitfield(32 - (bitPos + bitLength), bitLength);
        }

        private static Mutator<PaRiscDisassembler> u(int bitPos, int bitLength, PrimitiveType dt)
        {
            var field = PaRiscBitField(bitPos, bitLength);
            return (u, d) =>
            {
                var v = field.Read(u);
                d.ops.Add(new ImmediateOperand(Constant.Create(dt, v)));
                return true;
            };
        }
        private static Mutator<PaRiscDisassembler> u8(int bitPos, int bitLength) => u(bitPos, bitLength, PrimitiveType.Byte);
        private static Mutator<PaRiscDisassembler> u16(int bitPos, int bitLength) => u(bitPos, bitLength, PrimitiveType.Word16);


        private static Mutator<PaRiscDisassembler> r(int bitPos, int bitLength)
        {
            var field = PaRiscBitField(bitPos, bitLength);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                d.ops.Add(new RegisterOperand(Registers.GpRegs[iReg]));
                return true;
            };
        }
        private static readonly Mutator<PaRiscDisassembler> r1 = r(11, 5);
        private static readonly Mutator<PaRiscDisassembler> r2 = r(6, 5);
        private static readonly Mutator<PaRiscDisassembler> rt0 = r(27, 5);

        private static Mutator<PaRiscDisassembler> cf(int bitPos, ConditionOperand[] conds)
        {
            Debug.Assert(conds.Length == 16);
            var field = PaRiscBitField(bitPos, 4);
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
        private static Mutator<PaRiscDisassembler> cf16_log = cf(16, new[]
        {
            new ConditionOperand(ConditionType.Never, ""),
            new ConditionOperand(ConditionType.Eq, "="),
            new ConditionOperand(ConditionType.Lt, "<"),
            new ConditionOperand(ConditionType.Le, "<="),

            null,
            null,
            null,
            new ConditionOperand(ConditionType.Odd, "od"),

            new ConditionOperand(ConditionType.Tr, "tr"),
            new ConditionOperand(ConditionType.Ne, "<>"),
            new ConditionOperand(ConditionType.Ge, ">="),
            new ConditionOperand(ConditionType.Gt, ">"),

            null,
            null,
            null,
            new ConditionOperand(ConditionType.Even, "ev"),
        });

        // Register indirect with displacement
        private static Mutator<PaRiscDisassembler> M(PrimitiveType dt, int dispPos, int dispLen, int baseRegPos, int spacePos)
        {
            var dispField = PaRiscBitField(dispPos, dispLen);
            var baseRegField = PaRiscBitField(baseRegPos, 5);
            var spaceRegField = PaRiscBitField(spacePos, 2);
            return (u, d) =>
            {
                var disp = dispField.ReadSigned(u);
                var iBaseReg = baseRegField.Read(u);
                var iSpaceReg = spaceRegField.Read(u);
                d.ops.Add(MemoryOperand.Indirect(dt, disp, Registers.GpRegs[iBaseReg], Registers.SpaceRegs[iSpaceReg]));
                return true;
            };
        }

        // Register indirect indexed
        private static Mutator<PaRiscDisassembler> Mx(PrimitiveType dt, int baseRegPos, int idxRegPos)
        {
            var baseRegField = PaRiscBitField(baseRegPos, 5);
            var idxRegField = PaRiscBitField(idxRegPos, 5);
            return (u, d) =>
            {
                var iBaseReg = baseRegField.Read(u);
                var iIdxReg = idxRegField.Read(u);
                d.ops.Add(MemoryOperand.Indexed(dt, Registers.GpRegs[iBaseReg], Registers.GpRegs[iIdxReg]));
                return true;
            };

        }

        private static Mutator<PaRiscDisassembler> PcRel(params (int bitPos,int bitLength)[] flds)
        {
            var fields = flds.Select(x => PaRiscBitField(x.bitPos, x.bitLength)).ToArray();
            return (u, d) =>
            {
                var offset = Bitfield.ReadSignedFields(fields, u) + 8;
                var addrDst = d.addr + offset;
                d.ops.Add(AddressOperand.Create(addrDst));
                return true;
            };
        }

        private static Mutator<PaRiscDisassembler> Annul(int bitPos)
        {
            return (u, d) =>
            {
                d.annul = Bits.IsBitSet(u, 31 - bitPos);
                return true;

            };
        }

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
                    Operands = dasm.ops.ToArray(),
                    Annul = dasm.annul
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
            private readonly Opcode mnemonic;
            private readonly string message;

            public NyiDecoder(Opcode mnemonic, string message)
            {
                this.mnemonic = mnemonic;
                this.message = message;
            }

            public override PaRiscInstruction Decode(uint uInstr, PaRiscDisassembler dasm)
            {
                EmitUnitTest(uInstr);
                return new PaRiscInstruction
                {
                    IClass = 0,
                    Opcode = mnemonic,
                    Operands = new MachineOperand[0]
                };
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
            return new MaskDecoder(PaRiscBitField(bitPos, bitLength), decoders);
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
            return new MaskDecoder(PaRiscBitField(bitPos, bitLength), decoders);
        }

        private static NyiDecoder Nyi(string message)
        {
            return new NyiDecoder(Opcode.invalid, message);
        }

        private static NyiDecoder Nyi(Opcode opcode, string message)
        {
            return new NyiDecoder(opcode, message);
        }


        static PaRiscDisassembler()
        {
            invalid = Instr(Opcode.invalid, InstrClass.Invalid);

            var systemOp = Mask(19, 8, invalid,
                (0x00, Instr(Opcode.@break, InstrClass.Call|InstrClass.Transfer, u8(27,5), u16(6,13))),
                (0x20, Nyi(Opcode.sync, "")),
                (0x20, Nyi(Opcode.syncdma, "")),
                (0x60, Nyi(Opcode.rfi, "")),
                (0x65, Nyi(Opcode.rfir, "")),
                (0x6B, Nyi(Opcode.ssm, "")),
                (0x73, Nyi(Opcode.rsm, "")),
                (0xC3, Nyi(Opcode.mtsm, "")),
                (0x85, Nyi(Opcode.ldsid, "")),
                (0xC1, Nyi(Opcode.mtsp, "")),
                (0x25, Nyi(Opcode.mfsp, "")),
                (0xC2, Nyi(Opcode.mtctl, "")),
                (0x45, Nyi(Opcode.mfctl, "")));

            var memMgmt = Nyi("memMgmt");

            var arithLog = Mask(20, 6, invalid,
                (0x18, Instr(Opcode.add, r1,r2,rt0,cf16_add)),
                (0x38, Nyi(Opcode.addo, "")),
                (0x1C, Nyi(Opcode.addc, "")),
                (0x3C, Nyi(Opcode.addco, "")),
                (0x19, Nyi(Opcode.sh1add, "")),
                (0x39, Nyi(Opcode.sh1addo, "")),
                (0x1A, Nyi(Opcode.sh2add, "")),
                (0x3A, Nyi(Opcode.sh2addo, "")),
                (0x1B, Nyi(Opcode.sh3add, "")),
                (0x3B, Nyi(Opcode.sh3addo, "")),
                (0x10, Nyi(Opcode.sub, "")),
                (0x30, Nyi(Opcode.subo, "")),
                (0x13, Nyi(Opcode.subt, "")),
                (0x33, Nyi(Opcode.subto, "")),
                (0x14, Nyi(Opcode.subb, "")),
                (0x34, Nyi(Opcode.subbo, "")),
                (0x11, Nyi(Opcode.ds, "")),
                (0x00, Nyi(Opcode.andcm, "")),
                (0x08, Nyi(Opcode.and, "")),
                (0x09, Instr(Opcode.or, cf16_log,r2,r1,rt0)),
                (0x0A, Nyi(Opcode.xor, "")),
                (0x0E, Nyi(Opcode.uxor, "")),
                (0x22, Nyi(Opcode.comclr, "")),
                (0x26, Nyi(Opcode.uaddcm, "")),
                (0x27, Nyi(Opcode.uaddcmt, "")),
                (0x28, Nyi(Opcode.addl, "")),
                (0x29, Nyi(Opcode.sh1addl, "")),
                (0x2A, Nyi(Opcode.sh2addl, "")),
                (0x2B, Nyi(Opcode.sh3addl, "")),
                (0x2E, Nyi(Opcode.dcor, "")),
                (0x2F, Nyi(Opcode.idcor, "")));

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

            var branch = Mask(16, 3,
                Nyi(Opcode.bl, ""),
                Nyi(Opcode.gate, ""),
                Nyi(Opcode.blr, ""),
                Nyi(Opcode.blrpush, ""),

                invalid,
                Instr(Opcode.bl, PcRel((31,1),(11,5),(19,11),(6,5)),Annul(30)),
                Instr(Opcode.bv, Mx(PrimitiveType.Ptr32, 6, 11), Annul(30)),
                Nyi(Opcode.bve, ""));

            rootDecoder = Mask(0, 6,
                systemOp,
                memMgmt,
                arithLog,
                indexMem,

                spopN,
                Nyi(Opcode.diag, ""),
                Nyi(Opcode.fmpyadd, ""),
                invalid,

                Nyi(Opcode.ldil, ""),
                coprW,
                Nyi(Opcode.addil, ""),
                coprDw,

                Nyi(Opcode.copr, ""),
                Nyi(Opcode.ldo, ""),
                floatDecoder,
                productSpecific,

                Nyi(Opcode.ldb, ""),
                Nyi(Opcode.ldh, ""),
                Instr(Opcode.ldw, M(PrimitiveType.Word32, 18,14, 6, 16), r1),
                Nyi(Opcode.ldwm, ""),

                invalid,
                invalid,
                invalid,
                invalid,

                Nyi(Opcode.stb, ""),
                Nyi(Opcode.sth, ""),
                Nyi(Opcode.stw, ""),
                Nyi(Opcode.stwm, ""),

                invalid,
                invalid,
                invalid,
                invalid,

                // 20
                Nyi(Opcode.combt, ""),
                Nyi(Opcode.comibt, ""),
                Nyi(Opcode.combf, ""),
                Nyi(Opcode.comibf, ""),

                Nyi(Opcode.comiclr, ""),
                subi,
                Nyi(Opcode.fmpysub, ""),
                invalid,

                Nyi(Opcode.addbt, ""),
                Nyi(Opcode.addibt, ""),
                Nyi(Opcode.addbf, ""),
                Nyi(Opcode.addibf, ""),

                addit,
                addi,
                invalid,
                invalid,

                // 30
                Nyi(Opcode.bvb, ""),
                Nyi(Opcode.bb, ""),
                Nyi(Opcode.movb, ""),
                Nyi(Opcode.movib, ""),

                extract,
                deposit,
                invalid,
                invalid,

                Nyi(Opcode.be, ""),
                Nyi(Opcode.ble, ""),
                branch,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid);
        }
    }
}
