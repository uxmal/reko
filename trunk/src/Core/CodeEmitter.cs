/* 
 * Copyright (C) 1999-2008 John Källén.
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
		private Program prog;
		private Procedure proc;

		private Block blockCur;

		public CodeEmitter(Program prog, Procedure proc)
		{
			this.prog = prog;
			this.proc = proc;
		}

		public Block Block
		{
			get { return blockCur; }
			set { blockCur = value; }
		}

		/// <summary>
		/// Appends the instruction to the end of the list of intstructions of the current block.
		/// </summary>
		/// <param name="instr"></param>
		public void Emit(Instruction instr)
		{
			blockCur.Statements.Add(instr);
		}

		public SideEffect SideEffect(PseudoProcedure ppp, params Expression [] args)
		{
			SideEffect s = new SideEffect(PseudoProc(ppp, null, args));
			Emit(s);
			return s;
		}

		public SideEffect SideEffect(string name, params Expression [] args)
		{
			PseudoProcedure ppp = prog.EnsurePseudoProcedure(name, args.Length);
			return SideEffect(ppp, args);
		}

		public BinaryExpression Add(Expression left, int right)
		{
			return new BinaryExpression(Operator.add, left.DataType, left, new Constant(left.DataType, right));
		}

		public Expression PseudoProc(PseudoProcedure ppp, PrimitiveType retType, params Expression [] args)
		{
			if (args.Length != ppp.Arity)
				throw new ArgumentOutOfRangeException("Must pass correct # of arguments");

			return new Application(new ProcedureConstant(PrimitiveType.Pointer, ppp), retType, args);
		}


		public Expression PseudoProc(string name, PrimitiveType retType, params Expression [] args)
		{
			PseudoProcedure ppp = prog.EnsurePseudoProcedure(name, args.Length);
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
				Operator.eq, exp.DataType, exp, new Constant(exp.DataType, 0));
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

		public Store Store(Expression dst, Expression src)
		{
			Store s = new Store(dst, src);
			Emit(s);
			return s;
		}

		public BinaryExpression Sub(Expression left, int right)
		{
			return new BinaryExpression(Operator.sub, left.DataType, left, new Constant(left.DataType, right));
		}
	}
}
