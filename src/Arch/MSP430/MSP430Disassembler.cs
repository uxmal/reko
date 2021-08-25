#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

#pragma warning disable IDE1006

namespace Reko.Arch.Msp430
{
    using Decoder = Decoder<Msp430Disassembler, Mnemonics, Msp430Instruction>;
    using MaskDecoder = MaskDecoder<Msp430Disassembler, Mnemonics, Msp430Instruction>;

    public class Msp430Disassembler : DisassemblerBase<Msp430Instruction, Mnemonics>
    {
        private static Decoder[] s_decoders;

        private readonly EndianImageReader rdr;
        private readonly Msp430Architecture arch;
        private readonly List<MachineOperand> ops;
        private Address addr;
        private ushort uExtension;
        private PrimitiveType? dataWidth;

        public Msp430Disassembler(Msp430Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
            this.addr = null!;
        }

        public override Msp430Instruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadLeUInt16(out ushort uInstr))
                return null;
            uExtension = 0;
            ops.Clear();
            dataWidth = null;
            var instr = s_decoders[uInstr >> 12].Decode(uInstr, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override Msp430Instruction MakeInstruction(InstrClass iclass, Mnemonics mnemonic)
        {
            int rep = (this.uExtension & 0x0F);
            var instr = new Msp430Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                dataWidth = this.dataWidth,
                Operands = this.ops.ToArray(),
                repeatImm = (this.uExtension & 0x80) != 0 ? 0 : rep + 1,
                repeatReg = (this.uExtension & 0x80) != 0 ? Registers.GpRegisters[rep] : null,
            };
            instr = this.SpecialCase(instr);
            return instr;
        }

        private Msp430Instruction SpecialCase(Msp430Instruction instr)
        {
            if (instr.Mnemonic == Mnemonics.mov)
            {
                if (instr.Operands[1] is RegisterStorage dst &&
                    dst == Registers.pc)
                {
                    instr.InstructionClass = InstrClass.Transfer;
                    if (instr.Operands[0] is MemoryOperand mem &&
                        mem.PostIncrement &&
                        mem.Base == Registers.sp)
                    {
                        instr.Mnemonic = Mnemonics.ret;
                        instr.InstructionClass |= InstrClass.Return;
                        instr.Operands = MachineInstruction.NoOperands;
                    }
                    else
                    {
                        instr.Mnemonic = Mnemonics.br;
                        instr.Operands[1] = null!;
                        if (instr.Operands[0] is ImmediateOperand imm)
                        {
                            var uAddr = imm.Value.ToUInt16();
                            instr.Operands = new MachineOperand[] { AddressOperand.Ptr16(uAddr) };
                        }
                    }
                    return instr;
                }
            }
            return instr;
        }

        #region Mutators

        /// <summary>
        /// Set the current data width to 'byte'.
        /// </summary>
        private static bool b(uint uInstr, Msp430Disassembler dasm)
        {
            dasm.dataWidth = PrimitiveType.Byte;
            return true;
        }

        /// <summary>
        /// Set current data width to 'word16'
        /// </summary>
        private static bool w16(uint uInstr, Msp430Disassembler dasm)
        {
            dasm.dataWidth = PrimitiveType.Word16;
            return true;
        }
        
        // use width bit
        private static bool w(uint uInstr, Msp430Disassembler dasm)
        {
            dasm.dataWidth = (uInstr & 0x40) != 0 ? PrimitiveType.Byte : PrimitiveType.Word16;
            return true;
        }

        // a/w bit 4.
        private static bool a(uint uInstr, Msp430Disassembler dasm)
        {
            dasm.dataWidth = (uInstr & 0x04) != 0 ? PrimitiveType.Word16 : Msp430Architecture.Word20;
            return true;
        }

        private static bool x(uint uInstr, Msp430Disassembler dasm)
        {
            dasm.dataWidth = (dasm.uExtension != 0) && (dasm.uExtension & 0x40) == 0 ? Msp430Architecture.Word20 : PrimitiveType.Word16;
            return true;
        }

        /// <summary>
        /// Set data width to ptr20
        /// </summary>
        private static bool p(uint uInstr, Msp430Disassembler dasm)
        {
            dasm.dataWidth = Msp430Architecture.Word20;
            return true;
        }

