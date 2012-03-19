using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.PowerPC
{
    public class PowerPcRewriter : Rewriter
    {
        private IEnumerator<PowerPcInstruction> instrs;
        private Frame frame;

        public PowerPcRewriter(IEnumerable<PowerPcInstruction> instrs, Frame frame)
        {
            this.instrs = instrs.GetEnumerator();
            this.frame = frame;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            if (!instrs.MoveNext())
                yield break;
            var instr = instrs.Current;
            var cluster = new RtlInstructionCluster(instr.Address, 4);
            var emitter = new RtlEmitter(cluster.Instructions);
            switch (instrs.Current.Opcode)
            {
            default: throw new NotSupportedException(string.Format("PowerPC opcode {0} is not supported yet.", instr.Opcode));
            case Opcode.add:
                var sum = RewriteOperand(instr.op1);
                emitter.Assign(
                    sum,
                    emitter.Add(
                        RewriteOperand(instr.op2),
                        RewriteOperand(instr.op3)));
                if (instr.DefCc() != 0)
                    emitter.Assign(frame.EnsureFlagGroup(0xF, "SCZO", PrimitiveType.Byte),
                        emitter.Cond(sum));
                break;
            case Opcode.oris:
                emitter.Assign(
                    RewriteOperand(instrs.Current.op1),
                    emitter.Or(
                        RewriteOperand(instrs.Current.op2),
                        Shift16(instrs.Current.op3)));
                break;
            }
            yield return cluster;
        }

        private Expression Shift16(MachineOperand machineOperand)
        {
            var imm = (ImmediateOperand)machineOperand;
            return Constant.Word32(imm.Value.ToInt32() << 16);
        }

        private Expression RewriteOperand(MachineOperand machineOperand)
        {
            var rOp = machineOperand as RegisterOperand;
            if (rOp != null)
                return frame.EnsureRegister(rOp.Register);
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
