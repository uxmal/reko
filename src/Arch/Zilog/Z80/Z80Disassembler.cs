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
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Zilog.Z80
{
    using Decoder = Decoder<Z80Disassembler, Mnemonic, Z80Instruction>;
    using Mutator = Mutator<Z80Disassembler>;

    /// <summary>
    /// Disassembles both 8080 and Z80 instructions, with respective syntax.
    /// </summary>
    public class Z80Disassembler : DisassemblerBase<Z80Instruction, Mnemonic>
    {
#pragma warning disable IDE1006 // Naming Styles

        private readonly Z80Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;
        private RegisterStorage? IndexRegister;

        public Z80Disassembler(Z80Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.addr = rdr.Address;
            this.ops = new List<MachineOperand>();
        }

        public override Z80Instruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadByte(out byte op))
                return null;

            this.ops.Clear();
            this.IndexRegister = null;
            var decoder = decoders[op];
            var instr = decoder.Decode(op, this);
            instr.Address = this.addr;
            instr.Length = (int)(rdr.Address - addr);
            this.addr = rdr.Address;
            return instr;
        }

        public override Z80Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            var instr = new Z80Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray(),
            };
            return instr;
        }

        public override Z80Instruction CreateInvalidInstruction()
        {
            return new Z80Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.illegal,
                Address = this.addr,
                Operands = Array.Empty<MachineOperand>()
            };
        }

        public override Z80Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("Z80dis", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        private static readonly CondCode[] ConditionCode =
        {
            CondCode.nz,
            CondCode.z,
            CondCode.nc,
            CondCode.c,
            CondCode.po,
            CondCode.pe,
            CondCode.p,
            CondCode.m,
        };

        private static readonly RegisterStorage[] ByteRegister =
        {
            Registers.b,
            Registers.c,
            Registers.d,
            Registers.e,
            Registers.h,
            Registers.l,
            RegisterStorage.None,
            Registers.a
        };

        private class InstrDecoder : Decoder
        {
            private readonly InstrClass IClass;
            private readonly Mnemonic i8080mnemonic;
            private readonly Mnemonic Z80mnemonic;
            private readonly Mutator[] mutators;

            public InstrDecoder(InstrClass iclass, Mnemonic i8080, Mnemonic z80, params Mutator [] mutators)
            {
                this.IClass = iclass;
                this.i8080mnemonic = i8080;
                this.Z80mnemonic = z80;
                this.mutators = mutators;
            }

            public override Z80Instruction Decode(uint op, Z80Disassembler disasm)
            {
                foreach (var m in mutators)
                {
                    if (!m(op, disasm))
                        return disasm.CreateInvalidInstruction();
                }
                return disasm.MakeInstruction(IClass, Z80mnemonic);
            }
        }

        private class IndexPrefixDecoder : Decoder
        {
            private readonly RegisterStorage IndexRegister;

            public IndexPrefixDecoder(RegisterStorage idxReg)
            {
                this.IndexRegister = idxReg;
            }

            public override Z80Instruction Decode(uint bPrev, Z80Disassembler dasm)
            {
                dasm.IndexRegister = this.IndexRegister;
                if (!dasm.rdr.TryReadByte(out byte op))
                    return dasm.CreateInvalidInstruction();
                if (op == 0xCB)
                {
                    if (!dasm.rdr.TryReadByte(out byte uOffset))
                        return dasm.CreateInvalidInstruction();
                    sbyte offset = (sbyte) uOffset;
                    if (!dasm.rdr.TryReadByte(out op))
                        return dasm.CreateInvalidInstruction();
                    switch (op >> 6)
                    {
                    default:
                        //$TODO: some kind volunteer could implement the unofficial opcodes.
                        return dasm.CreateInvalidInstruction();
                    case 1:
                        dasm.ops.Add(Constant.Byte((byte)((op >> 3) & 0x07)));
                        dasm.ops.Add(new MemoryOperand(IndexRegister, offset, PrimitiveType.Byte));
                        return dasm.MakeInstruction(InstrClass.Linear, Mnemonic.bit);
                    case 2:
                        dasm.ops.Add(Constant.Byte((byte)((op >> 3) & 0x07)));
                        dasm.ops.Add(new MemoryOperand(IndexRegister, offset, PrimitiveType.Byte));
                        return dasm.MakeInstruction(InstrClass.Linear, Mnemonic.res);
                    case 3:
                        dasm.ops.Add(Constant.Byte((byte)((op >> 3) & 0x07)));
                        dasm.ops.Add(new MemoryOperand(IndexRegister, offset, PrimitiveType.Byte));
                        return dasm.MakeInstruction(InstrClass.Linear, Mnemonic.set);
                    }
                }
                else
                {
                    return decoders[op].Decode(op, dasm);
                }
            }
        }

        private class CbPrefixDecoder : Decoder
        {
            static readonly Mnemonic[] cbMnemonics = new Mnemonic[] {
                Mnemonic.rlc,
                Mnemonic.rrc,
                Mnemonic.rl,
                Mnemonic.rr,
                Mnemonic.sla,
                Mnemonic.sra,
                Mnemonic.sll,
                Mnemonic.srl,
            };

            static readonly Mutator[] cbFormats = new Mutator[] {
                R, R, R, R, R, R, Mb, R, 
            };

            public override Z80Instruction Decode(uint bPrev, Z80Disassembler dasm)
            {
                if (!dasm.rdr.TryReadByte(out var op))
                    return dasm.CreateInvalidInstruction();

                Mnemonic mnemonic;
                var y = (byte) ((op >> 3) & 0x07);
                switch (op >> 6)
                {
                default:
                    mnemonic = cbMnemonics[y];
                    break;
                case 1:
                    mnemonic = Mnemonic.bit;
                    dasm.ops.Add(Constant.Byte(y));
                    break;
                case 2:
                    mnemonic = Mnemonic.res;
                    dasm.ops.Add(Constant.Byte(y));
                    break;
                case 3:
                    mnemonic = Mnemonic.set;
                    dasm.ops.Add(Constant.Byte(y));
                    break;
                }
                if (!cbFormats[op & 0x07](op, dasm))
                    return dasm.CreateInvalidInstruction();
                return dasm.MakeInstruction(InstrClass.Linear, mnemonic);
            }
        }

        private class EdPrefixDecoder : Decoder
        {
            public override Z80Instruction Decode(uint bPrev, Z80Disassembler disasm)
            {
                if (!disasm.rdr.TryReadByte(out var op2))
                    return disasm.CreateInvalidInstruction();
                Decoder decoder;
                if (0x40 <= op2 && op2 < 0x80)
                    decoder = edDecoders[op2 - 0x40];
                else if (0xA0 <= op2 && op2 < 0xC0)
                    decoder = edDecoders[op2 - 0x60];
                else
                    return disasm.CreateInvalidInstruction();
                return decoder.Decode(op2, disasm);
            }
        }

        private static InstrDecoder Instr(Mnemonic op8080, Mnemonic opZ80, params Mutator[] mutators)
        {
            return new InstrDecoder(InstrClass.Linear, op8080, opZ80, mutators);
        }

        private static InstrDecoder Instr(Mnemonic op8080, Mnemonic opZ80, InstrClass iclass)
        {
            return new InstrDecoder(iclass, op8080, opZ80);
        }

        private static InstrDecoder Instr(Mnemonic op8080, Mnemonic opZ80, Mutator m0, InstrClass iclass)
        {
            return new InstrDecoder(iclass, op8080, opZ80, m0);
        }

        private static InstrDecoder Instr(Mnemonic op8080, Mnemonic opZ80, Mutator m0, Mutator m1, InstrClass iclass)
        {
            return new InstrDecoder(iclass, op8080, opZ80, m0, m1);
        }

        #region Mutators

        private static bool a(uint op, Z80Disassembler dasm)
        {
            dasm.ops.Add(Registers.a);
            return true;
        }

        // Absolute memory address.
        private static bool A(uint op, Z80Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort us))
                return false;
            dasm.ops.Add(Address.Ptr16(us));
            return true;
        }

        private static bool B(uint op, Z80Disassembler dasm)
        {
            dasm.ops.Add(new MemoryOperand(Registers.bc, PrimitiveType.Byte));
            return true;
        }

        private static bool D(uint op, Z80Disassembler dasm)
        {
            dasm.ops.Add(new MemoryOperand(Registers.de, PrimitiveType.Byte));
            return true;
        }

        // memory access using HL
        private static bool Hb(uint op, Z80Disassembler dasm)
        {
            dasm.ops.Add(new MemoryOperand(
                dasm.IndexRegister ?? Registers.hl, 
                PrimitiveType.Byte));
            return true;
        }

        private static bool C(uint op, Z80Disassembler dasm)
        {
            dasm.ops.Add(ConditionOperand.Create(ConditionCode[(op >> 3) & 7]));
            return true;
        }

        private static bool Q(uint op, Z80Disassembler dasm)
        {
            dasm.ops.Add(ConditionOperand.Create(ConditionCode[(op >> 3) & 3]));
            return true;
        }

        // register encoded in bits 3..5 of op
        private static bool r(uint op, Z80Disassembler dasm)
        {
            dasm.ops.Add(ByteRegister[(op >> 3)&7]);
            return true;
        }

        // register encoded in bits 0..2 of op.
        private static bool R(uint op, Z80Disassembler dasm)
        {
            dasm.ops.Add(ByteRegister[op & 7]);
            return true;
        }

        // Literal registers
        private static bool Li(uint op, Z80Disassembler dasm)
        {
            dasm.ops.Add(Registers.i);
            return true;
        }

        private static bool Lr(uint op, Z80Disassembler dasm)
        {
            dasm.ops.Add(Registers.r);
            return true;
        }

        private static Mutator x(uint imm) {
            return (u, d) =>
            {
                d.ops.Add(Constant.Byte((byte)imm));
                return true;
            };
        }

        // Relative jump
        private static bool Jb(uint op, Z80Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out var bOffset))
                return false;

            int offset = (sbyte) bOffset;
            dasm.ops.Add(dasm.rdr.Address + offset);
            return true;
        }

        private static bool Sw(uint op, Z80Disassembler dasm)
        {
            dasm.ops.Add(new MemoryOperand(Registers.sp, PrimitiveType.Word16));
            return true;
        }

        private static bool Wa(uint op, Z80Disassembler dasm)
        {
            dasm.ops.Add(Registers.af);
            return true;
        }

        private static bool Wb(uint op, Z80Disassembler dasm)
        {
            dasm.ops.Add(Registers.bc);
            return true;
        }

        private static bool Wd(uint op, Z80Disassembler dasm)
        {
            dasm.ops.Add(Registers.de);
            return true;
        }

        private static bool Wh(uint op, Z80Disassembler dasm)
        {
            dasm.ops.Add(dasm.IndexRegister ?? Registers.hl);
            return true;
        }

        private static bool Ws(uint op, Z80Disassembler dasm)
        {
            dasm.ops.Add(Registers.sp);
            return true;
        }


        private static bool Ib(uint op, Z80Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte imm))
                return false;
            dasm.ops.Add(Constant.Byte(imm));
            return true;
        }

        private static bool Iw(uint op, Z80Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort imm))
                return false;
            dasm.ops.Add(Constant.Word16(imm));
            return true;
        }

        private static Mutator M(PrimitiveType w)
        {
            return (op, dasm) =>
            {
                RegisterStorage baseReg = Registers.hl;
                sbyte offset = 0;
                if (dasm.IndexRegister is not null)
                {
                    baseReg = dasm.IndexRegister;
                    if (!dasm.rdr.TryReadByte(out byte bOff))
                        return false;
                    offset = (sbyte) bOff;
                }
                dasm.ops.Add(new MemoryOperand(baseReg, offset, w));
                return true;
            };
        }
        private static Mutator Mb => M(PrimitiveType.Byte);
        private static Mutator Mw => M(PrimitiveType.Word16);

        private static bool mc(uint op, Z80Disassembler dasm)
        {
            dasm.ops.Add(new MemoryOperand(Registers.c, PrimitiveType.Byte));
            return true;
        }

        private static bool Ob(uint op, Z80Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort dir))
                return false;
            dasm.ops.Add(new MemoryOperand(Constant.Word16(dir), PrimitiveType.Byte));
            return true;
        }

        private static bool ob(uint op, Z80Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte dir))
                return false;
            dasm.ops.Add(new MemoryOperand(Constant.Word16(dir), PrimitiveType.Byte));
            return true;
        }

        private static bool Ow(uint op, Z80Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort dir))
                return false;
            dasm.ops.Add(new MemoryOperand(Constant.Word16(dir), PrimitiveType.Word16));
            return true;
        }

        #endregion

        /// <summary>
        /// References:
        /// http://wikiti.brandonw.net/index.php?title=Z80_Instruction_Set
        /// http://www.zophar.net/fileuploads/2/10807fvllz/z80-1.txt
        /// </summary>
        private static readonly Decoder[] decoders = new Decoder[]
        {
            // 00
            Instr(Mnemonic.nop, Mnemonic.nop, InstrClass.Zero|InstrClass.Linear|InstrClass.Padding),
            Instr(Mnemonic.lxi, Mnemonic.ld, Wb,Iw),
            Instr(Mnemonic.stax, Mnemonic.ld, B,a),
            Instr(Mnemonic.inx, Mnemonic.inc, Wb),
            Instr(Mnemonic.inr, Mnemonic.inc, r),
            Instr(Mnemonic.dcr, Mnemonic.dec, r),
            Instr(Mnemonic.mvi, Mnemonic.ld, r,Ib),
            Instr(Mnemonic.illegal, Mnemonic.rlca),

            Instr(Mnemonic.illegal, Mnemonic.ex_af),
            Instr(Mnemonic.dad, Mnemonic.add, Wh,Wb),
            Instr(Mnemonic.ldax, Mnemonic.ld, a,B),
            Instr(Mnemonic.dcx, Mnemonic.dec, Wb),
            Instr(Mnemonic.inr, Mnemonic.inc, r),
            Instr(Mnemonic.dcr, Mnemonic.dec, r),
            Instr(Mnemonic.mvi, Mnemonic.ld, r,Ib),
            Instr(Mnemonic.illegal, Mnemonic.rrca),

            // 10
            Instr(Mnemonic.illegal, Mnemonic.djnz, Jb, InstrClass.ConditionalTransfer),
            Instr(Mnemonic.lxi, Mnemonic.ld, Wd,Iw),
            Instr(Mnemonic.stax, Mnemonic.ld, D,a),
            Instr(Mnemonic.inx, Mnemonic.inc, Wd),
            Instr(Mnemonic.inr, Mnemonic.inc, r),
            Instr(Mnemonic.dcr, Mnemonic.dec, r),
            Instr(Mnemonic.mvi, Mnemonic.ld, r,Ib),
            Instr(Mnemonic.illegal, Mnemonic.rla),

            Instr(Mnemonic.illegal, Mnemonic.jr, Jb, InstrClass.Transfer),
            Instr(Mnemonic.dad, Mnemonic.add, Wh,Wd),
            Instr(Mnemonic.ldax, Mnemonic.ld, a,D),
            Instr(Mnemonic.dcx, Mnemonic.dec, Wd),
            Instr(Mnemonic.inr, Mnemonic.inc, r),
            Instr(Mnemonic.dcr, Mnemonic.dec, r),
            Instr(Mnemonic.mvi, Mnemonic.ld, r,Ib),
            Instr(Mnemonic.illegal, Mnemonic.rra),

            // 20
            Instr(Mnemonic.illegal, Mnemonic.jr, Q, Jb,InstrClass.ConditionalTransfer),
            Instr(Mnemonic.lxi, Mnemonic.ld, Wh,Iw),
            Instr(Mnemonic.shld, Mnemonic.ld, Ow,Wh),
            Instr(Mnemonic.inx, Mnemonic.inc, Wh),
            Instr(Mnemonic.inr, Mnemonic.inc, r),
            Instr(Mnemonic.dcr, Mnemonic.dec, r),
            Instr(Mnemonic.mvi, Mnemonic.ld, r,Ib),
            Instr(Mnemonic.daa, Mnemonic.daa),

            Instr(Mnemonic.illegal, Mnemonic.jr, Q, Jb,InstrClass.ConditionalTransfer),
            Instr(Mnemonic.dad, Mnemonic.add, Wh,Wh),
            Instr(Mnemonic.lhld, Mnemonic.ld, Wh,Ow),
            Instr(Mnemonic.dcx, Mnemonic.dec, Wh),
            Instr(Mnemonic.inr, Mnemonic.inc, r),
            Instr(Mnemonic.dcr, Mnemonic.dec, r),
            Instr(Mnemonic.mvi, Mnemonic.ld, r,Ib),
            Instr(Mnemonic.cma, Mnemonic.cpl),

            // 30
            Instr(Mnemonic.illegal, Mnemonic.jr, Q, Jb,InstrClass.ConditionalTransfer),
            Instr(Mnemonic.lxi, Mnemonic.ld, Ws,Iw),
            Instr(Mnemonic.sta, Mnemonic.ld, Ob,a),
            Instr(Mnemonic.inx, Mnemonic.inc, Ws),
            Instr(Mnemonic.inr, Mnemonic.inc, Hb),
            Instr(Mnemonic.dcr, Mnemonic.dec, Hb),
            Instr(Mnemonic.mvi, Mnemonic.ld, Mb,Ib),
            Instr(Mnemonic.stc, Mnemonic.scf),

            Instr(Mnemonic.illegal, Mnemonic.jr, Q,Jb),
            Instr(Mnemonic.dad, Mnemonic.add, Wh,Ws),
            Instr(Mnemonic.lda, Mnemonic.ld, a,Ob),
            Instr(Mnemonic.dcx, Mnemonic.dec, Ws),
            Instr(Mnemonic.inr, Mnemonic.inc, r),
            Instr(Mnemonic.dcr, Mnemonic.dec, r),
            Instr(Mnemonic.mvi, Mnemonic.ld, r,Ib),
            Instr(Mnemonic.cmc, Mnemonic.ccf),

            // 40
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,Mb),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),

            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,Mb),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),

            // 50
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,Mb),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),

            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,Mb),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),

            // 60
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,Mb),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),

            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,Mb),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),

            // 70
            Instr(Mnemonic.mov, Mnemonic.ld, Mb,R),
            Instr(Mnemonic.mov, Mnemonic.ld, Mb,R),
            Instr(Mnemonic.mov, Mnemonic.ld, Mb,R),
            Instr(Mnemonic.mov, Mnemonic.ld, Mb,R),
            Instr(Mnemonic.mov, Mnemonic.ld, Mb,R),
            Instr(Mnemonic.mov, Mnemonic.ld, Mb,R),
            Instr(Mnemonic.hlt, Mnemonic.hlt, InstrClass.Terminates),
            Instr(Mnemonic.mov, Mnemonic.ld, Mb,R),

            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),
            Instr(Mnemonic.mov, Mnemonic.ld, r,Mb),
            Instr(Mnemonic.mov, Mnemonic.ld, r,R),

            // 80
            Instr(Mnemonic.add, Mnemonic.add, a,R),
            Instr(Mnemonic.add, Mnemonic.add, a,R),
            Instr(Mnemonic.add, Mnemonic.add, a,R),
            Instr(Mnemonic.add, Mnemonic.add, a,R),
            Instr(Mnemonic.add, Mnemonic.add, a,R),
            Instr(Mnemonic.add, Mnemonic.add, a,R),
            Instr(Mnemonic.add, Mnemonic.add, a,Mb),
            Instr(Mnemonic.add, Mnemonic.add, a,R),

            Instr(Mnemonic.adc, Mnemonic.adc, a,R),
            Instr(Mnemonic.adc, Mnemonic.adc, a,R),
            Instr(Mnemonic.adc, Mnemonic.adc, a,R),
            Instr(Mnemonic.adc, Mnemonic.adc, a,R),
            Instr(Mnemonic.adc, Mnemonic.adc, a,R),
            Instr(Mnemonic.adc, Mnemonic.adc, a,R),
            Instr(Mnemonic.adc, Mnemonic.adc, a,Mb),
            Instr(Mnemonic.adc, Mnemonic.adc, a,R),

            // 90
            Instr(Mnemonic.sub, Mnemonic.sub, a,R),
            Instr(Mnemonic.sub, Mnemonic.sub, a,R),
            Instr(Mnemonic.sub, Mnemonic.sub, a,R),
            Instr(Mnemonic.sub, Mnemonic.sub, a,R),
            Instr(Mnemonic.sub, Mnemonic.sub, a,R),
            Instr(Mnemonic.sub, Mnemonic.sub, a,R),
            Instr(Mnemonic.sub, Mnemonic.sub, a,Mb),
            Instr(Mnemonic.sub, Mnemonic.sub, a,R),

            Instr(Mnemonic.sbb, Mnemonic.sbc, a,R),
            Instr(Mnemonic.sbb, Mnemonic.sbc, a,R),
            Instr(Mnemonic.sbb, Mnemonic.sbc, a,R),
            Instr(Mnemonic.sbb, Mnemonic.sbc, a,R),
            Instr(Mnemonic.sbb, Mnemonic.sbc, a,R),
            Instr(Mnemonic.sbb, Mnemonic.sbc, a,R),
            Instr(Mnemonic.sbb, Mnemonic.sbc, a,Mb),
            Instr(Mnemonic.sbb, Mnemonic.sbc, a,R),

            // A0
            Instr(Mnemonic.ana, Mnemonic.and, a,R),
            Instr(Mnemonic.ana, Mnemonic.and, a,R),
            Instr(Mnemonic.ana, Mnemonic.and, a,R),
            Instr(Mnemonic.ana, Mnemonic.and, a,R),
            Instr(Mnemonic.ana, Mnemonic.and, a,R),
            Instr(Mnemonic.ana, Mnemonic.and, a,R),
            Instr(Mnemonic.ana, Mnemonic.and, a,Mb),
            Instr(Mnemonic.ana, Mnemonic.and, a,R),

            Instr(Mnemonic.xra, Mnemonic.xor, a,R),
            Instr(Mnemonic.xra, Mnemonic.xor, a,R),
            Instr(Mnemonic.xra, Mnemonic.xor, a,R),
            Instr(Mnemonic.xra, Mnemonic.xor, a,R),
            Instr(Mnemonic.xra, Mnemonic.xor, a,R),
            Instr(Mnemonic.xra, Mnemonic.xor, a,R),
            Instr(Mnemonic.xra, Mnemonic.xor, a,Mb),
            Instr(Mnemonic.xra, Mnemonic.xor, a,R),

            // B0
            Instr(Mnemonic.ora, Mnemonic.or, a,R),
            Instr(Mnemonic.ora, Mnemonic.or, a,R),
            Instr(Mnemonic.ora, Mnemonic.or, a,R),
            Instr(Mnemonic.ora, Mnemonic.or, a,R),
            Instr(Mnemonic.ora, Mnemonic.or, a,R),
            Instr(Mnemonic.ora, Mnemonic.or, a,R),
            Instr(Mnemonic.ora, Mnemonic.or, a,Mb),
            Instr(Mnemonic.ora, Mnemonic.or, a,R),

            Instr(Mnemonic.cmp, Mnemonic.cp, a,R),
            Instr(Mnemonic.cmp, Mnemonic.cp, a,R),
            Instr(Mnemonic.cmp, Mnemonic.cp, a,R),
            Instr(Mnemonic.cmp, Mnemonic.cp, a,R),
            Instr(Mnemonic.cmp, Mnemonic.cp, a,R),
            Instr(Mnemonic.cmp, Mnemonic.cp, a,R),
            Instr(Mnemonic.cmp, Mnemonic.cp, a,Mb),
            Instr(Mnemonic.cmp, Mnemonic.cp, a,R),

            // C0
            Instr(Mnemonic.illegal, Mnemonic.ret, C, InstrClass.ConditionalTransfer|InstrClass.Return),
            Instr(Mnemonic.illegal, Mnemonic.pop, Wb),
            Instr(Mnemonic.jnz, Mnemonic.jp, C, A,InstrClass.ConditionalTransfer),
            Instr(Mnemonic.jmp, Mnemonic.jp, A, InstrClass.Transfer),
            Instr(Mnemonic.illegal, Mnemonic.call, C, A,InstrClass.ConditionalTransfer|InstrClass.Call),
            Instr(Mnemonic.illegal, Mnemonic.push, Wb),
            Instr(Mnemonic.adi, Mnemonic.add, a,Ib),
            Instr(Mnemonic.illegal, Mnemonic.rst, x(00), InstrClass.Transfer|InstrClass.Call),

            Instr(Mnemonic.illegal, Mnemonic.ret, C, InstrClass.ConditionalTransfer|InstrClass.Return),
            Instr(Mnemonic.illegal, Mnemonic.ret, InstrClass.Transfer|InstrClass.Return),
            Instr(Mnemonic.jz, Mnemonic.jp, C, A,InstrClass.ConditionalTransfer),
            new CbPrefixDecoder(),
            Instr(Mnemonic.illegal, Mnemonic.call, C, A,InstrClass.ConditionalTransfer|InstrClass.Call),
            Instr(Mnemonic.illegal, Mnemonic.call, A, InstrClass.Transfer|InstrClass.Call),
            Instr(Mnemonic.aci, Mnemonic.adc, a,Ib),
            Instr(Mnemonic.illegal, Mnemonic.rst, x(0x08), InstrClass.Transfer|InstrClass.Call),

            // D0
            Instr(Mnemonic.illegal, Mnemonic.ret, C, InstrClass.ConditionalTransfer|InstrClass.Return),
            Instr(Mnemonic.illegal, Mnemonic.pop, Wd),
            Instr(Mnemonic.jnc, Mnemonic.jp, C, A,InstrClass.ConditionalTransfer),
            Instr(Mnemonic.illegal, Mnemonic.@out, ob,a),
            Instr(Mnemonic.illegal, Mnemonic.call, C, A,InstrClass.ConditionalTransfer|InstrClass.Call),
            Instr(Mnemonic.illegal, Mnemonic.push, Wd),
            Instr(Mnemonic.sui, Mnemonic.sub, a,Ib),
            Instr(Mnemonic.illegal, Mnemonic.rst, x(0x10), InstrClass.Transfer|InstrClass.Call),

            Instr(Mnemonic.illegal, Mnemonic.ret, C, InstrClass.ConditionalTransfer|InstrClass.Return),
            Instr(Mnemonic.illegal, Mnemonic.exx),
            Instr(Mnemonic.jc, Mnemonic.jp, C, A,InstrClass.ConditionalTransfer),
            Instr(Mnemonic.illegal, Mnemonic.@in, a,ob),
            Instr(Mnemonic.illegal, Mnemonic.call, C, A,InstrClass.ConditionalTransfer|InstrClass.Call),
            new IndexPrefixDecoder(Registers.ix),
            Instr(Mnemonic.sbi, Mnemonic.sbc, a,Ib),
            Instr(Mnemonic.illegal, Mnemonic.rst, x(0x18), InstrClass.Transfer|InstrClass.Call),

            // E0
            Instr(Mnemonic.illegal, Mnemonic.ret, C, InstrClass.ConditionalTransfer|InstrClass.Return),
            Instr(Mnemonic.illegal, Mnemonic.pop, Wh),
            Instr(Mnemonic.jpo, Mnemonic.jp, C, A,InstrClass.ConditionalTransfer),
            Instr(Mnemonic.illegal, Mnemonic.ex, Sw,Wh),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.push, Wh),
            Instr(Mnemonic.illegal, Mnemonic.add, a,Ib),
            Instr(Mnemonic.illegal, Mnemonic.rst, x(0x20),InstrClass.Transfer|InstrClass.Call),

            Instr(Mnemonic.illegal, Mnemonic.add, Ws,D),
            Instr(Mnemonic.pchl, Mnemonic.jp, Mw, InstrClass.Transfer),
            Instr(Mnemonic.jpe, Mnemonic.jp, C, A,InstrClass.ConditionalTransfer),
            Instr(Mnemonic.illegal, Mnemonic.ex, Wd,Wh),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            new EdPrefixDecoder(),
            Instr(Mnemonic.illegal, Mnemonic.xor, a,Ib),
            Instr(Mnemonic.illegal, Mnemonic.rst, x(0x28), InstrClass.Transfer|InstrClass.Call),

            // F0
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.pop, Wa),
            Instr(Mnemonic.jp, Mnemonic.jp, C, A,InstrClass.ConditionalTransfer),
            Instr(Mnemonic.di, Mnemonic.di),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.push, Wa),
            Instr(Mnemonic.illegal, Mnemonic.or, a,Ib),
            Instr(Mnemonic.illegal, Mnemonic.rst, x(0x30), InstrClass.Transfer|InstrClass.Call),

            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.sphl, Mnemonic.ld, Ws,Wh),
            Instr(Mnemonic.jm, Mnemonic.jp, C, A,InstrClass.ConditionalTransfer),
            Instr(Mnemonic.ei, Mnemonic.ei),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            new IndexPrefixDecoder(Registers.iy),
            Instr(Mnemonic.illegal, Mnemonic.cp, a,Ib),
            Instr(Mnemonic.illegal, Mnemonic.rst, x(0x38), InstrClass.Transfer|InstrClass.Call),
        };

        private static readonly Decoder[] edDecoders = new Decoder[] 
        {
            // 40
            Instr(Mnemonic.illegal, Mnemonic.@in, r,mc), 
            Instr(Mnemonic.illegal, Mnemonic.@out, mc,r), 
            Instr(Mnemonic.illegal, Mnemonic.sbc,  Wh,Wb),
            Instr(Mnemonic.illegal, Mnemonic.ld,  Ow,Wb),
            Instr(Mnemonic.illegal, Mnemonic.neg  ),
            Instr(Mnemonic.illegal, Mnemonic.retn, InstrClass.Transfer|InstrClass.Return),
            Instr(Mnemonic.illegal, Mnemonic.im,  x(0)),
            Instr(Mnemonic.illegal, Mnemonic.ld,  Li,a),

            Instr(Mnemonic.illegal, Mnemonic.@in,  r,mc),
            Instr(Mnemonic.illegal, Mnemonic.@out,  mc,r),
            Instr(Mnemonic.illegal, Mnemonic.adc,  Wh,Wb),
            Instr(Mnemonic.illegal, Mnemonic.ld,  Wb,Ow),
            Instr(Mnemonic.illegal, Mnemonic.illegal  ),
            Instr(Mnemonic.illegal, Mnemonic.reti, InstrClass.Transfer|InstrClass.Return),
            Instr(Mnemonic.illegal, Mnemonic.illegal  ),
            Instr(Mnemonic.illegal, Mnemonic.ld,  Lr,a),
            
            // 50
            Instr(Mnemonic.illegal, Mnemonic.@in, r,mc), 
            Instr(Mnemonic.illegal, Mnemonic.@out, mc,r), 
            Instr(Mnemonic.illegal, Mnemonic.sbc,  Wh,Wd),
            Instr(Mnemonic.illegal, Mnemonic.ld,  Ow,Wd),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.im,  x(1)),
            Instr(Mnemonic.illegal, Mnemonic.ld,  a,Li),

            Instr(Mnemonic.illegal, Mnemonic.@in,  r,mc),
            Instr(Mnemonic.illegal, Mnemonic.@out,  mc,r),
            Instr(Mnemonic.illegal, Mnemonic.adc,  Wh,Wd),
            Instr(Mnemonic.illegal, Mnemonic.ld,  Wd,Ow),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.im,  x(2)),
            Instr(Mnemonic.illegal, Mnemonic.ld,  a,Lr),

            // 60
            Instr(Mnemonic.illegal, Mnemonic.@in, r,mc), 
            Instr(Mnemonic.illegal, Mnemonic.@out, mc,r), 
            Instr(Mnemonic.illegal, Mnemonic.sbc,  Wh,Wh),
            Instr(Mnemonic.illegal, Mnemonic.ld,  Ow,Wh),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.rrd),

            Instr(Mnemonic.illegal, Mnemonic.@in,  r,mc),
            Instr(Mnemonic.illegal, Mnemonic.@out,  mc,r),
            Instr(Mnemonic.illegal, Mnemonic.adc,  Wh,Wh),
            Instr(Mnemonic.illegal, Mnemonic.ld,  Wh,Ow),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.rld),
            
            // 70
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.sbc,  Wh,Ws),
            Instr(Mnemonic.illegal, Mnemonic.ld,  Ow,Ws),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),

            Instr(Mnemonic.illegal, Mnemonic.@in,  r,mc),
            Instr(Mnemonic.illegal, Mnemonic.@out,  mc,r),
            Instr(Mnemonic.illegal, Mnemonic.adc,  Wh,Ws),
            Instr(Mnemonic.illegal, Mnemonic.ld,  Ws,Ow),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
 
            // A0
            Instr(Mnemonic.illegal, Mnemonic.ldi),
            Instr(Mnemonic.illegal, Mnemonic.cpi),
            Instr(Mnemonic.illegal, Mnemonic.ini),
            Instr(Mnemonic.illegal, Mnemonic.outi),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),

            Instr(Mnemonic.illegal, Mnemonic.ldd),
            Instr(Mnemonic.illegal, Mnemonic.cpd),
            Instr(Mnemonic.illegal, Mnemonic.ind),
            Instr(Mnemonic.illegal, Mnemonic.outd),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),

            // B0
            Instr(Mnemonic.illegal, Mnemonic.ldir),
            Instr(Mnemonic.illegal, Mnemonic.cpir),
            Instr(Mnemonic.illegal, Mnemonic.inir),
            Instr(Mnemonic.illegal, Mnemonic.otir),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),

            Instr(Mnemonic.illegal, Mnemonic.lddr),
            Instr(Mnemonic.illegal, Mnemonic.cpdr),
            Instr(Mnemonic.illegal, Mnemonic.indr),
            Instr(Mnemonic.illegal, Mnemonic.otdr),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
            Instr(Mnemonic.illegal, Mnemonic.illegal),
        };

