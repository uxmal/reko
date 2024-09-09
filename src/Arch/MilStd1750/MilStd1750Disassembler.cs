#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

#pragma warning disable IDE1006

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System.Collections.Generic;

namespace Reko.Arch.MilStd1750
{
    public class MilStd1750Disassembler : DisassemblerBase<Instruction, Mnemonic>
    {
        private static readonly Decoder<MilStd1750Disassembler, Mnemonic, Instruction> rootDecoder;
        private static readonly Bitfield bf0_4 = new Bitfield(0, 4);
        private static readonly Bitfield bf0_8 = new Bitfield(0, 8);
        private static readonly Bitfield bf4_4 = new Bitfield(4, 4);

        private readonly MilStd1750Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;
        private Mnemonic mnemonic;

        public MilStd1750Disassembler(MilStd1750Architecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
            this.addr = rdr.Address;
        }

        public override Instruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadUInt16(out ushort wInstr))
                return null;
            this.ops.Clear();
            this.mnemonic = Mnemonic.invalid;
            var instr = rootDecoder.Decode(wInstr, this);
            instr.Address = this.addr;
            instr.Length = (int) (rdr.Address - this.addr);
            return instr;
        }

        public override Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new Instruction
            {
                InstructionClass = iclass,
                Mnemonic = this.mnemonic != Mnemonic.invalid ? this.mnemonic : mnemonic,
                Operands = ops.ToArray()
            };
        }

        public override Instruction CreateInvalidInstruction()
        {
            return new Instruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.invalid,
            };
        }

        public override Instruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("MS1750Dis", this.addr, this.rdr, message);
            return CreateInvalidInstruction();
        }


        private static bool Is0(uint u) => u == 0;

        private static Mutator<MilStd1750Disassembler> Reg(int bitpos)
        {
            var bitfield = new Bitfield(bitpos, 4);
            return (u, d) =>
            {
                var iReg = bitfield.Read(u);
                d.ops.Add(Registers.GpRegs[iReg]);
                return true;
            };
        }
        private static readonly Mutator<MilStd1750Disassembler> Ra = Reg(4);
        private static readonly Mutator<MilStd1750Disassembler> Rb = Reg(0);

        private static bool N(uint uInstr, MilStd1750Disassembler dasm)
        {
            var n = (ushort)bf4_4.Read(uInstr);
            dasm.ops.Add(ImmediateOperand.Word16(n));
            return true;
        }

        /// <summary>
        /// Memory direct
        /// </summary>
        private static Mutator<MilStd1750Disassembler> Dx(PrimitiveType dt)
        {
            return (u, d) =>
            {
                if (!d.rdr.TryReadBeUInt16(out ushort disp))
                    return false;
                var ixReg = bf0_4.Read(u);
                var xReg = (ixReg != 0)
                    ? Registers.GpRegs[ixReg]
                    : null;
                var op = MemoryOperand.Direct(dt, disp, xReg);
                d.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<MilStd1750Disassembler> Dx_w16 = Dx(PrimitiveType.Word16);
        private static readonly Mutator<MilStd1750Disassembler> Dx_w32 = Dx(PrimitiveType.Word32);
        private static readonly Mutator<MilStd1750Disassembler> Dx_r32 = Dx(PrimitiveType.Real32);
        private static readonly Mutator<MilStd1750Disassembler> Dx_r48 = Dx(MilStd1750Architecture.Real48);

        /// <summary>
        /// Memory indirect
        /// </summary>
        private static Mutator<MilStd1750Disassembler> Ix(PrimitiveType dt)
        {
            return (u, d) =>
            {
                if (!d.rdr.TryReadBeUInt16(out ushort disp))
                    return false;
                var ixReg = bf0_4.Read(u);
                var xReg = (ixReg != 0)
                    ? Registers.GpRegs[ixReg]
                    : null;
                var op = MemoryOperand.Indirect(dt, disp, xReg);
                d.ops.Add(op);
                return true;
            };
        }
        private static readonly Mutator<MilStd1750Disassembler> Ix_w16 = Ix(PrimitiveType.Word16);
        private static readonly Mutator<MilStd1750Disassembler> Ix_w32 = Ix(PrimitiveType.Word32);

        /// <summary>
        /// Immediate long
        /// </summary>
        private static Mutator<MilStd1750Disassembler> Imx(PrimitiveType dt)
        {
            return (u, d) =>
            {
                if (!d.rdr.TryReadBeUInt16(out ushort imm))
                    return false;
                var op = new ImmediateOperand(Constant.Create(dt, imm));
                d.ops.Add(op);
                var ixReg = bf0_4.Read(u);
                if (ixReg != 0)
                {
                    var xReg = Registers.GpRegs[ixReg];
                    d.ops.Add(xReg);
                }
                return true;
            };
        }
        private static readonly Mutator<MilStd1750Disassembler> Imx_w16 = Imx(PrimitiveType.Word16);

        /// <summary>
        /// Base relative.
        /// </summary>
        private static Mutator<MilStd1750Disassembler> br(RegisterStorage reg)
        {
            return (u, d) =>
            {
                var disp = bf0_8.Read(u);
                d.ops.Add(reg);
                d.ops.Add(ImmediateOperand.Word16((ushort) disp));
                return true;
            };
        }
        private static readonly Mutator<MilStd1750Disassembler> br12 = br(Registers.GpRegs[12]);
        private static readonly Mutator<MilStd1750Disassembler> br13 = br(Registers.GpRegs[13]);
        private static readonly Mutator<MilStd1750Disassembler> br14 = br(Registers.GpRegs[14]);
        private static readonly Mutator<MilStd1750Disassembler> br15 = br(Registers.GpRegs[15]);

        /// <summary>
        /// Base indexed.
        /// </summary>
        private static Mutator<MilStd1750Disassembler> bx(RegisterStorage reg)
        {
            return (u, d) =>
            {
                d.ops.Add(reg);
                var idxReg = Registers.GpRegs[bf0_4.Read(u)];
                d.ops.Add(idxReg);
                return true;
            };
        }
        private static readonly Mutator<MilStd1750Disassembler> bx12 = bx(Registers.GpRegs[12]);
        private static readonly Mutator<MilStd1750Disassembler> bx13 = bx(Registers.GpRegs[13]);
        private static readonly Mutator<MilStd1750Disassembler> bx14 = bx(Registers.GpRegs[14]);
        private static readonly Mutator<MilStd1750Disassembler> bx15 = bx(Registers.GpRegs[15]);


        /// <summary>
        /// Address or indexed address
        /// </summary>
        private static bool Ax(uint uInstr, MilStd1750Disassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out ushort imm))
                return false;
            var ixReg = bf0_4.Read(uInstr);
            if (ixReg != 0)
            {
                var op = new ImmediateOperand(Constant.Create(PrimitiveType.Word16, imm));
                dasm.ops.Add(op);
                var xReg = Registers.GpRegs[ixReg];
                dasm.ops.Add(xReg);
            }
            else
            {
                var op = Address.Ptr16(imm);
                dasm.ops.Add(op);
            }
            return true;
        }

        /// <summary>
        /// Immediate short positive
        /// </summary>
        private static bool ISP_0(uint uInstr, MilStd1750Disassembler dasm)
        {
            var imm = bf0_4.Read(uInstr) + 1u;
            dasm.ops.Add(ImmediateOperand.Word16((ushort)imm));
            return true;
        }
        private static bool ISP_4(uint uInstr, MilStd1750Disassembler dasm)
        {
            var imm = bf4_4.Read(uInstr) + 1u;
            dasm.ops.Add(ImmediateOperand.Word16((ushort) imm));
            return true;
        }

        private static bool Imm4(uint uInstr, MilStd1750Disassembler dasm)
        {
            dasm.ops.Add(ImmediateOperand.Word16((ushort) bf0_4.Read(uInstr)));
            return true;
        }

        private static bool Imm8(uint uInstr, MilStd1750Disassembler dasm)
        {
            dasm.ops.Add(ImmediateOperand.Word16((ushort)bf0_8.Read(uInstr)));
            return true;
        }

        /// <summary>
        /// 16-bit immedate following the instruction word.
        /// </summary>
        private static bool IM(uint uInstr, MilStd1750Disassembler dasm)
        {
            if (!dasm.rdr.TryReadUInt16(out ushort imm))
                return false;
            dasm.ops.Add(ImmediateOperand.Word16(imm));
            return true;
        }

        /// <summary>
        /// IC-relative
        /// </summary>
        private static bool ICR(uint uInstr, MilStd1750Disassembler dasm)
        {
            var disp = bf0_8.ReadSigned(uInstr);
            var addrDst = dasm.addr + disp;
            dasm.ops.Add(addrDst);
            return true;
        }

        /// <summary>
        /// XIO command decoder
        /// </summary>
        private static bool Xio(uint uInstr, MilStd1750Disassembler dasm)
        {
            if (!dasm.rdr.TryReadBeUInt16(out ushort cmd))
                return false;
            var ra = Registers.GpRegs[bf4_4.Read(uInstr)];
            switch (cmd >> 12)
            {
            case 0:
                dasm.mnemonic = Mnemonic.xio_po;
                dasm.ops.Add(ra);
                dasm.ops.Add(ImmediateOperand.Word16((ushort) (cmd & 0x03FF)));
                return true;
            case 0x2:
                switch (cmd)
                {
                case 0x2000:
                    dasm.mnemonic = Mnemonic.xio_smk;
                    dasm.ops.Add(ra);
                    break;
                case 0x2001:
                    dasm.mnemonic = Mnemonic.xio_clir;
                    break;
                case 0x2002:
                    dasm.mnemonic = Mnemonic.xio_enbl;
                    break;
                case 0x2003:
                    dasm.mnemonic = Mnemonic.xio_dsbl;
                    break;
                case 0x2004:
                    dasm.mnemonic = Mnemonic.xio_rpi;
                    dasm.ops.Add(ra);
                    break;
                case 0x2005:
                    dasm.mnemonic = Mnemonic.xio_spi;
                    dasm.ops.Add(ra);
                    break;
                case 0x200E:
                    dasm.mnemonic = Mnemonic.xio_wsw;
                    dasm.ops.Add(ra);
                    break;
                }
                break;
            case 0x4:
                switch (cmd)
                {
                case 0x4000:
                    dasm.mnemonic = Mnemonic.xio_co;
                    dasm.ops.Add(ra);
                    return true;
                case 0x4001:
                    dasm.mnemonic = Mnemonic.xio_clc;
                    return true;
                }
                break;
            case 0xA:
                switch (cmd)
                {
                case 0xAD51:
                    // 'MQ': Seems to be legit, but can't find documentation for it.
                    dasm.mnemonic = Mnemonic.xio_unknown;
                    dasm.ops.Add(ImmediateOperand.UInt16(cmd));
                    return true;
                }
                break;
            }
            var testGenSvc = dasm.arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("MS1750Dis", dasm.addr, dasm.rdr, $"xio {cmd:X4}");
            return false;
        }

        private static Mutator<MilStd1750Disassembler> _(string msg)
        {
            return (u, d) =>
            {
                var testGenSvc = d.arch.Services.GetService<ITestGenerationService>();
                testGenSvc?.ReportMissingDecoder("MS1750Dis", d.addr, d.rdr, msg);
                return false;
            };
        }

        private static InstrDecoder<MilStd1750Disassembler, Mnemonic,Instruction> Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<MilStd1750Disassembler> [] mutators)
        {
            return new InstrDecoder<MilStd1750Disassembler, Mnemonic, Instruction>(iclass, mnemonic, mutators);
        }

        private static InstrDecoder<MilStd1750Disassembler, Mnemonic, Instruction> Instr(Mnemonic mnemonic, params Mutator<MilStd1750Disassembler>[] mutators)
        {
            return new InstrDecoder<MilStd1750Disassembler, Mnemonic, Instruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static NyiDecoder<MilStd1750Disassembler, Mnemonic, Instruction> Nyi(string message)
        {
            return new NyiDecoder<MilStd1750Disassembler, Mnemonic, Instruction>(message);
        }


        static MilStd1750Disassembler()
        {
            var invalid = Instr(Mnemonic.invalid, InstrClass.Invalid);
            var nyi = Nyi("");

            rootDecoder = Mask(8, 8, "MIL-STD-1750", new Decoder<MilStd1750Disassembler, Mnemonic, Instruction>[256]
            {
                // 00
                Instr(Mnemonic.lb, br12),
                Instr(Mnemonic.lb, br13),
                Instr(Mnemonic.lb, br14),
                Instr(Mnemonic.lb, br15),

                Instr(Mnemonic.dlb, br12),
                Instr(Mnemonic.dlb, br13),
                Instr(Mnemonic.dlb, br14),
                Instr(Mnemonic.dlb, br15),

                Instr(Mnemonic.stlb, br12),
                Instr(Mnemonic.stlb, br13),
                Instr(Mnemonic.stlb, br14),
                Instr(Mnemonic.stlb, br15),

                Instr(Mnemonic.dstb, br12),
                Instr(Mnemonic.dstb, br13),
                Instr(Mnemonic.dstb, br14),
                Instr(Mnemonic.dstb, br15),

                // 10
                Instr(Mnemonic.ab, br12),
                Instr(Mnemonic.ab, br13),
                Instr(Mnemonic.ab, br14),
                Instr(Mnemonic.ab, br15),

                Instr(Mnemonic.sbb, br12),
                Instr(Mnemonic.sbb, br13),
                Instr(Mnemonic.sbb, br14),
                Instr(Mnemonic.sbb, br15),

                Instr(Mnemonic.mb, br12),
                Instr(Mnemonic.mb, br13),
                Instr(Mnemonic.mb, br14),
                Instr(Mnemonic.mb, br15),


                Instr(Mnemonic.db, br12),
                Instr(Mnemonic.db, br13),
                Instr(Mnemonic.db, br14),
                Instr(Mnemonic.db, br15),
                
                // 20
                Instr(Mnemonic.fab, br12),
                Instr(Mnemonic.fab, br13),
                Instr(Mnemonic.fab, br14),
                Instr(Mnemonic.fab, br15),

                Instr(Mnemonic.fsb, br12),
                Instr(Mnemonic.fsb, br13),
                Instr(Mnemonic.fsb, br14),
                Instr(Mnemonic.fsb, br15),

                Instr(Mnemonic.fmb, br12),
                Instr(Mnemonic.fmb, br13),
                Instr(Mnemonic.fmb, br14),
                Instr(Mnemonic.fmb, br15),

                Instr(Mnemonic.fdb, br12),
                Instr(Mnemonic.fdb, br13),
                Instr(Mnemonic.fdb, br14),
                Instr(Mnemonic.fdb, br15),
                
                // 30
                Instr(Mnemonic.orb, br12),
                Instr(Mnemonic.orb, br13),
                Instr(Mnemonic.orb, br14),
                Instr(Mnemonic.orb, br15),

                Instr(Mnemonic.andb, br12),
                Instr(Mnemonic.andb, br13),
                Instr(Mnemonic.andb, br14),
                Instr(Mnemonic.andb, br15),

                Instr(Mnemonic.cb, br12),
                Instr(Mnemonic.cb, br13),
                Instr(Mnemonic.cb, br14),
                Instr(Mnemonic.cb, br15),

                Instr(Mnemonic.fcb, br12),
                Instr(Mnemonic.fcb, br13),
                Instr(Mnemonic.fcb, br14),
                Instr(Mnemonic.fcb, br15),

                // 40
                Mask(4, 4, "  brx br12", // a
                    Instr(Mnemonic.lbx, bx12),  // a
                    Instr(Mnemonic.dlbx, bx12),  // a
                    Instr(Mnemonic.stbx, bx12),  // a
                    Instr(Mnemonic.dstx, bx12),  // a

                    Instr(Mnemonic.abx, bx12),  // a
                    Instr(Mnemonic.sbbx, bx12),  // a
                    Instr(Mnemonic.mbx, bx12),  // a
                    Instr(Mnemonic.dbx, bx12),  // a

                    Instr(Mnemonic.fabx, bx12),  // a
                    Instr(Mnemonic.fsbx, bx12),  // a
                    Instr(Mnemonic.fmbx, bx12),  // a
                    Instr(Mnemonic.fdbx, bx12),  // a

                    Instr(Mnemonic.cbx, bx12),  // a
                    Instr(Mnemonic.fcbx, bx12),  // a
                    Instr(Mnemonic.andx, bx12),  // a
                    Instr(Mnemonic.orbx, bx12)),  // a
                Mask(4, 4, "  brx br13", // a
                    Instr(Mnemonic.lbx, bx13),  // a
                    Instr(Mnemonic.dlbx, bx13),  // a
                    Instr(Mnemonic.stbx, bx13),  // a
                    Instr(Mnemonic.dstx, bx13),  // a

                    Instr(Mnemonic.abx, bx13),  // a
                    Instr(Mnemonic.sbbx, bx13),  // a
                    Instr(Mnemonic.mbx, bx13),  // a
                    Instr(Mnemonic.dbx, bx13),  // a

                    Instr(Mnemonic.fabx, bx13),  // a
                    Instr(Mnemonic.fsbx, bx13),  // a
                    Instr(Mnemonic.fmbx, bx13),  // a
                    Instr(Mnemonic.fdbx, bx13),  // a

                    Instr(Mnemonic.cbx, bx13),  // a
                    Instr(Mnemonic.fcbx, bx13),  // a
                    Instr(Mnemonic.andx, bx13),  // a
                    Instr(Mnemonic.orbx, bx13)),  // a
                Mask(4, 4, "  brx br14", // a
                    Instr(Mnemonic.lbx, bx14),  // a
                    Instr(Mnemonic.dlbx, bx14),  // a
                    Instr(Mnemonic.stbx, bx14),  // a
                    Instr(Mnemonic.dstx, bx14),  // a

                    Instr(Mnemonic.abx, bx14),  // a
                    Instr(Mnemonic.sbbx, bx14),  // a
                    Instr(Mnemonic.mbx, bx14),  // a
                    Instr(Mnemonic.dbx, bx14),  // a

                    Instr(Mnemonic.fabx, bx14),  // a
                    Instr(Mnemonic.fsbx, bx14),  // a
                    Instr(Mnemonic.fmbx, bx14),  // a
                    Instr(Mnemonic.fdbx, bx14),  // a

                    Instr(Mnemonic.cbx, bx14),  // a
                    Instr(Mnemonic.fcbx, bx14),  // a
                    Instr(Mnemonic.andx, bx14),  // a
                    Instr(Mnemonic.orbx, bx14)),  // a
                Mask(4, 4, "  brx br15", // a
                    Instr(Mnemonic.lbx, bx15),  // a
                    Instr(Mnemonic.dlbx, bx15),  // a
                    Instr(Mnemonic.stbx, bx15),  // a
                    Instr(Mnemonic.dstx, bx15),  // a

                    Instr(Mnemonic.abx, bx15),  // a
                    Instr(Mnemonic.sbbx, bx15),  // a
                    Instr(Mnemonic.mbx, bx15),  // a
                    Instr(Mnemonic.dbx, bx15),  // a

                    Instr(Mnemonic.fabx, bx15),  // a
                    Instr(Mnemonic.fsbx, bx15),  // a
                    Instr(Mnemonic.fmbx, bx15),  // a
                    Instr(Mnemonic.fdbx, bx15),  // a

                    Instr(Mnemonic.cbx, bx15),  // a
                    Instr(Mnemonic.fcbx, bx15),  // a
                    Instr(Mnemonic.andx, bx15),  // a
                    Instr(Mnemonic.orbx, bx15)),  // a

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.xio, InstrClass.Linear|InstrClass.Privileged, Xio), // ab
                Instr(Mnemonic.vio, _("vio")), // ab
                Mask(0, 4, "  imm",  // ab
                    invalid,
                    Instr(Mnemonic.aim, Ra,IM),
                    Instr(Mnemonic.sim, Ra,IM),
                    Instr(Mnemonic.mim, Ra,IM),

                    Instr(Mnemonic.msim, Ra,IM),
                    Instr(Mnemonic.dim,  Ra,IM),
                    Instr(Mnemonic.dvim, Ra,IM),
                    Instr(Mnemonic.andm, Ra,IM),

                    Instr(Mnemonic.orim, Ra,IM),
                    Instr(Mnemonic.xorm, Ra,IM),
                    Instr(Mnemonic.cim,  Ra,IM),
                    Instr(Mnemonic.nim,  Ra,IM),

                    invalid,
                    invalid,
                    invalid,
                    invalid),
                invalid,

                invalid,
                invalid,
                invalid,
                Instr(Mnemonic.bif, Imm8), // c

                // 50
                Instr(Mnemonic.sb, N,Dx_w16),
                Instr(Mnemonic.sbr, N,Rb),
                Instr(Mnemonic.sbi, N,Ix_w16),
                Instr(Mnemonic.rb, N,Dx_w16),

                Instr(Mnemonic.rbr, N,Rb),
                Instr(Mnemonic.rbi, N,Ix_w16),
                Instr(Mnemonic.tb, _("tb")),
                Instr(Mnemonic.tbr, N,Rb),

                Instr(Mnemonic.tbi, _("tbi")),
                Instr(Mnemonic.tsb, _("tsb")),
                Instr(Mnemonic.svbr, Ra,Rb),
                invalid,

                Instr(Mnemonic.rvbr, Ra,Rb),
                invalid,
                Instr(Mnemonic.tvbr, Ra,Rb),
                invalid,

                // 60
                Instr(Mnemonic.sll, Rb,ISP_4),
                Instr(Mnemonic.srl, Rb,ISP_4),
                Instr(Mnemonic.sra, Rb,ISP_4),
                Instr(Mnemonic.slc, Rb,ISP_4),

                invalid,
                Instr(Mnemonic.dsll, Rb,ISP_4),
                Instr(Mnemonic.dsrl, Rb,ISP_4),
                Instr(Mnemonic.dsra, Rb,ISP_4),

                Instr(Mnemonic.dslc, Rb,ISP_4),
                invalid,
                Instr(Mnemonic.slr, Ra,Rb),
                Instr(Mnemonic.sar, Ra,Rb),

                Instr(Mnemonic.scr, Ra,Rb),
                Instr(Mnemonic.dslr, Ra,Rb),
                Instr(Mnemonic.dsar, Ra,Rb),
                Instr(Mnemonic.dscr, Ra,Rb),

                // 70
                Instr(Mnemonic.jc, InstrClass.ConditionalTransfer, N,Ax),
                Instr(Mnemonic.jci, _("jci")),
                Instr(Mnemonic.js, InstrClass.Transfer|InstrClass.Call, Ra,Ax),
                Instr(Mnemonic.soj, InstrClass.ConditionalTransfer, Ra,Ax),

                Instr(Mnemonic.br, InstrClass.Transfer, ICR),
                Instr(Mnemonic.bez, InstrClass.ConditionalTransfer, ICR),
                Instr(Mnemonic.blt, InstrClass.ConditionalTransfer, ICR),
                Instr(Mnemonic.bex, InstrClass.ConditionalTransfer, Imm4),

                Instr(Mnemonic.ble, InstrClass.ConditionalTransfer, ICR),
                Instr(Mnemonic.bgt, InstrClass.ConditionalTransfer, ICR),
                Instr(Mnemonic.bnz, InstrClass.ConditionalTransfer, ICR),
                Instr(Mnemonic.bge, InstrClass.ConditionalTransfer, ICR),

                Instr(Mnemonic.lsti, _("lsti")), // b
                Instr(Mnemonic.lst, _("lst")), // b
                Instr(Mnemonic.sjs, InstrClass.Transfer|InstrClass.Call, Ra,Ax),
                Instr(Mnemonic.urs, InstrClass.Transfer|InstrClass.Return, Ra),

                // 80
                Instr(Mnemonic.l, Ra,Dx_w16),
                Instr(Mnemonic.lr, Ra,Rb),
                Instr(Mnemonic.lisp, Ra,ISP_0),
                Instr(Mnemonic.lisn, Ra,ISP_0),

                Instr(Mnemonic.li, _("li")),
                Instr(Mnemonic.lim, Ra,Imx_w16),
                Instr(Mnemonic.dl, Ra,Dx_w32),
                Instr(Mnemonic.dlr, Ra,Rb),

                Instr(Mnemonic.dli, _("dli")),
                Instr(Mnemonic.lm, N,Dx_w16),
                Instr(Mnemonic.efl, Ra,Dx_r48),
                Instr(Mnemonic.lub, Ra,Dx_w16),

                Instr(Mnemonic.llb, Ra,Dx_w16),
                Instr(Mnemonic.lubi, _("lubi")),
                Instr(Mnemonic.llbi, _("llbi")),
                Instr(Mnemonic.popm, Ra,Rb),

                // 90
                Instr(Mnemonic.st, Ra,Dx_w16),
                Instr(Mnemonic.stc, N,Dx_w16),
                Instr(Mnemonic.stci, _("stci ")),
                Instr(Mnemonic.mov, Ra,Rb),

                Instr(Mnemonic.sti, _("sti ")),
                invalid,
                Instr(Mnemonic.dst, Ra,Dx_w32),
                Instr(Mnemonic.srm, _("srm ")),

                Instr(Mnemonic.dsti , Ra,Ix_w32),
                Instr(Mnemonic.stm , N,Dx_w16),
                Instr(Mnemonic.efst , Ra,Dx_r48),
                Instr(Mnemonic.stub, Ra,Dx_w16),

                Instr(Mnemonic.stlb, Ra,Dx_w16),
                Instr(Mnemonic.subi, _("subi")),
                Instr(Mnemonic.slbi, _("slbi")),
                Instr(Mnemonic.pshm, Ra,Rb),

                // A0
                Instr(Mnemonic.a, Ra,Dx_w16),
                Instr(Mnemonic.ar, Ra,Rb),
                Instr(Mnemonic.aisp, Ra,ISP_0),
                Instr(Mnemonic.incm, ISP_4,Dx_w16),

                Instr(Mnemonic.abs, Ra,Rb),
                Instr(Mnemonic.dabs, Ra,Rb),
                Instr(Mnemonic.da, Ra,Dx_w32),
                Instr(Mnemonic.dar, Ra,Rb),

                Instr(Mnemonic.fa, Ra,Dx_w16),
                Instr(Mnemonic.far, Ra,Rb),
                Instr(Mnemonic.efa, Ra,Dx_r48),
                Instr(Mnemonic.efar, Ra,Rb),

                Instr(Mnemonic.fabs, Ra,Rb),
                invalid,
                invalid,
                invalid,

                // B0
                Instr(Mnemonic.s, Ra,Dx_w16),
                Instr(Mnemonic.sr, Ra,Rb),
                Instr(Mnemonic.sisp, Ra,ISP_0),
                Instr(Mnemonic.decm, ISP_4,Dx_w16),

                Instr(Mnemonic.neg, Ra,Rb),
                Instr(Mnemonic.dneg, Ra,Rb),
                Instr(Mnemonic.ds, Ra,Dx_w32),
                Instr(Mnemonic.dsr, Ra,Rb),

                Instr(Mnemonic.fs, Ra,Dx_r32),
                Instr(Mnemonic.fsr, Ra,Rb),
                Instr(Mnemonic.efs, Ra,Dx_r48),
                Instr(Mnemonic.efsr, Ra,Rb),

                Instr(Mnemonic.fneg, Ra,Rb),
                invalid,
                invalid,
                invalid,

                // C0
                Instr(Mnemonic.ms, Ra,Dx_w16),
                Instr(Mnemonic.msr, Ra,Rb),
                Instr(Mnemonic.misp, Rb,ISP_4),
                Instr(Mnemonic.misn, Rb,ISP_4),

                Instr(Mnemonic.m, Ra,Dx_w16),
                Instr(Mnemonic.mr, Ra,Rb),
                Instr(Mnemonic.dm, Ra,Dx_w32),
                Instr(Mnemonic.dmr, Ra,Rb),

                Instr(Mnemonic.fm, Ra,Dx_w16),
                Instr(Mnemonic.fmr, Ra,Rb),
                Instr(Mnemonic.efm, Ra,Dx_r48),
                Instr(Mnemonic.efmr, Ra,Rb),
                    
                invalid,
                invalid,
                invalid,
                invalid,

                // D0
                Instr(Mnemonic.dv, _("dv")),
                Instr(Mnemonic.dvr, _("dvr")),
                Instr(Mnemonic.disp, Ra,ISP_0),
                Instr(Mnemonic.disn, Ra,ISP_0),

                Instr(Mnemonic.d, Ra,Dx_w16),
                Instr(Mnemonic.dr, Ra,Rb),
                Instr(Mnemonic.dd, Ra,Dx_w32),
                Instr(Mnemonic.ddr, Ra,Rb),

                Instr(Mnemonic.fd, Ra,Dx_r32),
                Instr(Mnemonic.fdr, Ra,Rb),
                Instr(Mnemonic.efd, Ra,Dx_r48),
                Instr(Mnemonic.efdr, Ra,Rb),
                    
                invalid,
                invalid,
                invalid,
                invalid,

                // E0
                Instr(Mnemonic.or, Ra,Dx_w16),
                Instr(Mnemonic.orr, Ra,Rb),
                Instr(Mnemonic.and, Ra,Dx_w16),
                Instr(Mnemonic.andr, Ra,Rb),

                Instr(Mnemonic.xor, Ra,Dx_w16),
                Instr(Mnemonic.xorr, Ra,Rb),
                Instr(Mnemonic.n, Ra,Dx_w16),
                Instr(Mnemonic.nr, Ra,Rb),

                Instr(Mnemonic.fix, Ra,Rb),
                Instr(Mnemonic.flt, Ra,Rb),
                Instr(Mnemonic.efix, Ra,Rb),
                Instr(Mnemonic.eflt, Ra,Rb),

                Select((0, 4), Is0, "  EC",
                    Instr(Mnemonic.xbr, Ra),
                    nyi),
                Instr(Mnemonic.xwr, Ra,Rb),
                invalid,
                invalid,

                // F0
                Instr(Mnemonic.c, Ra,Dx_w16),
                Instr(Mnemonic.cr, Ra,Rb),
                Instr(Mnemonic.cisp, Ra,ISP_0),
                Instr(Mnemonic.cisn, Ra,ISP_0),

                Instr(Mnemonic.cbl, Ra,Dx_w16),
                invalid,
                Instr(Mnemonic.dc, Ra,Dx_w32),
                Instr(Mnemonic.dcr, Ra,Rb),

                Instr(Mnemonic.fc, Ra,Dx_r32),
                Instr(Mnemonic.fcr, Ra,Rb),
                Instr(Mnemonic.efc, Ra,Dx_r48),
                Instr(Mnemonic.efcr, Ra,Rb),

                invalid,
                invalid,
                invalid,
                Select((0, 8), Is0, "  0xFF",
                    Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding),
                    Select((0, 8), u => u == 0xFF,
                        Instr(Mnemonic.bpt, InstrClass.Terminates),
                        invalid))
            });
        }
    }
}