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
using System;
using System.Collections;
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
 		public void DeleteStatement(Statement pstm)
		{
			// Remove all definitions and uses.

			ReplaceDefinitions(pstm, null);

			// For each used identifier remove this statement from its uses.

			RemoveUses(pstm);

			// Remove the statement itself.
			pstm.block.Statements.Remove(pstm);
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

		private void WriteIdentifier(SsaIdentifier id, TextWriter sb)
		{
			id.Write(sb);
			sb.WriteLine();
		}

#if DEBUG
		private void DumpDefSites(Set [] defsites)
		{
			for (int a = 0; a < defsites.Length; ++a)
			{
				Debug.WriteLine("v" + a + " defined in: ");
				foreach (Block it in defsites[a])
				{
					Debug.Write("#" + it.RpoNumber + " ");
				}
				Debug.WriteLine("");
			}
		}
#else
		public void Dump() {}
		private void DumpDefSites(Set [] defsites) {}
		private void DumpIdentifier(int v) {}
#endif 


		public SsaIdentifierCollection Identifiers
		{
			get { return ids; }
		}


		public void ReplaceDefinitions(Statement stmOld, Statement stmNew)
		{
			for (int i = 0; i < Identifiers.Count; ++i)
			{
				if (Identifiers[i].def == stmOld)
					Identifiers[i].def = stmNew;
			}
		}

		public void RemoveUses(Statement stm)
		{
			for (int i = 0; i < Identifiers.Count; ++i)
			{
				ArrayList uses = Identifiers[i].uses;
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
				return ssa.Identifiers[id].idOrig;
			}
		}

	}
}
