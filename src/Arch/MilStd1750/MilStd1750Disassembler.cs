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

#pragma warning disable IDE1006

using Reko.Core;
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
                Mnemonic = mnemonic,
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
                d.ops.Add(new RegisterOperand(Registers.GpRegs[iReg]));
                return true;
            };
        }
        private static readonly Mutator<MilStd1750Disassembler> Ra = Reg(4);
        private static readonly Mutator<MilStd1750Disassembler> Rb = Reg(0);

        private static bool N(uint uInstr, MilStd1750Disassembler dasm)
        {
            var n = (byte)bf4_4.Read(uInstr);
            dasm.ops.Add(ImmediateOperand.Byte(n));
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
        /// IC-relative
        /// </summary>
        private static bool ICR(uint uInstr, MilStd1750Disassembler dasm)
        {
            var disp = bf0_8.ReadSigned(uInstr);
            var addrDst = dasm.addr + (disp - 1);
            dasm.ops.Add(AddressOperand.Create(addrDst));
            return true;
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

        private static bool br12(uint uInstr, MilStd1750Disassembler dasm)
            => false;
        private static bool br13(uint uInstr, MilStd1750Disassembler dasm)
            => false;
        private static bool br14(uint uInstr, MilStd1750Disassembler dasm)
            => false;
        private static bool br15(uint uInstr, MilStd1750Disassembler dasm)
            => false;



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

                Instr(Mnemonic.stb, br12),
                Instr(Mnemonic.stb, br13),
                Instr(Mnemonic.stb, br14),
                Instr(Mnemonic.stb, br15),

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
                Instr(Mnemonic.brx, br12),  // a
                Instr(Mnemonic.brx, br13),  // a
                Instr(Mnemonic.brx, br14),  // a
                Instr(Mnemonic.brx, br15),  // a

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.xio, _("xio")), // ab
                Instr(Mnemonic.vio, _("vio")), // ab
                Instr(Mnemonic.imml, _("imml")), // ab
                invalid,

                invalid,
                invalid,
                invalid,
                Instr(Mnemonic.bif, _("bif")), // c

                // 50
                Instr(Mnemonic.sb, N,Dx_w16),
                Instr(Mnemonic.sbr, N,Rb),
                Instr(Mnemonic.sbi, N,Ix_w16),
                Instr(Mnemonic.rb, N,Dx_w16),

                Instr(Mnemonic.rbr, N,Rb),
                Instr(Mnemonic.rbi, N,Ix_w16),
                Instr(Mnemonic.tb, _("tb")),
                Instr(Mnemonic.tbr, _("tbr")),

                Instr(Mnemonic.tbi, _("tbi")),
                Instr(Mnemonic.tsb, _("tsb")),
                Instr(Mnemonic.svbr, _("svbr")),
                invalid,

                Instr(Mnemonic.rvbr, _("rvbr")),
                invalid,
                Instr(Mnemonic.tvbr, _("tvbr")),
                invalid,

                // 60
                Instr(Mnemonic.sll, _("sll")),
                Instr(Mnemonic.srl, _("srl")),
                Instr(Mnemonic.sra, _("sra")),
                Instr(Mnemonic.slc, _("slc")),

                invalid,
                Instr(Mnemonic.dsll, _("dsll")),
                Instr(Mnemonic.dsrl, _("dsrl")),
                Instr(Mnemonic.dsra, _("dsra")),

                Instr(Mnemonic.dslc, _("dslc")),
                invalid,
                Instr(Mnemonic.slr, _("slr")),
                Instr(Mnemonic.sar, _("sar")),

                Instr(Mnemonic.scr, _("scr")),
                Instr(Mnemonic.dslr, _("dslr")),
                Instr(Mnemonic.dsar, _("dsar")),
                Instr(Mnemonic.dscr, _("dscr")),

                // 70
                Instr(Mnemonic.jc, _("jc")),
                Instr(Mnemonic.jci, _("jci")),
                Instr(Mnemonic.js, _("js")),
                Instr(Mnemonic.soj, _("soj")),

                Instr(Mnemonic.br, _("br")),
                Instr(Mnemonic.bez, _("bez")),
                Instr(Mnemonic.blt, ICR),
                Instr(Mnemonic.bex, _("bex")),

                Instr(Mnemonic.ble, _("fc")),
                Instr(Mnemonic.bgt, _("fcr")),
                Instr(Mnemonic.bnz, _("efc")),
                Instr(Mnemonic.bge, _("efcr")),

                Instr(Mnemonic.lsti, _("lsti")), // b
                Instr(Mnemonic.lst, _("lst")), // b
                Instr(Mnemonic.sjs, _("sjs")),
                Instr(Mnemonic.urs, _("urs")),

                // 80
                Instr(Mnemonic.l, _("l")),
                Instr(Mnemonic.lr, _("lr")),
                Instr(Mnemonic.lisp, _("lisp")),
                Instr(Mnemonic.lisn, _("lisn")),

                Instr(Mnemonic.li, _("li")),
                Instr(Mnemonic.lim, _("lim")),
                Instr(Mnemonic.dl, _("dl")),
                Instr(Mnemonic.dlr, _("dlr")),

                Instr(Mnemonic.dli, _("dli")),
                Instr(Mnemonic.lm, _("lm")),
                Instr(Mnemonic.efl, _("efl")),
                Instr(Mnemonic.lub, _("lub")),

                Instr(Mnemonic.llb, Ra,Dx_w16),
                Instr(Mnemonic.lubi, _("lubi")),
                Instr(Mnemonic.llbi, _("llbi")),
                Instr(Mnemonic.popm, _("popm")),

                // 90
                Instr(Mnemonic.st , _("st ")),
                Instr(Mnemonic.stc, N,Dx_w16),
                Instr(Mnemonic.stci , _("stci ")),
                Instr(Mnemonic.mov , _("mov ")),

                Instr(Mnemonic.sti , _("sti ")),
                invalid,
                Instr(Mnemonic.dst , _("dst ")),
                Instr(Mnemonic.srm , _("srm ")),

                Instr(Mnemonic.dsti , Ra,Ix_w32),
                Instr(Mnemonic.stm , _("stm ")),
                Instr(Mnemonic.efst , _("efst ")),
                Instr(Mnemonic.stub, Ra,Dx_w16),

                Instr(Mnemonic.sltb, _("sltb")),
                Instr(Mnemonic.subi, _("subi")),
                Instr(Mnemonic.slbi, _("slbi")),
                Instr(Mnemonic.pshm, _("pshm")),

                // A0
                Instr(Mnemonic.a, _("a")),
                Instr(Mnemonic.ar, _("ar")),
                Instr(Mnemonic.aisp, _("aisp")),
                Instr(Mnemonic.incm, _("incm")),

                Instr(Mnemonic.abs, _("abs")),
                Instr(Mnemonic.dabs, _("dabs")),
                Instr(Mnemonic.da, _("da")),
                Instr(Mnemonic.dar, _("dar")),

                Instr(Mnemonic.fa, _("fa")),
                Instr(Mnemonic.far, _("far")),
                Instr(Mnemonic.efa, _("efa")),
                Instr(Mnemonic.efar, _("efar")),

                Instr(Mnemonic.fabs, _("fabs")),
                invalid,
                invalid,
                invalid,

                // B0
                Instr(Mnemonic.s, _("s")),
                Instr(Mnemonic.sr, _("sr")),
                Instr(Mnemonic.sisp, _("sisp")),
                Instr(Mnemonic.decm, _("decm")),

                Instr(Mnemonic.neg, _("neg")),
                Instr(Mnemonic.dneg, _("dneg")),
                Instr(Mnemonic.ds, _("ds")),
                Instr(Mnemonic.dsr, _("dsr")),

                Instr(Mnemonic.fs, _("fs")),
                Instr(Mnemonic.fsr, _("fsr")),
                Instr(Mnemonic.efs, _("efs")),
                Instr(Mnemonic.efsr, _("efsr")),

                Instr(Mnemonic.fneg, Ra,Rb),
                invalid,
                invalid,
                invalid,

                // C0
                Instr(Mnemonic.ms, Ra,Dx_w16),
                Instr(Mnemonic.msr, Ra,Rb),
                Instr(Mnemonic.misp, _("misp")),
                Instr(Mnemonic.misn, _("misn")),

                Instr(Mnemonic.m, _("m")),
                Instr(Mnemonic.mr, _("mr")),
                Instr(Mnemonic.dm, _("dm")),
                Instr(Mnemonic.dmr, _("dmr")),

                Instr(Mnemonic.fm, _("fm")),
                Instr(Mnemonic.fmr, _("fmr")),
                Instr(Mnemonic.efm, _("efm")),
                Instr(Mnemonic.efmr, _("efmr")),
                    
                invalid,
                invalid,
                invalid,
                invalid,

                // D0
                Instr(Mnemonic.dv, _("dv")),
                Instr(Mnemonic.dvr, _("dvr")),
                Instr(Mnemonic.disp, _("disp")),
                Instr(Mnemonic.disn, _("disn")),

                Instr(Mnemonic.d, _("d")),
                Instr(Mnemonic.dr, _("dr")),
                Instr(Mnemonic.dd, _("dd")),
                Instr(Mnemonic.ddr, _("ddr")),

                Instr(Mnemonic.fd, _("fd")),
                Instr(Mnemonic.fdr, _("fdr")),
                Instr(Mnemonic.efd, _("efd")),
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
                Instr(Mnemonic.n, _("n")),
                Instr(Mnemonic.nr, _("nr")),

                Instr(Mnemonic.flx, _("flx")),
                Instr(Mnemonic.flt, Ra,Rb),
                Instr(Mnemonic.eftx, _("eftx")),
                Instr(Mnemonic.eflt, _("eflt")),

                Select((0, 4), Is0, "  EC",
                    Instr(Mnemonic.xbr, Ra),
                    nyi),
                Instr(Mnemonic.xwr, _("xwr")),
                invalid,
                invalid,

                // F0
                Instr(Mnemonic.c, _("c")),
                Instr(Mnemonic.cr, _("cr")),
                Instr(Mnemonic.cisp, _("cisp")),
                Instr(Mnemonic.cism, _("cism")),

                Instr(Mnemonic.cbl, _("cbl")),
                invalid,
                Instr(Mnemonic.dc, Ra,Dx_w32),
                Instr(Mnemonic.dcr, _("dcr")),

                Instr(Mnemonic.fc, _("fc")),
                Instr(Mnemonic.fcr, Ra,Rb),
                Instr(Mnemonic.efc, _("efc")),
                Instr(Mnemonic.efcr, _("efcr")),

                invalid,
                invalid,
                invalid,
                Select((0, 8), Is0, "  0xFF",
                    Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding),
                    Nyi("FF"))
            });
        }
    }
}