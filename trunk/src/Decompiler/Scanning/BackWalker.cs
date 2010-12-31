#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decompiler.Scanning
{
    public class BackWalker2
    {
        private IProcessorArchitecture arch;
        private ProgramImage img;
        private Identifier regIdxDetected;
        private static TraceSwitch trace = new TraceSwitch("IntelBackWalker", "Traces the progress of x86 backward instruction walking");

        public BackWalker2(IProcessorArchitecture arch, ProgramImage img)
        {
            this.arch = arch;
            this.img = img;
        }

        [Conditional("DEBUG")]
        private void DumpInstructions(StatementList instrs, int idx)
        {
            for (int i = 0; i < instrs.Count; ++i)
            {
                Debug.WriteLineIf(trace.TraceInfo,
                    string.Format("{0} {1}",
                    idx == i ? '*' : ' ',
                    instrs[i]));
            }
        }

        public static bool IsEvenPowerOfTwo(int n)
        {
            return n != 0 && (n & (n - 1)) == 0;
        }

        public class Visitor : InstructionVisitor, IExpressionVisitor
        {
            Identifier regIdx;
            Identifier regDst;
            List<BackwalkOperation> operations;
            Frame frame;
            uint grfDst;
            uint grfUsedInBranch;

            public Visitor(Identifier regIdx, Frame frame, List<BackwalkOperation> operations)
            {
                this.regIdx = regIdx;
                this.frame = frame;
                this.operations = operations;
            }

            public Identifier BackwalkInstructions(StatementList instrs, int i)
            {
                for (; i >= 0; --i)
                {
                    Instruction instr = instrs[i].Instruction;
                    instr.Accept(this);
                    if (IndexRegister == null || ReturnToCaller)
                        break;
                }
                return IndexRegister;
#if OLD
            switch (instr.code)
            {

                break;
            case Opcode.adc:
                if (ropDst != null && ropDst.Register == regIdx)
                {
                    return MachineRegister.None;
                }
                break;
            case Opcode.inc:
            case Opcode.dec:
                if (ropDst != null && ropDst.Register == regIdx)
                {
                    BackwalkOperator op = instr.code == Opcode.inc ? BackwalkOperator.add : BackwalkOperator.sub;
                    operations.Add(new BackwalkOperation(op, 1));
                }
                break;
            case Opcode.ja:
                operations.Add(new BackwalkBranch(ConditionCode.UGT));
                break;
            case Opcode.mov:
                if (ropDst != null)
                {
                    if (ropDst.Register == regIdx)
                    {
                        if (ropSrc != null)
                            regIdx = ropSrc.Register;
                        else
                            regIdx = MachineRegister.None;	// haven't seen an immediate compare yet.
                    }
                }
                break;
            case Opcode.movzx:
                if (ropDst != null && ropDst.Register == regIdx)
                {
                    if (memSrc != null)
                    {
                        return MachineRegister.None;
                    }
                }
                break;
            case Opcode.pop:
                if (ropDst != null && ropDst.Register == regIdx)
                {
                    return MachineRegister.None;
                }
                break;
            case Opcode.push:
                break;
            case Opcode.shl:
                if (ropDst != null && ropDst.Register == regIdx)
                {
                    if (immSrc != null)
                    {
                        operations.Add(new BackwalkOperation(BackwalkOperator.mul, 1 << immSrc.Value.ToInt32()));
                    }
                    else
                        return MachineRegister.None;
                }
                break;
            case Opcode.test:
                return MachineRegister.None;
            case Opcode.xchg:
                if (ropDst != null && ropSrc != null)
                {
                    if (ropDst.Register == regIdx ||
                        ropSrc.Register == regIdx)
                    {
                        regIdx = ropDst.Register;
                    }
                }
                else
                {
                    regIdx = MachineRegister.None;
                }
                break;
            default:
                System.Diagnostics.Debug.WriteLine("Backwalking not supported: " + instr.code);
                DumpInstructions(instrs, i);
                break;
            }
#endif
                return regIdx;
            }


            public Identifier IndexRegister { get { return regIdx; } }

            public bool ReturnToCaller { get; set; }

            private Identifier HandleAddition(
                Identifier regIdx,
                Identifier ropDst,
                Identifier ropSrc,
                Constant immSrc,
                bool add)
            {
                if (ropSrc != null)
                {
                    if (ropSrc == ropDst && add)
                    {
                        operations.Add(new BackwalkOperation(BackwalkOperator.mul, 2));
                        return regIdx;
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (ropSrc == ropDst && immSrc != null)
                {
                    operations.Add(new BackwalkOperation(
                        add ? BackwalkOperator.add : BackwalkOperator.sub,
                        immSrc.ToInt32()));
                    return regIdx;
                }
                else
                    return null;
            }

            #region InstructionVisitor Members

            public void VisitAssignment(Assignment a)
            {
                if (a.Dst.Storage is RegisterStorage)
                {
                    regDst = a.Dst;
                    grfDst = 0;
                    a.Src.Accept(this);
                }
                var f = a.Dst.Storage as FlagGroupStorage;
                if (f != null)
                {
                    grfDst = f.FlagGroup;
                    regDst = null;
                    a.Src.Accept(this);
                }
            }

            public void VisitBranch(Branch b)
            {
                var cond = b.Condition as TestCondition;
                if (cond == null)
                    return;
                var flags = cond.Expression as Identifier;
                if (flags == null)
                    return;
                var f = flags.Storage as FlagGroupStorage;
                if (f == null)
                    return;
                grfUsedInBranch = f.FlagGroup;
                operations.Add(new BackwalkBranch(cond.ConditionCode));
            }

            public void VisitCallInstruction(CallInstruction ci)
            {
                throw new NotImplementedException();
            }

            public void VisitDeclaration(Declaration decl)
            {
                throw new NotImplementedException();
            }

            public void VisitDefInstruction(DefInstruction def)
            {
                throw new NotImplementedException();
            }

            public void VisitGotoInstruction(GotoInstruction gotoInstruction)
            {
            }

            public void VisitPhiAssignment(PhiAssignment phi)
            {
                throw new NotImplementedException();
            }

            public void VisitIndirectCall(IndirectCall ic)
            {
                throw new NotImplementedException();
            }

            public void VisitReturnInstruction(ReturnInstruction ret)
            {
                throw new NotImplementedException();
            }

            public void VisitSideEffect(SideEffect side)
            {
                throw new NotImplementedException();
            }

            public void VisitStore(Store store)
            {
            }

            public void VisitSwitchInstruction(SwitchInstruction si)
            {
                throw new NotImplementedException();
            }

            public void VisitUseInstruction(UseInstruction u)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IExpressionVisitor Members

            public void VisitAddress(Address addr)
            {
                throw new NotImplementedException();
            }

            public void VisitApplication(Application appl)
            {
                throw new NotImplementedException();
            }

            public void VisitArrayAccess(ArrayAccess acc)
            {
                throw new NotImplementedException();
            }

            public void VisitBinaryExpression(BinaryExpression binExp)
            {
                var regLeft = binExp.Left as Identifier;
                var immRight = binExp.Right as Constant;
                if (binExp.op == Operator.Add || binExp.op == Operator.Sub && regLeft == this.regDst)
                {
                    regIdx = HandleAddition(
                        regIdx,
                        regDst,
                        binExp.Left as Identifier,
                        binExp.Right as Constant,
                        binExp.op == Operator.Add);
                    return;
                }
                if (binExp.op == Operator.Sub && (grfDst&grfUsedInBranch) == grfUsedInBranch && regLeft != null)
                {
                    var rLeft = regLeft.Storage as RegisterStorage;
                    var rIdx = regIdx.Storage as RegisterStorage;
                    if (regLeft == regIdx || rLeft.Register == rIdx.Register.GetSubregister(0, 8))
                    {
                        if (immRight != null)
                        {
                            operations.Add(new BackwalkOperation(BackwalkOperator.cmp, immRight.ToInt32()));
                            ReturnToCaller = true;
                        }
                    }
                }
                if (binExp.op == Operator.And && regLeft == this.regDst && immRight != null)
                {
                    var mask_plus_1 = immRight.ToInt32() + 1;
                    if (IsEvenPowerOfTwo(mask_plus_1))
                    {
                        operations.Add(new BackwalkOperation(BackwalkOperator.cmp, mask_plus_1));
                        ReturnToCaller = true;
                    }
                    else
                    {
                        regIdx = null;
                    }
                    return;
                }
                if (binExp.op == Operator.Xor)
                {
                    if (regDst == null || regLeft == null)
                        return;
                    if (regDst != regLeft)
                        return;
                    var rSrc = regLeft.Storage as RegisterStorage;
                    if (rSrc == null)
                        return;
                    var rIdx = regIdx.Storage as RegisterStorage;
                    if (rSrc.Register != rIdx.Register.GetSubregister(8, 8))
                        return;
                     
                    operations.Add(new BackwalkOperation(BackwalkOperator.and, 0xFF));
                    regIdx = frame.EnsureRegister(rIdx.Register.GetSubregister(0, 8));
                }
            }

            public void VisitCast(Cast cast)
            {
                throw new NotImplementedException();
            }

            public void VisitConditionOf(ConditionOf cof)
            {
                cof.Expression.Accept(this);
            }

            public void VisitConstant(Constant c)
            {
            }

            public void VisitDepositBits(DepositBits d)
            {
                throw new NotImplementedException();
            }

            public void VisitDereference(Dereference deref)
            {
                throw new NotImplementedException();
            }

            public void VisitFieldAccess(FieldAccess acc)
            {
                throw new NotImplementedException();
            }

            public void VisitIdentifier(Identifier id)
            {
            }

            public void VisitMemberPointerSelector(MemberPointerSelector mps)
            {
                throw new NotImplementedException();
            }

            public void VisitMemoryAccess(MemoryAccess access)
            {
                if (regDst == null)
                    return;
                var rDst = regDst.Storage as RegisterStorage;
                var rIdx = regIdx.Storage as RegisterStorage;
                if (rDst == null || rIdx == null)
                    return;
                if (rDst.Register != rIdx.Register.GetSubregister(0, 8))
                    return;
                var binEa = access.EffectiveAddress as BinaryExpression;
                if (binEa == null)
                    return;
                var eaBase = binEa.Left as Identifier;
                var eaOffset = binEa.Right as Constant;
                if (eaBase == null || eaOffset == null)
                    return;
                operations.Add(new BackwalkDereference(eaOffset.ToInt32(), 1));
                regIdx = eaBase;
           }

            public void VisitMkSequence(MkSequence seq)
            {
                throw new NotImplementedException();
            }

            public void VisitPhiFunction(PhiFunction phi)
            {
                throw new NotImplementedException();
            }

            public void VisitPointerAddition(PointerAddition pa)
            {
                throw new NotImplementedException();
            }

            public void VisitProcedureConstant(ProcedureConstant pc)
            {
                throw new NotImplementedException();
            }

            public void VisitScopeResolution(ScopeResolution scopeResolution)
            {
                throw new NotImplementedException();
            }

            public void VisitSegmentedAccess(SegmentedAccess access)
            {
            }

            public void VisitSlice(Slice slice)
            {
                throw new NotImplementedException();
            }

            public void VisitTestCondition(TestCondition tc)
            {
                throw new NotImplementedException();
            }

            public void VisitUnaryExpression(UnaryExpression unary)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        public static bool FindIndexRegister(MemoryAccess mem, out Identifier id, out int scale, out Constant tableOffset)
        {
            id = null;
            scale = 0;
            tableOffset = null;
            var bin = mem.EffectiveAddress as BinaryExpression;
            if (bin != null && bin.op == Operator.Add)
            {
                var i = bin.Left as Identifier;
                var b2 = bin.Left as BinaryExpression;
                tableOffset = bin.Right as Constant;
                if (tableOffset == null)
                {
                    i = bin.Right as Identifier;
                    b2 = bin.Right as BinaryExpression;
                    tableOffset = bin.Left as Constant;
                }
                if (i != null)
                {
                    id = i;
                    scale = 1;
                    return (tableOffset != null);
                }
                if (b2 != null)
                {
                    var i2 = b2.Left as Identifier;
                    var c2 = b2.Right as Constant;
                    if (i2 != null && c2 != null && b2.op is MulOperator)
                    {
                        id = i2;
                        scale = c2.ToInt32();
                        return tableOffset != null;
                    }
                }
            }
            return false;
        }

        public List<BackwalkOperation> Backwalk(Address addrCallJump, Block block)
        {
            var operations = new List<BackwalkOperation>();
            int i = FindLastCallJump(addrCallJump, block);
            if (i < 0)
                throw new InvalidOperationException(string.Format("Expected an indirect call or jump at address {0}.", addrCallJump));
            var mem = GetIndirectTarget(block.Statements[i].Instruction);
            if (mem == null)
                throw new InvalidOperationException(string.Format("Expected an indirect call or jump at address {0}.", addrCallJump));
            Identifier regIdx;
            int scale;
            Constant tableOffset;
            if (!FindIndexRegister(mem, out regIdx, out scale, out tableOffset))
                return null;
            if (scale > 1)
                operations.Add(new BackwalkOperation(BackwalkOperator.mul, scale));

            var w = new Visitor(regIdx, block.Procedure.Frame, operations);
            regIdx = w.BackwalkInstructions(block.Statements, i);
            if (regIdx == null)
                return null;

            if (!w.ReturnToCaller)
            {
                Block pred = GetSinglePredcessesor(block);
                if (pred == null)
                {
                    return null;	// seems unguarded to me.	//$REVIEW: emit warning.
                }

                regIdx = w.BackwalkInstructions(pred.Statements, pred.Statements.Count - 1);
                if (regIdx == null)
                {
                    return null;
                }
            }
            operations.Reverse();
            regIdxDetected = regIdx;
            return operations;
        }

        private Block GetSinglePredcessesor(Block block)
        {
            Block pred = null;
            foreach (Block p in block.Procedure.ControlGraph.Predecessors(block))
            {
                if (pred == null)
                    pred = p;
                else
                    return null;
            }
            return pred;
        }

        private MemoryAccess GetIndirectTarget(Instruction instruction)
        {
            var call = instruction as IndirectCall;
            if (call != null)
                return call.Callee as MemoryAccess;
            var computedJump = instruction as GotoInstruction;
            if (computedJump != null)
                return computedJump.Target as MemoryAccess;
            return null;
        }

        private int FindLastCallJump(Address addrCallJump, Block block)
        {
            int i;
            var stms = block.Statements;
            for (i = stms.Count - 1; i >= 0; --i)
            {
                var inst = stms[i].Instruction;
                if (inst is CallBase || inst is GotoInstruction)
                    break;
            }
            return i;
        }

        public Identifier IndexRegister
        {
            get { return regIdxDetected; }
        }

        public Address MakeAddress(PrimitiveType size, ImageReader rdr, ushort segBase)
        {
            if (arch.WordWidth == PrimitiveType.Word16)
            {
                if (size == PrimitiveType.Word16)
                {
                    return new Address(segBase, rdr.ReadLeUint16());
                }
                else
                {
                    ushort off = rdr.ReadLeUint16();
                    ushort seg = rdr.ReadLeUint16();
                    return new Address(seg, off);
                }
            }
            else if (arch.WordWidth == PrimitiveType.Word32)
            {
                Debug.Assert(segBase == 0);
                return new Address(rdr.ReadLeUint32());
            }
            else
                throw new ApplicationException("Unexpected word width: " + size);
        }
    }
}