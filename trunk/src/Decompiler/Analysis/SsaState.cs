/* 
 * Copyright (C) 1999-2009 John Källén.
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Decompiler.Analysis
{
	public class SsaState
	{
		private Procedure proc;
		private SsaIdentifierCollection ids;

		public SsaState(Procedure proc)
		{
			this.proc = proc;
			this.ids = new SsaIdentifierCollection();
		}

		/// <summary>
		/// Given a procedure in SSA form, converts it back to "normal" form.
		/// </summary>
		/// <param name="fRenameVariables"></param>
		public void ConvertBack(bool fRenameVariables)
		{
			UnSSA unssa = new UnSSA(this);
			foreach (Block block in proc.RpoBlocks)
			{
				for (int st = 0; st < block.Statements.Count; ++st)
				{
					Instruction instr = block.Statements[st].Instruction;
					if (instr is PhiAssignment || instr is DefInstruction)
					{
						block.Statements.RemoveAt(st);
						--st;
					}
					else if (fRenameVariables)
					{
						instr.Accept(unssa);
					}
				}
			}

			// Remove any instructions in the return block, used only 
			// for computation of reaching definitions.

			proc.ExitBlock.Statements.Clear();
		}

#if DEBUG
		public void DebugDump(bool trace)
		{
			if (trace)
			{
				StringWriter sb = new StringWriter();
				Write(sb);
				proc.Write(false, sb);
				Debug.WriteLineIf(trace, sb.ToString());
			}
		}
#else
		public void DebugDump(bool trace)
		{
		}
#endif

		/// <summary>
		/// Deletes a statement by removing all the ids it references 
		/// from SSA state, then removes the statement itself from code.
		/// </summary>
		/// <param name="pstm"></param>
 		public void DeleteStatement(Statement stm)
		{
			// Remove all definitions and uses.
			ReplaceDefinitions(stm, null);

			// For each used identifier remove this statement from its uses.
			RemoveUses(stm);

			// Remove the statement itself.
			stm.Block.Statements.Remove(stm);
		}


		/// <summary>
		/// Dumps all SSA identifiers, showing the original variable,
		/// the defining statement, and the using statements.
		/// </summary>
		/// <param name="writer"></param>
		public void Write(TextWriter writer)
		{
			foreach (SsaIdentifier id in ids)
			{
				id.Write(writer);
				writer.WriteLine();
			}
		}

		public SsaIdentifierCollection Identifiers
		{
			get { return ids; }
		}


		public void ReplaceDefinitions(Statement stmOld, Statement stmNew)
		{
			for (int i = 0; i < Identifiers.Count; ++i)
			{
				if (Identifiers[i].DefStatement == stmOld)
					Identifiers[i].DefStatement = stmNew;
			}
		}

		public void RemoveUses(Statement stm)
		{
			for (int i = 0; i < Identifiers.Count; ++i)
			{
				List<Statement> uses = Identifiers[i].Uses;
				int jTo = 0;
				for (int j = 0; j < uses.Count; ++j)
				{
					if (uses[j] != stm)
					{
						uses[jTo] = uses[j];
						++jTo;
					}
				}
				uses.RemoveRange(jTo, uses.Count - jTo);
			}
		}


		/// <summary>
		/// Undoes the SSA renaming by replacing each ssa identifier with its original identifier.
		/// </summary>
		private class UnSSA : InstructionTransformer
		{
			private SsaState ssa;

			public UnSSA(SsaState ssa)
			{
				this.ssa = ssa;
			}

			public override Expression TransformIdentifier(Identifier id)
			{
				return ssa.Identifiers[id].OriginalIdentifier;
			}
		}
	}
}
