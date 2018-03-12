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
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using Reko.Core.Serialization;
using Reko.Core.CLanguage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Reko.Core
{
    /// <summary>
    /// Contains information about one input file, gathered during loading, 
    /// scanning and data analysis, as well as storing any user-specified 
    /// information.
    /// </summary>
    /// <remarks>
    /// A Decompiler project may consist of several of these Programs.
    /// </remarks>
    [Designer("Reko.Gui.Design.ProgramDesigner,Reko.Gui")]
    public class Program
    {
        private Identifier globals;
        private Encoding encoding;

        public Program()
        {
            this.EntryPoints = new SortedList<Address, ImageSymbol>();
            this.ImageSymbols = new SortedList<Address, ImageSymbol>();
            this.Procedures = new SortedList<Address, Procedure>();
            this.CallGraph = new CallGraph();
            this.EnvironmentMetadata = new TypeLibrary();
            this.ImportReferences = new Dictionary<Address, ImportReference>(new Address.Comparer());		// uint (offset) -> string
            this.InterceptedCalls = new Dictionary<Address, ExternalProcedure>(new Address.Comparer());
            this.PseudoProcedures = new Dictionary<string, Dictionary<FunctionType, PseudoProcedure>>();
            this.InductionVariables = new Dictionary<Identifier, LinearInductionVariable>();
            this.TypeFactory = new TypeFactory();
            this.TypeStore = new TypeStore();
            this.Resources = new ProgramResourceGroup();
            this.User = new UserData();
            this.GlobalFields = TypeFactory.CreateStructureType("Globals", 0);
            // Most binary images don't have procedure information left.
            this.NeedsScanning = true;
            // Most binary images are not in SSA form.
            this.NeedsSsaTransform = true;
            // Most binary images have no type information.
            this.NeedsTypeReconstruction = true;
        }

        public Program(SegmentMap segmentMap, IProcessorArchitecture arch, IPlatform platform) : this()
        {
            this.SegmentMap = segmentMap;
            this.ImageMap = segmentMap.CreateImageMap();
            this.Architecture = arch;
            this.Platform = platform;
        }

        public string Name { get; set; }

        /// <summary>
        /// The processor architecture to use for decompilation
        /// </summary>
		public IProcessorArchitecture Architecture { get; set; }

        public IPlatform Platform { get; set; }

        /// <summary>
        /// The image map stores items discovered by the scanner phase.
        /// </summary>
        public ImageMap ImageMap { get; set; }

        /// <summary>
        /// Contains the segments that the binary consists of.
        /// </summary>
        public SegmentMap SegmentMap { get; set; }

        /// <summary>
        /// Metadata obtained from the environment -- not
        /// from the decompilation of the program itself.
        /// </summary>
        public TypeLibrary EnvironmentMetadata { get; set; }

        /// <summary>
        /// The callgraph expresses the relationships between the callers (statements and procedures)
        /// and their callees (procedures).
        /// </summary>
        public CallGraph CallGraph { get; private set; }

        /// <summary>
        /// Represents a _fictitious_ pointer to a structure that contains all the 
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

        public StructureType GlobalFields { get; private set; }

        /// <summary>
        /// Default encoding to use when interpreting sequences of bytes as text.
        /// </summary>
        public Encoding TextEncoding
        {
            get
            {
                if (this.encoding != null)
                    return this.encoding;
                Encoding e;
                if (User.TextEncoding != null)
                    e = User.TextEncoding;
                else
                    e = Platform.DefaultTextEncoding;
                this.encoding = Encoding.GetEncoding(
                    e.CodePage,
                    new EncoderReplacementFallback(),
                    new CustomDecoderFallback());
                return this.encoding;
            }
        }

        /// <summary>
        /// Image loaders can tell the Reko decompiler whether scanning is 
        /// needed to discover the procedures and basic blocks of the source 
        /// binary. If this property is set to false, the image loader will
        /// have populated the Procedures collection with the IR code of all
        /// the procedures of the binary.
        /// </summary>
        [DefaultValue(true)]
        public bool NeedsScanning { get; set; }

        /// <summary>
        /// Image loaders can tell the Reko decompiler whether the procedures
        /// in the Procedures collection require SSA analysis or just a simple
        /// graph generation.
        /// </summary>
        /// <remarks>
        /// Some formats, like LLVM IR, are already in SSA form, so an SSA
        /// analysis is redundant. Note that if this property is false, it
        /// implies that the NeedsScanning property must also be false.
        /// </remarks>
        [DefaultValue(true)]
        public bool NeedsSsaTransform { get; set; }

        /// <summary>
        /// Some image formats have detailed type information that makes Reko's
        /// type inference unnecessary.
        /// </summary>
        [DefaultValue(true)]
        public bool NeedsTypeReconstruction { get; set; }

        private void EnsureGlobals()
        {
            if (Platform == null)
                throw new InvalidOperationException("The program's Platform property must be set before accessing the Globals property.");
            var ptrGlobals = TypeFactory.CreatePointer(GlobalFields, Platform.PointerType.Size);
            globals = new Identifier("globals", ptrGlobals, MemoryStorage.Instance);
        }

        /// <summary>
        /// Creates a symbol table for this program populated with the types 
        /// defined by the platform of the program and user-defined types.
        /// </summary>
        /// <returns>Prepopulated symbol table.
        /// </returns>
        public virtual SymbolTable CreateSymbolTable()
        {
            var namedTypes = new Dictionary<string, SerializedType>();
            var typedefs = EnvironmentMetadata.Types;
            var dtSer = new DataTypeSerializer();
            foreach (var typedef in typedefs)
            {
                namedTypes.Add(typedef.Key, typedef.Value.Accept(dtSer));
            }
            return new SymbolTable(Platform, namedTypes);
        }

        public ProcedureSerializer CreateProcedureSerializer()
        {
            var typeLoader = new TypeLibraryDeserializer(Platform, true, EnvironmentMetadata.Clone());
            return new ProcedureSerializer(Platform, typeLoader, Platform.DefaultCallingConvention);
        }

        public TypeLibraryDeserializer CreateTypeLibraryDeserializer()
        {
            return new TypeLibraryDeserializer(Platform, true, EnvironmentMetadata.Clone());
        }

        /// <summary>
        /// The entry points to the program.
        /// </summary>
        public SortedList<Address, ImageSymbol> EntryPoints { get; private set; }

        /// <summary>
        /// The name of the file from which this Program was loaded.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// All the image locations that the loader was aware of.
        /// </summary>
        /// <remarks>
        /// Starting with these locations, Reko can find more by scanning
        /// the image both recursively and by using heuristics.
        /// </remarks>
        public SortedList<Address, ImageSymbol> ImageSymbols { get; private set; }

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
        /// The program's pseudo procedures, indexed by name and by signature.
        /// </summary>
        public Dictionary<string, Dictionary<FunctionType, PseudoProcedure>> PseudoProcedures { get; private set; }

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
        /// The name of the file in which disassemblies are dumped.
        /// </summary>
        public string DisassemblyFilename { get; set; }

        /// <summary>
        /// The nsame of the file in which intermediate results are stored.
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
       
        // Convenience functions.
        public EndianImageReader CreateImageReader(Address addr)
        {
            if (!SegmentMap.TryFindSegment(addr, out var segment))
                 throw new ArgumentException(string.Format("The address {0} is invalid.", addr));
            return Architecture.CreateImageReader(segment.MemoryArea, addr);
        }

        public ImageWriter CreateImageWriter(Address addr)
        {
            if (!SegmentMap.TryFindSegment(addr, out var segment))
                throw new ArgumentException(string.Format("The address {0} is invalid.", addr));
            return Architecture.CreateImageWriter(segment.MemoryArea, addr);
        }

        public IEnumerable<MachineInstruction> CreateDisassembler(Address addr)
        {
            if (!SegmentMap.TryFindSegment(addr, out var segment))
                throw new ArgumentException(string.Format("The address {0} is invalid.", addr));
            return Architecture.CreateDisassembler(
                Architecture.CreateImageReader(segment.MemoryArea, addr));
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
            //$TODO: if user enters a segmented address, we need to 
            // place the item in the respective globals struct.
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
                DataType = dataType.Accept(new Serialization.DataTypeSerializer()),
            });
            return item;
        }

        /// <summary>
        /// Seed the imagemap with image symbols 
        /// </summary>
        public void BuildImageMap()
        {
            if (!this.NeedsScanning)
                return;
            this.ImageMap = SegmentMap.CreateImageMap();
            foreach (var sym in this.ImageSymbols.Values.Where(
                s => s.Type == SymbolType.Data && s.Size != 0))
            {
                this.ImageMap.AddItemWithSize(
                    sym.Address,
                    new ImageMapItem
                    {
                        Address = sym.Address,
                        DataType = sym.DataType,
                        Size = sym.Size,
                    });
            }
            var tlDeser = CreateTypeLibraryDeserializer();
            foreach (var kv in User.Globals)
            {
                var dt = kv.Value.DataType.Accept(tlDeser);
                var item = new ImageMapItem((uint)dt.Size)
                {
                    Address = kv.Key,
                    DataType = dt,
                    Name = kv.Value.Name,
                };
                if (item.Size > 0)
                {
                    this.ImageMap.AddItemWithSize(kv.Key, item);
                }
                else
                {
                    this.ImageMap.AddItem(kv.Key, item);
                }
                //$BUGBUG: what about x86 segmented binaries?
                int offset = (int)kv.Key.ToLinear();
                GlobalFields.Fields.Add(offset, dt, kv.Value.Name);
            }
        }

        public uint GetDataSize(Address addr, DataType dt)
        {
            var strDt = dt as StringType;
            if (strDt == null)
                return (uint)dt.Size;
            if (strDt.LengthPrefixType == null)
            {
                // Zero-terminated string.
                var rdr = this.CreateImageReader(addr);
                while (rdr.IsValid && !rdr.ReadNullCharTerminator(strDt.ElementType))
                    ;
                return (uint)(rdr.Address - addr);
            }
            else
            {
                return (uint)(strDt.Size + strDt.Size);
            }
        }

        public Address GetProcedureAddress(Procedure proc)
        {
            return Procedures.Where(de => de.Value == proc)
                .Select(de => de.Key)
                .FirstOrDefault();
        }

        public PseudoProcedure EnsurePseudoProcedure(string name, DataType returnType, params Expression[] args)
        {
            var sig = MakeSignatureFromApplication(returnType, args);
            return EnsurePseudoProcedure(name, sig);
        }

        private static FunctionType MakeSignatureFromApplication(DataType returnType, Expression[] args)
        {
            return new FunctionType(
                new Identifier("", returnType, null),
                args.Select((arg, i) => IdFromExpression(arg, i)).ToArray());
        }

        private static Identifier IdFromExpression(Expression arg, int i)
        {
            var id = arg as Identifier;
            var stg = id?.Storage;
            return new Identifier("", arg.DataType, stg);
        }

        public PseudoProcedure EnsurePseudoProcedure(string name, FunctionType sig)
        {
            if (!PseudoProcedures.TryGetValue(name, out var de))
            {
                de = new Dictionary<FunctionType, PseudoProcedure>(new DataTypeComparer());
                PseudoProcedures[name] = de;
            }
            if (!de.TryGetValue(sig, out var intrinsic))
            {
                intrinsic = new PseudoProcedure(name, sig);
                de.Add(sig, intrinsic);
            }
            return intrinsic;
        }

        public Procedure_v1 EnsureUserProcedure(Address address, string name, bool decompile = true)
        {
            if (!User.Procedures.TryGetValue(address, out var up))
            {
                up = new Procedure_v1
                {
                    Address = address.ToString(),
                    Name = name,
                    Decompile = decompile,
                };
                User.Procedures.Add(address, up);
            }
            return up;
        }

        public GlobalDataItem_v2 ModifyUserGlobal(Address address, SerializedType dataType, string name)
        {
            if (!User.Globals.TryGetValue(address, out var gbl))
            {
                gbl = new GlobalDataItem_v2()
                {
                    Address = address.ToString(),
                };
                User.Globals.Add(address, gbl);
            }

            gbl.Name = name;
            gbl.DataType = dataType;

            this.ImageMap.RemoveItem(address);

            var tlDeser = CreateTypeLibraryDeserializer();
            var dt = dataType.Accept(tlDeser);
            var size = GetDataSize(address, dt);
            var item = new ImageMapItem
            {
                Address = address,
                Size = size,
                Name = name,
                DataType = dt,
            };
            if (size != 0)
                this.ImageMap.AddItemWithSize(address, item);
            else
                this.ImageMap.AddItem(address, item);

            return gbl;
        }

        public void RemoveUserGlobal(Address address)
        {
            User.Globals.Remove(address);
            // Do not remove block data item
            if (ImageMap.TryFindItemExact(address, out var item) && item is ImageMapBlock)
                return;
            ImageMap.RemoveItem(address);
        }

        //$REVIEW: why not always call this from Reko.Decompiler right before 
        // scanning, in Decompiler.BuildImageMaps?
        public void Reset()
        {
            Procedures.Clear();
            globals = null;
            TypeFactory = new TypeFactory();
            TypeStore.Clear();
            GlobalFields = TypeFactory.CreateStructureType("Globals", 0);
            BuildImageMap();
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