        // b/w/a combined from the op and the extension
        private static bool W(uint uInstr, Msp430Disassembler dasm)
        {
            var w = ((dasm.uExtension & 0x40u) >> 5) | (uInstr & 0x040u) >> 6;
            switch (w)
            {
            case 0: return false;
            case 1: dasm.dataWidth = Msp430Architecture.Word20; break;
            case 2: dasm.dataWidth = PrimitiveType.Word16; break;
            case 3: dasm.dataWidth = PrimitiveType.Byte; break;
            }
            return true;
        }

        private static bool J(uint uInstr, Msp430Disassembler dasm)
        {
            int offset = (short) (uInstr << 6) >> 5;
            dasm.ops.Add(AddressOperand.Create(dasm.rdr.Address + offset));
            return true;
        }

        /// <summary>
        /// Get a source operand from bits 8..11.
        /// </summary>
        private static bool r(uint uInstr, Msp430Disassembler dasm)
        {
            var op = dasm.SourceOperand(0, (uInstr >> 8) & 0x0F, false, dasm.dataWidth!);
            if (op == null)
                return false;
            dasm.ops.Add(op);
            return true;
        }

        /// <summary>
        /// Get a dst operand from the low 4 bits. It must never
        /// be a constant.
        /// </summary>
        private static bool r2(uint uInstr, Msp430Disassembler dasm)
        {
            var n = uInstr & 0x0F;
            var op2 = dasm.SourceOperand(0, n, true, dasm.dataWidth!);
            if (op2 == null)
                return false;
            dasm.ops.Add(op2);
            return true;
        }

        private static bool S(uint uInstr, Msp430Disassembler dasm)
        {
            var aS = (uInstr >> 4) & 0x03;
            var iReg = (uInstr >> 8) & 0x0F;
            var op1 = dasm.SourceOperand(aS, iReg, false, dasm.dataWidth!);
            if (op1 == null)
                return false;
            dasm.ops.Add(op1);
            return true;
        }

        private static bool s(uint uInstr, Msp430Disassembler dasm)
        {
            var aS = (uInstr >> 4) & 0x03;
            var iReg = uInstr & 0x0F;
            var op1 = dasm.SourceOperand(aS, iReg, false, dasm.dataWidth!);
            if (op1 == null)
                return false;
            dasm.ops.Add(op1);
            return true;
        }

        /// <summary>
        /// Like 's' but must be a writeable destination.
        /// </summary>
        private static bool d(uint uInstr, Msp430Disassembler dasm)
        {
            var aS = (uInstr >> 4) & 0x03;
            var iReg = uInstr & 0x0F;
            var op1 = dasm.SourceOperand(aS, iReg, true, dasm.dataWidth!);
            if (op1 == null || op1 is ImmediateOperand || op1 is AddressOperand)
                return false;
            dasm.ops.Add(op1);
            return true;
        }

        private static bool n(uint uInstr, Msp430Disassembler dasm)
        {
            uint n = 1 + ((uInstr >> 4) & 0x0F);
            var op1 = ImmediateOperand.Byte((byte) n);
            dasm.ops.Add(op1);
            return true;
        }

        private static bool N(uint uInstr, Msp430Disassembler dasm)
        {
            var n = 1 + ((uInstr >> 10) & 3);
            var op1 = ImmediateOperand.Byte((byte) n);
            dasm.ops.Add(op1);
            return true;
        }

        private static bool Y(uint uInstr, Msp430Disassembler dasm)
        {
            return false;
        }

        /// <summary>
        /// Indirect register (symbolized by the '@' character)
        /// </summary>
        private static bool At(uint uInstr, Msp430Disassembler dasm)
        {
            var iReg = (uInstr >> 8) & 0x0F;
            var reg = Registers.GpRegisters[iReg];
            var op1 = new MemoryOperand(Msp430Architecture.Word20)
            {
                Base = reg,
            };
            dasm.ops.Add(op1);
            return true;
        }

        private static bool Post(uint uInstr, Msp430Disassembler dasm)
        {
            var iReg = (uInstr >> 8) & 0x0F;
            var reg = Registers.GpRegisters[iReg];
            var op1 = dasm.PostInc(reg, Msp430Architecture.Word20);
            if (op1 is null)
                return false;
            dasm.ops.Add(op1);
            return true;
        }

