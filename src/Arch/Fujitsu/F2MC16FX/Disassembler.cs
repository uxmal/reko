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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Fujitsu.F2MC16FX
{
    using Decoder = Decoder<Disassembler, Mnemonic, Instruction>;

    public class Disassembler : DisassemblerBase<Instruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;
        private static readonly PrimitiveType nybble; 

        private readonly F2MC16FXArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;
        private RegisterStorage? prefix;

        public Disassembler(F2MC16FXArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.addr = default!;
            this.ops = new List<MachineOperand>();
        }

        public override Instruction? DisassembleInstruction()
        {
            var offset = rdr.Offset;
            this.addr = rdr.Address;
            if (!rdr.TryReadByte(out byte uOpcode))
                return null;
            var instr = rootDecoder.Decode(uOpcode, this);

            ops.Clear();
            prefix = null;

            instr.Address = addr;
            instr.Length = (int) (rdr.Offset - offset);
            return instr;
        }

        public override Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new Instruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray(),
            };
        }

        public override Instruction CreateInvalidInstruction()
        {
            return new Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid
            };
        }

        public override Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("F2MC16FXDis", this.addr, rdr, message);
            //$DEBUG
            return new Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.nyi
            };
        }

        #region Mutators

        private static bool a(uint uInstr, Disassembler dasm)
        {
            dasm.ops.Add(Registers.a);
            return true;
        }

        private static bool ah(uint uInstr, Disassembler dasm)
        {
            dasm.ops.Add(Registers.ah);
            return true;
        }

        private static bool inda(uint uInstr, Disassembler dasm)
        {
            dasm.ops.Add(MemoryOperand.RegisterIndirect(Registers.a));
            return true;
        }

        private static bool indal(uint uInstr, Disassembler dasm)
        {
            dasm.ops.Add(MemoryOperand.RegisterIndirect(Registers.al));
            return true;
        }

        private static Mutator<Disassembler> r(int bitoffset)
        {
            var bitfield = new Bitfield(bitoffset, 3);
            return (u, d) =>
            {
                var iReg = bitfield.Read(u);
                d.ops.Add(Registers.r[iReg]);
                return true;
            };
        }
        private static readonly Mutator<Disassembler> r0 = r(0);
        private static readonly Mutator<Disassembler> r5 = r(5);

        private static Mutator<Disassembler> rw(int bitoffset)
        {
            var bitfield = new Bitfield(bitoffset, 3);
            return (u, d) =>
            {
                var iReg = bitfield.Read(u);
                d.ops.Add(Registers.rw[iReg]);
                return true;
            };
        }
        private static readonly Mutator<Disassembler> rw0 = rw(0);
        private static readonly Mutator<Disassembler> rw5 = rw(5);

        private static Mutator<Disassembler> Register(RegisterStorage reg)
        {
            return (u, d) =>
            {
                d.ops.Add(reg);
                return true;
            };
        }
        private static readonly Mutator<Disassembler> adb = Register(Registers.adb);
        private static readonly Mutator<Disassembler> ccr = Register(Registers.ccr);
        private static readonly Mutator<Disassembler> dpr = Register(Registers.dpr);
        private static readonly Mutator<Disassembler> dtb = Register(Registers.dtb);
        private static readonly Mutator<Disassembler> ilm = Register(Registers.ilm);
        private static readonly Mutator<Disassembler> pcb = Register(Registers.pcb);
        private static readonly Mutator<Disassembler> ps = Register(Registers.ps);
        private static readonly Mutator<Disassembler> r0b = Register(Registers.r[0]);
        private static readonly Mutator<Disassembler> rp = Register(Registers.rp);
        private static readonly Mutator<Disassembler> ssb = Register(Registers.ssb);
        private static readonly Mutator<Disassembler> sp = Register(Registers.usp);
        private static readonly Mutator<Disassembler> usb = Register(Registers.usb);


        private static bool rlst(uint uInstr, Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out var imm8))
                return false;
            dasm.ops.Add(new RegisterListOperand(imm8));
            return true;
        }

        private static bool rel(uint uInstr, Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out var imm8))
                return false;
            dasm.ops.Add(dasm.addr + (sbyte) imm8);
            return true;
        }

        private static bool dir(uint uInstr, Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out var imm8))
                return false;
            dasm.ops.Add(MemoryOperand.Direct(imm8));
            return true;
        }


        private static bool io(uint uInstr, Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out var imm8))
                return false;
            dasm.ops.Add(MemoryOperand.Io(imm8));
            return true;
        }

        private static bool imm8(uint uInstr, Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out var imm8))
                return false;
            dasm.ops.Add(ImmediateOperand.Byte(imm8));
            return true;
        }

        private static bool imm4(uint uInstr, Disassembler dasm)
        {
            dasm.ops.Add(new ImmediateOperand(Constant.Create(nybble, uInstr & 0xF)));
            return true;
        }

        private static bool imm16(uint uInstr, Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out var imm16))
                return false;
            dasm.ops.Add(ImmediateOperand.Word16(imm16));
            return true;
        }

        private static bool imm32(uint uInstr, Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt32(out var imm32))
                return false;
            dasm.ops.Add(ImmediateOperand.UInt32(imm32));
            return true;
        }

        private static bool br0(uint uInstr, Disassembler dasm)
        {
            dasm.ops.Add(ImmediateOperand.Byte((byte)(uInstr & 0x3)));
            return true;
        }

        private static bool br2(uint uInstr, Disassembler dasm)
        {
            dasm.ops.Add(ImmediateOperand.Byte((byte) ((uInstr & 0xC) >> 2)));
            return true;
        }

        private static bool vct8(uint uInstr, Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out var vct))
                return false;
            dasm.ops.Add(new VectorOperand(vct));
            return true;
        }

        private static Mutator<Disassembler> reg_disp8(RegisterStorage[] regs)
        {
            return (u, d) =>
            {
                if (!d.rdr.TryReadByte(out byte disp))
                    return false;
                var disp8 = Constant.SByte((sbyte) disp);
                var reg = regs[u & 0x7];
                d.ops.Add(MemoryOperand.RegisterIndirect(reg, disp8));
                return true;
            };
        }
        private static readonly Mutator<Disassembler> rl_disp8 = reg_disp8(Registers.rl);
        private static readonly Mutator<Disassembler> rw_disp8 = reg_disp8(Registers.rw);


        private static Mutator<Disassembler> ea(RegisterStorage?[] registers, bool indirect)
        {
            return (u, d) =>
            {
                uint iReg = u & 7;
                switch (u & 0x1F)
                {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                    var reg = registers[iReg];
                    if (reg is null)
                        return false;
                    d.ops.Add(reg);
                    break;
                case 8:
                case 9:
                case 0xA:
                case 0xB:
                    d.ops.Add(MemoryOperand.RegisterIndirect(Registers.rw[iReg & 3]));
                    break;
                case 0xC:
                case 0xD:
                case 0xE:
                case 0xF:
                    d.ops.Add(MemoryOperand.RegisterIndirectPost(Registers.rw[iReg & 3]));
                    break;
                case 0x10:
                case 0x11:
                case 0x12:
                case 0x13:
                case 0x14:
                case 0x15:
                case 0x16:
                case 0x17:
                    if (!d.rdr.TryReadByte(out var bdisp8))
                        return false;
                    var disp8 = Constant.SByte((sbyte) bdisp8);
                    d.ops.Add(MemoryOperand.RegisterIndirect(Registers.rw[iReg], disp8));
                    break;
                case 0x18:
                case 0x19:
                case 0x1A:
                case 0x1B:
                    if (!d.rdr.TryReadLeInt16(out var wdisp16))
                        return false;
                    var disp16 = Constant.Int16(wdisp16);
                    d.ops.Add(MemoryOperand.RegisterIndirect(Registers.rw[iReg & 3], disp16));
                    break;
                case 0x1C:
                    d.ops.Add(MemoryOperand.RegisterIndexed(Registers.rw[0], Registers.rw[7]));
                    break;
                case 0x1D:
                    d.ops.Add(MemoryOperand.RegisterIndexed(Registers.rw[1], Registers.rw[7]));
                    break;
                case 0x1E:
                    if (!d.rdr.TryReadLeInt16(out wdisp16))
                        return false;
                    disp16 = Constant.Int16(wdisp16);
                    d.ops.Add(MemoryOperand.RegisterIndirect(Registers.pc, disp16));
                    break;
                case 0x1F:
                    if (!d.rdr.TryReadLeUInt16(out var addr16))
                        return false;
                    d.ops.Add(MemoryOperand.Addr16(addr16));
                    break;
                }
                return true;
            };
        }
        private static readonly RegisterStorage?[] ea_l_registers = new[] {
            Registers.rl[0],
            null,
            Registers.rl[1],
            null,
            Registers.rl[2],
            null,
            Registers.rl[3],
            null
        };
        private static readonly Mutator<Disassembler> ea_b = ea(Registers.r, false);
        private static readonly Mutator<Disassembler> ea_w = ea(Registers.rw, false);
        private static readonly Mutator<Disassembler> ea_l = ea(ea_l_registers, false);
        private static readonly Mutator<Disassembler> ea_iw = ea(Registers.rw, true);
        private static readonly Mutator<Disassembler> ea_il = ea(ea_l_registers, true);


        /// <summary>
        /// Data address
        /// </summary>
        private static bool daddr16(uint uInstr, Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort addr16))
                return false;
            dasm.ops.Add(MemoryOperand.Addr16(addr16));
            return true;
        }

        /// <summary>
        /// Code address.
        /// </summary>
        private static bool addr16(uint uInstr, Disassembler dasm)
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort addr16))
                return false;
            dasm.ops.Add(Address.Ptr16(addr16));
            return true;
        }

        private static bool addr24(uint uInstr, Disassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte lo) ||
                 dasm.rdr.TryReadByte(out byte mi) ||
                 dasm.rdr.TryReadByte(out byte hi))
                return false;
            var addr24 = 
                   (uint) lo
                | ((uint) mi << 8)
                | ((uint) hi << 16);
            dasm.ops.Add(Address.Ptr32(addr24));
            return true;
        }

        private static bool bp(uint uInstr, Disassembler dasm)
        {
            var op = dasm.ops[^1];
            op = new BitPositionOperand(uInstr & 7, op);
            dasm.ops[^1] = op;
            return true;
        }

        #endregion

        private class PrefixDecoder : Decoder
        {
            private readonly RegisterStorage prefix;

            public PrefixDecoder(RegisterStorage reg)
            {
                this.prefix = reg;
            }

            public override Instruction Decode(uint wInstr, Disassembler dasm)
            {
                if (!dasm.rdr.TryReadByte(out byte uInstr))
                    return dasm.CreateInvalidInstruction();
                // When there are multiple prefixes, this will overwrite the last one.
                dasm.prefix = this.prefix;
                return rootDecoder.Decode(uInstr, dasm);
            }
        }

        private class TwoByteDecoder : Decoder
        {
            private readonly Decoder decoder;

            public TwoByteDecoder(Decoder decoder)
            {
                this.decoder = decoder;
            }

            public override Instruction Decode(uint wInstr, Disassembler dasm)
            {
                if (!dasm.rdr.TryReadByte(out byte uInstr))
                    return dasm.CreateInvalidInstruction();
                return decoder.Decode(uInstr, dasm);
            }
        }


        private static Decoder Instr(Mnemonic mnemonic, params Mutator<Disassembler>[] mutators)
        {
            return new InstrDecoder<Disassembler, Mnemonic, Instruction>(
                InstrClass.Linear,
                mnemonic,
                mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<Disassembler>[] mutators)
        {
            return new InstrDecoder<Disassembler, Mnemonic, Instruction>(
                iclass,
                mnemonic,
                mutators);
        }

        private static Decoder Prefix(RegisterStorage reg)
        {
            return new PrefixDecoder(reg);
        }

        private static Decoder TwoByteInstr(Decoder decoder)
        {
            return new TwoByteDecoder(decoder);
        }

        static Disassembler()
        {
            nybble = PrimitiveType.CreateWord(4);

            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);

            var decode_ea_70 = Mask(5, 3, "  70",
                Instr(Mnemonic.addl, a, ea_l),
                Instr(Mnemonic.subl, a, ea_l),
                Instr(Mnemonic.cwbne, ea_w, imm16, rel),
                Instr(Mnemonic.cmpl, a, ea_l),
                Instr(Mnemonic.andl, a, ea_l),
                Instr(Mnemonic.orl, a, ea_l),
                Instr(Mnemonic.xorl, a, ea_l),
                Instr(Mnemonic.cwbne, ea_b, imm16, rel));
            
            var decode_ea_71 = Mask(5, 3, "  71",
                Instr(Mnemonic.jmpp, ea_il),
                Instr(Mnemonic.callp, ea_il),
                Instr(Mnemonic.incl, ea_l),
                Instr(Mnemonic.decl, ea_l),
                Instr(Mnemonic.movl, a, ea_b),
                Instr(Mnemonic.movl, ea_l, a),
                Instr(Mnemonic.mov, ea_b, imm8),
                Instr(Mnemonic.movea, a, ea_w));

            var decode_ea_72 = Mask(5, 3, "  72",
                Instr(Mnemonic.rolc, ea_b),
                Instr(Mnemonic.rorc, ea_b),
                Instr(Mnemonic.inc, ea_b),
                Instr(Mnemonic.dec, ea_b),
                Instr(Mnemonic.mov, a, ea_b),
                Instr(Mnemonic.mov, ea_b, a),
                Instr(Mnemonic.movx, a, ea_b),
                Instr(Mnemonic.xch, a, ea_b));

            var decode_ea_73 = Mask(5, 3, "  73",
                Instr(Mnemonic.jmp, ea_iw),
                Instr(Mnemonic.call, ea_iw),
                Instr(Mnemonic.incw, ea_w),
                Instr(Mnemonic.decw, ea_w),
                Instr(Mnemonic.movw, a, ea_w),
                Instr(Mnemonic.movw, ea_w, a),
                Instr(Mnemonic.movw, ea_w, imm16),
                Instr(Mnemonic.xchw, a, ea_w));

            var decode_ea_74 = Mask(5, 3, "  74",
                Instr(Mnemonic.add, a, ea_b),
                Instr(Mnemonic.sub, a, ea_b),
                Instr(Mnemonic.addc, a, ea_b),
                Instr(Mnemonic.cmp, a, ea_b),
                Instr(Mnemonic.and, a, ea_b),
                Instr(Mnemonic.or, a, ea_b),
                Instr(Mnemonic.xor, a, ea_b),
                Instr(Mnemonic.dbnz, ea_b, rel));

            var decode_ea_75 = Mask(5, 3, "  75",
                Instr(Mnemonic.add, ea_b, a),
                Instr(Mnemonic.sub, ea_b, a),
                Instr(Mnemonic.subc, a, ea_b),
                Instr(Mnemonic.neg, ea_b),
                Instr(Mnemonic.and, ea_b, a),
                Instr(Mnemonic.or, ea_b, a),
                Instr(Mnemonic.xor, ea_b, a),
                Instr(Mnemonic.not, ea_b));

            var decode_ea_76 = Mask(5, 3, "  76",
                Instr(Mnemonic.addw, a, ea_w),
                Instr(Mnemonic.subw, a, ea_w),
                Instr(Mnemonic.addcw, a, ea_w),
                Instr(Mnemonic.cmpw, a, ea_w),
                Instr(Mnemonic.andw, a, ea_w),
                Instr(Mnemonic.orw, a, ea_w),
                Instr(Mnemonic.xorw, a, ea_w),
                Instr(Mnemonic.dwbnz, ea_w, rel));

            var decode_ea_77 = Mask(5, 3, "  77",
                Instr(Mnemonic.addw, ea_w, a),
                Instr(Mnemonic.subw, ea_w, a),
                Instr(Mnemonic.subcw, a, ea_w),
                Instr(Mnemonic.negw, ea_w, a),
                Instr(Mnemonic.andw, ea_w, a),
                Instr(Mnemonic.orw,  ea_w, a),
                Instr(Mnemonic.xorw, ea_w, a),
                Instr(Mnemonic.notw, ea_w));

            var decode_ea_78 = Mask(5, 3, "  78",
                Instr(Mnemonic.mulu, a, ea_b),
                Instr(Mnemonic.muluw, a, ea_w),
                Instr(Mnemonic.mul, a, ea_b),
                Instr(Mnemonic.mulw, a, ea_w),
                Instr(Mnemonic.divu, a, ea_b),
                Instr(Mnemonic.divuw, a, ea_w),
                Instr(Mnemonic.div, a, ea_b),
                Instr(Mnemonic.divw, a, ea_w));

            var decode_ea_79 = Instr(Mnemonic.movea, rw5, ea_w);

            var decode_ea_7a = Instr(Mnemonic.mov, r5, ea_b);

            var decode_ea_7b = Instr(Mnemonic.movw, rw5, ea_w);

            var decode_ea_7c = Instr(Mnemonic.mov, ea_b, r5);

            var decode_ea_7d = Instr(Mnemonic.mov, ea_w, rw5);

            var decode_ea_7e = Instr(Mnemonic.xch, r5, ea_b);

            var decode_ea_7f = Instr(Mnemonic.xchw, rw5, ea_w);

            var callv = Instr(Mnemonic.callv, InstrClass.Transfer|InstrClass.Call, vct8);
            var mov_a_ri = Instr(Mnemonic.movx, a, r0);
            var mov_ri_a = Instr(Mnemonic.movx, r0, a);
            var mov_ri_imm8 = Instr(Mnemonic.movx, r0, imm8);
            var mov_rwi_imm16 = Instr(Mnemonic.movx, rw0, imm16);
            var movn_a_imm4 = Instr(Mnemonic.movn, a, imm4);
            var movw_a_rwi = Instr(Mnemonic.movw, a, rw0);
            var movw_a_reg_disp8 = Instr(Mnemonic.movw, a, rw_disp8);
            var movw_reg_disp8_a = Instr(Mnemonic.movw, rw_disp8, a);
            var movw_rwi_a = Instr(Mnemonic.movw, rw0, a);
            var movx_a_ri = Instr(Mnemonic.movx, a, r0);
            var movx_a_reg_disp8 = Instr(Mnemonic.movx, a, rw_disp8);

            var decode_bit_manipulation = Sparse(3, 5, "  bit manipulation", invalid,
                (0x00, Instr(Mnemonic.movb, a, io, bp)),
                (0x01, Instr(Mnemonic.movb, a, dir, bp)),
                (0x03, Instr(Mnemonic.movb, a, addr16, bp)),
                (0x04, Instr(Mnemonic.movb, io, bp, a)),
                (0x05, Instr(Mnemonic.movb, dir, bp, a)),
                (0x07, Instr(Mnemonic.movb, addr16, a, bp)),
                (0x08, Instr(Mnemonic.clrb, io, bp)),
                (0x09, Instr(Mnemonic.clrb, dir, bp)),
                (0x0B, Instr(Mnemonic.clrb, addr16, bp)),
                (0x0C, Instr(Mnemonic.setb, io, bp)),
                (0x0D, Instr(Mnemonic.setb, dir, bp)),
                (0x0F, Instr(Mnemonic.setb, addr16, bp)),
                (0x10, Instr(Mnemonic.bbc, InstrClass.ConditionalTransfer, io, bp, rel)),
                (0x11, Instr(Mnemonic.bbc, InstrClass.ConditionalTransfer, dir, bp, rel)),
                (0x13, Instr(Mnemonic.bbc, InstrClass.ConditionalTransfer, addr16, bp, rel)),
                (0x14, Instr(Mnemonic.bbs, InstrClass.ConditionalTransfer, io, bp, rel)),
                (0x15, Instr(Mnemonic.bbs, InstrClass.ConditionalTransfer, dir, bp, rel)),
                (0x17, Instr(Mnemonic.bbs, InstrClass.ConditionalTransfer, addr16, bp, rel)),
                (0x18, Instr(Mnemonic.wbts, io, bp)),
                (0x1C, Instr(Mnemonic.wbtc, io, bp)),
                (0x1F, Instr(Mnemonic.sbbs, io, bp)));

            static bool bank_selector(uint u) => (u & 0xC0) == 0;

            var decode_string_manipulation = Sparse(4, 4, "  string manipulation", invalid,
                (0x0, Instr(Mnemonic.movsi, br2, br0)),
                (0x1, Instr(Mnemonic.movsd, br2, br0)),
                (0x2, Instr(Mnemonic.movswi, br2, br0)),
                (0x3, Instr(Mnemonic.movswd, br2, br0)),

                (0x8, If(bank_selector, Instr(Mnemonic.sceqi, br0))),
                (0x9, If(bank_selector, Instr(Mnemonic.sceqd, br0))),
                (0xA, If(bank_selector, Instr(Mnemonic.scweqi, br0))),
                (0xB, If(bank_selector, Instr(Mnemonic.scweqd, br0))),
                (0xC, If(bank_selector, Instr(Mnemonic.filsi, br0))),
                (0xE, If(bank_selector, Instr(Mnemonic.filswi, br0))));

            var decode_two_byte = Sparse(0, 8, "  two-byte", invalid,
                (0x00, Instr(Mnemonic.mov, a, dtb)),
                (0x01, Instr(Mnemonic.mov, a, adb)),
                (0x02, Instr(Mnemonic.mov, a, ssb)),
                (0x03, Instr(Mnemonic.mov, a, usb)),
                (0x04, Instr(Mnemonic.mov, a, dpr)),
                (0x05, Instr(Mnemonic.mov, a, inda)),
                (0x06, Instr(Mnemonic.mov, a, pcb)),
                (0x07, Instr(Mnemonic.rolc, a)),
                (0x0C, Instr(Mnemonic.lslw, a, r0b)),
                (0x0D, Instr(Mnemonic.movw, a, inda)),
                (0x0E, Instr(Mnemonic.asrw, a, r0b)),
                (0x0F, Instr(Mnemonic.lsrw, a, r0b)),
                (0x10, Instr(Mnemonic.mov, dtb, a)),
                (0x11, Instr(Mnemonic.mov, adb, a)),
                (0x12, Instr(Mnemonic.mov, ssb, a)),
                (0x13, Instr(Mnemonic.mov, usb, a)),
                (0x14, Instr(Mnemonic.mov, dpr, a)),
                (0x15, Instr(Mnemonic.mov, indal, ah)),
                (0x16, Instr(Mnemonic.mov, a, inda)),
                (0x17, Instr(Mnemonic.rorc, a)),
                (0x1C, Instr(Mnemonic.lsll, a, r0b)),
                (0x1D, Instr(Mnemonic.movw, indal, ah)),
                (0x1E, Instr(Mnemonic.asrl, a, r0b)),
                (0x1F, Instr(Mnemonic.lsrl, a, r0b)),

                (0x20, Instr(Mnemonic.movx, a, rl_disp8)),
                (0x22, Instr(Mnemonic.movx, a, rl_disp8)),
                (0x24, Instr(Mnemonic.movx, a, rl_disp8)),
                (0x26, Instr(Mnemonic.movx, a, rl_disp8)),

                (0x2C, Instr(Mnemonic.lsl, a, r0b)),
                (0x2D, Instr(Mnemonic.nrml, a, r0b)),
                (0x2E, Instr(Mnemonic.asr, a, r0b)),
                (0x2F, Instr(Mnemonic.lsr, a, r0b)),

                (0x30, Instr(Mnemonic.mov, rl_disp8, a)),
                (0x32, Instr(Mnemonic.mov, rl_disp8, a)),
                (0x34, Instr(Mnemonic.mov, rl_disp8, a)),
                (0x36, Instr(Mnemonic.mov, rl_disp8, a)),
                (0x38, Instr(Mnemonic.movw, rl_disp8, a)),
                (0x3A, Instr(Mnemonic.movw, rl_disp8, a)),
                (0x3C, Instr(Mnemonic.movw, rl_disp8, a)),
                (0x3E, Instr(Mnemonic.movw, rl_disp8, a)),

                (0x40, Instr(Mnemonic.mov, a, rl_disp8)),
                (0x42, Instr(Mnemonic.mov, a, rl_disp8)),
                (0x44, Instr(Mnemonic.mov, a, rl_disp8)),
                (0x46, Instr(Mnemonic.mov, a, rl_disp8)),
                (0x48, Instr(Mnemonic.movw, a, rl_disp8)),
                (0x4A, Instr(Mnemonic.movw, a, rl_disp8)),
                (0x4C, Instr(Mnemonic.movw, a, rl_disp8)),
                (0x4E, Instr(Mnemonic.movw, a, rl_disp8)));

            rootDecoder = Mask(0, 8, "F2MC16FX", new Decoder[]
            {
                // 00
                Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Zero|InstrClass.Padding),
                Instr(Mnemonic.int9),
                Instr(Mnemonic.adddc, a),
                Instr(Mnemonic.neg, a),
                Prefix(Registers.pcb),
                Prefix(Registers.dtb),
                Prefix(Registers.adb),
                Prefix(Registers.usb),
                
                Instr(Mnemonic.link, imm8),
                Instr(Mnemonic.unlink),
                Instr(Mnemonic.mov, rp, imm8),
                Instr(Mnemonic.negw, a),
                Instr(Mnemonic.lslw, a),
                Instr(Mnemonic.inte),
                Instr(Mnemonic.asrw, a),
                Instr(Mnemonic.lsrw, a),
                
                // 0x10
                Prefix(Registers.cmr),
                Prefix(Registers.ncc),
                Instr(Mnemonic.subdc, a),
                Instr(Mnemonic.jctx, InstrClass.Transfer, inda),
                Instr(Mnemonic.ext),
                Instr(Mnemonic.zext),
                Instr(Mnemonic.swap),
                Instr(Mnemonic.addsp, imm8),

                Instr(Mnemonic.addl, a, imm32),
                Instr(Mnemonic.subl, a, imm32),
                Instr(Mnemonic.mov, ilm, imm8),
                Instr(Mnemonic.cmpl, a, imm32),
                Instr(Mnemonic.extw),
                Instr(Mnemonic.zextw),
                Instr(Mnemonic.swapw),
                Instr(Mnemonic.addsp, imm16),

                // 0x20
                Instr(Mnemonic.add, a, dir),
                Instr(Mnemonic.sub, a, dir),
                Instr(Mnemonic.addc, a),
                Instr(Mnemonic.cmp, a),
                Instr(Mnemonic.and, ccr, imm8),
                Instr(Mnemonic.or, ccr, imm8),
                Instr(Mnemonic.divu, a),
                Instr(Mnemonic.mulu, a),

                Instr(Mnemonic.addw, a),
                Instr(Mnemonic.subw, a),
                Instr(Mnemonic.cbne, a, imm8, rel),
                Instr(Mnemonic.cmpw, a),
                Instr(Mnemonic.andw, a),
                Instr(Mnemonic.orw, a),
                Instr(Mnemonic.xorw, a),
                Instr(Mnemonic.muluw, a),

                // 0x30
                Instr(Mnemonic.add, a, imm8),
                Instr(Mnemonic.sub, a, imm8),
                Instr(Mnemonic.subc, a),
                Instr(Mnemonic.cmp, a, imm8),
                Instr(Mnemonic.and, a, imm8),
                Instr(Mnemonic.or, a, imm8),
                Instr(Mnemonic.xor, a, imm8),
                Instr(Mnemonic.not, a),

                Instr(Mnemonic.addw, a, imm16),
                Instr(Mnemonic.subw, a, imm16),
                Instr(Mnemonic.cwbne, a, imm16, rel),
                Instr(Mnemonic.cmpw, a, imm16),
                Instr(Mnemonic.andw, a, imm16),
                Instr(Mnemonic.orw, a, imm16),
                Instr(Mnemonic.xorw, a, imm16),
                Instr(Mnemonic.notw, a),

                // 0x40
                Instr(Mnemonic.mov, a, dir),
                Instr(Mnemonic.mov, dir, a),
                Instr(Mnemonic.mov, a, imm8),
                Instr(Mnemonic.movx, a, imm8),
                Instr(Mnemonic.mov, dir, imm8),
                Instr(Mnemonic.movx, a, dir),
                Instr(Mnemonic.movw, sp, a),
                Instr(Mnemonic.movw, a, sp),

                Instr(Mnemonic.movw, a, dir),
                Instr(Mnemonic.movw, dir, a),
                Instr(Mnemonic.movw, a, imm16),
                Instr(Mnemonic.movl, a, imm32),
                Instr(Mnemonic.pushw, a),
                Instr(Mnemonic.pushw, ah),
                Instr(Mnemonic.pushw, ps),
                Instr(Mnemonic.pushw, rlst),

                // 0x50
                Instr(Mnemonic.mov, a, io),
                Instr(Mnemonic.mov, io, a),
                Instr(Mnemonic.mov, a, daddr16),
                Instr(Mnemonic.mov, daddr16, a),
                Instr(Mnemonic.mov, io, imm8),
                Instr(Mnemonic.movx, a, io),
                Instr(Mnemonic.movw, io, imm16),
                Instr(Mnemonic.movx, a, daddr16),

                Instr(Mnemonic.movw, a, io),
                Instr(Mnemonic.movw, io, a),
                Instr(Mnemonic.movw, a, daddr16),
                Instr(Mnemonic.movw, daddr16, a),
                Instr(Mnemonic.popw, a),
                Instr(Mnemonic.popw, ah),
                Instr(Mnemonic.popw, ps),
                Instr(Mnemonic.popw, rlst),

                // 0x60
                Instr(Mnemonic.bra, InstrClass.ConditionalTransfer, rel),
                Instr(Mnemonic.jmp, InstrClass.Transfer, inda),
                Instr(Mnemonic.jmp, InstrClass.Transfer, addr16),
                Instr(Mnemonic.jmpp, InstrClass.Transfer, addr24),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, addr16),
                Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, addr24),
                Instr(Mnemonic.retp, InstrClass.Transfer|InstrClass.Return),
                Instr(Mnemonic.ret , InstrClass.Transfer|InstrClass.Return),

                Instr(Mnemonic.@int, vct8),
                Instr(Mnemonic.@int, addr16),
                Instr(Mnemonic.intp, addr24),
                Instr(Mnemonic.reti, InstrClass.Transfer|InstrClass.Return),

                TwoByteInstr(decode_bit_manipulation),
                invalid,
                TwoByteInstr(decode_string_manipulation),
                TwoByteInstr(decode_two_byte),

                // 0x70
                TwoByteInstr(decode_ea_70),
                TwoByteInstr(decode_ea_71),
                TwoByteInstr(decode_ea_72),
                TwoByteInstr(decode_ea_73),
                TwoByteInstr(decode_ea_74),
                TwoByteInstr(decode_ea_75),
                TwoByteInstr(decode_ea_76),
                TwoByteInstr(decode_ea_77),
                TwoByteInstr(decode_ea_78),
                TwoByteInstr(decode_ea_79),
                TwoByteInstr(decode_ea_7a),
                TwoByteInstr(decode_ea_7b),
                TwoByteInstr(decode_ea_7c),
                TwoByteInstr(decode_ea_7d),
                TwoByteInstr(decode_ea_7e),
                TwoByteInstr(decode_ea_7f),

                // 0x80
                mov_a_ri,
                mov_a_ri,
                mov_a_ri,
                mov_a_ri,
                mov_a_ri,
                mov_a_ri,
                mov_a_ri,
                mov_a_ri,

                movw_a_rwi,
                movw_a_rwi,
                movw_a_rwi,
                movw_a_rwi,
                movw_a_rwi,
                movw_a_rwi,
                movw_a_rwi,
                movw_a_rwi,

                // 0x90,
                mov_ri_a,
                mov_ri_a,
                mov_ri_a,
                mov_ri_a,
                mov_ri_a,
                mov_ri_a,
                mov_ri_a,
                mov_ri_a,

                movw_rwi_a,
                movw_rwi_a,
                movw_rwi_a,
                movw_rwi_a,
                movw_rwi_a,
                movw_rwi_a,
                movw_rwi_a,
                movw_rwi_a,

                // 0xA0
                mov_ri_imm8,
                mov_ri_imm8,
                mov_ri_imm8,
                mov_ri_imm8,
                mov_ri_imm8,
                mov_ri_imm8,
                mov_ri_imm8,
                mov_ri_imm8,

                mov_rwi_imm16,
                mov_rwi_imm16,
                mov_rwi_imm16,
                mov_rwi_imm16,
                mov_rwi_imm16,
                mov_rwi_imm16,
                mov_rwi_imm16,
                mov_rwi_imm16,

                // 0xB0
                movx_a_ri,
                movx_a_ri,
                movx_a_ri,
                movx_a_ri,
                movx_a_ri,
                movx_a_ri,
                movx_a_ri,
                movx_a_ri,

                movw_a_reg_disp8,
                movw_a_reg_disp8,
                movw_a_reg_disp8,
                movw_a_reg_disp8,
                movw_a_reg_disp8,
                movw_a_reg_disp8,
                movw_a_reg_disp8,
                movw_a_reg_disp8,

                // 0xC0
                movx_a_reg_disp8,
                movx_a_reg_disp8,
                movx_a_reg_disp8,
                movx_a_reg_disp8,
                movx_a_reg_disp8,
                movx_a_reg_disp8,
                movx_a_reg_disp8,
                movx_a_reg_disp8,

                movw_reg_disp8_a,
                movw_reg_disp8_a,
                movw_reg_disp8_a,
                movw_reg_disp8_a,
                movw_reg_disp8_a,
                movw_reg_disp8_a,
                movw_reg_disp8_a,
                movw_reg_disp8_a,

                movn_a_imm4,
                movn_a_imm4,
                movn_a_imm4,
                movn_a_imm4,
                movn_a_imm4,
                movn_a_imm4,
                movn_a_imm4,
                movn_a_imm4,

                movn_a_imm4,
                movn_a_imm4,
                movn_a_imm4,
                movn_a_imm4,
                movn_a_imm4,
                movn_a_imm4,
                movn_a_imm4,
                movn_a_imm4,

                // 0xE0
                callv,
                callv,
                callv,
                callv,

                callv,
                callv,
                callv,
                callv,

                callv,
                callv,
                callv,
                callv,

                callv,
                callv,
                callv,
                callv,

                // 0xF0
                Instr(Mnemonic.bz,  InstrClass.ConditionalTransfer, rel),
                Instr(Mnemonic.bnz, InstrClass.ConditionalTransfer, rel),
                Instr(Mnemonic.bc,  InstrClass.ConditionalTransfer, rel),
                Instr(Mnemonic.bnc, InstrClass.ConditionalTransfer, rel),
                Instr(Mnemonic.bn,  InstrClass.ConditionalTransfer, rel),
                Instr(Mnemonic.bp,  InstrClass.ConditionalTransfer, rel),
                Instr(Mnemonic.bv,  InstrClass.ConditionalTransfer, rel),
                Instr(Mnemonic.bnv, InstrClass.ConditionalTransfer, rel),

                Instr(Mnemonic.bt,  InstrClass.ConditionalTransfer, rel),
                Instr(Mnemonic.bnt, InstrClass.ConditionalTransfer, rel),
                Instr(Mnemonic.blt, InstrClass.ConditionalTransfer, rel),
                Instr(Mnemonic.bge, InstrClass.ConditionalTransfer, rel),
                Instr(Mnemonic.ble, InstrClass.ConditionalTransfer, rel),
                Instr(Mnemonic.bgt, InstrClass.ConditionalTransfer, rel),
                Instr(Mnemonic.bls, InstrClass.ConditionalTransfer, rel),
                Instr(Mnemonic.bhi, InstrClass.ConditionalTransfer, rel)
            });
        }
    }
}