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
using Decompiler.Core.Lib;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;

namespace Decompiler.Analysis
{
	public class Aliases : InstructionVisitorBase
	{
		private Procedure proc;
		private ProgramDataFlow pdf;
		private Block block;
		private int iStm;
		private bool [] deadIn;
		private List<int> [] aliases;
		private IProcessorArchitecture arch;

		public Aliases(Procedure proc, IProcessorArchitecture arch)
		{
			Init(proc, arch, null);
		}

		public Aliases(Procedure proc, IProcessorArchitecture arch, ProgramDataFlow pdf)
		{
			Init(proc, arch, pdf);
		}

		private void Init(Procedure proc, IProcessorArchitecture arch, ProgramDataFlow pdf)
		{
			this.proc = proc; 
			this.arch = arch;
			this.pdf = pdf;

			BuildAliases();
		}

		private void BuildAliases()
		{
			aliases = new List<int>[proc.Frame.Identifiers.Count];
			for (int i = 0; i < aliases.Length; ++i)
			{
				aliases[i] = new List<int>();
			}

			for (int i = 0; i < aliases.Length; ++i)
			{
				Identifier v = proc.Frame.Identifiers[i];
				for (int j = 0; j < aliases.Length; ++j)
				{
					Identifier var = proc.Frame.Identifiers[j];
					if (IsAlias(arch, var, v) && i < j)
					{
						// The bits interfere with each other.
						aliases[i].Add(j);
						aliases[j].Add(i);
					}
				}
			}
		}

		/// We've encountered a variable that is defined. We must generate an
		// alias statement for all aliased variables.

		private void Def(int v)
		{
			deadIn[v] = true;
			Identifier idar = proc.Frame.Identifiers[v];
			int iAt = iStm;
			foreach (int i in aliases[v])
			{
				if (!deadIn[i])
				{
					block.Statements.Insert(++iAt, CreateAliasInstruction(v, i));
					deadIn[i] = true;
				}
			}
		}

		public void Write(TextWriter text)
		{
			foreach (Identifier id in proc.Frame.Identifiers)
			{
				text.Write(id.Name);
				text.Write(":");
				id.Storage.Write(text);
				text.Write(" (aliases:", id);
				foreach (int a in aliases[id.Number])
				{
					text.Write(" {0}", proc.Frame.Identifiers[a].Name);
				}
				text.WriteLine(")");
			}
		}

		public static bool IsAlias(IProcessorArchitecture arch, Identifier id1, Identifier id2)
		{
			return 
				(id1.Storage.OffsetOf(id2.Storage) != -1) ||
				(id2.Storage.OffsetOf(id1.Storage) != -1);
		}

		/// <summary>
		/// Remove any alias statements.
		/// </summary>
		public void Restore()
		{
			foreach (Block block in proc.RpoBlocks)
			{
				for (int i = block.Statements.Count-1; i >= 0; --i)
				{
					if (block.Statements[i].Instruction is AliasAssignment)
						block.Statements.RemoveAt(i);
				}
			}
		}

		public void StartBlock(Block block)
		{
			if (this.pdf != null)
			{
				BlockFlow bf = pdf[block];
				AliasDeadVariableMarker marker = new AliasDeadVariableMarker(bf.DataOut, bf.grfOut);
				deadIn = marker.Compute(arch, proc.Frame);
			}
			else
			{
				// If no global analysis available, be pessimistic and assume
				// all variables are live-out of every block.
				deadIn = new bool[aliases.Length];
			}
		}

		/// <summary>
		/// For each statement in the procedure, traverse any defined operands. Upon 
		/// encountering, add alias statements for all the known aliases.
		/// We do this in reverse order, calculating at the same time the deadness
		/// of all registers. We may not need to generate an alias if the 
		/// register is dead.
		/// </summary>
		/// <param name="proc"></param>
		public void Transform()
		{
			for (int b = 0; b < proc.RpoBlocks.Count; ++b)
			{
				block = proc.RpoBlocks[b];
				StartBlock(block);
				for (iStm = block.Statements.Count-1; iStm >= 0; --iStm)
				{
					Instruction instr = block.Statements[iStm].Instruction;
					instr.Accept(this);
				}
			}
		}

		public override void VisitAssignment(Assignment ass)
		{
			Def(ass.Dst.Number);
			ass.Src.Accept(this);
		}

		public override void VisitIdentifier(Identifier id)
		{
			deadIn[id.Number] = false;		// used, so can't be dead.
		}

		public override void VisitMemoryAccess(MemoryAccess access)
		{
			access.EffectiveAddress.Accept(this);
		}

