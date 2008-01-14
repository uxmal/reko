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
using Decompiler.Core.Lib;
using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.Collections;
using System.Diagnostics;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Uses an interprocedural reaching definition analysis to detect which 
	/// registers are modified by the procedures, and which registers have
	/// their values preserved. 
	/// </summary>
	public class TrashedRegisterFinder : InstructionVisitorBase
	{
		private Program prog;
		private Identifier trash;
		private ProgramDataFlow flow;
		private WorkList worklist;
		private Frame frame;
		private TrashStorageHelper tsh;
		private DecompilerHost decompilerHost;

		public TrashedRegisterFinder(Program prog, ProgramDataFlow flow)
		{
			this.prog = prog;
			this.flow = flow;
			tsh = new TrashStorageHelper();
			this.trash = new Identifier("TRASH", -1, PrimitiveType.Void, null);
			this.worklist = new WorkList();
		}

		public void CompleteWork()
		{
			foreach (Procedure proc in prog.Procedures.Values)
			{
				ProcedureFlow pf = flow[proc];
				foreach (int r in pf.TrashedRegisters)
				{
					prog.Architecture.GetRegister(r).SetAliases(pf.TrashedRegisters, true);
				}
			}
		}

		public void Compute()
		{
			FillWorklist();
			Console.WriteLine("{0} items in worklist", worklist.Count);
			int initial = worklist.Count;
			while (!worklist.IsEmpty)
			{
				if (decompilerHost != null)
					decompilerHost.ShowProgress(string.Format("Blocks left: {0}", worklist.Count), initial - worklist.Count, initial);

				Block block = (Block) worklist.GetWorkItem();
				Debug.WriteLine(string.Format("{0} {1}", block.Procedure.Name, block.Name));

				CurrentFrame = block.Procedure.Frame;
				ProcessBlock(block);
			}
			CompleteWork();
		}

		public Frame CurrentFrame 
		{
			get { return frame; }
			set { frame = value; }
		}

		public DecompilerHost DecompilerHost
		{
			get { return decompilerHost; }
			set { decompilerHost = value; }
		}

		public void FillWorklist()
		{
			foreach (Procedure proc in prog.Procedures.Values)
			{
				foreach (Block block in proc.RpoBlocks)
				{
					worklist.Add(block);
				}
			}
		}

		public void ProcessBlock(Block block)
		{
			BlockFlow bf = flow[block];
			tsh = new TrashStorageHelper((Hashtable) bf.TrashedIn.Clone(), bf.grfTrashedIn);
			foreach (Statement stm in block.Statements)
			{
				stm.Instruction.Accept(this);
//				DumpRegisters();
//				foreach (object o in this.TrashedRegisters.Keys)
//				{
//					RegisterStorage stg = o as RegisterStorage;
//					if (stg == null)
//						continue;
//					System.Diagnostics.Debug.Write(string.Format("{0} ", stg.Register.Name));
//				}
//				System.Diagnostics.Debug.WriteLine("");
			}
			if (block == block.Procedure.ExitBlock)
			{
				PropagateToProcedureSummary(block.Procedure);
			}
			else
			{
				PropagateToSuccessorBlocks(block);
			}
		}

		public void PropagateToProcedureSummary(Procedure proc)
		{
			bool changed = false;
			ProcedureFlow pf = flow[proc];
			BitSet tr = prog.Architecture.CreateRegisterBitset();
			BitSet pr = prog.Architecture.CreateRegisterBitset();
			foreach (DictionaryEntry de in tsh.TrashedRegisters)
			{
				RegisterStorage r = de.Key as RegisterStorage;
				if (r == null)
					continue;

				if (de.Key != de.Value)
				{
					tr[r.Register.Number] = true;
				}
				else
				{
					pr[r.Register.Number] = true;
				}
			}

			if (!(tr & ~pf.TrashedRegisters).IsEmpty)
			{
				pf.TrashedRegisters |= tr;
				changed = true;
			}
			if (!(pr & ~pf.PreservedRegisters).IsEmpty)
			{
				pf.PreservedRegisters |= pr;
				changed = true;
			}
			uint grfNew = pf.grfTrashed | tsh.TrashedFlags;
			if (grfNew != pf.grfTrashed)
			{
				pf.grfTrashed = grfNew;
				changed = true;
			}


			if (changed)
			{
				foreach (Statement stm in prog.CallGraph.CallerStatements(proc))
				{
					worklist.Add(stm.block);
				}
			}
		}

		public void PropagateToSuccessorBlocks(Block block)
		{
			foreach (Block s in block.Succ)
			{
				bool changed = false;
				BlockFlow sf = flow[s];
				Hashtable trashed = sf.TrashedIn;
				foreach (DictionaryEntry de in tsh.TrashedRegisters)
				{
					object oldValue = trashed[de.Key];
					if (oldValue == null)
					{
						trashed[de.Key] = de.Value;
						changed = true;
					}
					else if (oldValue != de.Value && oldValue != trash)
					{
						trashed[de.Key] = trash;
						changed = true;
					}
				}
				
				uint grfNew = sf.grfTrashedIn | tsh.TrashedFlags;
				if (grfNew != sf.grfTrashedIn)
				{
					sf.grfTrashedIn = grfNew;
					changed = true;
				}
				if (changed)
				{
					worklist.Add(s);
				}
			}
		}

		public Hashtable TrashedRegisters
		{
			get { return tsh.TrashedRegisters; }
		}

		public uint TrashedFlags
		{
			get { return tsh.TrashedFlags; }
		}

		public override void VisitApplication(Application appl)
		{
			for (int i = 0; i < appl.Arguments.Length; ++i)
			{
				UnaryExpression u = appl.Arguments[i] as UnaryExpression;
				if (u != null && u.op == UnaryOperator.addrOf)
				{
					Identifier id = u.Expression as Identifier;
					if (id != null)
					{
						tsh.Trash(id, trash);
					}
				}
			}
		}

		public override void VisitAssignment(Assignment a)
		{
			base.VisitAssignment(a);
			Identifier idSrc = a.Src as Identifier;
			if (idSrc != null)
			{
				tsh.Copy(a.Dst, idSrc);
			}
			else
			{
				tsh.Trash(a.Dst, trash);
			}
		}

		public override void VisitCallInstruction(CallInstruction ci)
		{
			base.VisitCallInstruction(ci);
			ProcedureFlow pf = flow[ci.Callee];
			foreach (int r in pf.TrashedRegisters)
			{
				RegisterStorage reg = new RegisterStorage(prog.Architecture.GetRegister(r));
				tsh.TrashedRegisters[reg] = trash;
			}
			tsh.TrashedFlags |= pf.grfTrashed;
		}
	}
}
