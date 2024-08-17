#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

using Reko.Core.Expressions;
using Reko.Core.Hll.C;
using Reko.Core.Collections;
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Reko.Core.Text;
using Reko.Core.Graphs;
using Reko.Core.Analysis;
using System.Diagnostics.CodeAnalysis;
using System.Data;

namespace Reko.Core
{
    /// <summary>
    /// The central clearing house of information about one input file, 
    /// gathered during loading, scanning and data analysis, as well as 
    /// storing any user-specified information.
    /// </summary>
    /// <remarks>
    /// A Decompiler project may consist of several of these Programs.
    /// </remarks>
    [Designer("Reko.Gui.Design.ProgramDesigner,Reko.Gui")]
    public class Program : IReadOnlyProgram, ILoadedImage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public const string SingleFilePolicy = "SingleFile";
        public const string SegmentFilePolicy = "Segment";

        private IProcessorArchitecture archDefault;
        private Identifier globals;
        private Encoding encoding;

#nullable disable
        public Program()
        {
            this.Architectures = new Dictionary<string, IProcessorArchitecture>();
            this.EntryPoints = new SortedList<Address, ImageSymbol>();
            this.ImageSymbols = new SortedList<Address, ImageSymbol>();
            this.Procedures = new BTreeDictionary<Address, Procedure>();
            this.CallGraph = new CallGraph();
            this.EnvironmentMetadata = new TypeLibrary();
            this.ImportReferences = new Dictionary<Address, ImportReference>(new Address.Comparer());		// uint (offset) -> string
            this.InterceptedCalls = new Dictionary<Address, ExternalProcedure>(new Address.Comparer());
            this.Intrinsics = new Dictionary<string, Dictionary<FunctionType, IntrinsicProcedure>>();
            this.InductionVariables = new Dictionary<Identifier, LinearInductionVariable>();
            this.ExternalProcedures = new();
            this.TypeFactory = new TypeFactory();
            this.TypeStore = new TypeStore();
            this.Resources = new List<ProgramResource>();
            this.User = new UserData();
            this.GlobalFields = TypeFactory.CreateStructureType("Globals", 0);
            this.NamingPolicy = new NamingPolicy();

            // Most binary images don't have procedure information left.
            this.NeedsScanning = true;
            // Most binary images are not in SSA form.
            this.NeedsSsaTransform = true;
            // Most binary images have no type information.
            this.NeedsTypeReconstruction = true;
        }

        public Program(
            IMemory memory,
            IProcessorArchitecture arch, 
            IPlatform platform) : this()
        {
            this.Memory = memory;
            this.SegmentMap = Memory.SegmentMap;
            this.ImageMap = this.SegmentMap.CreateImageMap();
            this.Architecture = arch;
            this.Platform = platform;
        }

        public Program(
            IMemory memory,
            IProcessorArchitecture arch, 
            IPlatform platform,
            SortedList<Address, ImageSymbol> symbols,
            SortedList<Address, ImageSymbol> entryPoints) : this()
        {
            this.Memory = memory;
            this.SegmentMap = Memory.SegmentMap;
            this.ImageMap = SegmentMap.CreateImageMap();
            this.Architecture = arch;
            this.Platform = platform;
            this.ImageSymbols = symbols;
            this.EntryPoints = entryPoints;
        }

#nullable enable

        /// <summary>
        /// The program's file name and extension, but not its path.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The default processor architecture to use for decompilation.
        /// </summary>
        /// <remarks>
        /// Individual procedures may have different procedure architectures
        /// than the default architecture.
        /// </remarks>
		public IProcessorArchitecture Architecture
        {
            get { return archDefault; }
            set { SetDefaultArchitecture(value); }
        }

        /// <summary>
        /// The <see cref="IPlatform"/> describing the operating environment
        /// of the binary being decompiled.
        /// </summary>
        public IPlatform Platform { get; set; }

        /// <summary>
        /// The image map stores items discovered by the scanner phase.
        /// </summary>
        public ImageMap ImageMap { get; set; }