#if NEVER

---		LD	BC,(word)	ED4Bword	BC <- (word)
---		LD	DE,(word)	ED5Bword	DE <- (word)
---		LD	HL,(word)	ED6Bword	HL <- (word)
---		LD	SP,(word)	ED7Bword	SP <- (word)

---		LD	(word),BC	ED43word	(word) <- BC
---		LD	(word),DE	ED53word	(word) <- DE
---		LD	(word),HL	ED6Bword	(word) <- HL
---		LD	(word),SP	ED73word	(word) <- SP

Register Exchange Instructions

XCHG		EX	DE,HL		EB		HL <-> DE
XTHL		EX	(SP),HL		E3	    H <-> (SP+1); L <-> (SP)
---		EX	(SP),IX		DDE3	 IXh <-> (SP+1); IXl <-> (SP)
---		EX	(SP),IY		FDE3	 IYh <-> (SP+1); IYl <-> (SP)
---		EX	AF,AF'		08		AF <-> AF'
---		EXX			D9	BC/DE/HL <-> BC'/DE'/HL'


Double Word Add With Carry-In Instructions

---		ADC	HL,BC		ED4A		HL <- HL + BC + Carry
---		ADC	HL,DE		ED5A		HL <- HL + DE + Carry
---		ADC	HL,HL		ED6A		HL <- HL + HL + Carry
---		ADC	HL,SP		ED7A		HL <- HL + SP + Carry


