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

using Decompiler.Core.Code;
using Decompiler.Core.Types;
using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Decompiler.Core
{
	/// <summary>
	/// Contains information gathered during loading, scanning and data analysis,
	/// as well as storing any user-specified information.
	/// </summary>
	public class Program
	{
		private ProgramImage image;
		private IProcessorArchitecture arch;
		private Platform platform;
		private ProcedureCollection procedures;
		private CallGraph callGraph;
		private Map vectors;
		private Hashtable mpuintfn;
		private Hashtable trampolines;
		private Identifier globals;
		private Hashtable pseudoProcs;
		private TypeFactory typefactory;

		public Program()
		{
			procedures = new ProcedureCollection();
			vectors = new Map();
			callGraph = new CallGraph();
			mpuintfn = new Hashtable();		// uint (offset) -> string
			trampolines = new Hashtable();	// address -> string
			pseudoProcs = new Hashtable();
			typefactory = new TypeFactory();
			globals = new Identifier("globals", 0, PrimitiveType.Pointer, new MemoryStorage());
		}

		public void AddEntryPoint(EntryPoint ep)
		{
			Procedure proc = procedures[ep.Address];
			if (proc == null)
			{
				Frame frame = new Frame(arch.WordWidth);
				proc = Procedure.Create(ep.Name, ep.Address, frame);
				procedures.Add(ep.Address, proc);
			}
			CallGraph.AddEntryPoint(proc);
		}

		public IProcessorArchitecture Architecture
		{
			get { return arch; }
			set { arch = value; }
		}

		public CallGraph CallGraph
		{
			get { return callGraph; }
		}

		public void DumpAssembler(TextWriter wr)
		{
			Dumper dump = arch.CreateDumper();
			dump.Dump(this, Image.Map, wr);
		}



		public PseudoProcedure EnsurePseudoProcedure(string name, int arity)
		{
			PseudoProcedure p = pseudoProcs[name] as PseudoProcedure;
			if (p == null)
			{
				p = new PseudoProcedure(name, arity);
				pseudoProcs[name] = p;
			}
			return p;
		}

		public Identifier Globals
		{
			get { return globals; } 
		}
		
		public ProgramImage Image
		{
			get { return image; }
			set { image = value; }
		}
		
		public Hashtable ImportThunks
		{
			get { return mpuintfn; }
		}

		public Platform Platform
		{
			get { return platform; }
			set { platform = value; }
		}

		/// <summary>
		/// Provides access to the program's procedures, indexed by address.
		/// </summary>
		public ProcedureCollection Procedures
		{
			get { return procedures; }
		}

		/// <summary>
		/// Provides access to the program's pseudo procedures, indexed by name.
		/// </summary>
		public Hashtable PseudoProcedures
		{
			get { return pseudoProcs; }
		}

		public Hashtable Trampolines
		{
			get { return trampolines; }
		}

		public TypeFactory TypeFactory
		{
			get { return typefactory; }
		}
		
		/// <summary>
		/// Provides access to the program's jump and call tables, sorted by address.
		/// </summary>
		public Map Vectors
		{
			get { return vectors; }
		}
	}

	public class VectorUse
	{
		public Address TableAddress;
		public MachineRegister IndexRegister;

		public VectorUse(Address tblAddr, MachineRegister idxReg)
		{
			TableAddress = tblAddr;
			IndexRegister = idxReg;
		}
	}
}
