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

using System;
using System.Collections.Generic;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;

namespace Decompiler.Scanning
{
	/// <summary>
	/// The RewriterHost hosts the environment required to rewrite the machine code of the
	/// analyzed program into the machine-independent intermediate language.
	/// </summary>
	public class RewriterHost : IRewriterHost
	{
		private Program prog;
		private DecompilerEventListener eventListener;
		private Map<Address,VectorUse> vectorUses;
		private Dictionary<Procedure, Procedure> proceduresRewritten;
        private SortedList<Address, SystemService> syscalls;
		private CallRewriter crw;
		private Dictionary<Address,ProcedureSignature> callSignatures;

		public RewriterHost(
            Program prog, 
            DecompilerEventListener eventListener,
            SortedList<Address,SystemService> syscalls,
            Map<Address,VectorUse> vectorUses)
		{
			this.prog = prog;
            this.proceduresRewritten = new Dictionary<Procedure, Procedure>();
			this.vectorUses = vectorUses;
			this.syscalls = syscalls;
			this.crw = new CallRewriter();
			this.eventListener = eventListener;
            this.callSignatures = new Dictionary<Address, ProcedureSignature>();
		}

		public virtual void AddCallEdge(Procedure caller, Statement stm, Procedure callee)
		{
			prog.CallGraph.AddEdge(stm, callee);
		}

		public ImageReader CreateImageReader(Address addrStart)
		{
			return prog.Image.CreateReader(addrStart);
		}

		public ProgramImage Image
		{
			get { return prog.Image; }
		}

		public void LoadCallSignatures(ICollection<SerializedCall> serializedCalls)
		{
			foreach (SerializedCall sc in serializedCalls)
			{
				if (sc.Signature != null)
				{
                    Address addr = Address.ToAddress(sc.InstructionAddress, 16);
					ProcedureSerializer sser = new ProcedureSerializer(prog.Architecture, "stdapi");
                    callSignatures.Add(addr, sser.Deserialize(sc.Signature, prog.Architecture.CreateFrame()));
				}
			}
		}


		public void RewriteProgram()
		{
			foreach (KeyValuePair<Address, Procedure> de in prog.Procedures)
			{
				if (prog.CallGraph.EntryPoints.Contains(de.Value))
				{
					RewriteProcedure(de.Value, de.Key, prog.Architecture.WordWidth.Size);
				}
			}

            foreach (KeyValuePair<Address, Procedure> de in prog.Procedures)
			{
				RewriteProcedure(de.Value, de.Key, de.Value.Frame.ReturnAddressSize);
			}

			foreach (Procedure proc in prog.Procedures.Values)
			{
				crw.RewriteCalls(proc, prog.Architecture);
			}
		}

		public PseudoProcedure EnsurePseudoProcedure(string name, DataType returnType, int arity)
		{
			return prog.EnsurePseudoProcedure(name, returnType, arity);
		}

		public Procedure [] GetProceduresFromVector(Address addr, int cbReturnAddress)
		{
			VectorUse vu;
            if (!vectorUses.TryGetValue(addr, out vu))
            {
                return new Procedure[0];
            }
            ImageMapItem item;
            Image.Map.TryFindItemExact(vu.TableAddress, out item);
            ImageMapVectorTable vector = (ImageMapVectorTable) item;
			Procedure [] procs = new Procedure[vector.Addresses.Count];
			for (int i = 0; i < vector.Addresses.Count; ++i)
			{
				procs[i] = GetProcedureAtAddress(vector.Addresses[i], cbReturnAddress);
			}
			return procs;
		}

		public virtual PseudoProcedure GetImportThunkAtAddress(Address addrThunk)
		{
            //$REVIEW: should be external procedures, since they have real signatures.
            PseudoProcedure p;
            if (prog.ImportThunks.TryGetValue((uint) addrThunk.Linear, out p))
                return p;
            else
                return null;
		}

		public virtual Procedure GetProcedureAtAddress(Address addr, int cbReturnAddress)
		{
			if (addr == null)
				return null;
			Procedure proc;
            if (prog.Procedures.TryGetValue(addr, out proc))
			{
				RewriteProcedure(proc, addr, cbReturnAddress);
			}
			return proc;
		}

		/// <summary>
		/// Given an instruction address, returns a procedure signature for that address.
		/// </summary>
		/// <param name="addr"></param>
		/// <returns></returns>
		public virtual ProcedureSignature GetCallSignatureAtAddress(Address addr)
		{
            ProcedureSignature sig;
            if (callSignatures.TryGetValue(addr, out sig))
                return sig;
            else
                return null;
		}

		public void RewriteProcedure(Procedure proc, Address addrProc, int cbReturnAddress)
		{
			if (proceduresRewritten.ContainsKey(proc))
				return;

			try
			{
				proceduresRewritten.Add(proc, proc);
				proc.Frame.ReturnAddressSize = cbReturnAddress;
                RewriteProcedureBlocks(proc, addrProc, prog.Architecture);
			} 
			catch (Exception ex)
			{
                if (eventListener != null) eventListener.AddDiagnostic(new ErrorDiagnostic(null, "An error occurred while rewriting {0}. {1}", proc.Name, ex.Message));
				throw;
			}
		}

		public virtual void RewriteProcedureBlocks(Procedure proc, Address addrProc, IProcessorArchitecture arch)
		{
			// Rewrite the blocks in the procedure.

            ProcedureRewriter prw = new ProcedureRewriter(this, arch, proc);
            Rewriter rw = arch.CreateRewriter(prw, proc, this);
			prw.Rewriter = rw;

            rw.GrowStack(proc.Frame.ReturnAddressSize);

			prw.Rewrite(addrProc, proc.EntryBlock);
			proc.RenumberBlocks();

			// If the frame escaped, rewrite local/parameter accesses to memory fetches.

			if (proc.Frame.Escapes)
			{
				EscapedAccessRewriter esc = new EscapedAccessRewriter(proc);
				esc.Transform();
				esc.InsertFramePointerAssignment(arch);
				proc.RenumberBlocks();
			}
		}

		public SystemService SystemCallAt(Address addrInstr)
		{
			SystemService svc = null;
            if (!syscalls.TryGetValue(addrInstr, out svc))
            {
                eventListener.AddDiagnostic(new WarningDiagnostic(addrInstr, "No system call at this address was recorded previously."));
			}
			return svc;
		}

        public PseudoProcedure TrampolineAt(Address addr)
        {
            PseudoProcedure p;
            if (prog.Trampolines.TryGetValue(addr.Linear, out p))
                return p;
            else 
                return null;
        }

		public VectorUse VectorUseAt(Address addrInstr)
		{
            VectorUse vu;
            if (vectorUses.TryGetValue(addrInstr, out vu))
                return vu;
            else
                return null;
		}

		public void AddDiagnostic(Diagnostic diagnostic)
		{
			eventListener.AddDiagnostic(diagnostic);
		}
	}
}
