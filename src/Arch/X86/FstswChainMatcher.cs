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
using Reko.Core.Code;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.X86
{
    public class FstswChainMatcher
    {
        X86Instruction [] instrs;
        Dictionary<int, Mnemonic> zappedInstructions;
        List<Instruction> rewritten;
        OperandRewriter orw;

        /*
         * the C0 bit becomes the CF (carry) flag, 
 the C2 bit becomes the PF(parity) flag, and 
 the C3 bit becomes the ZF (zero) flag. 
         */
        //  >              0
        //  <              1
        //  =             40
        //  inordered     45

        public FstswChainMatcher(X86Instruction[] instrs, OperandRewriter orw)
        {
            this.instrs = instrs;
            this.orw = orw;
            this.zappedInstructions = new Dictionary<int, Mnemonic>();
            this.rewritten = new List<Instruction>();
        }

        public bool Matches(int iStart)
        {
            int i = iStart;
            if (instrs[i].Mnemonic != Mnemonic.fstsw)
                return false;
            ++i;
            if (i >= instrs.Length)
                return false;
            if (instrs[i].Mnemonic == Mnemonic.sahf)
            {
                zappedInstructions.Add(i, Mnemonic.nop);
                rewritten.Add(new Assignment(
                    orw.FlagGroup(FlagM.ZF | FlagM.CF | FlagM.SF | FlagM.OF),
                    orw.AluRegister(Registers.FPUF)));
                return true;
            }
            if (instrs[i].Mnemonic == Mnemonic.test)
            {
                RegisterOperand acc = instrs[i].Operands[0] as RegisterOperand;
                if (acc == null)
                    return false;
                ImmediateOperand imm = instrs[i].Operands[1] as ImmediateOperand;
                if (imm == null)
                    return false;
                int mask = imm.Value.ToInt32();
                if (acc.Register == Registers.ax || acc.Register == Registers.eax)
                    mask >>= 8;
                else if (acc.Register != Registers.ah)
                    return false;
                zappedInstructions.Add(i, Mnemonic.nop);
                rewritten.Add(new Assignment(
                    orw.FlagGroup(FlagM.ZF | FlagM.CF | FlagM.SF | FlagM.OF),
                    orw.AluRegister(Registers.FPUF)));

                i = FindConditionalJumpInstruction(++i);
                if (i < 0)
                    return false;
                switch (instrs[i].Mnemonic)
                {
                case Mnemonic.jz:
                    if (mask == 0x40)
                    {
                        zappedInstructions.Add(i, Mnemonic.jnz);
                        return true;
                    }
                    if (mask == 0x01)
                    {
                        zappedInstructions.Add(i, Mnemonic.jge);
                        return true;
                    }
                    if (mask == 0x41)
                    {
                        zappedInstructions.Add(i, Mnemonic.jg);
                        return true;
                    }
                    break;
                case Mnemonic.jnz:
                    if (mask == 0x40)
                    {
                        zappedInstructions.Add(i, Mnemonic.jz);
                        return true;
                    }
                    if (mask == 0x01)
                    {
                        zappedInstructions.Add(i, Mnemonic.jl);
                        return true;
                    }
                    if (mask == 0x41)
                    {
                        zappedInstructions.Add(i, Mnemonic.jle);
                        return true;
                    }
                    break;
                }
                return false;
            }
            return false;
        }

        private int FindConditionalJumpInstruction(int i)
        {
            while (i < instrs.Length)
            {
                switch (instrs[i].Mnemonic)
                {
                case Mnemonic.jz:
                case Mnemonic.jnz:
                    return i;
                }
                ++i;
            }
            return -1;
        }

        public void Rewrite(CodeEmitter emitter)
        {
            foreach (var de in this.zappedInstructions)
            {
                instrs[de.Key].Mnemonic = de.Value;
            }
            foreach (Instruction instr in rewritten)
            {
                emitter.Emit(instr);
            }
        }
    }
}