Double Word Subtract With Borrow-In Instructions

---		SBC	HL,BC		ED42		HL <- HL - BC - Carry
---		SBC	HL,DE		ED52		HL <- HL - DE - Carry
---		SBC	HL,HL		ED62		HL <- HL - HL - Carry
---		SBC	HL,SP		ED72		HL <- HL - SP - Carry


Control Instructions

---		IM	0		ED46		---
---		IM	1		ED56		---
---		IM	2		ED5E		---
---		LD	I,A		ED47		Interrupt Page <- A
---		LD	A,I		ED57		A <- Interrupt Page
---		LD	R,A		ED4F		Refresh Register <- A
---		LD	A,R		ED5F		A <- Refresh Register

Increment Byte Instructions

INR	A	INC	A		3C		A <- A + 1
INR	B	INC	B		04		B <- B + 1
INR	C	INC	C		0C		C <- C + 1
INR	D	INC	D		14		D <- D + 1
INR	E	INC	E		1C		E <- E + 1
INR	H	INC	H		24		H <- H + 1
INR	L	INC	L		2C		L <- L + 1
INR	M	INC	(HL)		34		(HL) <- (HL) + 1
---		INC	(IX+index)	DD34index (IX+index) <- (IX+index) + 1
---		INC	(IY+index)	FD34index (IY+index) <- (IY+index) + 1


