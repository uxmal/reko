#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Scanning
{
    /// <summary>
    /// This class builds up state in the provided <see cref="Program"/> instance.
    /// It converts zero or more <see cref="RtlProcedure"/>s to their corresponding
    /// <see cref="Procedure"/>s, and builds up the program's <see cref="CallGraph"/>.
    /// </summary>
    public class ProcedureGraphBuilder :
        RtlInstructionVisitor<Instruction, Block>, 
        ExpressionVisitor<Expression, Procedure>
    {
        private readonly ScanResultsV2 sr;
        private readonly Program program;
        private readonly Dictionary<Address, Block> blocksByAddress;

        public ProcedureGraphBuilder(ScanResultsV2 sr, Program program)
        {
            this.sr = sr;
            this.program = program;
            this.blocksByAddress = new Dictionary<Address, Block>();
        }

        public void Build(IEnumerable<RtlProcedure> rtlProcedures)
        {
            var rtlProcDict = rtlProcedures.ToDictionary(e => e.Address);
            foreach (var rtlProcedure in rtlProcedures.OrderBy(e => e.Address))
            {
                var proc = CreateEmptyProcedure(rtlProcedure);
                program.Procedures.Add(proc.EntryAddress, proc);
                program.CallGraph.AddProcedure(proc);
            }
            foreach (var rtlProc in rtlProcDict)
            {
                BuildProcedure(rtlProc.Value, program.Procedures[rtlProc.Key]);
            }
        }

        private void BuildProcedure(RtlProcedure rtlProc, Procedure proc)
        {
            var blockMap = new Dictionary<RtlBlock, Block>();
            foreach (var rtlBlock in rtlProc.Blocks)
            {
                var name = program.NamingPolicy.BlockName(rtlBlock.Address);
                var block = new Block(proc, rtlBlock.Address, name);
                proc.AddBlock(block);
                blockMap.Add(rtlBlock, block);
                blocksByAddress.Add(block.Address, block);
                var imageItem = new ImageMapBlock(block.Address)
                {
                    Block = block,
                    Size = (uint) (rtlBlock.FallThrough - rtlBlock.Address)
                };
                program.ImageMap.AddItem(block.Address, imageItem);
            }
            InjectProcedureEntryInstructions(proc.EntryAddress, proc);

            proc.ControlGraph.AddEdge(proc.EntryBlock, blocksByAddress[rtlProc.Address]);
            foreach (var rtlBlock in rtlProc.Blocks)
            {
                var block = blockMap[rtlBlock];
                BuildBlock(rtlBlock, block);
                if (sr.Successors.TryGetValue(block.Address, out var succ))
                {
                    foreach (var s in succ)
                    {
                        var nextBlock = this.blocksByAddress[s];
                        proc.ControlGraph.AddEdge(block, nextBlock);
                    }
                }
            }
        }

        /// <summary>
        /// Inject statements into the entry block that establish the frame,
        /// and if the procedure has been given a valid signature already,
        /// copy the input arguments into their local counterparts.
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="proc"></param>
        public void InjectProcedureEntryInstructions(Address addr, Procedure proc)
        {
            var bb = new StatementInjector(proc, proc.EntryBlock, addr);
            var sp = proc.Frame.EnsureRegister(proc.Architecture.StackRegister);
            bb.Assign(sp, proc.Frame.FramePointer);
            program.Platform.InjectProcedureEntryStatements(proc, addr, bb);
        }

        private void BuildBlock(RtlBlock rtlBlock, Block block)
        {
            foreach (var rtlCluster in rtlBlock.Instructions)
            {
                foreach (var rtl in rtlCluster.Instructions)
                {
                    var instr = rtl.Accept(this, block);
                    if (instr is null)
                        continue;
                    var stm = block.Statements.Add(rtlCluster.Address, instr);
                    if (instr is CallInstruction call)
                    {
                        AddEdgeToCallGraph(stm, call);
                    }
                }
            }
        }

        private void AddEdgeToCallGraph(Statement stm, CallInstruction call)
        {
            if (call.Callee is ProcedureConstant pc && 
                pc.Procedure is Procedure proc)
            {
                program.CallGraph.AddEdge(stm, proc);
            }
        }

        private Procedure CreateEmptyProcedure(RtlProcedure rtlProc)
        {
            var addr = rtlProc.Address;
            var arch = rtlProc.Architecture;
            var name = program.NamingPolicy.ProcedureName(addr);
            var proc = Procedure.Create(arch, name, addr, arch.CreateFrame());
            return proc;
        }

        public Instruction VisitAssignment(RtlAssignment ass, Block ctx)
        {
            var src = ass.Src.Accept(this, ctx.Procedure);
            var dst = ass.Dst.Accept(this, ctx.Procedure);
            if (dst is Identifier id)
                return new Assignment(id, src);
            else
                return new Store(dst, src);
        }

        public Instruction VisitBranch(RtlBranch branch, Block ctx)
        {
            var condition = branch.Condition.Accept(this, ctx.Procedure);
            var succ = sr.Successors[ctx.Address];
            var thenBlock = this.blocksByAddress[succ[1]];
            return new Branch(condition, thenBlock);
        }

        public Instruction VisitCall(RtlCall call, Block ctx)
        {
            var target = call.Target.Accept(this, ctx.Procedure);
            if (target is Address addr)
            {
                var procCallee = program.Procedures[addr];
                var pc = new ProcedureConstant(
                    ctx.Procedure.Architecture.PointerType,
                    procCallee);
                target = pc;
            }
            var site = new CallSite(call.ReturnAddressSize, 0);
            return new CallInstruction(target, site);
        }

        public Instruction VisitGoto(RtlGoto go, Block ctx)
        {
            return new GotoInstruction(go.Target.Accept(this, ctx.Procedure));
        }

        public Instruction VisitIf(RtlIf rtlIf, Block ctx)
        {
            throw new NotImplementedException();
        }

        public Instruction VisitInvalid(RtlInvalid invalid, Block ctx)
        {
            throw new NotImplementedException();
        }

        public Instruction VisitMicroGoto(RtlMicroGoto uGoto, Block ctx)
        {
            throw new NotImplementedException();
        }

        public Instruction VisitMicroLabel(RtlMicroLabel uLabel, Block ctx)
        {
            throw new NotImplementedException();
        }

        public Instruction VisitNop(RtlNop rtlNop, Block ctx)
        {
            return null!;
        }

        public Instruction VisitReturn(RtlReturn ret, Block ctx)
        {
            var proc = ctx.Procedure;
            ctx.Procedure.ControlGraph.AddEdge(ctx, proc.ExitBlock);
            return new ReturnInstruction();
        }

        public Instruction VisitSideEffect(RtlSideEffect side, Block ctx)
        {
            var exp = side.Expression.Accept(this, ctx.Procedure);
            return new SideEffect(exp);
        }

        public Instruction VisitSwitch(RtlSwitch sw, Block ctx)
        {
            throw new NotImplementedException();
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitAddress(Address addr, Procedure ctx)
        {
            return addr;
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitApplication(Application appl, Procedure ctx)
        {
            var cArgs = appl.Arguments.Length;
            var args = new Expression[cArgs];
            var proc = appl.Procedure.Accept(this, ctx);
            for (int i =0; i < cArgs; ++i)
            {
                args[i] = appl.Arguments[i].Accept(this, ctx);
            }
            return new Application(proc, appl.DataType, args);
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitArrayAccess(ArrayAccess acc, Procedure ctx)
        {
            throw new NotImplementedException();
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitBinaryExpression(BinaryExpression binExp, Procedure ctx)
        {
            var left = binExp.Left.Accept(this, ctx);
            var right = binExp.Right.Accept(this, ctx);
            return new BinaryExpression(
                binExp.Operator,
                binExp.DataType,
                left,
                right);
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitCast(Cast cast, Procedure ctx)
        {
            throw new NotImplementedException();
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitConditionalExpression(ConditionalExpression c, Procedure context)
        {
            var exp = c.Condition.Accept(this, context);
            var th = c.Condition.Accept(this, context);
            var el = c.Condition.Accept(this, context);
            return new ConditionalExpression(c.DataType, exp, th, el);
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitConditionOf(ConditionOf cof, Procedure ctx)
        {
            var exp = cof.Expression.Accept(this, ctx);
            return new ConditionOf(exp);
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitConstant(Constant c, Procedure ctx)
        {
            return c;
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitConversion(Conversion conversion, Procedure context)
        {
            var exp = conversion.Expression.Accept(this, context);
            return new Conversion(exp, conversion.SourceDataType, conversion.DataType);
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitDereference(Dereference deref, Procedure ctx)
        {
            throw new NotImplementedException();
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitFieldAccess(FieldAccess acc, Procedure ctx)
        {
            throw new NotImplementedException();
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitIdentifier(Identifier id, Procedure ctx)
        {
            var frame = ctx.Frame;
            //$TODO: changed name?
            if (id.Storage is TemporaryStorage tmp)
            {
                return frame.CreateTemporary(tmp.DataType);
            }
            return frame.EnsureIdentifier(id.Storage);
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitMemberPointerSelector(MemberPointerSelector mps, Procedure ctx)
        {
            throw new NotImplementedException();
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitMemoryAccess(MemoryAccess access, Procedure ctx)
        {
            var ea = access.EffectiveAddress.Accept(this, ctx);
            return new MemoryAccess(access.MemoryId, ea, access.DataType);
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitMkSequence(MkSequence seq, Procedure ctx)
        {
            throw new NotImplementedException();
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitOutArgument(OutArgument outArgument, Procedure ctx)
        {
            throw new NotImplementedException();
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitPhiFunction(PhiFunction phi, Procedure ctx)
        {
            throw new NotImplementedException();
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitPointerAddition(PointerAddition pa, Procedure ctx)
        {
            throw new NotImplementedException();
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitProcedureConstant(ProcedureConstant pc, Procedure ctx)
        {
            return pc;
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitScopeResolution(ScopeResolution scopeResolution, Procedure ctx)
        {
            throw new NotImplementedException();
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitSegmentedAccess(SegmentedAccess access, Procedure ctx)
        {
            throw new NotImplementedException();
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitSlice(Slice slice, Procedure ctx)
        {
            var exp = slice.Expression.Accept(this, ctx);
            return new Slice(slice.DataType, exp, slice.Offset);
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitTestCondition(TestCondition tc, Procedure ctx)
        {
            var exp = tc.Expression.Accept(this, ctx);
            return new TestCondition(tc.ConditionCode, exp);
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitUnaryExpression(UnaryExpression unary, Procedure ctx)
        {
            var exp = unary.Expression.Accept(this, ctx);
            return new UnaryExpression(unary.Operator, unary.DataType, exp);
        }

        private class StatementInjector : CodeEmitter
        {
            private readonly Address addr;
            private readonly Block block;
            private readonly Procedure proc;
            private int iStm;

            public StatementInjector(Procedure proc, Block block, Address addr)
            {
                this.proc = proc;
                this.addr = addr;
                this.block = block;
                this.iStm = 0;
            }

            public override Frame Frame => proc.Frame;

            public override Statement Emit(Instruction instr)
            {
                var stm = block.Statements.Insert(iStm, addr, instr);
                ++iStm;
                return stm;
            }
        }

    }
}
