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

using System;
using System.Collections;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Serialization;

namespace Decompiler.Scanning
{
	/// <summary>
	/// The RewriterHost hosts the environment required to rewrite the machine code of the
	/// analyzed program into the machine-independent intermediate language.
	/// </summary>
	public class RewriterHost : IRewriterHost
	{
		private Program prog;
		private DecompilerHost host;
		private Map vectorUses;
		private Hashtable proceduresRewritten;
		private SortedList syscalls;
		private CallRewriter crw;
		private Hashtable callSignatures;
		private ProcedureRewriter prw;

		public RewriterHost(Program prog, DecompilerHost host, SortedList syscalls, Map vectorUses)
		{
			this.prog = prog;
			this.proceduresRewritten = new Hashtable();
			this.vectorUses = vectorUses;
			this.syscalls = syscalls;
			this.crw = new CallRewriter();
			this.host = host;
			this.callSignatures = new Hashtable();
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

		public void LoadCallSignatures(ICollection serializedCalls)
		{
			foreach (SerializedCall sc in serializedCalls)
			{
				Address addr = Address.ToAddress(sc.InstructionAddress, 16);
				if (sc.Signature != null)
				{
					SignatureSerializer sser = new SignatureSerializer(prog.Architecture, "stdapi");
					callSignatures[addr] = sser.Deserialize(sc.Signature, new Frame(null));
				}
			}
		}


		public void RewriteProgram()
		{
			foreach (DictionaryEntry de in prog.Procedures)
			{
				Address addrProc = (Address) de.Key;
				Procedure p = (Procedure) de.Value;
				System.Diagnostics.Debug.WriteLine(string.Format("{0}: {1}, {2}", addrProc, p.Name, p.Frame.ReturnAddressSize));
				if (prog.CallGraph.EntryPoints.Contains(p))
				{
					RewriteProcedure(p, addrProc, prog.Architecture.WordWidth.Size);
				}
			}

			foreach (DictionaryEntry de in prog.Procedures)
			{
				Address addrProc = (Address) de.Key;
				Procedure p = (Procedure) de.Value;
				RewriteProcedure(p, addrProc, p.Frame.ReturnAddressSize);
			}

			foreach (Procedure proc in prog.Procedures.Values)
			{
				crw.RewriteCalls(proc);
			}
		}

		public PseudoProcedure EnsurePseudoProcedure(string name, int arity)
		{
			return prog.EnsurePseudoProcedure(name, arity);
		}

		public Procedure [] GetProceduresFromVector(Address addr, int cbReturnAddress)
		{
			VectorUse vu = (VectorUse) vectorUses[addr];
			if (vu == null)
				return new Procedure[0];
			ImageMapVectorTable vector = (ImageMapVectorTable) Image.Map.FindItemExact(vu.TableAddress);
			Procedure [] procs = new Procedure[vector.Addresses.Count];
			for (int i = 0; i < vector.Addresses.Count; ++i)
			{
				procs[i] = GetProcedureAtAddress((Address) vector.Addresses[i], cbReturnAddress);
			}
			return procs;
		}

		public virtual PseudoProcedure GetImportThunkAtAddress(Address addrThunk)
		{
			return (PseudoProcedure) prog.ImportThunks[(uint) addrThunk.Linear];		//$REVIEW: should be external procedures, since they have real signatures.
		}

		public virtual Procedure GetProcedureAtAddress(Address addr, int cbReturnAddress)
		{
			if (addr == null)
				return null;
			Procedure proc = (Procedure) prog.Procedures[addr];
			if (proc != null)
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
			return (ProcedureSignature) callSignatures[addr];
		}

		public void RewriteProcedure(Procedure proc, Address addrProc, int cbReturnAddress)
		{
			if (proceduresRewritten.Contains(proc))
				return;

			try
			{
				proceduresRewritten.Add(proc, proc);
				proc.Frame.ReturnAddressSize = cbReturnAddress;
				RewriteProcedureBlocks(proc, addrProc);
			} 
			catch (Exception ex)
			{
				if (host != null) host.WriteDiagnostic(Diagnostic.Error, "An error occurred while rewriting {0}. {1}", proc.Name, ex.Message);
				throw;
			}
		}

		public virtual void RewriteProcedureBlocks(Procedure proc, Address addrProc)
		{
			// Rewrite the blocks in the procedure.

			prw = new ProcedureRewriter(this, proc);
			Rewriter rw = prog.Architecture.CreateRewriter(prw, proc, this, new CodeEmitter(prog, proc));
			prw.Rewriter = rw;
			prw.RewriteBlock(addrProc, proc.EntryBlock);
			proc.RenumberBlocks();

			// If the frame escaped, rewrite local/parameter accesses to memory fetches.

			if (proc.Frame.Escapes)
			{
				EscapedAccessRewriter esc = new EscapedAccessRewriter(proc);
				esc.Transform();
				esc.InsertFramePointerAssignment();
				proc.RenumberBlocks();
			}
		}

		public SystemService SystemCallAt(Address addrInstr)
		{
			SystemService svc = (SystemService) syscalls[addrInstr];
			if (svc == null)
			{
				host.WriteDiagnostic(Diagnostic.Warning, "System call at address {0} wasn't previously recorded.", addrInstr);
			}
			return svc;
		}

		public PseudoProcedure TrampolineAt(Address addr)
		{
			return (PseudoProcedure) prog.Trampolines[addr.Linear];
		}

		public VectorUse VectorUseAt(Address addrInstr)
		{
			return (VectorUse) vectorUses[addrInstr];
		}

		public void WriteDiagnostic(Diagnostic diagnostic, string format, params object [] args)
		{
			host.WriteDiagnostic(diagnostic, format, args);
		}
	}
}