        private static bool Amp(uint uInstr, Msp430Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out var lo16))
                return false;
            var hi4 = (uInstr >> 8) & 0x0F;
            //$TODO: 20-bit address?
            var op1 = AddressOperand.Ptr32((hi4 << 16) | lo16);
            dasm.ops.Add(op1);
            return true;
        }

        private static Mutator<Msp430Disassembler> Indirect(int baseRegOffset)
        {
            return (u, d) =>
            {
                if (!d.rdr.TryReadLeInt16(out var idxOffset))
                    return false;
                var iReg = (u >> baseRegOffset) & 0x0F;
                var reg = Registers.GpRegisters[iReg];
                var op1 = new MemoryOperand(Msp430Architecture.Word20)
                {
                    Base = reg,
                    Offset = idxOffset
                };
                d.ops.Add(op1);
                return true;
            };
        }

        private static bool D(uint uInstr, Msp430Disassembler dasm)
        {
            var aD = (uInstr >> 7) & 0x01;
            var iReg = uInstr & 0x0F;
            if (iReg == 3)
            {
                return false;
            }
            var reg = Registers.GpRegisters[iReg];
            MachineOperand? op2;
            if (aD == 0)
            {
                if (iReg == 3)
                    return false;
                op2 = reg;
            }
            else
            {
                op2 = dasm.Indexed(reg, dasm.dataWidth!);
                if (op2 == null)
                    return false;
            }
            dasm.ops.Add(op2);
            return true;
        }

 

        #endregion

        private MachineOperand? SourceOperand(uint aS, uint iReg, bool isDestination, PrimitiveType dataWidth)
        {
            var reg = Registers.GpRegisters[iReg];
            switch (aS)
            {
            case 0:
                if (iReg == 3)
                {
                    return isDestination || dataWidth == null
                        ? null
                        : new ImmediateOperand(Constant.Create(dataWidth, 0));
                }
                return reg;
            case 1:
                if (iReg == 3)
                {
                    return isDestination || dataWidth == null
                        ? null
                        : new ImmediateOperand(Constant.Create(dataWidth, 1));
                }
                return Indexed(reg, dataWidth);
            case 2:
                if (iReg == 2)
                {
                    return isDestination || dataWidth == null
                        ? null
                        : new ImmediateOperand(Constant.Create(dataWidth, 4));
                }
                if (iReg == 3)
                {
                    return isDestination || dataWidth == null
                        ? null
                        : new ImmediateOperand(Constant.Create(dataWidth, 2));
                }
                return new MemoryOperand(dataWidth) { Base = reg };
            case 3:
                if (iReg == 2)
                {
                    return isDestination || dataWidth == null
                        ? null
                        : new ImmediateOperand(Constant.Create(dataWidth, 8));
                }
                else if (iReg == 3)
                {
                    return isDestination || dataWidth == null
                        ? null
                        : new ImmediateOperand(Constant.Create(dataWidth, -1));
                }
                return PostInc(reg, dataWidth);
            default:
                throw new NotImplementedException();
            }
        }

        private MachineOperand? PostInc(RegisterStorage reg, PrimitiveType dataWidth)
        {
            if (reg == Registers.pc)
            {
                if (!rdr.TryReadLeInt16(out short offset))
                    return null;
                return ImmediateOperand.Word16((ushort) offset);
            }
            else
            {
                return new MemoryOperand(dataWidth ?? PrimitiveType.Word16)
                {
                    Base = reg,
                    PostIncrement = true,
                };
            }

        }

        private MachineOperand? Indexed(RegisterStorage? reg, PrimitiveType dataWidth)
        {
            var delta = (rdr.Address - this.addr);
            if (!rdr.TryReadLeInt16(out short offset))
                return null;
            if (reg != null && reg.Number == 2)
            {
                // Absolute address will not use a base register.
                reg = null;
            }
            else if (reg == Registers.pc)
            {
                // We need to adjust the offset because we want it to be relative
                // to the beginning of the current instruction, not to the address
                // of the offset. 
                offset = (short) (offset + delta);
            }
            return new MemoryOperand(dataWidth ?? PrimitiveType.Word16)
            {
                Base = reg,
                Offset = offset
            };
        }

        public override Msp430Instruction CreateInvalidInstruction()
        {
            return new Msp430Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonics.invalid,
                Operands = new MachineOperand[0],
            };
        }

        public override Msp430Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("MSP430Dis", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        private static Decoder Instr(Mnemonics mnemonic, params Mutator<Msp430Disassembler>[] mutators)
        {
            return new InstrDecoder<Msp430Disassembler, Mnemonics, Msp430Instruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonics mnemonic, InstrClass iclass, params Mutator<Msp430Disassembler>[] mutators)
        {
            return new InstrDecoder<Msp430Disassembler, Mnemonics, Msp430Instruction>(iclass, mnemonic, mutators);
        }

        private class JmpDecoder : Decoder
        {
            (Mnemonics, InstrClass)[] jmps;

            public JmpDecoder((Mnemonics, InstrClass)[] jmps)
            {
                this.jmps = jmps;
            }

            public override Msp430Instruction Decode(uint uInstr, Msp430Disassembler dasm)
            {
                if (!J(uInstr, dasm))
                    return dasm.CreateInvalidInstruction();

                int rep = (dasm.uExtension & 0x0F);
                var jj = jmps[(uInstr >> 10) & 7];
                var instr = new Msp430Instruction
                {
                    Mnemonic = jj.Item1,
                    InstructionClass = jj.Item2,
                    dataWidth = dasm.dataWidth,
                    Operands = dasm.ops.ToArray(),
                    repeatImm = (dasm.uExtension & 0x80) != 0 ? 0 : rep + 1,
                    repeatReg = (dasm.uExtension & 0x80) != 0 ? Registers.GpRegisters[rep] : null,
                };
                instr = dasm.SpecialCase(instr);
                return instr;
            }
        }

        private class SubDecoder : Decoder
        {
            private readonly Dictionary<int, Decoder> decoders;
            private readonly ushort mask;
            private readonly int sh;

            public SubDecoder(int sh, ushort mask, Dictionary<int, Decoder> decoders)
            {
                this.sh = sh;
                this.mask = mask;
                this.decoders = decoders;
            }

            public override Msp430Instruction Decode(uint uInstr, Msp430Disassembler dasm)
            {
                var key = (ushort) (uInstr >> sh) & mask;
                if (!decoders.TryGetValue(key, out Decoder decoder))
                    return dasm.CreateInvalidInstruction();
                return decoder.Decode(uInstr, dasm);
            }
        }

        private class ExtDecoder : Decoder
        {
            private readonly Decoder[] decoders;

            public ExtDecoder(Decoder[] decoders)
            {
                this.decoders = decoders;
            }

            public override Msp430Instruction Decode(uint uInstr, Msp430Disassembler dasm)
            {
                if (!dasm.rdr.TryReadLeUInt16(out ushort u))
                    return dasm.CreateInvalidInstruction();
                dasm.uExtension = (ushort) uInstr;
                uInstr = u;
                return this.decoders[uInstr >> 12].Decode(uInstr, dasm);
            }
        }

        static Msp430Disassembler()
        {
            Decoder invalid = Instr(Mnemonics.invalid, InstrClass.Invalid);
            Decoder nyi = new NyiDecoder<Msp430Disassembler, Mnemonics, Msp430Instruction>("");

            Decoder[] extDecoders = new Decoder[16]
            {
                nyi,
                Sparse(6, 6, "  01", nyi, 
                    ( 0x00, nyi ),
                    ( 0x01, nyi ),
                    ( 0x02, nyi ),
                    ( 0x04, Instr(Mnemonics.rrax, W,s)),
                    ( 0x05, Instr(Mnemonics.rrax, W,s)),
                    ( 0x06, nyi ),
                    ( 0x08, nyi ),
                    ( 0x09, nyi ),
                    ( 0x0A, nyi ),
                    ( 0x0C, Sparse(0, 6,  "  0C", invalid,
                        ( 0x00, Instr(Mnemonics.reti, InstrClass.Transfer|InstrClass.Return))))),
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,
            };

            ExtDecoder extDecoder = new ExtDecoder(extDecoders);


            Decoder rotations = new MaskDecoder(8, 2, "  rotations",
               Instr(Mnemonics.rrcm, a,N,r2),
               Instr(Mnemonics.rram, a,N,r2),
               Instr(Mnemonics.rlam, a,N,r2),
               Instr(Mnemonics.rrum, a,N,r2));

            (Mnemonics, InstrClass)[] jmps = new (Mnemonics, InstrClass)[8]
            {
                ( Mnemonics.jnz,  InstrClass.ConditionalTransfer ),
                ( Mnemonics.jz,   InstrClass.ConditionalTransfer ),
                ( Mnemonics.jnc,  InstrClass.ConditionalTransfer ),
                ( Mnemonics.jc,   InstrClass.ConditionalTransfer ),
                ( Mnemonics.jn,   InstrClass.ConditionalTransfer ),
                ( Mnemonics.jge,  InstrClass.ConditionalTransfer ),
                ( Mnemonics.jl,   InstrClass.ConditionalTransfer ),
                ( Mnemonics.jmp,  InstrClass.Transfer ),
            };

            s_decoders = new Decoder[16]
            {
                new MaskDecoder(4, 4, "  op0",
                    Instr(Mnemonics.mova, p,At,r2),
                    Instr(Mnemonics.mova, p,Post,r2),
                    Instr(Mnemonics.mova, p,Amp,r2),
                    Instr(Mnemonics.mova, p,Indirect(8),r2),

                    rotations,
                    rotations,
                    Instr(Mnemonics.mova, p,r,Amp),
                    Instr(Mnemonics.mova, p,r,Indirect(0)),

                    Instr(Mnemonics.mova, p,Y,r2),
                    Instr(Mnemonics.cmpa, p,Y,r2),
                    Instr(Mnemonics.adda, p,Y,r2),
                    Instr(Mnemonics.suba, p,Y,r2),

                    Instr(Mnemonics.mova, p,r,r2),
                    Instr(Mnemonics.cmpa, p,r,r2),
                    Instr(Mnemonics.adda, p,r,r2),
                    Instr(Mnemonics.suba, p,r,r2)
                ),
                new MaskDecoder(6, 6, "  op1",
                    Instr(Mnemonics.rrc, w, d),
                    Instr(Mnemonics.rrc, w, d),
                    Instr(Mnemonics.swpb, w16, d),
                    invalid,

                    Instr(Mnemonics.rra, w,d),
                    Instr(Mnemonics.rra, w,d),
                    Instr(Mnemonics.sxt, w,d),
                    invalid,

                    Instr(Mnemonics.push, w,s),
                    Instr(Mnemonics.push, w,s),
                    Instr(Mnemonics.call, InstrClass.Transfer|InstrClass.Call, s),
                    invalid,

                    Sparse(0, 6, "  0C", invalid,
                        ( 0x00, Instr(Mnemonics.reti, InstrClass.Transfer) )),
                    nyi,
                    invalid,
                    invalid,

                    Instr(Mnemonics.pushm, x,n,r2),
                    Instr(Mnemonics.pushm, x,n,r2),
                    Instr(Mnemonics.pushm, x,n,r2),
                    Instr(Mnemonics.pushm, x,n,r2),

                    Instr(Mnemonics.pushm, x,n,r2),
                    Instr(Mnemonics.pushm, x,n,r2),
                    Instr(Mnemonics.pushm, x,n,r2),
                    Instr(Mnemonics.pushm, x,n,r2),

                    Instr(Mnemonics.popm, x,n,r2),
                    Instr(Mnemonics.popm, x,n,r2),
                    Instr(Mnemonics.popm, x,n,r2),
                    Instr(Mnemonics.popm, x,n,r2),

                    Instr(Mnemonics.popm, x,n,r2),
                    Instr(Mnemonics.popm, x,n,r2),
                    Instr(Mnemonics.popm, x,n,r2),
                    Instr(Mnemonics.popm, x,n,r2),

                    extDecoder,
                    extDecoder,
                    extDecoder,
                    extDecoder,

                    extDecoder,
                    extDecoder,
                    extDecoder,
                    extDecoder,

                    extDecoder,
                    extDecoder,
                    extDecoder,
                    extDecoder,

                    extDecoder,
                    extDecoder,
                    extDecoder,
                    extDecoder,

                    extDecoder,
                    extDecoder,
                    extDecoder,
                    extDecoder,

                    extDecoder,
                    extDecoder,
                    extDecoder,
                    extDecoder,

                    extDecoder,
                    extDecoder,
                    extDecoder,
                    extDecoder,

                    extDecoder,
                    extDecoder,
                    extDecoder,
                    extDecoder
                ),
                new JmpDecoder(jmps),
                new JmpDecoder(jmps),

                Instr(Mnemonics.mov, w,S,D),
                Instr(Mnemonics.add, w,S,D),
                Instr(Mnemonics.addc, w,S,D),
                Instr(Mnemonics.subc, w,S,D),

                Instr(Mnemonics.sub, w,S,D),
                Instr(Mnemonics.cmp, w,S,D),
                Instr(Mnemonics.dadd, w,S,D),
                Instr(Mnemonics.bit, w,S,D),

                Instr(Mnemonics.bic, w,S,D),
                Instr(Mnemonics.bis, w,S,D),
                Instr(Mnemonics.xor, w,S,D),
                Instr(Mnemonics.and, w,S,D),
            };
        }
    }
}
