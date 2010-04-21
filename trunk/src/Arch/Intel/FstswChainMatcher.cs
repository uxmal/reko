using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.Intel
{
    public class FstswChainMatcher
    {
        IntelInstruction [] instrs;
        Dictionary<int, Opcode> zappedInstructions;
        List<Instruction> rewritten;
        OperandRewriter orw;


        public FstswChainMatcher(IntelInstruction[] instrs, OperandRewriter orw)
        {
            this.instrs = instrs;
            this.orw = orw;
            this.zappedInstructions = new Dictionary<int, Opcode>();
            this.rewritten = new List<Instruction>();
        }

        public bool Matches(int iStart)
        {
            int i = iStart;
            if (instrs[i].code != Opcode.fstsw)
                return false;
            ++i;
            if (i >= instrs.Length)
                return false;
            if (instrs[i].code == Opcode.sahf)
            {
                zappedInstructions.Add(i, Opcode.nop);
                rewritten.Add(new Assignment(
                    orw.FlagGroup(FlagM.ZF | FlagM.CF | FlagM.SF | FlagM.OF),
                    orw.FlagGroup(FlagM.FPUF)));
                return true;
            }
            if (instrs[i].code == Opcode.test)
            {
                RegisterOperand acc = instrs[i].op1 as RegisterOperand;
                if (acc == null)
                    return false;
                ImmediateOperand imm = instrs[i].op2 as ImmediateOperand;
                if (imm == null)
                    return false;
                int mask = imm.Value.ToInt32();
                if (acc.Register == Registers.ax || acc.Register == Registers.eax)
                    mask >>= 8;
                else if (acc.Register != Registers.ah)
                    return false;
                zappedInstructions.Add(i, Opcode.nop);
                rewritten.Add(new Assignment(
                    orw.FlagGroup(FlagM.ZF | FlagM.CF | FlagM.SF | FlagM.OF),
                    orw.FlagGroup(FlagM.FPUF)));

                i = FindConditionalJumpInstruction(++i);
                if (i < 0)
                    return false;
                switch (instrs[i].code)
                {
                case Opcode.jz:
                    if (mask == 0x40)
                    {
                        zappedInstructions.Add(i, Opcode.jnz);
                        return true;
                    }
                    if (mask == 0x01)
                    {
                        zappedInstructions.Add(i, Opcode.jge);
                        return true;
                    }
                    if (mask == 0x41)
                    {
                        zappedInstructions.Add(i, Opcode.jg);
                        return true;
                    }
                    break;
                case Opcode.jnz:
                    if (mask == 0x40)
                    {
                        zappedInstructions.Add(i, Opcode.jz);
                        return true;
                    }
                    if (mask == 0x01)
                    {
                        zappedInstructions.Add(i, Opcode.jl);
                        return true;
                    }
                    if (mask == 0x41)
                    {
                        zappedInstructions.Add(i, Opcode.jle);
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
                switch (instrs[i].code)
                {
                case Opcode.jz:
                case Opcode.jnz:
                    return i;
                }
                ++i;
            }
            return -1;
        }

        private ImmediateOperand TestAhConstant(IntelInstruction instr)
        {
            RegisterOperand ah = instr.op1 as RegisterOperand;
            if (ah == null)
                return null;
            if (ah.Register != Registers.ah)
                return null;
            ImmediateOperand imm = instr.op2 as ImmediateOperand;
            if (imm == null)
                return null;
            return imm;
        }

        public void Rewrite(CodeEmitter emitter)
        {
            foreach (var de in this.zappedInstructions)
            {
                instrs[de.Key].code = de.Value;
            }
            foreach (Instruction instr in rewritten)
            {
                emitter.Emit(instr);
            }
        }
    }
}
