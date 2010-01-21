/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Core
{
	/// <summary>
	/// Emits code sequences into basic blocks.
	/// </summary>
	public class CodeEmitter
	{
        private IProcessorArchitecture arch;
		private IRewriterHost host;
		private Procedure proc;

		private Block blockCur;

		public CodeEmitter(IProcessorArchitecture arch, IRewriterHost host, Procedure proc, Block block)
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

        public Branch Branch(Expression condition, Block target)
        {
            Branch b = new Branch(condition, target);
            Emit(b);
            return b;
        }

		/// <summary>
		/// Appends the instruction to the end of the list of intstructions of the current block.
		/// </summary>
		/// <param name="instr"></param>
		public void Emit(Instruction instr)
		{
			blockCur.Statements.Add(instr);
		}

        public BinaryExpression Add(Expression left, Expression right)
        {
            return new BinaryExpression(Operator.add, left.DataType, left, right);
        }

		public BinaryExpression Add(Expression left, int right)
		{
			return new BinaryExpression(Operator.add, left.DataType, left, new Constant(left.DataType, right));
		}

        public BinaryExpression And(Expression left, Expression right)
        {
            return new BinaryExpression(Operator.and, left.DataType, left, right);
        }

        public void Call(Procedure procCallee, CallSite site)
        {
            ProcedureConstant pc = new ProcedureConstant(arch.PointerType, procCallee);
            Emit(new CallInstruction(pc, site));
        }

        public Cast Cast(DataType dataType, Expression expr)
        {
            return new Cast(dataType, expr);
        }

        public Constant Constant(DataType dataType, int n)
        {
            return new Constant(dataType, n);
        }


        public UnaryExpression Neg(Expression expr)
        {
            return new UnaryExpression(Operator.neg, expr.DataType, expr);
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


		public Assignment Assign(Identifier dst, Expression opSrc)
		{
			Assignment ass = new Assignment(dst, opSrc);
			Emit(ass);
			return ass;
		}

		public BinaryExpression Eq0(Expression exp)
		{
			return new BinaryExpression(
				Operator.eq, PrimitiveType.Bool, exp, new Constant(exp.DataType, 0));
		}

        public BinaryExpression Ne0(Expression expr)
        {
            return new BinaryExpression(
                Operator.ne, PrimitiveType.Bool, expr, new Constant(expr.DataType, 0));
        }

        public UnaryExpression Not(Expression expr)
        {
            return new UnaryExpression(Operator.not, PrimitiveType.Bool, expr);
        }


		public ReturnInstruction Return()
		{
			return Return(null);
		}

		public ReturnInstruction Return(Expression rv)
		{
			ReturnInstruction r = new ReturnInstruction(rv);
			Emit(r);
			return r;
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

        public BinaryExpression Sub(Expression left, Expression right)
        {
            return new BinaryExpression(Operator.sub, left.DataType, left, right);
        }

		public BinaryExpression Sub(Expression left, int right)
		{
			return new BinaryExpression(Operator.sub, left.DataType, left, new Constant(left.DataType, right));
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
}
