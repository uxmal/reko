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

using System;
using System.Collections.Generic;
using System.Text;
using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Machine;

namespace Reko.Arch.Cray.Ymp
{
    using Decoder = Reko.Core.Machine.Decoder<YmpDisassembler, Mnemonic, CrayInstruction>;

    // Based on "Cray Y-MP Computer Systems Function Description Manual (HR-04001-0C)

    public class YmpDisassembler : DisassemblerBase<CrayInstruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;
        
        private CrayYmpArchitecture arch;
        private EndianImageReader rdr;
        private List<MachineOperand>  ops;

        public YmpDisassembler(CrayYmpArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override CrayInstruction DisassembleInstruction()
        {
            var addr = rdr.Address;
            if (!rdr.TryReadBeUInt16(out ushort hInstr))
                return null;
            ops.Clear();
            var instr = rootDecoder.Decode(hInstr, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override CrayInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            var instr = new CrayInstruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = this.ops.ToArray()
            };
            return instr;
        }

        public override CrayInstruction CreateInvalidInstruction()
        {
            return new CrayInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
                Operands = MachineInstruction.NoOperands
            };
        }

        #region Mutators

        private static Mutator<YmpDisassembler> Reg(int bitOffset, int bitSize, RegisterStorage[] regs)
        {
            var field = new Bitfield(bitOffset, bitSize);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                var reg = regs[iReg];
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }
        private static readonly Mutator<YmpDisassembler> Si = Reg(6, 3, Registers.SRegs);
        private static readonly Mutator<YmpDisassembler> Sj = Reg(3, 3, Registers.SRegs);
        private static readonly Mutator<YmpDisassembler> Sk = Reg(0, 3, Registers.SRegs);

        private static readonly Mutator<YmpDisassembler> Ai = Reg(6, 3, Registers.ARegs);
        private static readonly Mutator<YmpDisassembler> Aj = Reg(3, 3, Registers.ARegs);
        private static readonly Mutator<YmpDisassembler> Ak = Reg(0, 3, Registers.ARegs);

        private static readonly Mutator<YmpDisassembler> Bjk = Reg(0, 6, Registers.BRegs);


        private static readonly Mutator<YmpDisassembler> Vi = Reg(6, 3, Registers.VRegs);
        private static readonly Mutator<YmpDisassembler> Vj = Reg(3, 3, Registers.VRegs);
        private static readonly Mutator<YmpDisassembler> Vk = Reg(0, 3, Registers.VRegs);


        #endregion

        #region Decoders

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<YmpDisassembler>[] mutators)
        {
            return new InstrDecoder<YmpDisassembler, Mnemonic, CrayInstruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<YmpDisassembler>[] mutators)
        {
            return new InstrDecoder<YmpDisassembler, Mnemonic, CrayInstruction>(iclass, mnemonic, mutators);
        }

        protected static NyiDecoder<YmpDisassembler, Mnemonic, CrayInstruction> Nyi(string message)
        {
            return new NyiDecoder<YmpDisassembler, Mnemonic, CrayInstruction>(message);
        }

        #endregion

        static YmpDisassembler()
        {
            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);
            
            rootDecoder = Sparse(9, 7, "YMP",
                Nyi("YMP"),
                (0x05, Select((6, 3), u => u == 0, "  005x",
                    Instr(Mnemonic.j, InstrClass.Transfer, Bjk),
                    invalid)),
                (0x13, Instr(Mnemonic._mov, Ai, Sj)),       // 0o023
                (0x23, Instr(Mnemonic._and, Si, Sj, Sk)),   // 0o043
                (0x34, Instr(Mnemonic._fmul, Si, Sj, Sk)),  // 0o064
                (0x3E, Instr(Mnemonic._mov, Si, Vj, Ak))    // 0o076
                );
        }
    }
}
