#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Gee.External.Capstone;
using Gee.External.Capstone.Arm;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Arm
{
    public partial class ThumbRewriter : IEnumerable<RtlInstructionCluster>
    {
        private ThumbProcessorArchitecture arch;
        private IEnumerator<Arm32Instruction> instrs;
        private ThumbProcessorState state;
        private Frame frame;
        private IRewriterHost host;
        private Instruction<ArmInstruction,ArmRegister,ArmInstructionGroup,ArmInstructionDetail> instr;
        private ArmInstructionOperand[] ops;
        private  RtlInstructionCluster ric;
        private  RtlEmitter emitter;

        public ThumbRewriter(ThumbProcessorArchitecture arch, ImageReader rdr, ThumbProcessorState state, Frame frame, IRewriterHost host)
        {
            this.arch = arch;
            this.instrs = CreateInstructionStream(rdr);
            this.state = state;
            this.frame = frame;
            this.host = host;
        }

        private IEnumerator<Arm32Instruction> CreateInstructionStream(ImageReader rdr)
        {
            return new ThumbDisassembler(rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (instrs.MoveNext())
            {
                this.instr = instrs.Current.Internal;
                this.ops = instr.ArchitectureDetail.Operands;

                this.ric = new RtlInstructionCluster(instrs.Current.Address, instr.Bytes.Length);
                this.emitter = new RtlEmitter(ric.Instructions);
                switch (instr.Id)
                {
                default:
                    throw new AddressCorrelatedException(
                      instrs.Current.Address,
                      "Rewriting ARM opcode '{0}' is not supported yet.",
                      instr.Mnemonic);
                case ArmInstruction.ADD: RewriteBinop((a, b)=>emitter.IAdd(a, b)); break;
                case ArmInstruction.BL: RewriteBl(); break;
                case ArmInstruction.LDR: RewriteLdr(); break;
                case ArmInstruction.MOV: RewriteMov(); break;
                case ArmInstruction.POP: RewritePop(); break;
                case ArmInstruction.PUSH: RewritePush(); break;
                case ArmInstruction.STR: RewriteStr(); break;
                case ArmInstruction.SUB: RewriteBinop((a, b)=> emitter.ISub(a, b)); break;
                }
                yield return ric;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Expression GetReg(ArmRegister armRegister)
        {
            return frame.EnsureRegister(A32Registers.RegisterByCapstoneID[armRegister]);
        }

        private Expression RewriteOp(ArmInstructionOperand op, DataType accessSize= null)
        {
            switch (op.Type)
            {
            case ArmInstructionOperandType.Register:
                return GetReg(op.RegisterValue.Value);
            case ArmInstructionOperandType.Immediate:
                return Constant.Int32(op.ImmediateValue.Value);
            case ArmInstructionOperandType.Memory:
                var mem = op.MemoryValue;
                var baseReg = GetReg(mem.BaseRegister);
                var ea = baseReg;
                if (mem.Displacement > 0)
                {
                    ea = emitter.IAdd(ea, Constant.Int32(mem.Displacement));
                }
                else if (mem.Displacement < 0)
                {
                    ea = emitter.ISub(ea, Constant.Int32(-mem.Displacement));
                }
                return emitter.Load(accessSize, ea);
            default:
                throw new NotImplementedException(op.Type.ToString());
            }
        }
    }
}