Decrement Byte Instructions

DCR	A	DEC	A		3D		A <- A - 1
DCR	B	DEC	B		05		B <- B - 1
DCR	C	DEC	C		0D		C <- C - 1
DCR	D	DEC	D		15		D <- D - 1
DCR	E	DEC	E		1D		E <- E - 1
DCR	H	DEC	H		25		H <- H - 1
DCR	L	DEC	L		2D		L <- L - 1
DCR	M	DEC	(HL)		35		(HL) <- (HL) - 1
---		DEC	(IX+index)	DD35index (IX+index) <- (IX+index) - 1
---		DEC	(IY+index)	FD35index (IY+index) <- (IY+index) - 1


Increment Register Pair Instructions

INX	B	INC	BC		03		BC <- BC + 1
INX	D	INC	DE		13		DE <- DE + 1
INX	H	INC	HL		23		HL <- HL + 1
INX	SP	INC	SP		33		SP <- SP + 1
---		INC	IX		DD23		IX <- IX + 1
---		INC	IY		FD23		IY <- IY + 1


8080/Z80  Instruction  Set					Page 8

Decrement Register Pair Instructions

8080  Mnemonic	Z80  Mnemonic		Code		Operation

DCX	B	DEC	BC		0B		BC <- BC - 1
DCX	D	DEC	DE		1B		DE <- DE - 1
DCX	H	DEC	HL		2B		HL <- HL - 1
DCX	SP	DEC	SP		3B		SP <- SP - 1
---		DEC	IX		DD2B		IX <- IX - 1
---		DEC	IY		FD2B		IY <- IY - 1