		/// <summary>
		/// Inserts an alias statement.
		/// </summary>
		/// <param name="v">Variable that has been defined</param>
		/// <param name="vAlias">Other variable that is aliased by v</param>
		/// <param name="iAt"></param>
		// If a wide register is being defined by aliasing of a smaller
		// register, the expression needs to take into account the previous
		// value of the wide register. For instance, modifying cl aliases
		// cx = f(cl, cx)
		// thus:
		//	mov cl,xx
		//  mov cx,DPB(cx,cl,0,8)
		public Assignment CreateAliasInstruction(int v, int vAlias)
		{
			Identifier varFrom = proc.Frame.Identifiers[v];
			Identifier varTo = proc.Frame.Identifiers[vAlias];
			if (!IsAlias(arch, varFrom, varTo))
				throw new ApplicationException(string.Format("Unexpected alias pair {0} and {1}", varTo.Name, varFrom.Name));

			Expression aliasExpr;

			int offsetTo = varTo.Storage.OffsetOf(varFrom.Storage);
			int offsetFrom = varFrom.Storage.OffsetOf(varTo.Storage);

			int cbitsFrom = varFrom.DataType.BitSize;
			int cbitsTo = varTo.DataType.BitSize;
			if (cbitsFrom < cbitsTo)
			{
				// We are replacing a part of a wider register with a narrower one.

				SequenceStorage seq = varTo.Storage as SequenceStorage;
				if (seq != null && (seq.Head == varFrom || seq.Tail == varFrom))
				{
					aliasExpr = new MkSequence(varTo.DataType, seq.Head, seq.Tail);
				}
				else
				{
					aliasExpr = new DepositBits(varTo, varFrom, offsetTo, cbitsFrom);
				}
			}		
			else if (cbitsFrom > cbitsTo)
			{
				if (offsetFrom == 0)
				{
					aliasExpr = new Cast(varTo.DataType, varFrom);
				}
				else
				{
					aliasExpr = new Slice(varTo.DataType, varFrom, (uint) offsetFrom);
				}
			}
			else
			{
				aliasExpr = varFrom;
			}
			return new AliasAssignment(varTo, aliasExpr);
		}

		public static void Undo(Procedure proc)
		{
			foreach (Block block in proc.RpoBlocks)
			{
				for (int i = block.Statements.Count - 1; i >= 0; --i)
				{
					if (block.Statements[i].Instruction is AliasAssignment)
						block.Statements.RemoveAt(i);
				}
			}
		}
	}


	/// <summary>
	/// Represents an assignment due to alias expansion. 
	/// </summary>
	public class AliasAssignment : Assignment
	{
		public AliasAssignment(Identifier idDst, Expression expSrc) : base(idDst, expSrc)
		{
		}

		public override bool IsAlias
		{
			get { return true; }
		}
	}

	
	public class AliasDeadVariableMarker : StorageVisitor
	{
		private Identifier idCur;
		private BitSet liveRegs;
		private uint liveGrf;
		private bool [] liveVars;

		public AliasDeadVariableMarker(BitSet regs, uint grfLive)
		{
			this.liveRegs = regs;
			this.liveGrf = grfLive;
		}
		
		public bool [] Compute(IProcessorArchitecture arch, Frame frame)
		{
			this.liveVars = new bool[frame.Identifiers.Count];

			foreach (Identifier id in frame.Identifiers)
			{
				idCur = id;
				id.Storage.Accept(this);
			}
			for (int i = 0; i < liveVars.Length; ++i)
			{
				if (liveVars[i])
				{
					Identifier v = frame.Identifiers[i];
					foreach (Identifier w in frame.Identifiers)
					{
						if (Aliases.IsAlias(arch, v, w))
							liveVars[w.Number] = true;
					}
				}
			}
			bool [] deadVariables = new bool[liveVars.Length];
			for (int i = 0; i < liveVars.Length; ++i)
			{
				deadVariables[i] = !liveVars[i];
			}
			return deadVariables;
		}

		#region StorageVisitor Members

		public void VisitFpuStackStorage(FpuStackStorage fpu)
		{
		}

		public void VisitFlagGroupStorage(FlagGroupStorage grf)
		{
			liveVars[idCur.Number] = (grf.FlagGroup & liveGrf) != 0;
		}

		public void VisitTemporaryStorage(TemporaryStorage temp)
		{
		}

		public void VisitStackArgumentStorage(StackArgumentStorage stack)
		{
		}

		public void VisitMemoryStorage(MemoryStorage global)
		{
		}

		public void VisitSequenceStorage(SequenceStorage seq)
		{
		}

		public void VisitRegisterStorage(RegisterStorage reg)
		{
			liveVars[idCur.Number] = liveRegs[reg.Register.Number];
		}

		public void VisitStackLocalStorage(StackLocalStorage local)
		{
		}

		public void VisitOutArgumentStorage(OutArgumentStorage arg)
		{
			arg.OriginalIdentifier.Storage.Accept(this);
		}
		#endregion
	}
}
