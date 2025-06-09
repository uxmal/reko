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
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Graphs;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Scanning
{
    /// <summary>
    /// This class builds up state in the provided <see cref="Program"/> instance.
    /// It converts zero or more <see cref="RtlProcedure"/>s to their corresponding
    /// <see cref="Procedure"/>s, and builds up the program's <see cref="CallGraph"/>.
    /// </summary>
    public class ProcedureGraphBuilder :
        IRtlInstructionVisitor<Instruction, ProcedureGraphBuilder.Context>, 
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
                var ctx = new Context(block, rtlCluster);
                foreach (var rtl in rtlCluster.Instructions)
                {
                    var instr = rtl.Accept(this, ctx);
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
            if (call.Callee is ProcedureConstant pc)
            {
                program.CallGraph.AddEdge(stm, pc.Procedure);
            }
        }

        private Procedure CreateEmptyProcedure(RtlProcedure rtlProc)
        {
            var addr = rtlProc.Address;
            var arch = rtlProc.Architecture;
            if (program.User.Procedures.TryGetValue(addr, out var userProc))
            {
                var proc = CreateEmptyProcedure(arch, addr, userProc.Name, userProc.Signature, userProc.Characteristics);
                return proc;
            }
            if (program.ImageSymbols.TryGetValue(addr, out var sym))
            {
                var proc = CreateEmptyProcedure(sym.Architecture ?? arch, addr, sym.Name, sym.Signature, null);
                return proc;
            }
            else
            {
                var name = program.NamingPolicy.ProcedureName(addr);
                var proc = Procedure.Create(arch, name, addr, arch.CreateFrame());
                return proc;
            }
        }


        private Procedure CreateEmptyProcedure(
            IProcessorArchitecture arch,
            Address addr,
            string? name,
            SerializedSignature? ssig,
            ProcedureCharacteristics? characteristics)
        {
            var proc = Procedure.Create(arch, name, addr, arch.CreateFrame());
            if (ssig is not null)
            {
                var sser = program.CreateProcedureSerializer();
                var sig = sser.Deserialize(ssig, proc.Frame);
                if (sig is not null)
                {
                    proc.Signature = sig;
                }
                proc.EnclosingType = ssig.EnclosingType;
            }
            else if (!string.IsNullOrEmpty(name))
            {
                var sProc = program.Platform.SignatureFromName(name);
                if (sProc is not null)
                {
                    var loader = program.CreateTypeLibraryDeserializer();
                    var exp = loader.LoadExternalProcedure(sProc);
                    if (exp is not null)
                    {
                        proc.Name = exp!.Name;
                        proc.Signature = exp.Signature;
                        proc.EnclosingType = exp.EnclosingType;
                    }
                }
                else
                {
                    proc.Name = name;
                }
            }
            return proc;
        }

 
        public Instruction VisitAssignment(RtlAssignment ass, Context ctx)
        {
            var src = ass.Src.Accept(this, ctx.Procedure);
            var dst = ass.Dst.Accept(this, ctx.Procedure);
            if (dst is Identifier id)
                return new Assignment(id, src);
            else
                return new Store(dst, src);
        }

        public Instruction VisitBranch(RtlBranch branch, Context ctx)
        {
            var condition = branch.Condition.Accept(this, ctx.Procedure);
            var target = (Address) branch.Target;
            var thenBlock = this.blocksByAddress[target];
            return new Branch(condition, thenBlock);
        }

        public Instruction VisitCall(RtlCall call, Context ctx)
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

        public Instruction VisitGoto(RtlGoto go, Context ctx)
        {
            if (go.Target is Address addr)
            {
                if (program.Procedures.TryGetValue(addr, out var procCallee))
                {
                    var state = ctx.Procedure.Architecture.CreateProcessorState();
                    var thunk = CreateCallRetThunk(ctx.Cluster.Address, ctx.Procedure, state, procCallee);
                    ctx.Procedure.ControlGraph.AddEdge(ctx.Block, thunk);
                    return null!;
                }
            }
            return new GotoInstruction(go.Target.Accept(this, ctx.Procedure));
        }

        /// <summary>
        /// Creates a small basic block, consisting solely of a 'call' followed by a 'return'
        /// instruction.
        /// </summary>
        /// <remarks>
        /// This is done when encountering tail calls (i.e. jumps) from one 
        /// procedure into another.
        /// </remarks>
        /// <param name="addrFrom"></param>
        /// <param name="procOld"></param>
        /// <param name="procNew"></param>
        /// <returns></returns>
        public Block CreateCallRetThunk(Address addrFrom, Procedure procOld, ProcessorState state, Procedure procNew)
        {
            //$BUG: ReturnAddressOnStack property needs to be properly set, the
            // EvenOdd sample shows how this doesn't work currently. 
            var blockName = string.Format(
                "{0}_thunk_{1}",
                program.NamingPolicy.BlockName(addrFrom),
                procNew.Name);
            var callRetThunkBlock = procOld.AddSyntheticBlock(
                addrFrom,
                blockName);
            if (program.User.BlockLabels.TryGetValue(blockName, out var userLabel))
                callRetThunkBlock.UserLabel = userLabel;

            var stmLast = callRetThunkBlock.Statements.Add(
                addrFrom,
                new CallInstruction(
                    new ProcedureConstant(program.Platform.PointerType, procNew),
                    new CallSite(0, 0)));
            //$TODO: look at what is being done in Scanner.CreateCallRetThunk.
            program.CallGraph.AddEdge(stmLast, procNew);

            callRetThunkBlock.Statements.Add(addrFrom, new ReturnInstruction());
            procOld.ControlGraph.AddEdge(callRetThunkBlock, procOld.ExitBlock);
            //$NYI: stack deltas and return addresses on stack.
            //if (procNew.Frame.ReturnAddressKnown)
            //{
            //    SetProcedureReturnAddressBytes(
            //        procOld, procNew.Frame.ReturnAddressSize, addrFrom);
            //}
            //SetProcedureStackDelta(procOld, procNew.Signature.StackDelta, addrFrom);
            return callRetThunkBlock;
        }


        public Instruction VisitIf(RtlIf rtlIf, Context ctx)
        {
            throw new NotImplementedException();
        }

        public Instruction VisitInvalid(RtlInvalid invalid, Context ctx)
        {
            throw new NotImplementedException();
        }

        public Instruction VisitMicroGoto(RtlMicroGoto uGoto, Context ctx)
        {
            throw new NotImplementedException();
        }

        public Instruction VisitNop(RtlNop rtlNop, Context ctx)
        {
            return null!;
        }

        public Instruction VisitReturn(RtlReturn ret, Context ctx)
        {
            var proc = ctx.Procedure;
            ctx.Procedure.ControlGraph.AddEdge(ctx.Block, proc.ExitBlock);
            return new ReturnInstruction();
        }

        public Instruction VisitSideEffect(RtlSideEffect side, Context ctx)
        {
            var exp = side.Expression.Accept(this, ctx.Procedure);
            return new SideEffect(exp);
        }

        public Instruction VisitSwitch(RtlSwitch sw, Context ctx)
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

        Expression ExpressionVisitor<Expression, Procedure>.VisitSegmentedAddress(SegmentedPointer addr, Procedure ctx)
        {
            var selector = addr.BasePointer.Accept(this, ctx);
            var ea = addr.Offset.Accept(this, ctx);
            return new SegmentedPointer(addr.DataType, selector, ea);
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitSlice(Slice slice, Procedure ctx)
        {
            var exp = slice.Expression.Accept(this, ctx);
            return new Slice(slice.DataType, exp, slice.Offset);
        }

        Expression ExpressionVisitor<Expression, Procedure>.VisitStringConstant(StringConstant s, Procedure ctx)
        {
            return s;
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

        public struct Context
        {
            public Context(Block block, RtlInstructionCluster cluster)
            {
                Block = block;
                Cluster = cluster;
            }

            public Block Block { get; }

            public RtlInstructionCluster Cluster { get; }

            public Procedure Procedure => Block.Procedure;
        }

    }
}
