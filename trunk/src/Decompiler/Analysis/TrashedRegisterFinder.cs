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
using Decompiler.Core.Lib;
using Decompiler.Core.Operators;
using Decompiler.Core.Services;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Uses an interprocedural reaching definition analysis to detect which 
	/// registers are modified by the procedures, which registers are constant at block
    /// exits, and which registers have their values preserved. 
	/// </summary>
    public class TrashedRegisterFinder : InstructionVisitorBase
    {
        private Program prog;
        private ProgramDataFlow flow;
        private WorkList<Block> worklist;
        private TrashStorageHelper tsh;
        private SymbolicEvaluator se;
        private DecompilerEventListener eventListener;

        public TrashedRegisterFinder(
            Program prog,
            ProgramDataFlow flow,
            DecompilerEventListener eventListener)
        {
            this.prog = prog;
            this.flow = flow;
            this.eventListener = eventListener;
            this.se = new SymbolicEvaluator(prog.Architecture, new TrashedExpressionSimplifier(this, new EvaluationContextImpl(this)));
            this.tsh = new TrashStorageHelper(null);
            this.worklist = new WorkList<Block>();
        }


        private class EvaluationContextImpl : EvaluationContext
        {
            TrashedRegisterFinder trf;

            public EvaluationContextImpl(TrashedRegisterFinder trf)
            {
                this.trf = trf;
            }
            #region EvaluationContext Members

            public Expression GetValue(Identifier id)
            {
                return trf.se.GetValue(id);
            }

            public Expression GetValue(MemoryAccess access)
            {
                return trf.se.GetValue(access);
            }

            public Expression GetValue(SegmentedAccess access)
            {
                return trf.se.GetValue(access);
            }

            public Expression GetValue(Application appl)
            {
                return trf.se.GetValue(appl);
            }

            public void RemoveIdentifierUse(Identifier id)
            {
                trf.se.RemoveIdentifierUse(id);
            }

            public void UseExpression(Expression expr)
            {
                trf.se.UseExpression(expr);
            }

            #endregion
        }
        public Dictionary<RegisterStorage, Expression> RegisterSymbolicValues { get { return se.RegisterState; } }
        public IDictionary<int, Expression> StackSymbolicValues { get { return se.StackState; } } 
        [Obsolete("Use symbolicvalue collection")]
        public Dictionary<Storage, Storage> TrashedRegisters { get { return tsh.TrashedRegisters; } }
        public uint TrashedFlags {get { return se.TrashedFlags; } }


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
            int initial = worklist.Count;
            Block block;
            while (worklist.GetWorkItem(out block))
            {
                var e = eventListener;
                if (e != null)
                    eventListener.ShowProgress(string.Format("Blocks left: {0}", worklist.Count), initial - worklist.Count, initial);

                ProcessBlock(block);
            }
            CompleteWork();
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

        public bool IsTrashed(Storage storage)
        {
            var reg = storage as RegisterStorage;
            if (reg != null)
            {
                Expression regVal;
                if (!se.RegisterState.TryGetValue(reg, out regVal))
                    return false;
                var id = regVal as Identifier;
                if (id == null)
                    return true;
                return id.Storage == reg;
            }
            throw new NotImplementedException();
        }

        public void ProcessBlock(Block block)
        {
            var bf = flow[block];
            tsh = CreateTrashStorageHelper(bf);
            foreach (Statement stm in block.Statements)
            {
                stm.Instruction.Accept(this);
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

        private TrashStorageHelper CreateTrashStorageHelper(BlockFlow bf)
        {
            return new TrashStorageHelper(new Dictionary<Storage, Storage>(bf.TrashedIn), bf.grfTrashedIn, null);
        }

        public void PropagateToProcedureSummary(Procedure proc)
        {
            bool changed = false;
            ProcedureFlow pf = flow[proc];
            BitSet tr = prog.Architecture.CreateRegisterBitset();
            BitSet pr = prog.Architecture.CreateRegisterBitset();
            if (pf.TerminatesProcess)
            {
                if (!pf.TrashedRegisters.IsEmpty)
                {
                    changed = true;
                    pf.TrashedRegisters.SetAll(false);
                }
                if (pf.grfTrashed != 0)
                {
                    changed = true;
                    pf.grfTrashed = 0;
                }
            }
            else
            {
                foreach (KeyValuePair<RegisterStorage, Expression> de in se.RegisterState)
                {
                    var idValue = de.Value as Identifier;
                    if (idValue != null)
                    {
                        if (de.Key != idValue.Storage)
                        {
                            tr[de.Key.Register.Number] = true;
                        }
                        else
                        {
                            pr[de.Key.Register.Number] = true;
                        }
                    }
                    else
                    {
                        tr[de.Key.Register.Number] = true;
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
                uint grfNew = pf.grfTrashed | se.TrashedFlags;
                if (grfNew != pf.grfTrashed)
                {
                    pf.grfTrashed = grfNew;
                    changed = true;
                }
            }

            if (changed)
            {
                foreach (Statement stm in prog.CallGraph.CallerStatements(proc))
                {
                    worklist.Add(stm.Block);
                }
            }
        }

        public void PropagateToSuccessorBlocks(Block block)
        {
            foreach (Block s in block.Procedure.ControlGraph.Successors(block))
            {
                bool changed = false;
                BlockFlow sf = flow[s];
                var successorState = sf.SymbolicIn;
                foreach (KeyValuePair<RegisterStorage, Expression> de in se.RegisterState)
                {
                    Expression oldValue;
                    if (!successorState.TryGetValue(de.Key, out oldValue))
                    {
                        successorState[de.Key] = de.Value;
                        changed = true;
                    }
                    else if (oldValue != de.Value && oldValue != Constant.Invalid)
                    {
                        successorState[de.Key] = Constant.Invalid;
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




        public override void VisitAssignment(Assignment a)
        {
            a.Accept(se);
        }

        public override void VisitStore(Store store)
        {
            store.Accept(se);
        }

        public override void VisitCallInstruction(CallInstruction ci)
        {
            ci.Accept(se);
            if (ProcedureTerminates(ci.Callee))
            {
                // A terminating procedure has no trashed registers because caller will never see those effects!
                tsh.TrashedFlags = 0;
                tsh.TrashedRegisters.Clear();
                return;
            }

            var callee = ci.Callee as Procedure;
            if (callee == null)
                return;             //$TODO: get trash information from signature?
            ProcedureFlow pf = flow[callee];
            foreach (int r in pf.TrashedRegisters)
            {
                RegisterStorage reg = new RegisterStorage(prog.Architecture.GetRegister(r));
                se.RegisterState[reg] = Constant.Invalid;
            }
            tsh.TrashedFlags |= pf.grfTrashed;
        }

        private bool ProcedureTerminates(ProcedureBase proc)
        {
            if (proc.Characteristics.Terminates)
                return true;
            var p = proc as Procedure;
            return (p != null && flow[p].TerminatesProcess);
        }

        public class TrashedExpressionSimplifier : ExpressionSimplifier
        {
            private TrashedRegisterFinder trf;

            public TrashedExpressionSimplifier(TrashedRegisterFinder trf, EvaluationContext ctx)
                : base(ctx)
            {
                this.trf = trf;
            }

            public override Expression VisitApplication(Application appl)
            {
                var e = base.VisitApplication(appl);
                var pc = appl.Procedure as ProcedureConstant;
                if (pc != null)
                {
                    if (trf.ProcedureTerminates(pc.Procedure))
                    {
                        trf.se.TrashedFlags = 0;
                        trf.se.RegisterState.Clear();
                        return appl;
                    }
                }
                for (int i = 0; i < appl.Arguments.Length; ++i)
                {
                    UnaryExpression u = appl.Arguments[i] as UnaryExpression;
                    if (u != null && u.op == UnaryOperator.AddrOf)
                    {
                        Identifier id = u.Expression as Identifier;
                        if (id != null)
                        {
                            trf.se.SetValue(id, Constant.Invalid);
                        }
                    }
                }
                return e;
            }
        }
    }

    [Obsolete("Use TrashedRegisterFinder")]
	public class TrashedRegisterFinderOld : InstructionVisitorBase
	{
		private Program prog;
		private ProgramDataFlow flow;
		private WorkList<Block> worklist;
		private TrashStorageHelper tsh;
        private readonly TrashStorage trash;
		private DecompilerEventListener eventListener;

		public TrashedRegisterFinderOld(
            Program prog, 
            ProgramDataFlow flow, 
            DecompilerEventListener eventListener)
		{
			this.prog = prog;
			this.flow = flow;
            this.eventListener = eventListener;
            this.trash = new TrashStorage();
            this.tsh = new TrashStorageHelper(trash);
			this.worklist = new WorkList<Block>();
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
			int initial = worklist.Count;
            Block block;
			while (worklist.GetWorkItem(out block))
			{
                var e = eventListener;
				if (e != null)
					eventListener.ShowProgress(string.Format("Blocks left: {0}", worklist.Count), initial - worklist.Count, initial);

				ProcessBlock(block);
			}
			CompleteWork();
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
            tsh = CreateTrashStorageHelper(bf);
			foreach (Statement stm in block.Statements)
			{
				stm.Instruction.Accept(this);
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

        private TrashStorageHelper CreateTrashStorageHelper(BlockFlow bf)
        {
            return new TrashStorageHelper(new Dictionary<Storage, Storage>(bf.TrashedIn), bf.grfTrashedIn, trash);
        }

		public void PropagateToProcedureSummary(Procedure proc)
		{
			bool changed = false;
			ProcedureFlow pf = flow[proc];
			BitSet tr = prog.Architecture.CreateRegisterBitset();
			BitSet pr = prog.Architecture.CreateRegisterBitset();
            if (pf.TerminatesProcess)
            {
                if (!pf.TrashedRegisters.IsEmpty)
                {
                    changed = true;
                    pf.TrashedRegisters.SetAll(false);
                }
                if (pf.grfTrashed != 0)
                {
                    changed = true;
                    pf.grfTrashed = 0;
                }
            }
            else
            {
                foreach (KeyValuePair<Storage, Storage> de in tsh.TrashedRegisters)
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
            }

			if (changed)
			{
				foreach (Statement stm in prog.CallGraph.CallerStatements(proc))
				{
					worklist.Add(stm.Block);
				}
			}
		}

		public void PropagateToSuccessorBlocks(Block block)
		{
			foreach (Block s in block.Succ)
			{
				bool changed = false;
				BlockFlow sf = flow[s];
				Dictionary<Storage,Storage> trashed = sf.TrashedIn;
				foreach (KeyValuePair<Storage,Storage> de in tsh.TrashedRegisters)
				{
					Storage oldValue;
					if (!trashed.TryGetValue(de.Key, out oldValue))
					{
						trashed[de.Key] = de.Value;
						changed = true;
					}
					else if (oldValue != de.Value && oldValue != tsh.TrashedStorage)
					{
						trashed[de.Key] = tsh.TrashedStorage;
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

		public Dictionary<Storage,Storage> TrashedRegisters
		{
			get { return tsh.TrashedRegisters; }
		}

		public uint TrashedFlags
		{
			get { return tsh.TrashedFlags; }
		}

        public Storage TrashedStorage
        {
            get { return trash; }
        } 

		public override void VisitApplication(Application appl)
		{
            var pc = appl.Procedure as ProcedureConstant;
            if (pc != null)
            {
                if (ProcedureTerminates(pc.Procedure))
                {
                    tsh.TrashedFlags = 0;
                    tsh.TrashedRegisters.Clear();
                    return;
                }
            }
			for (int i = 0; i < appl.Arguments.Length; ++i)
			{
				UnaryExpression u = appl.Arguments[i] as UnaryExpression;
				if (u != null && u.op == UnaryOperator.AddrOf)
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

        private bool ProcedureTerminates(ProcedureBase proc)
        {
            if (proc.Characteristics.Terminates)
                return true;
            var p = proc as Procedure;
            return (p != null && flow[p].TerminatesProcess);
        }

		public override void VisitCallInstruction(CallInstruction ci)
		{
			base.VisitCallInstruction(ci);
            if (ProcedureTerminates(ci.Callee))
            {
                // A terminating procedure has no trashed registers because caller will never see those effects!
                tsh.TrashedFlags = 0;
                tsh.TrashedRegisters.Clear();
                return;                     
            }

            var callee = ci.Callee as Procedure;
            if (callee == null)
                return;             //$TODO: get trash information from signature?
            ProcedureFlow pf = flow[callee];
			foreach (int r in pf.TrashedRegisters)
			{
				RegisterStorage reg = new RegisterStorage(prog.Architecture.GetRegister(r));
				tsh.TrashedRegisters[reg] = trash;
			}
			tsh.TrashedFlags |= pf.grfTrashed;
		}

        /// <summary>
        /// A "fake" Storage object that indicates that the value of a register is irretrievably lost.
        /// </summary>
        private class TrashStorage : Storage
        {
            public TrashStorage()
                : base("Trash")
            {
            }

            public override void Accept(StorageVisitor visitor)
            {
                throw new NotImplementedException();
            }

            public override int OffsetOf(Storage storage)
            {
                throw new NotImplementedException();
            }

            public override void Write(System.IO.TextWriter writer)
            {
                writer.Write("TRASH");
            }
        }
    }
}
