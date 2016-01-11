#region License
/* 
 * Copyright (C) 1999-2016 John K�ll�n.
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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using Reko.Core.Serialization;
using Reko.Core.CLanguage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Contains information about one input file, gathered during loading, scanning and data analysis,
    /// as well as storing any user-specified information.
    /// </summary>
    /// <remarks>
    /// A Decompiler project may consist of several of these Programs.
    /// </remarks>
    [Designer("Reko.Gui.Design.ProgramDesigner,Reko.Gui")]
    public class Program
    {
        private SortedList<Address, ImageMapVectorTable> vectors;
        private Identifier globals;
        private StructureType globalFields;
        private Dictionary<string, PseudoProcedure> pseudoProcs;

        public Program()
        {
            this.EntryPoints = new List<EntryPoint>();
            this.FunctionHints = new List<Address>();
            this.Procedures = new SortedList<Address, Procedure>();
            this.vectors = new SortedList<Address, ImageMapVectorTable>();
            this.CallGraph = new CallGraph();
            this.ImportReferences = new Dictionary<Address, ImportReference>(new Address.Comparer());		// uint (offset) -> string
            this.InterceptedCalls = new Dictionary<Address, ExternalProcedure>(new Address.Comparer());
            this.pseudoProcs = new Dictionary<string, PseudoProcedure>();
            this.InductionVariables = new Dictionary<Identifier, LinearInductionVariable>();
            this.TypeFactory = new TypeFactory();
            this.TypeStore = new TypeStore();
            this.Resources = new ProgramResourceGroup();
            this.User = new UserData();
        }

        public Program(LoadedImage image, ImageMap imageMap, IProcessorArchitecture arch, Platform platform) : this()
        {
            this.Image = image;
            this.ImageMap = imageMap;
            this.Architecture = arch;
            this.Platform = platform;
        }

        public string Name { get; set; }

        /// <summary>
        /// The processor architecture to use for decompilation
        /// </summary>
		public IProcessorArchitecture Architecture { get; set; }

        /// <summary>
        /// The callgraph expresses the relationships between the callers (statements and procedures)
        /// and their callees (procedures).
        /// </summary>
        public CallGraph CallGraph { get; private set; }

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

        public Serialization.Procedure_v1 EnsureUserProcedure(Address address, string name)
        {
            Serialization.Procedure_v1 up;
            if (!User.Procedures.TryGetValue(address, out up))
            {
                up = new Serialization.Procedure_v1
                {
                    Address = address.ToString(),
                    Name = name,
                };
                User.Procedures.Add(address, up);
            }
            return up;
        }

        /// <summary>
        /// Represents a _pointer_ to a structure that contains all the 
        /// global variables of the program. 
        /// </summary>
        /// <remarks>
        /// This property is used heavily in the type inference phases of 
        /// the decompiler to provide a place in which to collect global 
        /// variables.
        /// </remarks>
        public Identifier Globals
        {
            get
            {
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
            var ptrGlobals = TypeFactory.CreatePointer(globalFields, Platform.PointerType.Size);
            globals = new Identifier("globals", ptrGlobals, new MemoryStorage());
        }

        /// <summary>
        /// The unpacked, relocated, in-memory image of the program to be decompiled.
        /// </summary>
        public LoadedImage Image { get; set; }

        public ImageMap ImageMap { get; set; }

        public Platform Platform { get; set; }

        /// <summary>
        /// The list of known entry points to the program.
        /// </summary>
        public List<EntryPoint> EntryPoints { get; private set; }

        /// <summary>
        /// List of function hints.
        /// </summary>
        public List<Address> FunctionHints
        {
            get; private set;
        }
        public string Filename { get; set; }

        /// <summary>
        /// A collection of memory locations and the external library references
        /// they each refer to.
        /// </summary>
		public Dictionary<Address, ImportReference> ImportReferences { get; private set; }

        /// <summary>
        /// A collection of pseudo-addresses and the external library function
        /// they should be resolved to.
        /// </summary>
        /// <remarks>
        /// This is used by platform emulators to handle functions like Win32's 
        /// GetProcAddress(). When the emulator is called with GetProcAddress()
        /// it will return a new pseudo-address guaranteed not to be a valid 
        /// address in the LoadedImage. Later, when a call is made to that 
        /// pseudo-address, it is translated to one of the registered 
        /// ExternalProcedures.
        /// </remarks>
        public Dictionary<Address, ExternalProcedure> InterceptedCalls { get; private set; }

        //$REVIEW: shouldnt these belong in Procedure?
        public Dictionary<Identifier, LinearInductionVariable> InductionVariables { get; private set; }

        /// <summary>
        /// The program's decompiled procedures, indexed by address.
        /// </summary>
        public SortedList<Address, Procedure> Procedures { get; private set; }

        /// <summary>
        /// The program's pseudo procedures, indexed by name.
        /// </summary>
        public Dictionary<string, PseudoProcedure> PseudoProcedures
        {
            get { return pseudoProcs; }
        }

        /// <summary>
        /// List of resources stored in the binary. Some executable file formats support the
        /// inclusion of resources in the binary itself (MacOS classic resource forks also count)
        /// </summary>
        public ProgramResourceGroup Resources { get; private set; }
    
		public TypeFactory TypeFactory { get; private set; }
		
		public TypeStore TypeStore { get; private set; }

        /// <summary>
        /// User-specified data.
        /// </summary>
        public UserData User { get; set; }

		/// <summary>
		/// Provides access to the program's jump and call tables, sorted by address.
		/// </summary>
        //$REVIEW: is this ever used? What for?
		public SortedList<Address, ImageMapVectorTable> Vectors
		{
			get { return vectors; }
		}

        /// <summary>
        /// The name of the file in which disassemblies are dumped.
        /// </summary>
        public string DisassemblyFilename { get; set; }

        /// <summary>
        /// The name of the file in which intermediate results are stored.
        /// </summary>
        public string IntermediateFilename { get; set; }

        /// <summary>
        /// The name of the file in which final output is stored
        /// </summary>
        public string OutputFilename { get; set; }

        /// <summary>
        /// The name of the file in which recovered types are written.
        /// </summary>
        public string TypesFilename { get; set; }

        /// <summary>
        /// The name of the file in which the global variables are written.
        /// </summary>
        public string GlobalsFilename { get; set; }

        public void EnsureFilenames(string fileName)
        {
            this.DisassemblyFilename = DisassemblyFilename ?? Path.ChangeExtension(fileName, ".asm");
            this.IntermediateFilename = IntermediateFilename ?? Path.ChangeExtension(fileName, ".dis");
            this.OutputFilename = OutputFilename ?? Path.ChangeExtension(fileName, ".c");
            this.TypesFilename = TypesFilename ?? Path.ChangeExtension(fileName, ".h");
            this.GlobalsFilename = GlobalsFilename ?? Path.ChangeExtension(fileName, ".globals.c");
        }
       
        public ImageReader CreateImageReader(Address addr)
        {
            return Architecture.CreateImageReader(Image, addr);
        }

        public IEnumerable<MachineInstruction> CreateDisassembler(Address addr)
        {
            return Architecture.CreateDisassembler(
                Architecture.CreateImageReader(Image, addr));
        }

        // Mutators /////////////////////////////////////////////////////////////////

        /// <summary>
        /// This method is called when the user has created a global item.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public ImageMapItem AddUserGlobalItem(Address address, DataType dataType)
        {
            var size = GetDataSize(address, dataType);
            var item = new ImageMapItem
            {
                Address = address,
                Size = size,
                DataType = dataType,
            };
            if (size != 0)
                this.ImageMap.AddItemWithSize(address, item);
            else
                this.ImageMap.AddItem(address, item);

            this.User.Globals.Add(address, new Serialization.GlobalDataItem_v2
            {
                Address = address.ToString(),
                DataType = dataType.Accept(new  Serialization.DataTypeSerializer()),
            });
            return item;
        }

        public uint GetDataSize(Address addr, DataType dt)
        {
            var strDt = dt as StringType;
            if (strDt == null)
                return (uint)dt.Size;
            if (strDt.LengthPrefixType == null)
            {
                // Zero-terminated string.
                var rdr = this.Architecture.CreateImageReader(this.Image, addr);
                while (rdr.IsValid)
                {
                    var ch = rdr.ReadChar(strDt.ElementType);
                    if (ch == 0)
                        break;
                }
                return (uint)(rdr.Address - addr);
            }
            throw new NotImplementedException();
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