Special Accumulator and Flag Instructions

DAA		DAA			27		---
CMA		CPL			2F		A <- NOT A
STC		SCF			37		CF (Carry Flag) <- 1
CMC		CCF			3F		CF (Carry Flag) <-
							NOT CF
---		NEG			ED44		A <- 0-A

Rotate Instructions

RLC		RLCA			07		---
RRC		RRCA			0F		---
RAL		RLA			17		---
RAR		RRA			1F		---
---		RLD			ED6F		---
---		RRD			ED67		---
---		RLC	A		CB07		---
---		RLC	B		CB00		---
---		RLC	C		CB01		---
---		RLC	D		CB02		---
---		RLC	E		CB03		---
---		RLC	H		CB04		---
---		RLC	L		CB05		---
---		RLC	(HL)		CB06		---
---		RLC	(IX+index)	DDCBindex06	---
---		RLC	(IY+index)	FDCBindex06	---

---		RL	A		CB17		---
---		RL	B		CB10		---
---		RL	C		CB11		---
---		RL	D		CB12		---
---		RL	E		CB13		---
---		RL	H		CB14		---
---		RL	L		CB15		---
---		RL	(HL)		CB16		---
---		RL	(IX+index)	DDCBindex16	---
---		RL	(IY+index)	FDCBindex16	---


8080/Z80  Instruction  Set					Page 9

Rotate Instructions

8080  Mnemonic	Z80  Mnemonic		Code		Operation

---		RRC	A		CB0F		---
---		RRC	B		CB08		---
---		RRC	C		CB09		---
---		RRC	D		CB0A		---
---		RRC	E		CB0B		---
---		RRC	H		CB0C		---
---		RRC	L		CB0D		---
---		RRC	(HL)		CB0E		---
---		RRC	(IX+index)	DDCBindex0E	---
---		RRC	(IY+index)	FDCBindex0E	---

---		RL	A		CB1F		---
---		RL	B		CB18		---
---		RL	C		CB19		---
---		RL	D		CB1A		---
---		RL	E		CB1B		---
---		RL	H		CB1C		---
---		RL	L		CB1D		---
---		RL	(HL)		CB1E		---
---		RL	(IX+index)	DDCBindex1E	---
---		RL	(IY+index)	FDCBindex1E	---


8080/Z80  Instruction  Set					Page 10

Logical Byte Instructions

8080  Mnemonic	Z80  Mnemonic		Code		Operation

ANA	A	AND	A		A7		A <- A AND A
ANA	B	AND	B		A0		A <- A AND B
ANA	C	AND	C		A1		A <- A AND C
ANA	D	AND	D		A2		A <- A AND D
ANA	E	AND	E		A3		A <- A AND E
ANA	H	AND	H		A4		A <- A AND H
ANA	L	AND	L		A5		A <- A AND L
ANA	M	AND	(HL)		A6		A <- A AND (HL)
---		AND	(IX+index)	DDA6index	A <- A AND (IX+index)
---		AND	(IY+index)	FDA6index	A <- A AND (IY+index)
ANI	byte	AND	byte		E6byte		A <- A AND byte

XRA	A	XOR	A		AF		A <- A XOR A
XRA	B	XOR	B		A8		A <- A XOR B
XRA	C	XOR	C		A9		A <- A XOR C
XRA	D	XOR	D		AA		A <- A XOR D
XRA	E	XOR	E		AB		A <- A XOR E
XRA	H	XOR	H		AC		A <- A XOR H
XRA	L	XOR	L		AD		A <- A XOR L
XRA	M	XOR	(HL)		AE		A <- A XOR (HL)
---		XOR	(IX+index)	DDAEindex	A <- A XOR (IX+index)
---		XOR	(IY+index)	FDAEindex	A <- A XOR (IY+index)
XRI	byte	XOR	byte		EEbyte		A <- A XOR byte

