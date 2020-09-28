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

using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Machine;

namespace Reko.Arch.WE32100
{
    using Reko.Core.Types;
    using System;
    using System.Linq;
    using Decoder = Reko.Core.Machine.Decoder<WE32100Disassembler, Mnemonic, WE32100Instruction>;

    public class WE32100Disassembler : DisassemblerBase<WE32100Instruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;

        private readonly WE32100Architecture arch;
        private readonly EndianImageReader rdr;
        private List<MachineOperand> ops;
        private Address addr;

        public WE32100Disassembler(WE32100Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override WE32100Instruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadByte(out byte op))
                return null;
            var instr = rootDecoder.Decode(op, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override WE32100Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new WE32100Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray()
            };
        }

        public override WE32100Instruction CreateInvalidInstruction()
        {
            return new WE32100Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.invalid,
                Operands = new MachineOperand[0]
            };
        }

        public override WE32100Instruction NotYetImplemented(uint wInstr, string message)
        {
            var rdr2 = rdr.Clone();
            var len = rdr.Address - this.addr;
            rdr.Offset -= len;
            var hexBytes = string.Join("", rdr.ReadBytes((int) len).Select(b => $"{b:X2}"));

            EmitUnitTest("WE32100", hexBytes, message, "WE32100Dis", this.addr, w =>
            {
                w.WriteLine("AssertCode(\"@@@\", \"{0}\");", hexBytes);
            });
            return base.NotYetImplemented(wInstr, message);
        }

        /*
        Absolute                        $expr       7   15          5 
        Absolute deferred               *$expr      14  15          5
        
        Displacement (from a register)
        Byte displacement               expr(%rn)   12  0-10,12-15  2
        Byte displacement deferred      *expr(%rn)  13  0-10,12-15  2
        Halfword displacement           expr(%rn)   10  0-10,12-15  3
        Halfword displacement deferred  *expr(%rn)  11  0-10,12-15  3
        Word displacement               expr(%rn)   8   0-10,12-15  5
        Word displacement deferred      expr(%rn)   9   0-10,12-15  5
        AP short offset                 so(%ap)     7   0-14        1   1
        FP short offset                 so(%fp)     6   0-14        1   1
        
        Immediate
        Byte immediate                  &imm8       6   15          2   2,3
        Halfword immediate              &imm16      5   15          3   2,3
        Word immediate                  &imm32      4   15          5   2,3
        Positive literal                &lit        0-3 0-15        1   2,3
        Negative literal                &lit        15  0-15        1   2,3
        
        Register
        Register                        %rn         4   0-14        1   1,3
        Register deferred               (%rn)       5   0-10,12-14  1
        
        Special Mode
        Expanded-operand type           {type}opnd 14   0-14        2-6
        */
        /*
        Notes:
        1. Mode field has special meaning if register field is 15; see absolute or
        immediate mode.
        2. Mode may not be used for a destination operand.
        3. Mode may not be used if the instruction takes effective address of the
        operand.
        4. type overrides instruction type; type determines the operand type,
        except that it does not determine the length for immediates or literals
        or whether literals are signed or unsigned. opnd determines actual
        address mode. For total bytes, add 1 to byte count for address mode
        determined by opnd.
        1
        4  */

        #region Mutators

        private static bool PositiveLiteral(int register, PrimitiveType dt, WE32100Disassembler dasm)
        {
            throw new NotImplementedException();
        }

        private static bool Register_WordImm(int register, PrimitiveType dt, WE32100Disassembler dasm)
        {
            if (register == 0xF)
                throw new NotImplementedException();
            dasm.ops.Add(new RegisterOperand(Registers.GpRegs[register]));
            return true;
        }

        private static bool RegisterDeferred_HalfwordImm(int register, PrimitiveType dt, WE32100Disassembler dasm)
        {
            throw new NotImplementedException();
        }

        private static bool FPshortOffset_ByteImm(int register, PrimitiveType dt, WE32100Disassembler dasm)
        {
            throw new NotImplementedException();
        }

        private static bool APshortOffset_Absolute(int register, PrimitiveType dt, WE32100Disassembler dasm)
        {
            if (register == 0xF)
            {
                if (!dasm.rdr.TryReadInt32(out int wAbsolute))
                    return false;
                dasm.ops.Add(new MemoryOperand(dt)
                {
                    Offset = wAbsolute,
                });
                return true;
            }
            throw new NotImplementedException();
        }

        private static bool WordDisplacement(int register, PrimitiveType dt, WE32100Disassembler dasm)
        {
            throw new NotImplementedException();
        }

        private static bool WordDisplacementDeferred(int register, PrimitiveType dt, WE32100Disassembler dasm)
        {
            throw new NotImplementedException();
        }

        private static bool HalfwordDisplacement(int register, PrimitiveType dt, WE32100Disassembler dasm)
        {
            throw new NotImplementedException();
        }

        private static bool HalfwordDisplacementDeferred(int register, PrimitiveType dt, WE32100Disassembler dasm)
        {
            throw new NotImplementedException();
        }

        private static bool ByteDisplacement(int register, PrimitiveType dt, WE32100Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte sOffset))
                return false;
            dasm.ops.Add(new MemoryOperand(dt)
            {
                Base = Registers.GpRegs[register],
                Offset = (sbyte) sOffset,
            });
            return true;
        }

        private static bool ByteDisplacementDeferred(int register, PrimitiveType dt, WE32100Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte sOffset))
                return false;
            dasm.ops.Add(new MemoryOperand(dt)
            {
                Base = Registers.GpRegs[register],
                Offset = (sbyte) sOffset,
                Deferred  = true,
            });
            return true;
        }

        private static bool ExpandedOperandType_AbsoluteDeferred(int register, PrimitiveType dt, WE32100Disassembler dasm)
        {
            if (register == 0xF)
            {
                if (!dasm.rdr.TryReadInt32(out int wAbsolute))
                    return false;
                dasm.ops.Add(new MemoryOperand(dt)
                {
                    Offset = wAbsolute,
                    Deferred = true,
                });
                return true;
            }
            throw new NotImplementedException();
        }

        private static bool NegativeLiteral(int register, PrimitiveType dt, WE32100Disassembler dasm)
        {
            throw new NotImplementedException();
        }

        private static bool InvalidOperand(int register, PrimitiveType dt, WE32100Disassembler dasm)
        {
            throw new NotImplementedException();
        }

        private static bool Register(int register, PrimitiveType dt, WE32100Disassembler dasm)
        {
            dasm.ops.Add(new RegisterOperand(Registers.GpRegs[register]));
            return true;
        }

        private static bool RegisterDeferred(int register, PrimitiveType dt, WE32100Disassembler dasm)
        {
            throw new NotImplementedException();
        }

        private static bool FPshortOffset(int register, PrimitiveType dt, WE32100Disassembler dasm)
        {
            throw new NotImplementedException();
        }

        private static readonly Func<int, PrimitiveType, WE32100Disassembler, bool>[] readAddressMode = new Func<int, PrimitiveType, WE32100Disassembler, bool>[16]
        {
            PositiveLiteral,
            PositiveLiteral,
            PositiveLiteral,
            PositiveLiteral,

            Register_WordImm,
            RegisterDeferred_HalfwordImm,
            FPshortOffset_ByteImm,
            APshortOffset_Absolute,

            WordDisplacement,
            WordDisplacementDeferred,
            HalfwordDisplacement,
            HalfwordDisplacementDeferred,

            ByteDisplacement,
            ByteDisplacementDeferred,
            ExpandedOperandType_AbsoluteDeferred,
            NegativeLiteral,
        };

        private static readonly Func<int, PrimitiveType, WE32100Disassembler, bool>[] writeAddressMode = new Func<int, PrimitiveType, WE32100Disassembler, bool>[16]
        {
            InvalidOperand,
            InvalidOperand,
            InvalidOperand,
            InvalidOperand,

            Register,
            RegisterDeferred,
            FPshortOffset,
            APshortOffset_Absolute,

            WordDisplacement,
            WordDisplacementDeferred,
            HalfwordDisplacement,
            HalfwordDisplacementDeferred,

            ByteDisplacement,
            ByteDisplacementDeferred,
            ExpandedOperandType_AbsoluteDeferred,
            InvalidOperand,
        };

        private static Mutator<WE32100Disassembler> R(PrimitiveType dt)
        {
            return (u, d) =>
            {
                if (!d.rdr.TryReadByte(out byte descriptor))
                    return false;
                return readAddressMode[descriptor >> 4](descriptor & 0xF, dt, d);
            };
        }
        private static readonly Mutator<WE32100Disassembler> Rb = R(PrimitiveType.Byte);
        private static readonly Mutator<WE32100Disassembler> Rh = R(PrimitiveType.Word16);
        private static readonly Mutator<WE32100Disassembler> Rw = R(PrimitiveType.Word32);

        private static Mutator<WE32100Disassembler> W(PrimitiveType dt)
        {
            return (u, d) =>
            {
                if (!d.rdr.TryReadByte(out byte descriptor))
                    return false;
                return writeAddressMode[descriptor >> 4](descriptor & 0xF, dt, d);
            };
        }

        private static readonly Mutator<WE32100Disassembler> Wb = W(PrimitiveType.Byte);
        private static readonly Mutator<WE32100Disassembler> Wh = W(PrimitiveType.Word16);
        private static readonly Mutator<WE32100Disassembler> Ww = W(PrimitiveType.Word32);

        private static Mutator<WE32100Disassembler> X(string msg)
        {
            return (u, d) =>
            {
                d.NotYetImplemented(u, msg);
                return false;
            };
        }
        #endregion

        #region Decoders

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<WE32100Disassembler>[] mutators)
        {
            return new InstrDecoder<WE32100Disassembler,Mnemonic,WE32100Instruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<WE32100Disassembler>[] mutators)
        {
            return new InstrDecoder<WE32100Disassembler, Mnemonic, WE32100Instruction>(iclass, mnemonic, mutators);
        }

        private class ExtDecoder : Decoder
        {
            private readonly Decoder extDecoder;

            public ExtDecoder(Decoder extDecoder)
            {
                this.extDecoder = extDecoder;
            }

            public override WE32100Instruction Decode(uint wInstr, WE32100Disassembler dasm)
            {
                if (!dasm.rdr.TryReadByte(out byte extOp))
                    return dasm.CreateInvalidInstruction();
                return extDecoder.Decode(extOp, dasm);
            }
        }

        private static Decoder Nyi(string msg)
        {
            return new NyiDecoder<WE32100Disassembler, Mnemonic, WE32100Instruction>(msg);
        }

        #endregion

        static WE32100Disassembler()
        {
            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);
            rootDecoder = Mask(0, 8, "WE32100",
                // 0x00
                Instr(Mnemonic.Invalid, InstrClass.Invalid | InstrClass.Zero | InstrClass.Padding),
                invalid,
                Instr(Mnemonic.spoprd, X("")),     // 0x02 Coprocessor operation read double
                Instr(Mnemonic.spopd2, X("")),         // 0x03 Coprocessor operation double, 2 - address

                Instr(Mnemonic.movaw, X("")),         // 0x04 Move address(word)
                Instr(Mnemonic.spoprt, X("")),         // 0x06 Coprocessor operation read triple
                Instr(Mnemonic.spopt2, X("")),         // 0x07 Coprocessor operation triple, 2 - address
                Instr(Mnemonic.ret, X("")),         // 0x08 Return from procedure

                invalid,
                invalid,
                invalid,
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                // 0x10
                Instr(Mnemonic.save, X("")),         // 0x 10 Save registers
                invalid,
                invalid,
                Instr(Mnemonic.spopwd, X("")),         // 0xl3 Coprocessor operation write double

                Instr(Mnemonic.ex, X("")),         // TOP 0xl4 Extended opcode
                invalid,
                invalid,
                Instr(Mnemonic.spopwt, X("")),         // 0xl7 Coprocessor operation write triple

                Instr(Mnemonic.restore, X("")),         // 0xl8 Restore registers
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.swapwi, X("")),         // 0x1C Swap word interlocked
                invalid,
                Instr(Mnemonic.swaphi, X("")),         // 0x1E Swap halfword interlocked
                Instr(Mnemonic.swapbi, X("")),         // 0x1F Swap byte interlocked

                // 0x20
                Instr(Mnemonic.popw, X("")),         // 0x20 Pop word
                invalid,
                Instr(Mnemonic.spoprs, X("")),         // 0x22 Coprocessor operation read single
                Instr(Mnemonic.spops2, X("")),         // 0x23 Coprocessor operation single, 2 - address

                Instr(Mnemonic.jmp, X("")),         // 0x24 Jump
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.tstw, X("")),         // 0x28 Test word
                invalid,
                Instr(Mnemonic.tsth, X("")),         // 0x2A Test halfword
                Instr(Mnemonic.tstb, X("")),         // 0x2B Test byte

                Instr(Mnemonic.call, X("")),         // 0x2C Call procedure
                invalid,
                Instr(Mnemonic.bpt, X("")),         // 0x2E Breakpoint trap
                invalid,

                // 0x30
                new ExtDecoder(Sparse(0, 8, "  30", invalid,

                    (0x09, Instr(Mnemonic.mverno, X(""))),         // 0x3009 Move version number
                    (0x19, Instr(Mnemonic.movblw, X(""))),         // 0x3019 Move block of words
                    (0x1F, Instr(Mnemonic.strend, X(""))),         // 0x301F String end
                    (0x35, Instr(Mnemonic.strcpy, X(""))))),         // 0x3035 String copy
                    invalid,
                Instr(Mnemonic.spop, X("")),         // 0x32
                Instr(Mnemonic.spopws, X("")),         // 0x33

                Instr(Mnemonic.jsb, X("")),          // 0x34
                invalid,
                Instr(Mnemonic.bsbh, X("")),         // 0x36
                Instr(Mnemonic.bsbb, X("")),         // 0x37

                Instr(Mnemonic.bitw, X("")),         // 0x38
                invalid,
                Instr(Mnemonic.bith, X("")),         // 0x3A
                Instr(Mnemonic.bitb, X("")),         // 0x3B

                Instr(Mnemonic.cmpw, X("")),         // 0x3C
                invalid,
                Instr(Mnemonic.cmph, X("")),         // 0x3E
                Instr(Mnemonic.cmpb, X("")),         // 0x3F

                // 0x40
                Instr(Mnemonic.rgeq, X("")),         // 0x40
                invalid,
                Instr(Mnemonic.bgeh, X("")),         // 0x42
                Instr(Mnemonic.bgeb, X("")),         // 0x43

                Instr(Mnemonic.rgtr, X("")),         // 0x44
                invalid,
                Instr(Mnemonic.bgh, X("")),         // 0x46
                Instr(Mnemonic.bgb, X("")),         // 0x47

                Instr(Mnemonic.rlss, X("")),         // 0x48
                invalid,
                Instr(Mnemonic.blh, X("")),         // 0x4A
                Instr(Mnemonic.blb, X("")),         // 0x4B

                Instr(Mnemonic.rleq, X("")),         // 0x4C
                invalid,
                Instr(Mnemonic.bleh, X("")),         // 0x4E
                Instr(Mnemonic.bleb, X("")),         // 0x4F

                // 0x50
                Instr(Mnemonic.rcc, X("")),         // 0x50 *
                invalid,
                //Instr(Mnemonic.rgequ, X("")),         // 0x50 *
                Instr(Mnemonic.bcch, X("")),         // 0x52 *
                                              //Instr(Mnemonic.bgeuh, X("")),         // 0x52 *
                Instr(Mnemonic.bccb, X("")),         // 0x53 *
                                  //Instr(Mnemonic.bgeub, X("")),         // 0x53 *

                Instr(Mnemonic.rgtru, X("")),         // 0x54
                invalid,
                Instr(Mnemonic.bguh, X("")),         // 0x56
                Instr(Mnemonic.bgub, X("")),         // 0x57

                Instr(Mnemonic.rcs, X("")),         // 0x58 *
                                             //Instr(Mnemonic.rlssu, X("")),         // 0x58 *
                invalid,
                Instr(Mnemonic.bcsh, X("")),         // 0x5A *
                                              //Instr(Mnemonic.bluh, X("")),         // 0x5A *
                Instr(Mnemonic.bcsb, X("")),         // 0x5B *
                                              //Instr(Mnemonic.blub, X("")),         // 0x5B *

                Instr(Mnemonic.rlequ, X("")),         // 0x5C
                invalid,
                Instr(Mnemonic.bleuh, X("")),         // 0x5E
                Instr(Mnemonic.bleub, X("")),         // 0x5F

    //Instr(Mnemonic.Coprocessor, X("")),         // operation
    //Instr(Mnemonic.Coprocessor, X("")),         // operation write single
    //Instr(Mnemonic.Jump, X("")),         // to subroutine
    //Instr(Mnemonic.Branch, X("")),         // to subroutine, halfword displacement
    //Instr(Mnemonic.Branch, X("")),         // to subroutine, byte displacement
    //Instr(Mnemonic.Bit, X("")),         // test word
    //Instr(Mnemonic.Bit, X("")),         // test halfword
    //Instr(Mnemonic.Bit, X("")),         // test byte
    //Instr(Mnemonic.Compare, X("")),         // word
    //Instr(Mnemonic.Compare, X("")),         // halfword
    //Instr(Mnemonic.Compare, X("")),         // byte
    //Instr(Mnemonic.Return, X("")),         // on greater than or equal(signed)
    //Instr(Mnemonic.Branch),         // on greater than or equal halfword(signed)
    //Instr(Mnemonic.Branch),         // on greater than or equal byte(signed)
    //Instr(Mnemonic.Return),         // on greater than(signed)
    //Instr(Mnemonic.Branch),         // on greater than halfword(signed)
    //Instr(Mnemonic.Branch),         // on greater than byte(signed)
    //Instr(Mnemonic.Return),         // on less than(signed)
    //Instr(Mnemonic.Branch),         // on less than halfword(signed)
    //Instr(Mnemonic.Branch),         // on less than byte(signed)
    //Instr(Mnemonic.Return),         // on less than or equal(signed)
    //Instr(Mnemonic.Branch),         // on less than or equal halfword(signed)
    //Instr(Mnemonic.Branch),         // on less than or equal byte(signed)
    //Instr(Mnemonic.Return),         // on carry clear
    //Instr(Mnemonic.Return),         // on greater than or equal(unsigned)
    //Instr(Mnemonic.Branch),         // on carry clear halfword
    //Instr(Mnemonic.Branch),         // on greater than or equal halfword(unsigned)
    //Instr(Mnemonic.Branch),         // on carry clear byte
    //Instr(Mnemonic.Branch),         // on greater than or equal byte(unsigned)
    //Instr(Mnemonic.Return),         // on greater than(unsigned)
    //Instr(Mnemonic.Branch),         // on greater than halfword(unsigned)
    //Instr(Mnemonic.Branch),         // on greater than byte(unsigned)
    //Instr(Mnemonic.Return),         // on carry set
    //Instr(Mnemonic.Return),         // on less than(unsigned)
    //Instr(Mnemonic.Branch),         // on carry set halfword
    //Instr(Mnemonic.Branch),         // on less than halfword(unsigned)
    //Instr(Mnemonic.Branch),         // on carry set byte
    //Instr(Mnemonic.Branch),         // on less than byte(unsigned)
    //Instr(Mnemonic.Return),         // on less than or equal(unsigned)
    //Instr(Mnemonic.Branch),         // on less than or equal halfword(unsigned)
    //Instr(Mnemonic.Branch),         // on less than or equal byte(unsigned)

                // 0x60
                Instr(Mnemonic.rvc, X("")),         // 0x60 Return on overflow clear
                invalid,
                Instr(Mnemonic.bvch, X("")),         // 0x62 Branch on overflow clear, halfword displacement
                Instr(Mnemonic.bvcb, X("")),         // 0x63 Branch on overflow clear, byte displacement

                Instr(Mnemonic.rnequ, X("")),         // 0x64 Return on not equal(unsigned)
                invalid,
                Instr(Mnemonic.bneh, X("")),         // 0x66 Branch on not equal halfword(duplicate)
                Instr(Mnemonic.bneb, X("")),         // 0x67 Branch on not equal byte(duplicate)

                Instr(Mnemonic.rvs, X("")),         // 0x68 Return on overflow set
                invalid,
                Instr(Mnemonic.bvsh, X("")),         // 0x6A Branch on overflow set, halfword displacement
                Instr(Mnemonic.bvsb, X("")),         // 0x6B Branch on overflow set, byte displacement

                Instr(Mnemonic.reqlu, X("")),         // 0x6C Return on equal(unsigned)
                invalid,
                Instr(Mnemonic.beh, X("")),         // 0x6E Branch on equal halfword(duplicate)
                Instr(Mnemonic.beb, X("")),         // 0x6F Branch on equal byte(duplicate)

                // 0x70
                Instr(Mnemonic.nop, X("")),         // 0x70 No operation, 1 byte
                invalid,
                Instr(Mnemonic.nop3, X("")),         // 0x72 No operation, 3 bytes
                Instr(Mnemonic.nop2, X("")),         // 0x73 No operation, 2 bytes

                Instr(Mnemonic.rneq, X("")),         // 0x74 Return on not equal(signed)
                invalid,
                Instr(Mnemonic.bneh, X("")),         // 0x76 Branch on not equal halfword
                Instr(Mnemonic.bneb, X("")),         // 0x77 Branch on not equal

                Instr(Mnemonic.rsb, X("")),         // 0x78 Return from subroutine
                invalid,
                Instr(Mnemonic.brh, X("")),         // 0x7A Branch with halfword(J 6 - bit) displacement
                Instr(Mnemonic.brh, X("")),         // 0x7B Branch with byte(8 - bit) displacement

                Instr(Mnemonic.reql, X("")),         // 0x7C Return on equal(signed)
                invalid,
                Instr(Mnemonic.beh, X("")),         // 0x7E Branch on equal halfword
                Instr(Mnemonic.beb, X("")),         // 0x7F Branch on equal byte

                // 0x80
                Instr(Mnemonic.clrw, X("")),         // 0x80 Clear word
                invalid,
                Instr(Mnemonic.clrh, X("")),         // 0x82 Clear halfword
                Instr(Mnemonic.clrb, X("")),         // 0x83 Clear byte

                Instr(Mnemonic.movw, X("")),         // 0x84 Move word
                invalid,
                Instr(Mnemonic.movh, X("")),         // 0x86 Move halfword
                Instr(Mnemonic.movb, Rb,Wb),         // 0x87 Move byte

                Instr(Mnemonic.mcomw, X("")),         // 0x88 Move complemented word
                invalid,
                Instr(Mnemonic.mcomh, X("")),         // 0x8A Move complemented halfword
                Instr(Mnemonic.mcomb, X("")),         // 0x8B Move complemented byte

                Instr(Mnemonic.mnegw, X("")),         // 0x8C Move negated word
                invalid,
                Instr(Mnemonic.mnegh, X("")),         // 0x8E Move negated halfword
                Instr(Mnemonic.mnegb, X("")),         // 0x8F Move negated byte

                // 0x90
            Instr(Mnemonic.incw, X("")),         // 0x90 Increment word
            invalid,
            Instr(Mnemonic.inch, X("")),         // 0x92 Increment halfword
            Instr(Mnemonic.incb, X("")),         // 0x93 Increment byte

            Instr(Mnemonic.decw, X("")),         // 0x94 Decrement word
            invalid,
            Instr(Mnemonic.dech, Wh),         // 0x96 Decrement halfword
            Instr(Mnemonic.decb, X("")),         // 0x97 Decrement byte

            invalid,
            invalid,
            invalid,
            invalid,

            Instr(Mnemonic.addw2, X("")),         // 0x9C Add word
            invalid,
            Instr(Mnemonic.addh2, X("")),         // 0x9E Add halfword
            Instr(Mnemonic.addb2, X("")),         // 0x9F Add byte

            // 0xA0
            Instr(Mnemonic.pushw, X("")),          // 0xA0         // Push word
            invalid,
            invalid,
            invalid,

            Instr(Mnemonic.modw2, X("")),         // 0xA4),         // Modulo word
            invalid,
            Instr(Mnemonic.modh2, X("")),         // 0xA6),         // Modulo halfword
            Instr(Mnemonic.modb2, X("")),         // 0xA7),         // Modulo byte

            Instr(Mnemonic.mulw2, X("")),         // 0xA8),         // Multiply word
            invalid,
            Instr(Mnemonic.mulh2, X("")),         // 0xAA),         // Multiply halfword
            Instr(Mnemonic.mulb2, X("")),         // 0xAB),         // Multiply byte

            Instr(Mnemonic.divw2, X("")),         // 0xAC),         // Divide word
            invalid,
            Instr(Mnemonic.divh2, X("")),         // 0xAE),         // Divide halfword
            Instr(Mnemonic.divb2, X("")),         // 0xAF),         // Divide byte

            // 0xB0
            Instr(Mnemonic.orw2, X("")),         // 0xB0),         // OR word
            invalid,
            Instr(Mnemonic.orh2, X("")),         // 0xB2),         // OR halfword
            Instr(Mnemonic.orb2, X("")),         // 0xB3),         // OR byte

            Instr(Mnemonic.xorw2, Rw,Ww),         // 0xB4),         // Exclusive OR word
            invalid,
            Instr(Mnemonic.xorh2, Rh,Wh),         // 0xB6),         // Exclusive OR halfword
            Instr(Mnemonic.xorb2, Rb,Wb),         // 0xB7),         // Exclusive OR byte

            Instr(Mnemonic.andw2, X("")),         // 0xB8),         // AND word
            invalid,
            Instr(Mnemonic.andh2, X("")),         // 0xBA),         // AND halfword
            Instr(Mnemonic.andb2, X("")),         // 0xBB),         // AND byte

            Instr(Mnemonic.subw2, X("")),         // 0xBC),         // Subtract word
            invalid,
            Instr(Mnemonic.subh2, X("")),         // 0xBE),         // Subtract halfword
            Instr(Mnemonic.subb2, X("")),         // 0xBF),         // Subtract byte

            // 0xC0
            Instr(Mnemonic.alsw3, X("")),         // 0xCO),         // Arithmetic left shift word
            invalid,
            invalid,
            invalid,

            Instr(Mnemonic.arsw3, X("")),         // 0xC4),         // Arithmetic right shift word
            invalid,
            Instr(Mnemonic.arsh3, X("")),         // 0xC6),         // Arithmetic right shift halfword
            Instr(Mnemonic.arsb3, X("")),         // 0xC7),         // Arithmetic right shift byte

            Instr(Mnemonic.insfw, X("")),         // 0xC8),         // Insert field word
            invalid,
            Instr(Mnemonic.insfh, X("")),         // 0xCA),         // Insert field halfword
            Instr(Mnemonic.insfb, X("")),         // 0xCB),         // Insert field byte

            Instr(Mnemonic.extfw, X("")),         // 0xCC),         // Extract field word
            invalid,
            Instr(Mnemonic.extfh, X("")),         // 0xCE),         // Extract field halfword
            Instr(Mnemonic.extfb, X("")),         // 0xCF),         // Extract field byte

            // 0xD0
            Instr(Mnemonic.llsw3, X("")),         // 0xD0         // Logical left shift word
            invalid,
            Instr(Mnemonic.llsh3, X("")),         // 0xD2),         // Logical left shift halfword
            Instr(Mnemonic.llsb3, X("")),         // 0xD3),         // Logical left shift byte

            Instr(Mnemonic.lrsw3, X("")),         // 0xD4),         // Logical right shift word
            invalid,
            invalid,
            invalid,

            Instr(Mnemonic.rotw, X("")),         // 0xD8),         // Rotate word
            invalid,
            invalid,
            invalid,

            Instr(Mnemonic.addw3, Rw,Rw,Ww),      // 0xDC Add word, 3 - address
            invalid,
            Instr(Mnemonic.addh3, Rh,Rh,Wh),      // 0xDE Add halfword, 3 - address
            Instr(Mnemonic.addb3, Rb,Rb,Wb),      // 0xDF Add byte, 3 - address

            // 0xE0
            Instr(Mnemonic.pushaw, X("")),  // 0xEO Push address word
            invalid,
            invalid,
            invalid,

            Instr(Mnemonic.modw3, X("")),         // 0xE4 Modulo word, 3 - address
            invalid,
            Instr(Mnemonic.modh3, X("")),         // 0xE6 Modulo halfword, 3 - address
            Instr(Mnemonic.modb3, X("")),         // 0xE? Modulo byte, 3 - address

            Instr(Mnemonic.mulw3, X("")),         // 0xE8 Multiply word, 3 - address
            invalid,
            Instr(Mnemonic.mulh3, X("")),         // 0xEA Multiply halfword, 3 - address
            Instr(Mnemonic.mulb3, X("")),         // 0xEB Multiply byte, 3 - address

            Instr(Mnemonic.divw3, X("")),         // 0xEC Divide word, 3 - address
            invalid,
            Instr(Mnemonic.divh3, X("")),         // 0xEE Divide halfword, 3 - address
            Instr(Mnemonic.divb3, X("")),         // 0xEF Divide byte, 3 - address

            // 0xF0
            Instr(Mnemonic.orw3, X("")),         // 0xF0 OR word, 3 - address
            invalid,
            Instr(Mnemonic.orh3, X("")),         // 0xF2 OR halfword, 3 - address
            Instr(Mnemonic.orb3, X("")),         // 0xF3 OR byte, 3 - address

            Instr(Mnemonic.xorw3, X("")),         // 0xF4 Exclusive OR word, 3 - address
            invalid,
            Instr(Mnemonic.xorh3, X("")),         // 0xF6 Exclusive OR halfword, 3 - address
            Instr(Mnemonic.xorb3, X("")),         // 0xF? Exclusive OR byte, 3 - address

            Instr(Mnemonic.andw3, X("")),         // 0xF8 AND word, 3 - address
            invalid,
            Instr(Mnemonic.andh3, X("")),         // 0xFA AND halfword, 3 - address
            Instr(Mnemonic.andb3, X("")),         // 0xFB AND byte, 3 - address

            Instr(Mnemonic.subw3, X("")),         // 0xFC Subtract word, 3 - address
            invalid,
            Instr(Mnemonic.subh3, X("")),         // 0x FE Subtract halfword, 3 - address
            Instr(Mnemonic.subb3, X(""))); // 0xFF Subtract byte, 3 - address

        }
    }
}