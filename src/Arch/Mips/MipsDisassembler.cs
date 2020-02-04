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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Arch.Mips
{
    using Decoder = Reko.Core.Machine.Decoder<MipsDisassembler, Mnemonic, MipsInstruction>;

    public partial class MipsDisassembler : DisassemblerBase<MipsInstruction, Mnemonic>
    {
        private const InstrClass TD = InstrClass.Transfer | InstrClass.Delay;
        private const InstrClass CTD = InstrClass.Call | InstrClass.Transfer | InstrClass.Delay;
        private const InstrClass DCT = InstrClass.ConditionalTransfer | InstrClass.Delay;

        private readonly MipsProcessorArchitecture arch;
        private readonly Decoder rootDecoder;
        private readonly EndianImageReader rdr;
        private readonly PrimitiveType signedWord;
        private readonly List<MachineOperand> ops;
        private MipsInstruction instrCur;
        private Address addr;

        public MipsDisassembler(MipsProcessorArchitecture arch, Decoder decoder, EndianImageReader imageReader)
        {
            this.arch = arch;
            this.rootDecoder = decoder;
            this.rdr = imageReader;
            this.signedWord = PrimitiveType.Create(Domain.SignedInt, arch.WordWidth.BitSize);
            this.ops = new List<MachineOperand>();
        }

        public override MipsInstruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadUInt32(out uint wInstr))
            {
                return null;
            }
            this.ops.Clear();
            instrCur = rootDecoder.Decode(wInstr, this);
            instrCur.Address = this.addr;
            instrCur.Length = 4;
            return instrCur;
        }

        public override MipsInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new MipsInstruction
            {
                Mnemonic = mnemonic,
                InstructionClass = iclass,
                Address = this.addr,
                Length = 4,
                Operands = this.ops.ToArray()
            };
        }

        public override MipsInstruction CreateInvalidInstruction()
        {
            return new MipsInstruction 
            {
                Mnemonic = Mnemonic.illegal,
                InstructionClass = InstrClass.Invalid,
                Address = this.addr,
                Length = 4,
                Operands = MachineInstruction.NoOperands
            };
        }

        public override MipsInstruction NotYetImplemented(uint wInstr, string message)
        {
            var instr = CreateInvalidInstruction();
            EmitUnitTest(wInstr, message);
            return instr;
        }

        [Conditional("DEBUG")]
        public void EmitUnitTest(uint wInstr, string message)
        {
            var op = (wInstr >> 26);
            if (op == 0 || op == 1)
                return;
            var instrHex = $"{wInstr:X8}";
            base.EmitUnitTest("MIPS", instrHex, message, "MipsDis", this.addr, w =>
            {
                w.WriteLine("    AssertCode(\"@@@\", \"0x{0:X8}\");", wInstr);
            });
        }

        private RegisterOperand Reg(uint regNumber)
        {
            return new RegisterOperand(arch.GetRegister((int) regNumber & 0x1F));
        }

        private RegisterOperand FReg(uint regNumber)
        {
            return new RegisterOperand(arch.fpuRegs[regNumber & 0x1F]);
        }

        private bool TryGetFCReg(uint regNumber, out RegisterOperand op)
        {
            if (arch.fpuCtrlRegs.TryGetValue(regNumber & 0x1F, out RegisterStorage fcreg))
            {
                op = new RegisterOperand(fcreg);
                return true;
            }
            else
            {
                op = null;
                return false;
            }
        }

        private RegisterOperand CCodeFlag(uint wInstr, int regPos)
        {
            var regNo = (wInstr >> regPos) & 0x7;
            return new RegisterOperand(arch.ccRegs[regNo]);
        }

        private RegisterOperand FpuCCodeFlag(uint wInstr, int regPos)
        {
            var regNo = (wInstr >> regPos) & 0x7;
            return new RegisterOperand(arch.fpuCcRegs[regNo]);
        }

        private AddressOperand RelativeBranch(uint wInstr)
        {
            int off = (short) wInstr;
            off <<= 2;
            return AddressOperand.Create(rdr.Address + off);
        }

        private AddressOperand LargeBranch(uint wInstr)
        {
            var off = (wInstr & 0x03FFFFFF) << 2;
            ulong linAddr = (rdr.Address.ToLinear() & ~0x0FFFFFFFul) | off;
            return AddressOperand.Create(
                Address.Create(arch.PointerType, linAddr));
        }

        private IndirectOperand Ea(uint wInstr, PrimitiveType dataWidth, int shift, short offset)
        {
            var baseReg = arch.GetRegister((int) (wInstr >> shift) & 0x1F);
            return new IndirectOperand(dataWidth, offset, baseReg);
        }
    }
}