ORA	A	OR	A		B7		A <- A OR A
ORA	B	OR	B		B0		A <- A OR B
ORA	C	OR	C		B1		A <- A OR C
ORA	D	OR	D		B2		A <- A OR D
ORA	E	OR	E		B3		A <- A OR E
ORA	H	OR	H		B4		A <- A OR H
ORA	L	OR	L		B5		A <- A OR L
ORA	M	OR	(HL)		B6		A <- A OR (HL)
---		OR	(IX+index)	DDB6index	A <- A OR (IX+index)
---		OR	(IY+index)	FDB6index	A <- A OR (IY+index)
ORI	byte	OR	byte		F6byte		A <- A OR byte

CMP	A	CP	A		BF		A - A
CMP	B	CP	B		B8		A - B
CMP	C	CP	C		B9		A - C
CMP	D	CP	D		BA		A - D
CMP	E	CP	E		BB		A - E
CMP	H	CP	H		BC		A - H
CMP	L	CP	L		BD		A - L
CMP	M	CP	(HL)		BE		A - (HL)
---		CP	(IX+index)	DDBEindex	A - (IX+index)
---		CP	(IY+index)	FDBEindex	A - (IY+index)
CPI	byte	CP	byte		FEbyte		A - byte
---		CPI			EDA1	A - (HL);HL <- HL+1;BC <- BC-1
---		CPIR			EDB1	A - (HL);HL <- HL+1;BC <- BC-1
---		CPD			EDA9	A - (HL);HL <- HL-1;BC <- BC-1
---		CPDR			EDB9	A - (HL);HL <- HL-1;BC <- BC-1


8080/Z80  Instruction  Set					Page 11

Branch Control/Program Counter Load Instructions

8080  Mnemonic	Z80  Mnemonic		Code		Operation

JMP	address	JP	address		C3address	PC <- address
JNZ	address	JP	NZ,address	C2address   	If NZ, PC <- address
JZ	address	JP	Z,address	CAaddress	If  Z, PC <- address
JNC	address	JP	NC,address	D2address	If NC, PC <- address
JC	address	JP	C,address	DAaddress	If  C, PC <- address
JPO	address	JP	PO,address	E2address	If PO, PC <- address
JPE	address	JP	PE,address	EAaddress	If PE, PC <- address
JP	address	JP	P,address	F2address	If  P, PC <- address
JM	address	JP	M,address	FAaddress	If  M, PC <- address
PCHL		JP	(HL)		E9		PC <- HL
---		JP	(IX)		DDE9		PC <- IX
---		JP	(IY)		FDE9		PC <- IY
---		JR	index		18index		PC <- PC + index
---		JR	NZ,index	20index	      If NZ, PC <- PC + index
---		JR	Z,index		28index	      If  Z, PC <- PC + index
---		JR	NC,index	30index	      If NC, PC <- PC + index
---		JR	C,index		38index	      If  C, PC <- PC + index
---		DJNZ	index		10index	    B <- B - 1;
						    If B > 0, PC <- PC + index

CALL	address	CALL	address		CDaddress  (SP-1) <- PCh;(SP-2) <- PCl
						   SP <- SP - 2;PX <- address
CNZ	address	CALL	NZ,address	C4address	If NZ, CALL address
CZ	address	CALL	Z,address	CCaddress	If  Z, CALL address
CNC	address	CALL	NC,address	D4address	If NC, CALL address
CC	address	CALL	C,address	DCaddress	If  C, CALL address
CPO	address	CALL	PO,address	E4address	If PO, CALL address
CPE	address	CALL	PE,address	ECaddress	If PE, CALL address
CP	address	CALL	P,address	F4address	If  P, CALL address
CM	address	CALL	M,address	FCaddress	If  M, CALL address


RET		RET			C9	    PCl <- (SP);PCh <- (SP+1)
							SP <- SP + 2
RNZ		RET	NZ		C0		If NZ, RET
RZ		RET	Z		C8		If  Z, RET
RNC		RET	NC		D0		If NC, RET
RC		RET	C		D8		If  C, RET
RPO		RET	PO		E0		If PO, RET
RPE		RET	PE		E8		If PE, RET
RP		RET	P		F0		If  P, RET
RM		RET	M		F8		If  M, RET
---		RETI			ED4D		Return from Interrupt
---		RETN			ED45		IFF1 <- IFF2;RETI


RST	0	RST	0		C7		CALL	0
RST	1	RST	8		CF		CALL	8
RST	2	RST	10H		D7		CALL	10H
RST	3	RST	18H		DF		CALL	18H
RST	4	RST	20H		E7		CALL	20H
RST	5	RST	28H		EF		CALL	28H
RST	6	RST	30H		F7		CALL	30H
RST	7	RST	38H		FF		CALL	38H


8080/Z80  Instruction  Set					Page 12

Stack Operation Instructions

8080  Mnemonic	Z80  Mnemonic		Code		Operation

PUSH	B	PUSH	BC		C5	(SP-2) <- C; (SP-1) <- B;
						SP <- SP - 2
PUSH	D	PUSH	DE		D5	(SP-2) <- E; (SP-1) <- D;
						SP <- SP - 2
PUSH	H	PUSH	HL		E5	(SP-2) <- L; (SP-1) <- H;
						SP <- SP - 2
PUSH	PSW	PUSH	AF		F5	(SP-2) <- Flags; (SP-1) <- A;
						SP <- SP - 2
---		PUSH	IX		DDE5	(SP-2) <- IXl; (SP-1) <- IXh
						SP <- SP - 2
---		PUSH	IY		FDE5	(SP-2) <- IYl; (SP-1) <- IYh
						SP <- SP - 2

POP	B	POP	BC		C1	B <- (SP+1); C <- (SP);
						SP <- SP + 2
POP	D	POP	DE		D1	D <- (SP+1); E <- (SP);
						SP <- SP + 2
POP	H	POP	HL		E1	H <- (SP+1); L <- (SP);
						SP <- SP + 2
POP	PSW	POP	AF		F1	A <- (SP+1); Flags <- (SP);
						SP <- SP + 2
---		POP	IX		DDE1	IXh <- (SP+1); IXl <- (SP);
						SP <- SP + 2
---		POP	IY		FDE1	IYh <- (SP+1); IYl <- (SP);


Input/Output Instructions

IN	byte	IN	A,(byte)	DBbyte		A <- [byte]
---		IN	A,(C)		ED78		A <- [C]
---		IN	B,(C)		ED40		B <- [C]
---		IN	C,(C)		ED48		C <- [C]
---		IN	D,(C)		ED50		D <- [C]
---		IN	E,(C)		ED58		E <- [C]
---		IN	H,(C)		ED60		H <- [C]
---		IN	L,(C)		ED68		L <- [C]
---		INI			EDA2  (HL) <- [C];B <- B-1;HL <- HL+1
---		INIR			EDB2  (HL) <- [C];B <- B-1;HL <- HL+1
---		IND			EDAA  (HL) <- [C];B <- B-1;HL <- HL-1
---		INDR			EDBA  (HL) <- [C];B <- B-1;HL <- HL-1

OUT	byte	OUT	(byte),A	D320		[byte] <- A
---		OUT	(C),A		ED79		[C] <- A
---		OUT	(C),B		ED41		[C] <- B
---		OUT	(C),C		ED49		[C] <- C
---		OUT	(C),D		ED51		[C] <- D
---		OUT	(C),E		ED59		[C] <- E
---		OUT	(C),H		ED61		[C] <- H
---		OUT	(C),L		ED69		[C] <- L
---		OUTI			EDA3  [C] <- (HL);B <- B-1;HL <- HL+1
---		OTIR			EDB3  [C] <- (HL);B <- B-1;HL <- HL+1
---		OUTD			EDAB  [C] <- (HL);B <- B-1;HL <- HL-1
---		OTDR			EDBB  [C] <- (HL);B <- B-1;HL <- HL-1


8080/Z80  Instruction  Set					Page 13

Data Transfer Instructions (Z80 Only)

8080  Mnemonic	Z80  Mnemonic		Code		Operation

---		LDI			EDA0	(DE) <- (HL);HL <- HL+1
						DE <- DE+1; BC <- BC-1
---		LDIR			EDB0	(DE) <- (HL);HL <- HL+1
						DE <- DE+1; BC <- BC-1
---		LDD			EDA8	(DE) <- (HL);HL <- HL-1
						DE <- DE-1; BC <- BC-1
---		LDDR			EDB8	(DE) <- (HL);HL <- HL-1
						DE <- DE-1; BC <- BC-1

Bit Manipulation Instructions (Z80 Only)

