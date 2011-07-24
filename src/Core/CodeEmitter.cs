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
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Core
{
	/// <summary>
	/// Emits code sequences into basic blocks.
	/// </summary>
    /// <remarks>Only used by the old x86 rewriting code. When that is obsoleted, this class may be deleted.</remarks>
    [Obsolete("Don't use this class anymore; RtlEmitter or ProcedureBuilder are your friends.")]
	public class CodeEmitterOld : CodeEmitter
	{
        private IProcessorArchitecture arch;
		private IRewriterHostOld host;
		private Procedure proc;

		private Block blockCur;

		public CodeEmitterOld(IProcessorArchitecture arch, IRewriterHostOld host, Procedure proc, Block block)
		{
            this.arch = arch;
			this.host = host;
			this.proc = proc;
            this.blockCur = block;
		}

		public Block Block
		{
			get { return blockCur; }
		}

        public override Frame Frame
        {
            get { return proc.Frame; } 
        }

        public override Identifier Register(int i)
        {
            return Frame.EnsureRegister(arch.GetRegister(i));
        }

		/// <summary>
		/// Appends the instruction to the end of the list of intstructions of the current block.
		/// </summary>
		/// <param name="instr"></param>
		public override Statement Emit(Instruction instr)
		{
			blockCur.Statements.Add(0, instr);
            return blockCur.Statements.Last;
		}

        public void Call(Procedure procCallee, CallSite site)
        {
            ProcedureConstant pc = new ProcedureConstant(arch.PointerType, procCallee);
            Emit(new CallInstruction(pc, site));
        }



        public UnaryExpression Neg(Expression expr)
        {
            return new UnaryExpression(Operator.Neg, expr.DataType, expr);
        }

		public Expression PseudoProc(PseudoProcedure ppp, PrimitiveType retType, params Expression [] args)
		{
            if (args.Length != ppp.Arity)
                throw new ArgumentOutOfRangeException(
                    string.Format("Pseudoprocedure {0} expected {1} arguments, but was passed {2}.",
                    ppp.Name,
                    ppp.Arity,
                    args.Length));
            
			return new Application(
                new ProcedureConstant(arch.PointerType, ppp),
                retType, 
                args);
		}

		public Expression PseudoProc(string name, PrimitiveType retType, params Expression [] args)
		{
			PseudoProcedure ppp = host.EnsurePseudoProcedure(name, retType, args.Length);
			return PseudoProc(ppp, retType, args);
		}



        /// <summary>
        /// Emits a call to a pseudoprocedure as a side effect, implying the pseudoprocedure is a void function.
        /// </summary>
        /// <param name="ppp"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public SideEffect SideEffect(PseudoProcedure ppp, params Expression[] args)
        {
            SideEffect s = new SideEffect(PseudoProc(ppp, null, args));
            Emit(s);
            return s;
        }

        /// <summary>
        /// Emits a call to a named pseudoprocedure as a side effect, implying the pseudoprocedure is a void function.
        /// </summary>
        /// <param name="ppp"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public SideEffect SideEffect(string name, params Expression[] args)
        {
            PseudoProcedure ppp = host.EnsurePseudoProcedure(name, PrimitiveType.Void, args.Length);
            return SideEffect(ppp, args);
        }

        public SideEffect SideEffect(Application appl)
        {
            SideEffect s = new SideEffect(appl);
            Emit(s);
            return s;
        }

		public Store Store(MemoryAccess dst, Expression src)
		{
			Store s = new Store(dst, src);
			Emit(s);
			return s;
		}

        public void Switch(Expression expr, Block[] jumps)
        {
            Emit(new SwitchInstruction(expr, jumps));
        }

        public IndirectCall IndirectCall(Expression e, CallSite callSite)
        {
            IndirectCall i = new IndirectCall(e, callSite);
            Emit(i);
            return i;
        }
    }

    public abstract class CodeEmitter : ExpressionEmitter
    {
        private int localStackOffset;

        public abstract Statement Emit(Instruction instr);

        public virtual Assignment Assign(Identifier dst, Expression src)
        {
            var ass = new Assignment(dst, src);
            Emit(ass);
            return ass;
        }

        public virtual Assignment Assign(Identifier dst, int n)
        {
            var ass = new Assignment(dst, new Constant(dst.DataType, n));
            Emit(ass);
            return ass;
        }

        public virtual Assignment Assign(Identifier dst, bool f)
        {
            return Assign(dst, new Constant(PrimitiveType.Bool, f ? 1 : 0));
        }

        public Branch Branch(Expression condition, Block target)
        {
            Branch b = new Branch(condition, target);
            Emit(b);
            return b;
        }

        public Identifier Flags(uint grf, string name)
        {
            return Frame.EnsureFlagGroup(grf, name, PrimitiveType.Byte);
        }

        public abstract Frame Frame { get; }


        public GotoInstruction Goto(Expression dest)
        {
            var gi = new GotoInstruction(dest);
            Emit(gi);
            return gi;
        }

        public GotoInstruction Goto(uint linearAddress)
        {
            var gi = new GotoInstruction(new Address(linearAddress));
            Emit(gi);
            return gi;
        }

        public GotoInstruction IfGoto(Expression condition, Address addr)
        {
            var gi = new GotoInstruction(condition, addr);
            Emit(gi);
            return gi;
        }

        public GotoInstruction IfGoto(Expression condition, uint linearAddress)
        {
            var gi = new GotoInstruction(condition,
                new Constant(PrimitiveType.Word32, linearAddress));
            Emit(gi);
            return gi;
        }

        public void Load(Identifier reg, Expression ea)
        {
            Assign(reg, new MemoryAccess(MemoryIdentifier.GlobalMemory, ea, reg.DataType));
        }

        public Statement Phi(Identifier idDst, params Identifier[] ids)
        {
            return Emit(new PhiAssignment(idDst, new PhiFunction(idDst.DataType, ids)));
        }

        public abstract Identifier Register(int i);

        public void Return()
        {
            Return(null);
        }

        //public virtual void Return(Expression exp)
        //{
        //    Emit(new ReturnInstruction(exp));
        //}

        public virtual void Return(Expression rv)
        {
            Emit(new ReturnInstruction(rv));
        }

        public Statement SideEffect(Expression side)
        {
            return Emit(new SideEffect(side));
        }

        public Statement Store(Expression ea, int n)
        {
            return Store(ea, Int32(n));
        }

        public Statement Store(Expression ea, Expression expr)
        {
            Store s = new Store(new MemoryAccess(MemoryIdentifier.GlobalMemory, ea, expr.DataType), expr);
            return Emit(s);
        }

        public Statement SegStoreW(Expression basePtr, Expression ea, Expression src)
        {
            Store s = new Store(new SegmentedAccess(MemoryIdentifier.GlobalMemory, basePtr, ea, PrimitiveType.Word16), src);
            return Emit(s);
        }

        public Statement Store(SegmentedAccess s, Expression exp)
        {
            return Emit(new Store(s, exp));
        }

        public Statement Sub(Identifier diff, Expression left, Expression right)
        {
            return Emit(new Assignment(diff, Sub(left, right)));
        }

        public Statement Sub(Identifier diff, Expression left, int right)
        {
            return Sub(diff, left, Int32(right));
        }


        public Identifier Local(PrimitiveType primitiveType, string name)
        {
            localStackOffset -= primitiveType.Size;
            return Frame.EnsureStackLocal(localStackOffset, primitiveType, name);
        }

        public Identifier LocalBool(string name)
        {
            localStackOffset -= PrimitiveType.Word32.Size;
            return Frame.EnsureStackLocal(localStackOffset, PrimitiveType.Bool, name);
        }

        public Identifier LocalByte(string name)
        {
            localStackOffset -= PrimitiveType.Word32.Size;
            return Frame.EnsureStackLocal(localStackOffset, PrimitiveType.Byte, name);
        }

        public Identifier Local16(string name)
        {
            localStackOffset -= PrimitiveType.Word32.Size;
            return Frame.EnsureStackLocal(localStackOffset, PrimitiveType.Word16, name);
        }

        public Identifier Local32(string name)
        {
            localStackOffset -= PrimitiveType.Word32.Size;
            return Frame.EnsureStackLocal(localStackOffset, PrimitiveType.Word32, name);
        }

        public Statement Use(Identifier id)
        {
            return Emit(new UseInstruction(id));
        }

    }
}