        /// <summary>
        /// Contains the segments that the binary consists of. This data
        /// is discovered by the loader, with optional additional input
        /// from the user.
        /// </summary>
        public SegmentMap SegmentMap { get; set; }

        /// <summary>
        /// Abstracts the program's contents loaded into memory.
        /// </summary>
        public IMemory Memory { get; set; }

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
                return globals!;
            }
        }

        /// <summary>
        /// The global variables of the program, represented as fields.
        /// </summary>
        public StructureType GlobalFields { get; private set; }


        /// <summary>
        /// If applicable, this property contains the ABI-defined
        /// global register, represented as a <see cref="RegisterStorage"/>.
        /// </summary>
        /// <remarks>
        /// For example, in the MIPS ABI, r28 is used as the global register. On
        /// PA-RISC, the r27 performs the same role.
        /// </remarks>
        public RegisterStorage? GlobalRegister { get; set; }

        /// <summary>
        /// If applicable, this property contains the value of the ABI-defined
        /// global register.
        /// </summary>
        /// <remarks>
        /// For example, in the MIPS ABI, r28 is used as the global register. On
        /// PA-RISC, the r27 performs the same role.
        /// </remarks>
        public Constant? GlobalRegisterValue { get; set; }

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
            var ptrGlobals = TypeFactory.CreatePointer(GlobalFields, Platform.PointerType.BitSize);
            globals = Identifier.Global("globals", ptrGlobals);
        }

        /// <summary>
        /// Creates a symbol table for this program populated with the types 
        /// defined by the platform of the program and user-defined types.
        /// </summary>
        /// <returns>Prepopulated symbol table.
        /// </returns>
        public virtual SymbolTable CreateSymbolTable()
        {
            var dtSer = new DataTypeSerializer();
            var primitiveTypes = PrimitiveType.AllTypes
                .ToDictionary(d => d.Key, d => (PrimitiveType_v1) d.Value.Accept(dtSer));
            var namedTypes = new Dictionary<string, SerializedType>();
            var typedefs = EnvironmentMetadata.Types;
            foreach (var typedef in typedefs)
            {
                namedTypes[typedef.Key] = typedef.Value.Accept(dtSer);
            }
            return new SymbolTable(Platform, primitiveTypes, namedTypes);
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

        public virtual ProcedureCharacteristics? LookupCharacteristicsByName(string procName)
        {
            if (EnvironmentMetadata.Characteristics.TryGetValue(
                procName,
                out var chr)
            )
                return chr;
            return Platform.LookupCharacteristicsByName(procName);
        }

        /// <summary>
        /// The processor architectures that exist in the Program. 
        /// </summary>
        /// <remarks>
        /// Normally there is only one architecture. But there are examples
        /// of binaries that have two or more processor architectures. E.g.
        /// "fat binaries" on MacOS, or ELF binaries with both ARM32 and 
        /// Thumb instructions.</remarks>
        public Dictionary<string, IProcessorArchitecture> Architectures { get; }

        /// <summary>
        /// The entry points to the program.
        /// </summary>
        public SortedList<Address, ImageSymbol> EntryPoints { get; set; }

        /// <summary>
        /// The location from which this Program was loaded.
        /// </summary>
        public ImageLocation Location { get; set; }

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
        /// The program's decompiled procedures, ordereds by address.
        /// </summary>
        public BTreeDictionary<Address, Procedure> Procedures { get; private set; }

        /// <summary>
        /// The program's intrinsic procedures, indexed by name and by signature.
        /// </summary>
        public Dictionary<string, Dictionary<FunctionType, IntrinsicProcedure>> Intrinsics { get; private set; }

        /// <summary>
        /// The program's imported external demangled procedures. Contains
        /// tuple of dll name, calling convention and ExtrernalProcedure,
        /// indexed by mangled import name. External declarations from system
        /// or user-defined metafiles aren't here to avoid dublication.
        /// </summary>
        public Dictionary<string, (string?, string?, ExternalProcedure)> ExternalProcedures { get; private set; }

        /// <summary>
        /// List of resources stored in the binary. Some executable file formats support the
        /// inclusion of resources in the binary itself (MacOS classic resource forks also count)
        /// </summary>
        public List<ProgramResource> Resources { get; private set; }
    
		public TypeFactory TypeFactory { get; private set; }
		
		public TypeStore TypeStore { get; private set; }

        /// <summary>
        /// User-specified data.
        /// </summary>
        public UserData User { get; set; }

        IReadOnlyUserData IReadOnlyProgram.User => this.User;

        /// <summary>
        /// Absolute path of the directory into which disassemblies are dumped.
        /// </summary>
        public string DisassemblyDirectory { get; set; }

        /// <summary>
        /// Absolute path of the directory into which final source code is stored
        /// </summary>
        public string SourceDirectory { get; set; }

        /// <summary>
        /// Absolute path of the directory into which type definitions are stored.
        /// </summary>
        public string IncludeDirectory { get; set; }

        /// <summary>
        /// Absolute path the directory in which embedded resources will be written.
        /// </summary>
        public string ResourcesDirectory { get; set; }

        public T Accept<T, C>(ILoadedImageVisitor<T, C> visitor, C context)
            => visitor.VisitProgram(this, context);

        /// <summary>
        /// Given the absolute file name of a binary being decompiled, make sure that 
        /// absolute file names for each of the output directories.
        /// </summary>
        /// <param name="absFileName">Absolute URI of the binary being decompiled.</param>
        public void EnsureDirectoryNames(ImageLocation imageLocation)
        {
            //$TODO how to handle nested archives? 
            var absFileName = imageLocation.FilesystemPath;
            var dir = Path.GetDirectoryName(absFileName) ?? "";
            var filename = Path.GetFileName(absFileName);
            var outputDir = Path.Combine(dir, Path.ChangeExtension(filename, ".reko"));
            this.DisassemblyDirectory = DisassemblyDirectory ?? outputDir;
            this.SourceDirectory = SourceDirectory ?? outputDir;
            this.IncludeDirectory = IncludeDirectory ?? outputDir;
            this.ResourcesDirectory = ResourcesDirectory ?? Path.Combine(outputDir, "resources");
        }

        /// <inheritdoc/>
        public NamingPolicy NamingPolicy { get; set; }

        /// <summary>
        /// Range of procedures to use for typing.
        /// </summary>
        /// <remarks>
        /// Used for debugging regressions in type inference.
        /// </remarks>
        public (int, int) DebugProcedureRange { get; set; }

        // Convenience functions.

        /// <summary>
        /// Creates an <see cref="EndianByteImageReader"/> of the appropriate
        /// endianness for the given <see cref="IProcessorArchitecture"/>.
        /// </summary>
        /// <param name="arch"><see cref="IProcessorArchitecture"/> whose endianness
        /// is used.</param>
        /// <param name="addr"><see cref="Address"/> at which to start reading.</param>
        /// <param name="rdr">An <see cref="EndianByteImageReader"/> spanning the memory area 
        /// starting at <paramref name="addr"> and ending at the end of the segment in which
        /// the address is located.
        /// </param>
        /// <returns>True if the address was a valid memory address, false if not.</returns>
        public bool TryCreateImageReader(
            IProcessorArchitecture arch,
            Address addr, 
            [MaybeNullWhen(false)] out EndianImageReader rdr)
        {
            return arch.Endianness.TryCreateImageReader(this.Memory, addr, out rdr);
        }

        public bool TryCreateImageReader(Address addr, [MaybeNullWhen(false)] out EndianImageReader rdr) =>
            TryCreateImageReader(this.Architecture, addr, out rdr);

        /// <summary>
        /// Creates an <see cref="EndianByteImageReader"/> of the appropriate
        /// endianness for the given <see cref="IProcessorArchitecture"/>.
        /// </summary>
        /// <param name="arch"><see cref="IProcessorArchitecture"/> whose endianness
        /// is used.</param>
        /// <param name="addrBegin"><see cref="Address"/> at which to start reading.</param>
        /// <param name="rdr">An <see cref="EndianByteImageReader"/> spanning the memory area 
        /// starting at <paramref name="addrBegin"> and ending at <paramref name="addrEnd" />.
        /// </param>
        /// <returns>True if the address was a valid memory address, false if not.</returns>
        public bool TryCreateImageReader(
            IProcessorArchitecture arch,
            Address addrBegin,
            long cUnits,
            [MaybeNullWhen(false)] out EndianImageReader rdr)
        {
            return arch.Endianness.TryCreateImageReader(this.Memory, addrBegin, cUnits, out rdr);
        }

        public bool TryCreateImageReader(Address addrBegin, long cUnits, [MaybeNullWhen(false)] out EndianImageReader rdr) =>
            TryCreateImageReader(this.Architecture, addrBegin, cUnits, out rdr);

        public ImageWriter CreateImageWriter(IProcessorArchitecture arch, Address addr)
        {
            if (!SegmentMap.TryFindSegment(addr, out var segment))
                throw new ArgumentException(string.Format("The address {0} is invalid.", addr));
            return arch.CreateImageWriter(segment.MemoryArea, addr);
        }

        public IEnumerable<MachineInstruction> CreateDisassembler(IProcessorArchitecture arch, Address addr)
        {
            if (!SegmentMap.TryFindSegment(addr, out var segment))
                throw new ArgumentException(string.Format("The address {0} is invalid.", addr));
            return arch.CreateDisassembler(
                arch.CreateImageReader(segment.MemoryArea, addr));
        }

        public Dictionary<ImageSegment, List<ImageMapItem>> GetItemsBySegment()
        {
            return (from seg in this.SegmentMap.Segments.Values
                    from item in this.ImageMap.Items.Values
                    where seg.IsInRange(item.Address) && !seg.IsHidden
                    group new { seg, item } by seg into g
                    orderby g.Key.Address
                    select new { g.Key, Items = g.Select(gg => gg.item) })
                .ToDictionary(a => a.Key, a => a.Items.OrderBy(i => i.Address).ToList());
        }


        IReadOnlyCallGraph IReadOnlyProgram.CallGraph => this.CallGraph;
        IReadOnlyDictionary<Identifier, LinearInductionVariable> IReadOnlyProgram.InductionVariables => this.InductionVariables;
        IReadOnlySegmentMap IReadOnlyProgram.SegmentMap => this.SegmentMap;


        public bool IsPtrToReadonlySection(Address addr)
        {
            if (!this.SegmentMap.TryFindSegment(addr, out ImageSegment? seg))
                return false;
            return (seg.Access & AccessMode.ReadWrite) == AccessMode.Read;
        }

        public bool TryInterpretAsAddress(Expression e, bool codeAlign, [MaybeNullWhen(false)] out Address addr)
        {
            if (e is Address a)
            {
                addr = a;
                return true;
            }
            if (e is Constant c)
            {
                addr = Platform.MakeAddressFromConstant(c, codeAlign);
                return addr is not null;
            }
            addr = null;
            return false;
        }


        // Mutators /////////////////////////////////////////////////////////////////

        public IProcessorArchitecture EnsureArchitecture(string archLabel, Func<string,IProcessorArchitecture> getter)
        {
            if (Architectures.TryGetValue(archLabel, out var arch))
                return arch;
            arch = getter(archLabel);
            Architectures[arch.Name] = arch;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Architectures)));
            return arch;
        }

        private void SetDefaultArchitecture(IProcessorArchitecture arch)
        {
            this.archDefault = arch;
            if (arch != null && !Architectures.ContainsKey(arch.Name))
            {
                Architectures.Add(arch.Name, arch);
            }
            //$REVIEW: raise an event if this option becomes user-available.
        }

        /// <summary>
        /// This method is called when the user has created a global item.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public ImageMapItem AddUserGlobalItem(IProcessorArchitecture arch, Address address, DataType dataType)
        {
            ImageMapItem item = AddGlobalItem(arch, address, dataType);

            this.User.Globals.Add(address, new UserGlobal(address, UserGlobal.GenerateDefaultName(address), dataType.Accept(new Serialization.DataTypeSerializer())));

            return item;
        }

        public ImageMapItem AddGlobalItem(IProcessorArchitecture arch, Address address, DataType dataType)
        {
            //$TODO: if user enters a segmented address, we need to 
            // place the item in the respective globals struct.
            var size = GetDataSize(arch, address, dataType);
            var item = new ImageMapItem(address)
            {
                Size = size,
                DataType = dataType,
            };
            if (size != 0)
                this.ImageMap.AddItemWithSize(address, item);
            else
                this.ImageMap.AddItem(address, item);
            return item;
        }

        /// <summary>
        /// Seed the <see cref="ImageMap"/> with image symbols 
        /// </summary>
        public void BuildImageMap()
        {
            if (!this.NeedsScanning)
                return;
            this.ImageMap = SegmentMap.CreateImageMap();
            foreach (var sym in this.ImageSymbols.Values.Where(
                s => s.Type == SymbolType.Data && s.DataType != null && s.DataType.BitSize != 0))
            {
                this.ImageMap.AddItemWithSize(
                    sym.Address!,
                    new ImageMapItem(sym.Address!)
                    {
                        DataType = sym.DataType!,
                        Size = (uint) sym.DataType!.Size,
                    });
            }
            var tlDeser = CreateTypeLibraryDeserializer();
            foreach (var kv in User.Globals)
            {
                var dt = kv.Value.DataType!.Accept(tlDeser);
                var size = GetDataSize(Architecture, kv.Key, dt);
                var item = new ImageMapItem(kv.Key, size)
                {
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
                AddGlobalField(kv.Key, dt, kv.Value.Name);
            }
        }

        public uint GetDataSize(IProcessorArchitecture arch, Address addr, DataType dt)
        {
            if (dt is not StringType strDt)
                return (uint) dt.Size;
            if (strDt.LengthPrefixType == null)
            {
                // Zero-terminated string.
                if (!this.TryCreateImageReader(arch, addr, out var rdr))
                    return 0;
                while (rdr.IsValid && !rdr.ReadNullCharTerminator(strDt.ElementType))
                    ;
                strDt.Length = (int) (rdr.Address - addr);
                return (uint) (rdr.Address - addr);
            }
            else
            {
                return (uint) (strDt.Size + strDt.Size);
            }
        }

        /// <summary>
        /// Ensure that there is a procedure at address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">The address at which there must be a procedure after 
        /// this method returns.
        /// </param>
        /// <param name="procedureName">The name of the procedure. If null or empty,
        /// this method will synthesize a new name.</param>
        /// <returns>
        /// The procedure, located at address <paramref name="addr"/>.
        /// </returns>
        public Procedure EnsureProcedure(IProcessorArchitecture arch, Address addr, string? procedureName)
        {
            if (this.Procedures.TryGetValue(addr, out Procedure proc))
                return proc;

            bool deduceSignatureFromName = !string.IsNullOrEmpty(procedureName);
            if (this.ImageSymbols.TryGetValue(addr, out ImageSymbol? sym))
            {
                deduceSignatureFromName |= !string.IsNullOrEmpty(procedureName);
                var generatedName = procedureName ?? sym.Name ?? this.NamingPolicy.ProcedureName(addr);
                if (generatedName.Length == 0)
                    generatedName = this.NamingPolicy.ProcedureName(addr);
                proc = Procedure.Create(arch, generatedName, addr, arch.CreateFrame());
                if (sym.Signature != null)
                {
                    var sser = this.CreateProcedureSerializer();
                    proc.Signature = sser.Deserialize(sym.Signature, proc.Frame)!;
                    deduceSignatureFromName = proc.Signature != null;
                }
            }
            else
            {
                var generatedName = procedureName ?? this.NamingPolicy.ProcedureName(addr);
                proc = Procedure.Create(arch, generatedName, addr, arch.CreateFrame());
            }

            if (deduceSignatureFromName)
            {
                var sProc = this.Platform.SignatureFromName(procedureName!);
                if (sProc != null)
                {
                    var loader = this.CreateTypeLibraryDeserializer();
                    var exp = loader.LoadExternalProcedure(sProc);
                    if (exp is {})
                    {
                        proc.Name = exp.Name;
                        proc.Signature = exp.Signature;
                        proc.EnclosingType = exp.EnclosingType;
                    }
                }
            }
            this.Procedures.Add(addr, proc);
            this.CallGraph.AddProcedure(proc);
            return proc;
        }


        public IntrinsicProcedure EnsureIntrinsicProcedure(string name, bool isIdempotent, FunctionType sig)
        {
            if (!Intrinsics.TryGetValue(name, out var de))
            {
                de = new Dictionary<FunctionType, IntrinsicProcedure>(new DataTypeComparer());
                Intrinsics[name] = de;
            }
            if (!de.TryGetValue(sig, out var intrinsic))
            {
                intrinsic = new IntrinsicProcedure(name, isIdempotent, sig);
                de.Add(sig, intrinsic);
            }
            return intrinsic;
        }

        public UserProcedure EnsureUserProcedure(Address address, string? name, bool decompile = true)
        {
            if (!User.Procedures.TryGetValue(address, out var up))
            {
                up = new UserProcedure(address, name ?? NamingPolicy.ProcedureName(address))
                {
                    Decompile = decompile,
                };
                User.Procedures.Add(address, up);
            }

            return up;
        }

        public UserGlobal ModifyUserGlobal(IProcessorArchitecture arch, Address address, SerializedType dataType, string name)
        {
            if (!User.Globals.TryGetValue(address, out var gbl))
            {
                gbl = new UserGlobal(address, name, dataType);
                User.Globals.Add(address, gbl);
            }
            else
            {
                gbl.Name = name;
                gbl.DataType = dataType;
            }

            this.ImageMap.RemoveItem(address);

            var tlDeser = CreateTypeLibraryDeserializer();
            var dt = dataType.Accept(tlDeser);
            var size = GetDataSize(arch, address, dt);
            var item = new ImageMapItem(address)
            {
                Size = size,
                Name = name,
                DataType = dt,
            };
            if (size != 0)
                this.ImageMap.AddItemWithSize(address, item);
            else
                this.ImageMap.AddItem(address, item);
            AddGlobalField(address, dt, name);
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
            globals = null!;
            TypeFactory = new TypeFactory();
            TypeStore.Clear();
            GlobalFields = TypeFactory.CreateStructureType("Globals", 0);
            BuildImageMap();
        }

        public void AddGlobalField(Address address, DataType dt, string name)
        {
            int offset;
            StructureFieldCollection fields;
            if (address.Selector.HasValue &&
                SegmentMap.TryFindSegment(address, out var seg))
            {
                offset = (int) address.Offset;
                fields = seg.Fields.Fields;
            }
            else
            {
                offset = (int) address.ToLinear();
                fields = GlobalFields.Fields;
            }
            var globalField = fields.AtOffset(offset);
            if (globalField != null)
            {
                fields.Remove(globalField);
            }
            fields.Add(offset, dt, name);
        }

        public DataType? FindGlobalField(Address address)
        {
            int offset;
            StructureFieldCollection fields;
            if (address.Selector.HasValue &&
                SegmentMap.TryFindSegment(address, out var seg))
            {
                offset = (int) address.Offset;
                fields = seg.Fields.Fields;
            }
            else
            {
                offset = (int) address.ToLinear();
                fields = GlobalFields.Fields;
            }
            var globalField = fields.AtOffset(offset);
            if (globalField != null)
            {
                return globalField.DataType;
            }
            return null;
        }

        /// <summary>
        /// Add new imported external procedure if there is not one with same
        /// import name.
        /// </summary>
        /// <param name="moduleName">
        /// Name of module of external procedure.
        /// </param>
        /// <param name="importName">
        /// Mangled import name of external procedure.
        /// </param>
        /// <param name="callingConvention">
        /// Calling convention of external procedure.
        /// </param>
        /// <param name="ep">
        /// <see cref="ExternalProcedure" /> object
        /// </param>
        public void EnsureExternalProcedure(
            string? moduleName, string importName, string? callingConvention,
            ExternalProcedure ep)
        {
            if (ExternalProcedures.ContainsKey(importName))
                return;
            ExternalProcedures.Add(
                importName, (moduleName, callingConvention, ep));
        }
    }
}