---		BIT	0,A		CB47		Z flag <- NOT 0b
---		BIT	0,B		CB40		Z flag <- NOT 0b
---		BIT	0,C		CB41		Z flag <- NOT 0b
---		BIT	0,D		CB42		Z flag <- NOT 0b
---		BIT	0,E		CB43		Z flag <- NOT 0b
---		BIT	0,H		CB44		Z flag <- NOT 0b
---		BIT	0,L		CB45		Z flag <- NOT 0b
---		BIT	0,(HL)		CB46		Z flag <- NOT 0b
---		BIT	0,(IX+index)	DDCBindex46	Z flag <- NOT 0b
---		BIT	0,(IY+index)	FDCBindex46	Z flag <- NOT 0b

---		BIT	1,A		CB4F		Z flag <- NOT 1b
---		BIT	1,B		CB48		Z flag <- NOT 1b
---		BIT	1,C		CB49		Z flag <- NOT 1b
---		BIT	1,D		CB4A		Z flag <- NOT 1b
---		BIT	1,E		CB4B		Z flag <- NOT 1b
---		BIT	1,H		CB4C		Z flag <- NOT 1b
---		BIT	1,L		CB4D		Z flag <- NOT 1b
---		BIT	1,(HL)		CB4E		Z flag <- NOT 1b
---		BIT	1,(IX+index)	DDCBindex4E	Z flag <- NOT 1b
---		BIT	1,(IY+index)	FDCBindex4E	Z flag <- NOT 1b

---		BIT	2,A		CB57		Z flag <- NOT 2b
---		BIT	2,B		CB50		Z flag <- NOT 2b
---		BIT	2,C		CB51		Z flag <- NOT 2b
---		BIT	2,D		CB52		Z flag <- NOT 2b
---		BIT	2,E		CB53		Z flag <- NOT 2b
---		BIT	2,H		CB54		Z flag <- NOT 2b
---		BIT	2,L		CB55		Z flag <- NOT 2b
---		BIT	2,(HL)		CB56		Z flag <- NOT 2b
---		BIT	2,(IX+index)	DDCBindex56	Z flag <- NOT 2b
---		BIT	2,(IY+index)	FDCBindex56	Z flag <- NOT 2b

---		BIT	3,A		CB5F		Z flag <- NOT 3b
---		BIT	3,B		CB58		Z flag <- NOT 3b
---		BIT	3,C		CB59		Z flag <- NOT 3b
---		BIT	3,D		CB5A		Z flag <- NOT 3b
---		BIT	3,E		CB5B		Z flag <- NOT 3b
---		BIT	3,H		CB5C		Z flag <- NOT 3b
---		BIT	3,L		CB5D		Z flag <- NOT 3b
---		BIT	3,(HL)		CB5E		Z flag <- NOT 3b
---		BIT	3,(IX+index)	DDCBindex5E	Z flag <- NOT 3b
---		BIT	3,(IY+index)	FDCBindex5E	Z flag <- NOT 3b



8080/Z80  Instruction  Set					Page 14

Bit Manipulation Instructions (Z80 Only)

8080  Mnemonic	Z80  Mnemonic		Code		Operation

---		BIT	4,A		CB67		Z flag <- NOT 4b
---		BIT	4,B		CB60		Z flag <- NOT 4b
---		BIT	4,C		CB61		Z flag <- NOT 4b
---		BIT	4,D		CB62		Z flag <- NOT 4b
---		BIT	4,E		CB63		Z flag <- NOT 4b
---		BIT	4,H		CB64		Z flag <- NOT 4b
---		BIT	4,L		CB65		Z flag <- NOT 4b
---		BIT	4,(HL)		CB66		Z flag <- NOT 4b
---		BIT	4,(IX+index)	DDCBindex66	Z flag <- NOT 4b
---		BIT	4,(IY+index)	FDCBindex66	Z flag <- NOT 4b

---		BIT	5,A		CB6F		Z flag <- NOT 5b
---		BIT	5,B		CB68		Z flag <- NOT 5b
---		BIT	5,C		CB69		Z flag <- NOT 5b
---		BIT	5,D		CB6A		Z flag <- NOT 5b
---		BIT	5,E		CB6B		Z flag <- NOT 5b
---		BIT	5,H		CB6C		Z flag <- NOT 5b
---		BIT	5,L		CB6D		Z flag <- NOT 5b
---		BIT	5,(HL)		CB6E		Z flag <- NOT 5b
---		BIT	5,(IX+index)	DDCBindex6E	Z flag <- NOT 5b
---		BIT	5,(IY+index)	FDCBindex6E	Z flag <- NOT 5b

---		BIT	6,A		CB77		Z flag <- NOT 6b
---		BIT	6,B		CB70		Z flag <- NOT 6b
---		BIT	6,C		CB71		Z flag <- NOT 6b
---		BIT	6,D		CB72		Z flag <- NOT 6b
---		BIT	6,E		CB73		Z flag <- NOT 6b
---		BIT	6,H		CB74		Z flag <- NOT 6b
---		BIT	6,L		CB75		Z flag <- NOT 6b
---		BIT	6,(HL)		CB76		Z flag <- NOT 6b
---		BIT	6,(IX+index)	DDCBindex76	Z flag <- NOT 6b
---		BIT	6,(IY+index)	FDCBindex76	Z flag <- NOT 6b

---		BIT	7,A		CB7F		Z flag <- NOT 7b
---		BIT	7,B		CB78		Z flag <- NOT 7b
---		BIT	7,C		CB79		Z flag <- NOT 7b
---		BIT	7,D		CB7A		Z flag <- NOT 7b
---		BIT	7,E		CB7B		Z flag <- NOT 7b
---		BIT	7,H		CB7C		Z flag <- NOT 7b
---		BIT	7,L		CB7D		Z flag <- NOT 7b
---		BIT	7,(HL)		CB7E		Z flag <- NOT 7b
---		BIT	7,(IX+index)	DDCBindex7E	Z flag <- NOT 7b
---		BIT	7,(IY+index)	FDCBindex7E	Z flag <- NOT 7b


---		RES	0,A		CB87		0b <- 0
---		RES	0,B		CB80		0b <- 0
---		RES	0,C		CB81		0b <- 0
---		RES	0,D		CB82		0b <- 0
---		RES	0,E		CB83		0b <- 0
---		RES	0,H		CB84		0b <- 0
---		RES	0,L		CB85		0b <- 0
---		RES	0,(HL)		CB86		0b <- 0
---		RES	0,(IX+index)	DDCBindex86	0b <- 0
---		RES	0,(IY+index)	FDCBindex86	0b <- 0



8080/Z80  Instruction  Set					Page 15

Bit Manipulation Instructions (Z80 Only)

8080  Mnemonic	Z80  Mnemonic		Code		Operation

---		RES	1,A		CB8F		1b <- 0
---		RES	1,B		CB88		1b <- 0
---		RES	1,C		CB89		1b <- 0
---		RES	1,D		CB8A		1b <- 0
---		RES	1,E		CB8B		1b <- 0
---		RES	1,H		CB8C		1b <- 0
---		RES	1,L		CB8D		1b <- 0
---		RES	1,(HL)		CB8E		1b <- 0
---		RES	1,(IX+index)	DDCBindex8E	1b <- 0
---		RES	1,(IY+index)	FDCBindex8E	1b <- 0

---		RES	2,A		CB97		2b <- 0
---		RES	2,B		CB90		2b <- 0
---		RES	2,C		CB91		2b <- 0
---		RES	2,D		CB92		2b <- 0
---		RES	2,E		CB93		2b <- 0
---		RES	2,H		CB94		2b <- 0
---		RES	2,L		CB95		2b <- 0
---		RES	2,(HL)		CB96		2b <- 0
---		RES	2,(IX+index)	DDCBindex96	2b <- 0
---		RES	2,(IY+index)	FDCBindex96	2b <- 0

---		RES	3,A		CB9F		3b <- 0
---		RES	3,B		CB98		3b <- 0
---		RES	3,C		CB99		3b <- 0
---		RES	3,D		CB9A		3b <- 0
---		RES	3,E		CB9B		3b <- 0
---		RES	3,H		CB9C		3b <- 0
---		RES	3,L		CB9D		3b <- 0
---		RES	3,(HL)		CB9E		3b <- 0
---		RES	3,(IX+index)	DDCBindex9E	3b <- 0
---		RES	3,(IY+index)	FDCBindex9E	3b <- 0

