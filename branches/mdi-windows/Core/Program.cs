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

using Decompiler.Core.Code;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
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
		private SortedList<Address,Procedure> procedures;
		private CallGraph callGraph;
        private SortedList<Address, ImageMapVectorTable> vectors;
        private Dictionary<uint, PseudoProcedure> mpuintfn;
        private Dictionary<int, PseudoProcedure> trampolines;
		private Identifier globals;
		private Dictionary<string, PseudoProcedure> pseudoProcs;
        private Dictionary<Identifier, LinearInductionVariable> ivs;
		private TypeFactory typefactory;
		private TypeStore typeStore;

		public Program()
		{
			procedures = new SortedList<Address,Procedure>();
            vectors = new SortedList<Address, ImageMapVectorTable>();
			callGraph = new CallGraph();
            mpuintfn = new Dictionary<uint, PseudoProcedure>();		// uint (offset) -> string
			trampolines = new Dictionary<int, PseudoProcedure>();	// address -> string
            pseudoProcs = new Dictionary<string, PseudoProcedure>();
            ivs = new Dictionary<Identifier, LinearInductionVariable>();
			typefactory = new TypeFactory();
			typeStore = new TypeStore();
		}

		public void AddEntryPoint(EntryPoint ep)
		{
			Procedure proc; 
			if (!procedures.TryGetValue(ep.Address, out proc))
			{
				proc = Procedure.Create(ep.Name, ep.Address, arch.CreateFrame());
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
			if (wr == null || arch == null)
				return;
			Dumper dump = arch.CreateDumper();
			dump.Dump(this, Image.Map, wr);
		}

        public PseudoProcedure EnsurePseudoProcedure(string name, DataType returnType, int arity)
		{
            PseudoProcedure p;
            if (!pseudoProcs.TryGetValue(name, out p))
            {
                p = new PseudoProcedure(name, returnType, arity);
				pseudoProcs[name] = p;
			}
			return p;
		}

		public Identifier Globals
		{
			get {
                if (globals == null)
                {
                    if (arch == null)
                        throw new InvalidOperationException("The program's Architecture property must be set before accessing the Globals property.");
                    globals = new Identifier("globals", 0, arch.PointerType, new MemoryStorage()); 
                }
                return globals; 
            } 
		}
		
        /// <summary>
        /// The unpacked, relocated, in-memory image of the program to be decompiled.
        /// </summary>
		public ProgramImage Image
		{
			get { return image; }
			set { image = value; }
		}
		
		public Dictionary<uint, PseudoProcedure> ImportThunks
		{
			get { return mpuintfn; }
		}

        public Dictionary<Identifier, LinearInductionVariable> InductionVariables
        {
            get { return ivs; }
        }


		public Platform Platform
		{
			get { return platform; }
			set { platform = value; }
		}

		/// <summary>
		/// Provides access to the program's procedures, indexed by address.
		/// </summary>
		public SortedList<Address, Procedure> Procedures
		{
			get { return procedures; }
		}

		/// <summary>
		/// Provides access to the program's pseudo procedures, indexed by name.
		/// </summary>
		public Dictionary<string,PseudoProcedure> PseudoProcedures
		{
			get { return pseudoProcs; }
		}

        public Dictionary<int, PseudoProcedure> Trampolines
		{
			get { return trampolines; }
		}

		public TypeFactory TypeFactory
		{
			get { return typefactory; }
		}
		
		public TypeStore TypeStore
		{
			get { return typeStore; }
		}

		/// <summary>
		/// Provides access to the program's jump and call tables, sorted by address.
		/// </summary>
		public SortedList<Address, ImageMapVectorTable> Vectors
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
