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
    public class PowerPcRewriter : IEnumerable<RtlInstructionCluster>
    {
        private IEnumerator<PowerPcInstruction> instrs;
        private Frame frame;
        private RtlEmitter emitter;

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
            this.emitter = new RtlEmitter(cluster.Instructions);
            Expression op1;
            Expression op2;
            Expression op3;
            Expression ea;
            switch (instrs.Current.Opcode)
            {
            default: throw new AddressCorrelatedException(
                instr.Address,
                "PowerPC opcode {0} is not supported yet.", 
                instr.Opcode);
            case Opcode.add:
                var sum = RewriteOperand(instr.op1);
                emitter.Assign(
                    sum,
                    emitter.IAdd(
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
            case Opcode.lwz:
                op1 = RewriteOperand(instrs.Current.op1);
                ea = EffectiveAddress_r0(instrs.Current.op2, emitter);
                emitter.Assign(op1, emitter.LoadDw(ea));
                break;
            case Opcode.lwzu:
                op1 = RewriteOperand(instrs.Current.op1); 
                ea = EffectiveAddress(instrs.Current.op2, emitter);
                emitter.Assign(op1, emitter.LoadDw(ea));
                emitter.Assign(UpdatedRegister(ea), ea);
                break;
            case Opcode.stbu:
                op1 = RewriteOperand(instrs.Current.op1);
                ea = EffectiveAddress(instrs.Current.op2, emitter);
                emitter.Assign(emitter.LoadB(ea), emitter.Cast(PrimitiveType.Byte, op1));
                emitter.Assign(UpdatedRegister(ea), ea);
                break;
            case Opcode.stbux:
                op1 = RewriteOperand(instrs.Current.op1);
                op2 = RewriteOperand(instrs.Current.op2);
                op3 = RewriteOperand(instrs.Current.op3);
                ea = emitter.IAdd(op2, op3);
                emitter.Assign(emitter.LoadB(ea), emitter.Cast(PrimitiveType.Byte, op1));
                emitter.Assign(op2, emitter.IAdd(op2, op3));
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

        private Expression EffectiveAddress(MachineOperand operand, RtlEmitter emitter)
        {
            var mop = (MemoryOperand) operand;
            var reg = frame.EnsureRegister(mop.BaseRegister);
            var offset = mop.Offset;
            return emitter.IAdd(reg, offset);
        }

        private Expression EffectiveAddress_r0(MachineOperand operand, RtlEmitter emitter)
        {
            var mop = (MemoryOperand) operand;
            if (mop.BaseRegister.Number == 0)
            {
                return Constant.Word32((int) mop.Offset.ToInt16());
            }
            else
            {
                var reg = frame.EnsureRegister(mop.BaseRegister);
                var offset = mop.Offset;
                return emitter.IAdd(reg, offset);
            }
        }

        private Expression UpdatedRegister(Expression effectiveAddress)
        {
            var bin = (BinaryExpression) effectiveAddress;
            return bin.Left;
        }
    }
}