---		RES	4,A		CBA7		4b <- 0
---		RES	4,B		CBA0		4b <- 0
---		RES	4,C		CBA1		4b <- 0
---		RES	4,D		CBA2		4b <- 0
---		RES	4,E		CBA3		4b <- 0
---		RES	4,H		CBA4		4b <- 0
---		RES	4,L		CBA5		4b <- 0
---		RES	4,(HL)		CBA6		4b <- 0
---		RES	4,(IX+index)	DDCBindexA6	4b <- 0
---		RES	4,(IY+index)	FDCBindexA6	4b <- 0

---		RES	5,A		CBAF		5b <- 0
---		RES	5,B		CBA8		5b <- 0
---		RES	5,C		CBA9		5b <- 0
---		RES	5,D		CBAA		5b <- 0
---		RES	5,E		CBAB		5b <- 0
---		RES	5,H		CBAC		5b <- 0
---		RES	5,L		CBAD		5b <- 0
---		RES	5,(HL)		CBAE		5b <- 0
---		RES	5,(IX+index)	DDCBindexAE	5b <- 0
---		RES	5,(IY+index)	FDCBindexAE	5b <- 0



8080/Z80  Instruction  Set					Page 16

Bit Manipulation Instructions (Z80 Only)

8080  Mnemonic	Z80  Mnemonic		Code		Operation

---		RES	6,A		CBB7		6b <- 0
---		RES	6,B		CBB0		6b <- 0
---		RES	6,C		CBB1		6b <- 0
---		RES	6,D		CBB2		6b <- 0
---		RES	6,E		CBB3		6b <- 0
---		RES	6,H		CBB4		6b <- 0
---		RES	6,L		CBB5		6b <- 0
---		RES	6,(HL)		CBB6		6b <- 0
---		RES	6,(IX+index)	DDCBindexB6	6b <- 0
---		RES	6,(IY+index)	FDCBindexB6	6b <- 0

---		RES	7,A		CBBF		7b <- 0
---		RES	7,B		CBB8		7b <- 0
---		RES	7,C		CBB9		7b <- 0
---		RES	7,D		CBBA		7b <- 0
---		RES	7,E		CBBB		7b <- 0
---		RES	7,H		CBBC		7b <- 0
---		RES	7,L		CBBD		7b <- 0
---		RES	7,(HL)		CBBE		7b <- 0
---		RES	7,(IX+index)	DDCBindexBE	7b <- 0
---		RES	7,(IY+index)	FDCBindexBE	7b <- 0

---		SET	0,A		CBC7		0b <- 1
---		SET	0,B		CBC0		0b <- 1
---		SET	0,C		CBC1		0b <- 1
---		SET	0,D		CBC2		0b <- 1
---		SET	0,E		CBC3		0b <- 1
---		SET	0,H		CBC4		0b <- 1
---		SET	0,L		CBC5		0b <- 1
---		SET	0,(HL)		CBC6		0b <- 1
---		SET	0,(IX+index)	DDCBindexC6	0b <- 1
---		SET	0,(IY+index)	FDCBindexC6	0b <- 1

---		SET	1,A		CBCF		1b <- 1
---		SET	1,B		CBC8		1b <- 1
---		SET	1,C		CBC9		1b <- 1
---		SET	1,D		CBCA		1b <- 1
---		SET	1,E		CBCB		1b <- 1
---		SET	1,H		CBCC		1b <- 1
---		SET	1,L		CBCD		1b <- 1
---		SET	1,(HL)		CBCE		1b <- 1
---		SET	1,(IX+index)	DDCBindexCE	1b <- 1
---		SET	1,(IY+index)	FDCBindexCE	1b <- 1

---		SET	2,A		CBD7		2b <- 1
---		SET	2,B		CBD0		2b <- 1
---		SET	2,C		CBD1		2b <- 1
---		SET	2,D		CBD2		2b <- 1
---		SET	2,E		CBD3		2b <- 1
---		SET	2,H		CBD4		2b <- 1
---		SET	2,L		CBD5		2b <- 1
---		SET	2,(HL)		CBD6		2b <- 1
---		SET	2,(IX+index)	DDCBindexD6	2b <- 1
---		SET	2,(IY+index)	FDCBindexD6	2b <- 1



8080/Z80  Instruction  Set					Page 17

Bit Manipulation Instructions (Z80 Only)

8080  Mnemonic	Z80  Mnemonic		Code		Operation

---		SET	3,A		CBDF		3b <- 1
---		SET	3,B		CBD8		3b <- 1
---		SET	3,C		CBD9		3b <- 1
---		SET	3,D		CBDA		3b <- 1
---		SET	3,E		CBDB		3b <- 1
---		SET	3,H		CBDC		3b <- 1
---		SET	3,L		CBDD		3b <- 1
---		SET	3,(HL)		CBDE		3b <- 1
---		SET	3,(IX+index)	DDCBindexDE	3b <- 1
---		SET	3,(IY+index)	FDCBindexDE	3b <- 1

---		SET	4,A		CBE7		4b <- 1
---		SET	4,B		CBE0		4b <- 1
---		SET	4,C		CBE1		4b <- 1
---		SET	4,D		CBE2		4b <- 1
---		SET	4,E		CBE3		4b <- 1
---		SET	4,H		CBE4		4b <- 1
---		SET	4,L		CBE5		4b <- 1
---		SET	4,(HL)		CBE6		4b <- 1
---		SET	4,(IX+index)	DDCBindexE6	4b <- 1
---		SET	4,(IY+index)	FDCBindexE6	4b <- 1

---		SET	5,A		CBEF		5b <- 1
---		SET	5,B		CBE8		5b <- 1
---		SET	5,C		CBE9		5b <- 1
---		SET	5,D		CBEA		5b <- 1
---		SET	5,E		CBEB		5b <- 1
---		SET	5,H		CBEC		5b <- 1
---		SET	5,L		CBED		5b <- 1
---		SET	5,(HL)		CBEE		5b <- 1
---		SET	5,(IX+index)	DDCBindexEE	5b <- 1
---		SET	5,(IY+index)	FDCBindexEE	5b <- 1

---		SET	6,A		CBF7		6b <- 1
---		SET	6,B		CBF0		6b <- 1
---		SET	6,C		CBF1		6b <- 1
---		SET	6,D		CBF2		6b <- 1
---		SET	6,E		CBF3		6b <- 1
---		SET	6,H		CBF4		6b <- 1
---		SET	6,L		CBF5		6b <- 1
---		SET	6,(HL)		CBF6		6b <- 1
---		SET	6,(IX+index)	DDCBindexF6	6b <- 1
---		SET	6,(IY+index)	FDCBindexF6	6b <- 1

---		SET	7,A		CBFF		7b <- 1
---		SET	7,B		CBF8		7b <- 1
---		SET	7,C		CBF9		7b <- 1
---		SET	7,D		CBFA		7b <- 1
---		SET	7,E		CBFB		7b <- 1
---		SET	7,H		CBFC		7b <- 1
---		SET	7,L		CBFD		7b <- 1
---		SET	7,(HL)		CBFE		7b <- 1
---		SET	7,(IX+index)	DDCBindexFE	7b <- 1
---		SET	7,(IY+index)	FDCBindexFE	7b <- 1



8080/Z80  Instruction  Set					Page 18

Bit Shift Instructions (Z80 Only)

8080  Mnemonic	Z80  Mnemonic		Code		Operation

---		SLA	A		CB27		---
---		SLA	B		CB20		---
---		SLA	C		CB21		---
---		SLA	D		CB22		---
---		SLA	E		CB23		---
---		SLA	H		CB24		---
---		SLA	L		CB25		---
---		SLA	(HL)		CB26		---
---		SLA	(IX+index)	DDCBindex26	---
---		SLA	(IY+index)	FDCBindex26	---

---		SRA	A		CB2F		---
---		SRA	B		CB28		---
---		SRA	C		CB29		---
---		SRA	D		CB2A		---
---		SRA	E		CB2B		---
---		SRA	H		CB2C		---
---		SRA	L		CB2D		---
---		SRA	(HL)		CB2E		---
---		SRA	(IX+index)	DDCBindex2E	---
---		SRA	(IY+index)	FDCBindex2E	---

---		SRL	A		CB3F		---
---		SRL	B		CB38		---
---		SRL	C		CB39		---
---		SRL	D		CB3A		---
---		SRL	E		CB3B		---
---		SRL	H		CB3C		---
---		SRL	L		CB3D		---
---		SRL	(HL)		CB3E		---
---		SRL	(IX+index)	DDCBindex3E	---
---		SRL	(IY+index)	FDCBindex3E	---
#endif
    }
}
