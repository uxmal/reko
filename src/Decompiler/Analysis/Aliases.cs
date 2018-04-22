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
using Reko.Core.Lib;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reko.Analysis
{
    /// <summary>
    /// Builds the alias graph for a procedure. The idea is to discover
    /// relationships like(on the x86 architecture) eax <=> ax, eax <=> ah, 
    /// etc. Similarly, relationships like the one between the sequence
    /// edx:eax and dh are discovered.
    /// </summary>
	public class Aliases : InstructionVisitorBase
	{
		private Procedure proc;
		private ProgramDataFlow pdf;
		private Block block;
		private int iStm;
		private HashSet<Identifier> deadIn;
		private Dictionary<Identifier, List<Identifier>> aliases;
		private IProcessorArchitecture arch;
        private Statement stm;

        public Aliases(Procedure proc) : this(proc, null)
        {
		}

		public Aliases(Procedure proc, ProgramDataFlow pdf)
		{
			this.proc = proc; 
			this.arch = proc.Architecture;
			this.pdf = pdf;

			BuildAliases();
		}

		private void BuildAliases()
		{
            aliases = new Dictionary<Identifier, List<Identifier>>();
			foreach (var i in proc.Frame.Identifiers)
			{
				aliases[i] = new List<Identifier>();
			}

			foreach (var v in proc.Frame.Identifiers)
			{
				foreach (var var in proc.Frame.Identifiers)
				{
					if (IsAlias(arch, var, v) && string.Compare(v.Name, var.Name) < 0)
					{
						// The bits interfere with each other.
						aliases[v].Add(var);
						aliases[var].Add(v);
					}
				}
			}
		}

		// We've encountered a variable that is defined. We must generate an
		// alias statement for all aliased variables.
		private void Def(Identifier idar)
		{
			deadIn.Add(idar);
			int iAt = iStm;
			foreach (Identifier i in aliases[idar])
			{
				if (!deadIn.Contains(i))
				{
					block.Statements.Insert(++iAt, stm.LinearAddress, CreateAliasInstruction(idar, i));
					deadIn.Add(i);
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
                List<Identifier> aa;
                if (aliases.TryGetValue(id, out aa))
                {
                    foreach (var a in aliases[id])
                    {
                        text.Write(" {0}", a.Name);
                    }
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
			foreach (Block block in proc.ControlGraph.Blocks)
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
            this.block = block;
			if (this.pdf != null)
			{
				BlockFlow bf = pdf[block];
				var marker = new AliasDeadVariableMarker(bf.DataOut, bf.grfOut);
				deadIn = marker.Compute(arch, proc.Frame);
			}
			else
			{
				// If no global analysis available, be pessimistic and assume
				// all variables are live-out of every block.
				deadIn = new HashSet<Identifier>();
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
			foreach (var block in proc.ControlGraph.Blocks)
			{
				StartBlock(block);
				for (iStm = block.Statements.Count-1; iStm >= 0; --iStm)
				{
                    this.stm = block.Statements[iStm];
					Instruction instr = stm.Instruction;
					instr.Accept(this);
				}
			}
		}

		public override void VisitAssignment(Assignment ass)
		{
			Def(ass.Dst);
			ass.Src.Accept(this);
		}

		public override void VisitIdentifier(Identifier id)
		{
			deadIn.Remove(id);		// used, so can't be dead.
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
		public Assignment CreateAliasInstruction(Identifier varFrom, Identifier varTo) // Identifier v, Identifier vAlias)
		{
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
				if (seq != null && (seq.Head == varFrom.Storage || seq.Tail == varFrom.Storage))
				{
					aliasExpr = new MkSequence(
                        varTo.DataType,
                        proc.Frame.EnsureIdentifier(seq.Head), 
                        proc.Frame.EnsureIdentifier(seq.Tail));
				}
				else
				{
					aliasExpr = new DepositBits(varTo, varFrom, offsetTo);
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
					aliasExpr = new Slice(varTo.DataType, varFrom, offsetFrom);
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
			foreach (Block block in proc.ControlGraph.Blocks)
			{
				for (int i = block.Statements.Count - 1; i >= 0; --i)
				{
					if (block.Statements[i].Instruction is AliasAssignment)
						block.Statements.RemoveAt(i);
				}
			}
		}
	}


    public class AliasDeadVariableMarker : StorageVisitor<Storage>
    {
        private Identifier idCur;
        private HashSet<RegisterStorage> liveRegs;
        private uint liveGrf;
        private HashSet<Identifier> liveVars;

        public AliasDeadVariableMarker(HashSet<RegisterStorage> regs, uint grfLive)
        {
            this.liveRegs = regs;
            this.liveGrf = grfLive;
        }

        public HashSet<Identifier> Compute(IProcessorArchitecture arch, Frame frame)
        {
            this.liveVars = new HashSet<Identifier>();

            foreach (Identifier id in frame.Identifiers)
            {
                idCur = id;
                id.Storage.Accept(this);
            }
            foreach (Identifier v in liveVars.ToArray())
            {
                foreach (Identifier w in frame.Identifiers)
                {
                    if (Aliases.IsAlias(arch, v, w))
                        liveVars.Add(w);
                }
            }
            var deadVariables = new HashSet<Identifier>();
            foreach (var i in frame.Identifiers)
            {
                if (!liveVars.Contains(i))
                    deadVariables.Add(i);
                else
                    deadVariables.Remove(i);
            }
            return deadVariables;
        }

        #region StorageVisitor Members

        public Storage VisitFpuStackStorage(FpuStackStorage fpu)
        {
            return null;
        }

        public Storage VisitFlagGroupStorage(FlagGroupStorage grf)
        {
            if ((grf.FlagGroupBits & liveGrf) != 0)
                liveVars.Add(idCur);
            else
                liveVars.Remove(idCur);
            return null;
        }

        public Storage VisitTemporaryStorage(TemporaryStorage temp)
        {
            return null;
        }

        public Storage VisitStackArgumentStorage(StackArgumentStorage stack)
        {
            return null;
        }

        public Storage VisitMemoryStorage(MemoryStorage global)
        {
            return null;
        }

        public Storage VisitSequenceStorage(SequenceStorage seq)
        {
            return null;
        }

        public Storage VisitRegisterStorage(RegisterStorage reg)
        {
            if (liveRegs.Contains(reg))
                liveVars.Add(idCur);
            else 
                liveVars.Remove(idCur);
            return null;
        }

        public Storage VisitStackLocalStorage(StackLocalStorage local)
        {
            return null;
        }

        public Storage VisitOutArgumentStorage(OutArgumentStorage arg)
        {
            arg.OriginalIdentifier.Storage.Accept(this);
            return null;
        }
        #endregion
    }
}
