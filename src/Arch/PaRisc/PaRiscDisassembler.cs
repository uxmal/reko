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
        private ConditionOperand cond;
        private int coprocessor;

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
            this.cond = null;
            this.coprocessor = -1;

            if (!rdr.TryReadBeUInt32(out uint uInstr))
                return null;
            var instr = rootDecoder.Decode(uInstr, this);
            instr.Address = this.addr;
            instr.Length = (int) (rdr.Address - addr);
            instr.IClass |= uInstr == 0 ? InstrClass.Zero : 0;
            return instr;
        }

        /// <summary>
        /// PA Risc instruction bits are numbered from the MSB to LSB.
        /// </summary>
        private static Bitfield BeField(int bitPos, int bitLength)
        {
            return new Bitfield(32 - (bitPos + bitLength), bitLength);
        }

        private static Bitfield[] BeFields(params (int bitPos, int bitLength)[] flds)
        {
            return flds.Select(f => BeField(f.bitPos, f.bitLength)).ToArray();
        }

        private static Mutator<PaRiscDisassembler> u(int bitPos, int bitLength, PrimitiveType dt)
        {
            var field = BeField(bitPos, bitLength);
            return (u, d) =>
            {
                var v = field.Read(u);
                d.ops.Add(new ImmediateOperand(Constant.Create(dt, v)));
                return true;
            };
        }
        private static Mutator<PaRiscDisassembler> u8(int bitPos, int bitLength) => u(bitPos, bitLength, PrimitiveType.Byte);
        private static Mutator<PaRiscDisassembler> u16(int bitPos, int bitLength) => u(bitPos, bitLength, PrimitiveType.Word16);

        private static Mutator<PaRiscDisassembler> s(int bitPos, int bitLength, PrimitiveType dt)
        {
            var field = BeField(bitPos, bitLength);
            return (u, d) =>
            {
                var v = field.ReadSigned(u);
                d.ops.Add(new ImmediateOperand(Constant.Create(dt, v)));
                return true;
            };
        }
        private static Mutator<PaRiscDisassembler> s32(int bitPos, int bitLength) => u(bitPos, bitLength, PrimitiveType.Int32);


        private static Mutator<PaRiscDisassembler> r(int bitPos, int bitLength)
        {
            var field = BeField(bitPos, bitLength);
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

        private static Mutator<PaRiscDisassembler> reg(RegisterStorage r)
        {
            return (u, d) =>
            {
                d.ops.Add(new RegisterOperand(r));
                return true;
            };
        }

        private static Mutator<PaRiscDisassembler> sr(int bitPos)
        {
            var field = BeField(bitPos, 3);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                d.ops.Add(new RegisterOperand(Registers.SpaceRegs[iReg]));
                return true;
            };
        }

        private static Mutator<PaRiscDisassembler> cf(int bitPos, int bitLen, ConditionOperand[] conds)
        {
            Debug.Assert(conds.Length == (1 << bitLen));
            var field = BeField(bitPos, bitLen);
            return (u, d) =>
            {
                var iCond = field.Read(u);
                var cond = conds[iCond];
                if (cond == null)
                    return false;
                if (cond.Type != ConditionType.Never)
                {
                    d.cond = cond;
                }
                return true;
            };
        }
        private static Mutator<PaRiscDisassembler> cf16_add = cf(16, 4, new[]
        {
            ConditionOperand.Never,
            ConditionOperand.Eq,
            ConditionOperand.Lt,
            ConditionOperand.Le,

            ConditionOperand.Nuv,
            ConditionOperand.Znv,
            ConditionOperand.Sv,
            ConditionOperand.Odd,

            ConditionOperand.Tr,
            ConditionOperand.Ne,
            ConditionOperand.Ge,
            ConditionOperand.Gt,

            ConditionOperand.Uv,
            ConditionOperand.Vnz,
            ConditionOperand.Nsv,
            ConditionOperand.Even,
        });
        private static Mutator<PaRiscDisassembler> cf16_log = cf(16, 4, new[]
        {
            ConditionOperand.Never,
            ConditionOperand.Eq,
            ConditionOperand.Lt,
            ConditionOperand.Le,

            null,
            null,
            null,
            ConditionOperand.Odd,

            ConditionOperand.Tr,
            ConditionOperand.Ne,
            ConditionOperand.Ge,
            ConditionOperand.Gt,

            null,
            null,
            null,
            ConditionOperand.Even,
        });
        private static Mutator<PaRiscDisassembler> cf16_cmp32_t = cf(16, 3, new[]
        {
            ConditionOperand.Never,
            ConditionOperand.Eq,
            ConditionOperand.Lt,
            ConditionOperand.Le,

            ConditionOperand.Ult,
            ConditionOperand.Ule,
            ConditionOperand.Sv,
            ConditionOperand.Odd,
        });
        private static Mutator<PaRiscDisassembler> cf16_cmp32_f = cf(16, 3, new[]
        {
            ConditionOperand.Tr,
            ConditionOperand.Ne,
            ConditionOperand.Ge,
            ConditionOperand.Gt,

            ConditionOperand.Uge,
            ConditionOperand.Ugt,
            ConditionOperand.Nsv,
            ConditionOperand.Even,
        });

        // Register indirect with displacement with space register
        private static Mutator<PaRiscDisassembler> M(PrimitiveType dt, int baseRegPos, Bitfield [] dispFields, Func<uint,Bitfield[],uint> permutator)
        {
            var baseRegField = BeField(baseRegPos, 5);
            var totalLength = dispFields.Sum(f => f.Length);
            return (u, d) =>
            {
                var disp = (int) Bits.SignExtend(permutator(u, dispFields), totalLength);
                var iBaseReg = baseRegField.Read(u);
                d.ops.Add(MemoryOperand.Indirect(dt, disp, Registers.GpRegs[iBaseReg]));
                return true;
            };
        }

        // Register indirect with displacement with space register
        private static Mutator<PaRiscDisassembler> M(PrimitiveType dt, int dispPos, int dispLen, int baseRegPos, int spacePos)
        {
            var dispField = BeField(dispPos, dispLen);
            var baseRegField = BeField(baseRegPos, 5);
            var spaceRegField = BeField(spacePos, 2);
            return (u, d) =>
            {
                var disp = dispField.ReadSigned(u);
                var iBaseReg = baseRegField.Read(u);
                var iSpaceReg = spaceRegField.Read(u);
                d.ops.Add(MemoryOperand.Indirect(dt, disp, Registers.GpRegs[iBaseReg], Registers.SpaceRegs[iSpaceReg]));
                return true;
            };
        }

        // Register indirect, with offset in multiple fields, shifted by the element size
        private static Mutator<PaRiscDisassembler> Msh(PrimitiveType dt, Bitfield[] fields, int baseRegPos, int spacePos)
        {
            var baseRegField = BeField(baseRegPos, 5);
            var spaceRegField = BeField(spacePos, 2);
            return (u, d) =>
            {
                var disp = Bitfield.ReadSignedFields(fields, u) * dt.Size;
                var iBaseReg = baseRegField.Read(u);
                var iSpaceReg = spaceRegField.Read(u);
                d.ops.Add(MemoryOperand.Indirect(dt, disp, Registers.GpRegs[iBaseReg], Registers.SpaceRegs[iSpaceReg]));
                return true;
            };
        }

        // Register indirect indexed
        private static Mutator<PaRiscDisassembler> Mx(PrimitiveType dt, int baseRegPos, int idxRegPos)
        {
            var baseRegField = BeField(baseRegPos, 5);
            var idxRegField = BeField(idxRegPos, 5);
            return (u, d) =>
            {
                var iBaseReg = baseRegField.Read(u);
                var iIdxReg = idxRegField.Read(u);
                d.ops.Add(MemoryOperand.Indexed(dt, Registers.GpRegs[iBaseReg], Registers.GpRegs[iIdxReg]));
                return true;
            };
        }

        private static Mutator<PaRiscDisassembler> Mx(PrimitiveType dt, int baseRegPos, int idxRegPos, int spaceRegPos)
        {
            var baseRegField = BeField(baseRegPos, 5);
            var idxRegField = BeField(idxRegPos, 5);
            var spaceRegField = BeField(spaceRegPos, 2);
            return (u, d) =>
            {
                var iBaseReg = baseRegField.Read(u);
                var iIdxReg = idxRegField.Read(u);
                var iSpaceRegField = spaceRegField.Read(u);
                d.ops.Add(MemoryOperand.Indexed(dt, Registers.GpRegs[iBaseReg], Registers.GpRegs[iIdxReg], Registers.SpaceRegs[iSpaceRegField]));
                return true;
            };
        }
        private static Mutator<PaRiscDisassembler> PcRel(Func<uint, Bitfield[],uint> permutator, Bitfield[] fields)
        {
            int totalWidth = fields.Sum(f => f.Length);
            return (u, d) =>
            {
                var offset = (int)Bits.SignExtend(permutator(u, fields), totalWidth) * 4 + 8;
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

        private static Mutator<PaRiscDisassembler> cop(int bitPos, int bitLength)
        {
            var field = BeField(bitPos, bitLength);
            return (u, d) =>
            {
                d.coprocessor = (int) field.Read(u);
                return true;
            };
        }

        private static bool Eq0(uint u) => u == 0;

        private static uint assemble_12(uint u, Bitfield[] fields)
        {
            // return(cat(y,x{10},x{0..9}))
            var x = fields[0].Read(u);
            var y = fields[1].Read(u);
            var p = y;
            p = (p << fields[0].Length) 
                | ((x << 10) & 0b1_00000_00000)
                | ((x >> 1) & 0b0_11111_11111);
            return p;
        }

        private static uint assemble_16(uint u, Bitfield[] fields)
        {
            //$TODO: 64-bit
            //if (PSW[W])
            //    return (cat(z, xor(z, x{ 0}),xor(z, x{ 1}),y,0{ 0..1}))
            var y = fields[1].Read(u);
            return Bits.SignExtend(y, 13);
        }

        private static uint assemble_17(uint u, Bitfield[] fields)
        {
            var x = fields[0].Read(u);
            var y = fields[1].Read(u);
            var z = fields[2].Read(u);
            var p = z;
            p = (p << fields[0].Length) | x;
            p = (p << fields[1].Length)
                | ((y << 10) & 0b1_00000_00000)
                | ((y >> 1)  & 0b0_11111_11111);
            return p;
        }

        private static uint assemble_22(uint u, Bitfield[] fields)
        {
            var a = fields[0].Read(u);
            var b = fields[1].Read(u);
            var c = fields[2].Read(u);
            var d = fields[3].Read(u);
            var p = d;
            p = (p << fields[0].Length) | a;
            p = (p << fields[1].Length) | b;
            p = (p << fields[2].Length)
                | ((c << 10) & 0b1_00000_00000)
                | ((c >> 1) & 0b0_11111_11111);
            return p;
        }

        private abstract class Decoder
        {
            public abstract PaRiscInstruction Decode(uint uInstr, PaRiscDisassembler dasm);

            protected void DumpMaskedInstruction(uint wInstr, uint shMask, string tag)
            {
                var hibit = 0x80000000u;
                var sb = new StringBuilder();
                for (int i = 0; i < 32; ++i)
                {
                    if ((shMask & hibit) != 0)
                    {
                        sb.Append((wInstr & hibit) != 0 ? '1' : '0');
                    }
                    else
                    {
                        sb.Append((wInstr & hibit) != 0 ? ':' : '.');
                    }
                    shMask <<= 1;
                    wInstr <<= 1;
                }
                if (!string.IsNullOrEmpty(tag))
                {
                    sb.AppendFormat(" {0}", tag);
                }
                Debug.Print(sb.ToString());
            }


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
                    Coprocessor = dasm.coprocessor,
                    Condition = dasm.cond,
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
                base.DumpMaskedInstruction(uInstr, bitfield.Mask << bitfield.Position, "");
                uint code = bitfield.Read(uInstr);
                return decoders[code].Decode(uInstr, dasm);
            }
        }

        private class ConditionalDecoder : Decoder
        {
            private readonly Bitfield field;
            private readonly Predicate<uint> predicate;
            private readonly Decoder trueDecoder;
            private readonly Decoder falseDecoder;

            public ConditionalDecoder(int bitPos, int bitLength, Predicate<uint> predicate, Decoder trueDecoder, Decoder falseDecoder)
            {
                this.field = BeField(bitPos, bitLength);
                this.predicate = predicate;
                this.trueDecoder = trueDecoder;
                this.falseDecoder = falseDecoder;
            }

            public override PaRiscInstruction Decode(uint uInstr, PaRiscDisassembler dasm)
            {
                base.DumpMaskedInstruction(uInstr, field.Mask << field.Position, "");
                var u = field.Read(uInstr);
                var cond = predicate(u);
                var decoder = cond ? trueDecoder : falseDecoder;
                return decoder.Decode(uInstr, dasm);
            }
        }

        private class NyiDecoder : Decoder
        {
            private readonly Opcode mnemonic;
            private readonly string message;

            public NyiDecoder(Opcode mnemonic, string message)
            {
                this.mnemonic = mnemonic;
                this.message = !string.IsNullOrEmpty(message) ? message : mnemonic.ToString();
            }

            public override PaRiscInstruction Decode(uint uInstr, PaRiscDisassembler dasm)
            {
                dasm.EmitUnitTest(
                    dasm.arch.Name,
                    uInstr.ToString("X8"),
                    message,
                    "PaRiscDis",
                    dasm.addr,
                    Console =>
                    {
                        Console.WriteLine($"    AssertCode(\"@@@\", 0x{uInstr:X8});");
                        Console.WriteLine();
                    });
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
                Console.WriteLine("    // Reko: a decoder for hppa instruction {0} at address(.*?) has not");
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
            return new MaskDecoder(BeField(bitPos, bitLength), decoders);
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
            return new MaskDecoder(BeField(bitPos, bitLength), decoders);
        }

        private static ConditionalDecoder Cond(
            int bitPos,
            int bitLength,
            Predicate<uint> predicate,
            Decoder trueDecoder,
            Decoder falseDecoder)
        {
            return new ConditionalDecoder(bitPos, bitLength, predicate, trueDecoder, falseDecoder);
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
                (0x85, Cond(16,2, Eq0,
                    Instr(Opcode.ldsid, r2,rt0),
                    Instr(Opcode.ldsid, sr(16),rt0))),
                (0xC1, Instr(Opcode.mtsp, r(11,5),sr(16))),
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

            var indexMem = Mask(19, 1,  // opc=3
                Mask(22, 4, invalid,
                    (0x0, Nyi(Opcode.ldb, "(index")),
                    (0x1, Nyi(Opcode.ldh, "(index")),
                    (0x2, Nyi(Opcode.ldw, "(index")),
                    (0x3, Nyi(Opcode.ldd, "(index")),
                    (0x4, Nyi(Opcode.ldda, "(index")),
                    (0x5, Nyi(Opcode.ldcd, "(index")),
                    (0x6, Nyi(Opcode.ldwa, "(index")),
                    (0x7, Nyi(Opcode.ldcw, "(index"))),
                Mask(22, 4, invalid,
                    (0x0, Nyi(Opcode.ldb, "(short")),
                    (0x1, Nyi(Opcode.ldh, "(short")),
                    (0x2, Instr(Opcode.ldw, M(PrimitiveType.Word32, 11,5, 6, 16), rt0)), //$TODO: cache completeion hints
                    (0x3, Nyi(Opcode.ldd, "(short")),
                    (0x4, Nyi(Opcode.ldda, "(short")),
                    (0x5, Nyi(Opcode.ldcd, "(short")),
                    (0x6, Nyi(Opcode.ldwa, "(short")),
                    (0x7, Nyi(Opcode.ldcw, "(short")),
                    (0x8, Nyi(Opcode.stb, "(short")),
                    (0x9, Instr(Opcode.sth, M(PrimitiveType.Word16, 11, 5, 6, 16), rt0)),
                    (0xA, Instr(Opcode.stw, M(PrimitiveType.Word32, 11, 5, 6, 16), rt0)),
                    (0xB, Nyi(Opcode.std, "(short")),
                    (0xC, Nyi(Opcode.stby, "(short")),
                    (0xD, Nyi(Opcode.stdby, "(short")),
                    (0xE, Nyi(Opcode.stwa, "(short")),
                    (0xF, Nyi(Opcode.stda, "(short"))));

            var spopN = Nyi("spopN");
            var coprW = Nyi("coprW");
            var floatDecoder = Nyi("floatDecoder");
            var productSpecific = Nyi("productSpecific");
            var subi = Nyi("subi");
            var addit = Nyi("addit");
            var addi = Nyi("addi");
            var extract = Nyi("extract");
            var deposit = Nyi("deposit");

            var branch = Mask(16, 3,
                Instr(Opcode.bl, PcRel(assemble_17, BeFields((11,5),(19,11),(31,1))),r2, Annul(30)),
                Nyi(Opcode.gate, ""),
                Instr(Opcode.blr, r1,r2,Annul(30)),
                Nyi(Opcode.blrpush, ""),

                invalid,
                Instr(Opcode.bl, PcRel(assemble_22, BeFields((6,5),(11,5),(19,11),(31,1))),reg(Registers.GpRegs[2]), Annul(30)),
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

                Instr(Opcode.ldil, u(11, 21, PrimitiveType.Word32), r2),
                coprW,
                Nyi(Opcode.addil, ""),
                Mask(19, 1,
                    Instr(Opcode.cstd, cop(23, 3), rt0,Mx(PrimitiveType.Real64,6,11,16)),
                    Instr(Opcode.cstd, cop(23, 3), rt0,Msh(PrimitiveType.Real64,BeFields((11,5)), 6, 16))),

                Nyi(Opcode.copr, ""),
                Instr(Opcode.ldo, M(PrimitiveType.Word32, 6, BeFields((16,2),(18,14)), assemble_16),r1),
                floatDecoder,
                productSpecific,
                
                // 10
                Nyi(Opcode.ldb, ""),
                Nyi(Opcode.ldh, ""),
                Instr(Opcode.ldw, M(PrimitiveType.Word32, 18,14, 6, 16), r1),
                Nyi(Opcode.ldwm, ""),

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Opcode.stb, r1,M(PrimitiveType.Byte, 6, BeFields((16,2), (18,14)),assemble_16)),
                Instr(Opcode.sth, r1,M(PrimitiveType.Word16, 6, BeFields((16,2), (18,14)),assemble_16)),
                Instr(Opcode.stw, r1,M(PrimitiveType.Word32, 18, 14, 6, 16)),
                Nyi(Opcode.stwm, ""),

                invalid,
                invalid,
                invalid,
                invalid,

                // 20
                Instr(Opcode.cmpb, cf16_cmp32_t,r1,r2,PcRel(assemble_12, BeFields((19,11),(31,1))), Annul(30)),
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
                Instr(Opcode.addibf, s32(11,5),r2,PcRel(assemble_12, BeFields((19,11),(31,1))),Annul(30)),

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

                Instr(Opcode.be, Msh(PrimitiveType.Ptr32, BeFields((11,5),(19,11),(31,1)),6,16),Annul(30)),
                Instr(Opcode.ble, Msh(PrimitiveType.Ptr32, BeFields((11,5),(19,11),(31,1)),6,16),Annul(30)),
                branch,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid);
        }
    }
}
