#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Graphs;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Scanning
{
    /// <summary>
    /// Copies a basic block.
    /// </summary>
    public class BlockCloner :
        InstructionVisitor<Instruction>,
        ExpressionVisitor<Expression>,
        StorageVisitor<Identifier>
    {
        private readonly Block blockToClone;
        private readonly Procedure procCalling;
        private readonly CallGraph callGraph;
        private readonly Dictionary<Block, Block> mpBlocks;
        private readonly Dictionary<TemporaryStorage, Identifier> tmps;
        private DataType? dt;

        public BlockCloner(Block blockToClone, Procedure procCalling, CallGraph callGraph)
        {
            this.blockToClone = blockToClone;
            this.procCalling = procCalling;
            this.callGraph = callGraph;
            this.mpBlocks = new Dictionary<Block, Block>();
            this.tmps = new Dictionary<TemporaryStorage, Identifier>();
        }

        public Statement? Statement { get; set; }
        public Statement? StatementNew { get; set; }
        public Identifier? Identifier { get; set; }

        public Block? Execute()
        {
            return CloneBlock(blockToClone);
        }

        public Block? CloneBlock(Block blockOrig)
        {
            if (blockOrig == blockOrig.Procedure.ExitBlock)
                return null;

            if (mpBlocks.TryGetValue(blockOrig, out Block? blockNew))
            {
                return blockNew;
            }
            blockNew = new Block(procCalling, blockOrig.Address, blockOrig.Id + "_in_" + procCalling.Name);
            mpBlocks.Add(blockOrig, blockNew);
            var succ = blockOrig.Succ.Count > 0 ? CloneBlock(blockOrig.Succ[0]) : null;
            foreach (var stm in blockOrig.Statements)
            {
                Statement = stm;
                StatementNew = new Statement(
                    stm.Address,
                    null!,
                    blockNew);
                StatementNew.Instruction = stm.Instruction.Accept(this);
                blockNew.Statements.Add(StatementNew);
            }
            procCalling.AddBlock(blockNew);
            if (succ is null)
                procCalling.ControlGraph.AddEdge(blockNew, procCalling.ExitBlock);
            else
                procCalling.ControlGraph.AddEdge(blockNew, succ);
            return blockNew;
        }

        public Instruction VisitAssignment(Assignment ass)
        {
            var id = (Identifier) ass.Dst.Accept(this);
            var src = ass.Src.Accept(this);
            return new Assignment(id, src);
        }

        public Instruction VisitBranch(Branch branch)
        {
            //$TODO: this may not be necessary once scanner-development is done.
            return new SideEffect(Constant.String(string.Format("cloned {0}", branch), StringType.NullTerminated(PrimitiveType.Char)));
        }

        public Instruction VisitCallInstruction(CallInstruction ci)
        {
            var callee = ci.Callee.Accept(this);
            if (callee is ProcedureConstant pc)
            {
                callGraph.AddEdge(StatementNew!, pc.Procedure);
            }
            var ciNew = new CallInstruction(ci.Callee, new CallSite(ci.CallSite.SizeOfReturnAddressOnStack, ci.CallSite.FpuStackDepthBefore));
            return ciNew;  
        }

        public Instruction VisitComment(CodeComment comment)
        {
            return new CodeComment(comment.Text);
        }

        public Instruction VisitDefInstruction(DefInstruction def)
        {
            throw new NotImplementedException();
        }

        public Instruction VisitGotoInstruction(GotoInstruction gotoInstruction)
        {
            return new GotoInstruction(gotoInstruction.Target.Accept(this));
        }

        public Instruction VisitPhiAssignment(PhiAssignment phi)
        {
            throw new NotImplementedException();
        }

        public Instruction VisitReturnInstruction(ReturnInstruction ret)
        {
            var exp = ret.Expression?.Accept(this);
            return new ReturnInstruction(exp);
        }

        public Instruction VisitSideEffect(SideEffect side)
        {
            return new SideEffect(side.Expression.Accept(this));
        }

        public Instruction VisitStore(Store store)
        {
            var dst = store.Dst.Accept(this);
            var src = store.Src.Accept(this);
            return new Store(dst, src);
        }

        public Instruction VisitSwitchInstruction(SwitchInstruction si)
        {
            throw new NotImplementedException();
        }

        public Instruction VisitUseInstruction(UseInstruction use)
        {
            throw new NotImplementedException();
        }

        public Expression VisitAddress(Address addr)
        {
            return addr.CloneExpression();
        }

        public Expression VisitApplication(Application appl)
        {
            var proc = appl.Procedure.Accept(this);
            var args = appl.Arguments.Select(a => a.Accept(this)).ToArray();
            return new Application(proc, appl.DataType, args);
        }

        public Expression VisitArrayAccess(ArrayAccess acc)
        {
            throw new NotImplementedException();
        }

        public Expression VisitBinaryExpression(BinaryExpression binExp)
        {
            var left = binExp.Left.Accept(this);
            var right = binExp.Right.Accept(this);
            return new BinaryExpression(
                binExp.Operator,
                binExp.DataType,
                left, right);
        }

        public Expression VisitCast(Cast cast)
        {
            return new Cast(cast.DataType, cast.Expression.Accept(this));
        }

        public Expression VisitConditionalExpression(ConditionalExpression c)
        {
            return new ConditionalExpression(
                c.DataType,
                c.Condition.Accept(this),
                c.ThenExp.Accept(this),
                c.FalseExp.Accept(this));
        }

        public Expression VisitConditionOf(ConditionOf cof)
        {
            return new ConditionOf(cof.Expression.Accept(this));
        }

        public Expression VisitConstant(Constant c)
        {
            return c.CloneExpression();
        }

        public Expression VisitConversion(Conversion conversion)
        {
            return new Conversion(
                conversion.Expression.Accept(this),
                conversion.SourceDataType,
                conversion.DataType);
        }

        public Expression VisitDereference(Dereference deref)
        {
            throw new NotImplementedException();
        }

        public Expression VisitFieldAccess(FieldAccess acc)
        {
            throw new NotImplementedException();
        }

        public Expression VisitIdentifier(Identifier id)
        {
            this.Identifier = id;
            this.dt = id.DataType;
            return id.Storage.Accept(this);
        }

        public Expression VisitOutArgument(OutArgument outArg)
        {
            var exp = outArg.Expression.Accept(this);
            return new OutArgument(outArg.DataType, exp);
        }

        public Expression VisitMemberPointerSelector(MemberPointerSelector mps)
        {
            throw new NotImplementedException();
        }

        public Expression VisitMemoryAccess(MemoryAccess access)
        {
            var mem = (Identifier) access.MemoryId.Accept(this);
            var ea = access.EffectiveAddress.Accept(this);
            return new MemoryAccess(mem, ea, access.DataType);
        }

        public Expression VisitMkSequence(MkSequence seq)
        {
            var newSeq = seq.Expressions.Select(e => e.Accept(this)).ToArray();
            return new MkSequence(seq.DataType, newSeq);
        }

        public Expression VisitPhiFunction(PhiFunction phi)
        {
            throw new NotImplementedException();
        }

        public Expression VisitPointerAddition(PointerAddition pa)
        {
            throw new NotImplementedException();
        }

        public Expression VisitProcedureConstant(ProcedureConstant pc)
        {
            return new ProcedureConstant(pc.DataType, pc.Procedure);
        }

        public Expression VisitScopeResolution(ScopeResolution scopeResolution)
        {
            throw new NotImplementedException();
        }

        public Expression VisitSegmentedAddress(SegmentedPointer address)
        {
            return new SegmentedPointer(
                address.DataType,
                address.BasePointer.Accept(this),
                address.Offset.Accept(this));
        }

        public Expression VisitSlice(Slice slice)
        {
            return new Slice(
                slice.DataType,
                slice.Expression.Accept(this),
                slice.Offset);
        }

        public Expression VisitStringConstant(StringConstant str)
        {
            return str.CloneExpression();
        }

        public Expression VisitTestCondition(TestCondition tc)
        {
            return new TestCondition(
                tc.ConditionCode,
                tc.Expression.Accept(this));
        }

        public Expression VisitUnaryExpression(UnaryExpression unary)
        {
            return new UnaryExpression(
                unary.Operator,
                unary.DataType,
                unary.Expression.Accept(this));
        }

        public Identifier VisitFlagGroupStorage(FlagGroupStorage grf)
        {
            return procCalling.Frame.EnsureFlagGroup(grf);
        }

        public Identifier VisitFpuStackStorage(FpuStackStorage fpu)
        {
            throw new NotImplementedException();
        }

        public Identifier VisitMemoryStorage(MemoryStorage global)
        {
            return procCalling.Frame.Memory;
        }

        public Identifier VisitRegisterStorage(RegisterStorage reg)
        {
            return procCalling.Frame.EnsureRegister(reg);
        }

        public Identifier VisitSequenceStorage(SequenceStorage seq)
        {
            var dt = this.dt!;
            var clones = seq.Elements.Select(e => e.Accept(this).Storage);
            return procCalling.Frame.EnsureSequence(dt, clones.ToArray());
        }

        public Identifier VisitStackStorage(StackStorage stack)
        {
            throw new NotImplementedException();
        }

        public Identifier VisitTemporaryStorage(TemporaryStorage temp)
        {
            if (!tmps.TryGetValue(temp, out var id))
            {
                id = procCalling.Frame.CreateTemporary(temp.DataType);
                tmps.Add(temp, id);
            }
            return id;
        }
    }
}
