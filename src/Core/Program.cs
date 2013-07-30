#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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
using Decompiler.Core.Expressions;
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
		private LoadedImage image;
		private SortedList<Address,Procedure> procedures;
		private CallGraph callGraph;
        private SortedList<Address, ImageMapVectorTable> vectors;
        private Dictionary<uint, PseudoProcedure> mpuintfn;
        private Dictionary<uint, PseudoProcedure> trampolines;
		private Identifier globals;
        private StructureType globalFields;
		private Dictionary<string, PseudoProcedure> pseudoProcs;
        private Dictionary<Identifier, LinearInductionVariable> ivs;
		private TypeFactory typefactory;
        private ImageMap imageMap;

		public Program()
		{
			this.procedures = new SortedList<Address,Procedure>();
            this.vectors = new SortedList<Address, ImageMapVectorTable>();
			this.callGraph = new CallGraph();
            this.mpuintfn = new Dictionary<uint, PseudoProcedure>();		// uint (offset) -> string
			this.trampolines = new Dictionary<uint, PseudoProcedure>();	// linear address -> string
            this.pseudoProcs = new Dictionary<string, PseudoProcedure>();
            this.ivs = new Dictionary<Identifier, LinearInductionVariable>();
			this.typefactory = new TypeFactory();
			this.TypeStore = new TypeStore();
		}

		public IProcessorArchitecture Architecture { get; set; }

		public CallGraph CallGraph
		{
			get { return callGraph; }
		}

		public void DumpAssembler(TextWriter wr)
		{
			if (wr == null || Architecture == null)
				return;
			Dumper dump = new Dumper(Architecture);
			dump.Dump(this, ImageMap, wr);
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
                    EnsureGlobals();
                }
                return globals; 
            } 
		}

        private void EnsureGlobals()
        {
            if (Architecture == null)
                throw new InvalidOperationException("The program's Architecture property must be set before accessing the Globals property.");
            globalFields = TypeFactory.CreateStructureType("Globals", 0);
            var ptrGlobals = TypeFactory.CreatePointer(globalFields, Architecture.PointerType.Size);
            globals = new Identifier("globals", 0,  ptrGlobals, new MemoryStorage());
        }
		
        /// <summary>
        /// The unpacked, relocated, in-memory image of the program to be decompiled.
        /// </summary>
		public LoadedImage Image
		{
			get { return image; }
            set { image = value; }
		}

        public ImageMap ImageMap
        {
            get { return imageMap; }
            set { imageMap = value; }
        }

		public Dictionary<uint, PseudoProcedure> ImportThunks
		{
			get { return mpuintfn; }
		}

        public Dictionary<Identifier, LinearInductionVariable> InductionVariables
        {
            get { return ivs; }
        }

		public Platform Platform { get; set; }

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

        public Dictionary<uint, PseudoProcedure> Trampolines
		{
			get { return trampolines; }
		}

		public TypeFactory TypeFactory
		{
			get { return typefactory; }
		}
		
		public TypeStore TypeStore { get; private set; }

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
		public RegisterStorage IndexRegister;

		public VectorUse(Address tblAddr, RegisterStorage idxReg)
		{
			TableAddress = tblAddr;
			IndexRegister = idxReg;
		}
	}
}
