#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Analysis
{
	/// <summary>
	/// Transforms out parameters to be pointers, and changes references of the type
	///		regOut  = foo
	/// to
	///		(*regOut) = foo
	/// </summary>
	public class OutParameterTransformer : InstructionTransformer		//$REFACTOR: rename to OutArgumentTransformer.
	{
		private Procedure proc;
		private SsaIdentifierCollection ssaIds;

		private Identifier idOut;
		private SsaIdentifier ssa;
		private Statement stmDef;
		private int iStmDef;
		private WorkList<Identifier> wl;

		public OutParameterTransformer(Procedure proc, SsaIdentifierCollection ssaIds)
		{
			this.proc = proc;
			this.ssaIds = ssaIds;
		}

		public void ReplaceDefinitionsWithOutParameter(Identifier id, Identifier idOut)
		{
			this.idOut = idOut;
			wl = new WorkList<Identifier>();
			wl.Add(id);
			var visited = new HashSet<Statement>();

			while (wl.GetWorkItem(out id))
			{
				ssa = ssaIds[id];
				stmDef = ssa.DefStatement;
				if (stmDef != null && !visited.Contains(stmDef))
				{
					visited.Add(stmDef);
					iStmDef = stmDef.Block.Statements.IndexOf(stmDef);
					stmDef.Instruction = stmDef.Instruction.Accept(this);
				}
			}
		}

		public void Transform()
		{
			for (int i = proc.ExitBlock.Statements.Count - 1; i >= 0; --i)
			{
				Statement stm  = proc.ExitBlock.Statements[i];
				UseInstruction use = stm.Instruction as UseInstruction;
				if (use != null)
				{
					Identifier id = (Identifier) use.Expression;
					ssaIds[id].Uses.Remove(stm);
					ReplaceDefinitionsWithOutParameter(id, use.OutArgument);
					proc.ExitBlock.Statements.RemoveAt(i);
				}
			}
		}

		public override Instruction TransformAssignment(Assignment a)
		{
			a.Src = a.Src.Accept(this);
			Identifier id = a.Dst;
			if (ssa.Identifier == id)
			{
				if (ssa.Uses.Count == 0)
				{
					return new Store(Dereference(idOut, a.Src.DataType), a.Src);
				}
				else
				{
					var stm = stmDef.Block.Statements.Insert(
                        iStmDef + 1,
                        stmDef.LinearAddress,
                        new Store(Dereference(idOut, a.Dst.DataType), a.Dst));
                    ssa.Uses.Add(stm);
				}
			}
			return a;
		}

		public Dereference Dereference(Identifier idOut, DataType dt)
		{
			return new Dereference(dt, idOut);
		}

		public override Instruction TransformDefInstruction(DefInstruction def)
		{
            var stm = stmDef.Block.Statements.Insert(
                iStmDef + 1,
                stmDef.LinearAddress,
                new Store(Dereference(idOut, def.Identifier.DataType), def.Identifier));
            ssa.Uses.Add(stm);
            return def;
		}

		public override Instruction TransformPhiAssignment(PhiAssignment phi)
		{
			for (int i = 0; i < phi.Src.Arguments.Length; ++i)
			{
				Identifier idSrc = (Identifier) phi.Src.Arguments[i].Value;
				ssaIds[idSrc].Uses.Remove(stmDef);
				wl.Add(idSrc);
			}
			return phi;
		}

		public override Expression VisitUnaryExpression(UnaryExpression unary)
		{
			if (unary.Operator == Operator.AddrOf)
			{
				Identifier id = unary.Expression as Identifier;
				if (id != null && ssa.Identifier == id)
				{
					return idOut;
				}
			}
			return base.VisitUnaryExpression(unary);
		}

	}
}
