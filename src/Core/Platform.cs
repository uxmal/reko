#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Core.Code;
using Reko.Core.Collections;
using Reko.Core.Configuration;
using Reko.Core.Emulation;
using Reko.Core.Expressions;
using Reko.Core.Hll.C;
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// A Platform is an abstraction of the operating environment,
    /// say MS-DOS, Win32, or Posix.
    /// </summary>
    public interface IPlatform
    {
        /// <summary>
        /// A string identifier for this platform.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Default architecture of this platform.
        /// </summary>
        IProcessorArchitecture Architecture { get; }

        /// <summary>
        /// The platform-defined calling conventions, organized by name.
        /// </summary>
        IReadOnlyDictionary<string, IReadOnlyCollection<string>> CallingConventions { get; }

        /// <summary>
        /// The name of the default calling convention of this platform.
        /// </summary>
        string DefaultCallingConvention { get; }

        /// <summary>
        /// Default encoding for text strings. This is used when reading strings
        /// from the program image.
        /// </summary>
        Encoding DefaultTextEncoding { get; set; }

        /// <summary>
        /// Human-readable description of the platform.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// The size of pointers used to access the stack frame in this platform.
        /// </summary>
        PrimitiveType FramePointerType { get; }

        /// <summary>
        /// Any platform-specific heuristics to use on this platform.
        /// </summary>
        PlatformHeuristics Heuristics { get; }

        /// <summary>
        /// Some platforms, especially microcomputers, have well-known
        /// procedures and global variables at absolute addresses. These are
        /// available in the MemoryMap.
        /// </summary>
        MemoryMap_v1? MemoryMap { get; set; }

        /// <summary>
        /// Unique identifier for this platform.
        /// </summary>
        string PlatformIdentifier { get; }

        /// <summary>
        /// The default pointer type for this platform.
        /// </summary>
        PrimitiveType PointerType { get; }

        /// <summary>
        /// Procedure prologs specific to this platform. Platform-independent
        /// prologs are found in <see cref="IProcessorArchitecture.ProcedurePrologs"/>.
        /// </summary>
        MaskedPattern[] ProcedurePrologs { get; }

        /// <summary>
        /// Creates a set of registers that the "standard" ABI cannot 
        /// guarantee will survive a call.
        /// </summary>
        /// <remarks>
        /// Reko will do its best to determine what registers are trashed by a
        /// procedure, but when indirect calls are involved we have to guess.
        /// If Reko's guess is incorrect, users can override it by proving
        /// oracular type information.
        /// </remarks>
        /// <returns>A set of registers</returns>
        IReadOnlySet<RegisterStorage> TrashedRegisters { get; }

        /// <summary>
        /// Creates a set of registers that the "standard" ABI guarantees will
        /// survive a call.
        /// </summary>
        /// <remarks>
        /// Reko will do its best to determine what registers are preserved by a
        /// procedure, but when indirect calls are involved we have to guess.
        /// If Reko's guess is incorrect, users can override it by proving
        /// oracular type information.
        /// </remarks>
        IReadOnlySet<RegisterStorage> PreservedRegisters { get; }

        /// <summary>
        /// The structure member alignment for this platform, measured in
        /// storage units (e.g. bytes). Objects larger than or equal to this
        /// size will be aligned at multiples of this size, smaller objects 
        /// are aligned to their natural positions.
        /// </summary>
        int StructureMemberAlignment { get; }

        /// <summary>
        /// Some architectures platforms (looking at you, ARM Thumb) will use addresses
        /// that are offset by 1. Most don't.
        /// </summary>
        /// <param name="addrCode"></param>
        /// <returns>Adjusted address</returns>
        Address AdjustProcedureAddress(Address addrCode);

        /// <summary>
        /// Creates a pointer scanner for this platform.
        /// </summary>
        /// <param name="map">Segment map describing the memory layout.</param>
        /// <param name="rdr">Image reader position at the start of the memory to be scanned.</param>
        /// <param name="addresses">Known addresses.</param>
        /// <param name="flags">Flags controlling the pointer scanner.</param>
        /// <returns>A new pointer scanner instance.</returns>
        IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> addresses, PointerScannerFlags flags);

        /// <summary>
        /// Loads the metadata for this platform into a <see cref="TypeLibrary"/>.
        /// </summary>
        /// <returns>A <see cref="TypeLibrary"/> with metadata for this platform.</returns>
        TypeLibrary CreateMetadata();

        /// <summary>
        /// Creates a platform emulator for this platform.
        /// </summary>
        /// <param name="segmentMap">Loaded program image.</param>
        /// <param name="importReferences">Imported procedures.</param>
        /// <returns>The created platform emulator.
        /// </returns>
        IPlatformEmulator CreateEmulator(SegmentMap segmentMap, Dictionary<Address, ImportReference> importReferences);

        /// <summary>
        /// Creates an empty SegmentMap based on the absolute memory map. It is 
        /// the caller's responsibility to fill in the MemoryArea properties
        /// of each resulting ImageSegment.
        /// </summary>
        /// <returns>A <see cref="SegmentMap"/> or null if this platform doesn't support 
        /// memory maps.</returns>
        SegmentMap? CreateAbsoluteMemoryMap();

        /// <summary>
        /// Create a <see cref="CParser"/> capable of parsing header files for this platform. 
        /// Platforms may have different dialects of C. 
        /// </summary>
        /// <returns>An instance of a <see cref="CParser"/>.</returns>
        CParser CreateCParser(TextReader rdr, ParserState? state = null);

        /// <summary>
        /// Given a procedure signature, determines whether it conforms to any
        /// of the platform's defined calling conventions.
        /// </summary>
        /// <remarks>
        /// Some platforms, like Win32, will have several well known 
        /// calling conventions. Others, like many ELF implementations,
        /// will have one and only one calling convention. On such platforms
        /// we will assume that the calling convention is represented by
        /// the empty string "". 
        /// </remarks>
        /// <param name="signature"></param>
        /// <param name="arch">Optional architecture that can be used to override
        /// the default architecture for situations when a <see cref="Program"/>
        /// has more than one architecture instantiated.
        /// </param>
        /// <returns>The name of the calling convention, or null
        /// if no calling convention could be determined.</returns>
        ICallingConvention? DetermineCallingConvention(FunctionType signature, IProcessorArchitecture? arch);

        /// <summary>
        /// Given a C basic type, returns the number of bits that type is
        /// represented with on this platform.
        /// </summary>
        /// <param name="cb">A C Basic type, like int, float etc.</param>
        /// <returns>Number of bytes used by this platform.
        /// </returns>
        int GetBitSizeFromCBasicType(CBasicType cb);

        /// <summary>
        /// Given the name of a calling convention and a return address size,
        /// creates an instance of a <see cref="ICallingConvention"/>.
        /// </summary>
        /// <param name="ccName">The name of the calling convention. Passing
        /// null or the empty string will use the default convention for this
        /// platform.</param>
        /// <returns>A <see cref="ICallingConvention"/> instance if one with a
        /// matching name was found.
        /// </returns>
        ICallingConvention? GetCallingConvention(string? ccName);

        /// <summary>
        /// Given a primitive type <paramref name="t"/> returns the
        /// rendering of that primitive in the programming language
        /// <paramref name="language"/>
        /// </summary>
        /// <param name="t">Primitive type</param>
        /// <param name="language">Programming language to use.</param>
        /// <returns></returns>
        string? GetPrimitiveTypeName(PrimitiveType t, string language);

        /// <summary>
        /// Determines whether the instructions <paramref name="clusters"/> 
        /// starting at address <paramref name="addrJumpInstr"/> are a "trampoline"
        /// or program linkage table (PLT) stub. 
        /// </summary>
        /// <param name="addrJumpInstr">The address at which the potential stub
        /// makes an indirect jump.</param>
        /// <param name="clusters">A sequence of lifted instructions ending
        /// at <paramref name="addrJumpInstr"/>.</param>
        /// <param name="host">An instance of <see cref="IRewriterHost"/> used
        /// to get details from the hosting environment.
        /// </param>
        /// <returns>A <see cref="Trampoline"/> instance , if a trampoline stub
        /// was identified, otherwise null.
        /// </returns>
        Trampoline? GetTrampolineDestination(Address addrJumpInstr, List<RtlInstructionCluster> clusters, IRewriterHost host);

        /// <summary>
        /// Determines whether the instructions <paramref name="instrs"/> 
        /// starting at address <paramref name="addrInstr"/> are a "trampoline"
        /// or program linkage table (PLT) stub. 
        /// </summary>
        /// <param name="addrInstr">The address at which the potential stub
        /// starts.</param>
        /// <param name="instrs">A sequence of rewritten instructions starting
        /// at <paramref name="addrInstr" />.</param>
        /// <param name="host">An instance of <see cref="IRewriterHost"/> used
        /// to get details from the hosting environment.
        /// </param>
        /// <returns>A <see cref="ProcedureBase"/> instance to the trampoline
        /// "target", if a trampoline stub was identified, otherwise null.
        /// </returns>
        ProcedureBase? GetTrampolineDestination(Address addrInstr, IEnumerable<RtlInstruction> instrs, IRewriterHost host);

        /// <summary>
        /// Given an executable entry point, find the initial value of the global
        /// pointer -- if the architecture supports it.
        /// </summary>
        /// <param name="program">Program in which to search</param>
        /// <param name="addrStart">The entrypoint according to the image.</param>
        /// <returns>null if no global pointer initialization code was found, otherwise the 
        /// an <see cref="Address"/> corresponding to the global pointer of this 
        /// program.</returns>
        Constant? FindGlobalPointerValue(Program program, Address addrStart);

        /// <summary>
        /// Given an executable entry point, find the location of the "main" program,
        /// bypassing any runtime startup code.
        /// </summary>
        /// <param name="program">Program in which to search</param>
        /// <param name="addrStart">The entrypoint according to the image.</param>
        /// <returns>null if no known runtime code was found, otherwise the 
        /// an <see cref="ImageSymbol"/> corresponding to the "real" user main procedure.</returns>
        ImageSymbol? FindMainProcedure(Program program, Address addrStart);

        /// <summary>
        /// Given a vector and the current processor state, finds a system
        /// service.
        /// </summary>
        /// <remarks>
        /// This method is used to resolve system calls or traps where 
        /// the actual service are selected by registers or stack values.
        /// </remarks>
        /// <param name="vector">Interrupt vector selected by the program code.</param>
        /// <param name="state">Register and stack values at the point where the system
        /// service was invoked.</param>
        /// <param name="memory">Memory state to use .</param>
        /// <returns>A <see cref="SystemService"/> instance if one can be found
        /// matching the system call criteria given by <paramref name="vector"/> and 
        /// <paramref name="state"/>; otherwise null.
        /// </returns>
        SystemService? FindService(int vector, ProcessorState? state, IMemory? memory);

        /// <summary>
        /// Given an <see cref="RtlInstruction"/> and the current processor state, finds a system
        /// service.
        /// </summary>
        /// <remarks>
        /// This method is used to resolve system calls or traps where 
        /// the actual service are selected by registers or stack values.
        /// </remarks>
        /// <param name="call">RTL instruction invoking the stystem call.</param>
        /// <param name="state">Register and stack values at the point where the system
        /// service was invoked.</param>
        /// <param name="memory">Memory state to use .</param>
        /// <returns>A <see cref="SystemService"/> instance if one can be found
        /// matching the system call criteria given by <paramref name="call"/> and 
        /// <paramref name="state"/>; otherwise null.
        /// </returns>
        SystemService? FindService(RtlInstruction call, ProcessorState? state, IMemory? memory);

        /// <summary>
        /// Finds a known dispatcher procedure at the given address.
        /// </summary>
        /// <param name="addr">Address of a known dispatcher procedure.</param>
        /// <returns>True if the platform defines a dispatcher procedure at the address;
        /// otherwise null.
        /// </returns>
        DispatchProcedure_v1? FindDispatcherProcedureByAddress(Address addr);

        /// <summary>
        /// Injects any platform specific instructions to the beginning 
        /// of a procedure.
        /// </summary>
        /// <param name="proc"><see cref="Procedure"/> into which instructions will be injected.</param>
        /// <param name="addr">The <see cref="Address"/> address of the procedure.</param>
        /// <param name="emitter">A <see cref="CodeEmitter"/> that can be used to generate IR code.</param>
        void InjectProcedureEntryStatements(Procedure proc, Address addr, CodeEmitter emitter);

        /// <summary>
        /// Determines whether a register is never used
        /// as arguments to a procedure. 
        /// </summary>
        /// <remarks>
        /// Typically, the stack pointer register is such a register.
        /// Some architectures or ABIs define global registers that are preserved 
        /// across calls; these should also be present in this set.
        /// </remarks>
        bool IsImplicitArgumentRegister(RegisterStorage reg);

        /// <summary>
        /// Returns true if a register _could_ be used as an explicit argument
        /// to a procedure. 
        /// </summary>
        /// <param name="reg">Register to be tested.</param>
        /// <returns>True if the register _could_ be used as an argument,
        /// otherwise false.</returns>
        /// <remarks>
        /// This method is used for heuristic determination of procedure 
        /// arguments (read "guessing"). Some platforms have multiple possible
        /// calling conventions, so we can't restrict ourselves to the
        /// "standard" calling convention of the platform.
        /// </remarks>
        bool IsPossibleArgumentRegister(RegisterStorage reg);

        /// <summary>
        /// Load user options for this platform.
        /// </summary>
        /// <param name="options">Options to load into the platform.</param>
        void LoadUserOptions(Dictionary<string, object> options);
        
        /// <summary>
        /// Returns the platform-defined procedure at the address <paramref name="address"/>.
        /// </summary>
        /// <param name="address"></param>
        /// <returns>If a procedure exists for this platform at the specified address, an instance of
        /// <see cref="ProcedureBase"/>. Otherwise, returns null.
        /// </returns>
        /// <remarks>
        /// This is useful for platforms that have ROMs with system procedures at well-defined addresses.
        /// </remarks>
        ProcedureBase? LookupProcedureByAddress(Address address);

        /// <summary>
        /// Look up an imported procedure by its name.
        /// </summary>
        /// <param name="moduleName">Optional module name.</param>
        /// <param name="procName">Name of the external procedure.</param>
        /// <returns>A known external procedure if one is found; otherwise null.
        /// </returns>
        ExternalProcedure? LookupProcedureByName(string? moduleName, string procName);

        /// <summary>
        /// Look up an imported procedure by its module name and ordinal.
        /// </summary>
        /// <param name="moduleName">Module name.</param>
        /// <param name="ordinal">Ordinal of the external procedure within the module.</param>
        /// <returns>A known external procedure if one is found; otherwise null.
        /// </returns>
        ExternalProcedure? LookupProcedureByOrdinal(string moduleName, int ordinal);

        /// <summary>
        /// Looks up an imported global variable by its name, and optionally 
        /// its module name.
        /// </summary>
        /// <param name="moduleName">Optional module name.</param>
        /// <param name="globalName">Name of the global variable.</param>
        /// <returns>An expression for the global variable if the name is resolved;
        /// otherwise null.
        /// </returns>
        Expression? ResolveImportByName(string? moduleName, string globalName);

        /// <summary>
        /// Looks up an imported global variable by its module name and ordinal.
        /// </summary>
        /// <param name="moduleName">Module name.</param>
        /// <param name="ordinal">Ordinal of the global variable.</param>
        /// <returns>An expression for the global variable if the name is resolved;
        /// otherwise null.
        /// </returns>
        Expression? ResolveImportByOrdinal(string moduleName, int ordinal);

        /// <summary>
        /// Given a procedure name, look up its characteristics.
        /// </summary>
        /// <param name="procName">Procedure name.</param>
        /// <returns>The procedure's characteristics; or null if there are none.
        /// </returns>
        ProcedureCharacteristics? LookupCharacteristicsByName(string procName);

        /// <summary>
        /// Given a constant <pararef name="c" /> attempt to make a corresponding <see cref="Address"/>.
        /// </summary>
        /// <param name="c">The <see cref="Constant"/> to convert.</param>
        /// <param name="codeAlign">If true, ensures that the resulting <see cref="Address"/> is correctly
        /// aligned for code addresses.</param>
        /// <returns>An <see cref="Address"/> if the platform supports the conversion, or null if not.
        /// </returns>
        Address? MakeAddressFromConstant(Constant c, bool codeAlign);

        /// <summary>
        /// Given a linear address, create an <see cref="Address"/>.
        /// </summary>
        /// <param name="uAddr">The linear address to convert.</param>
        /// <param name="codeAlign">If true, ensures that the resulting <see cref="Address"/> is correctly
        /// aligned for code addresses.</param>
        /// <returns>The resulting address.</returns>
        Address MakeAddressFromLinear(ulong uAddr, bool codeAlign);

        /// <summary>
        /// Given a sequence of <see cref="Storage"/>s, determine whether one
        /// or more of the storages could contstitute a return value from a
        /// procedure call.
        /// </summary>
        /// <param name="storages">A sequence of storages that are live 
        /// out from a call site.</param>
        /// <returns>A <see cref="Storage"/> representing the returned value,
        /// if the give <paramref name="storages"/> comply with the
        /// return value registers of the platform ABI. The returned value 
        /// can either be a <see cref="RegisterStorage"/> or a <see cref="SequenceStorage"/>
        /// if "long" values are allowed on the platform.
        /// </returns>
        Storage? PossibleReturnValue(IEnumerable<Storage> storages);

        /// <summary>
        /// Given an indirect call, attempt to resolve it into an address.
        /// </summary>
        /// <param name="instr"></param>
        /// <returns>null if the call couldn't be resolved, or an Address to
        /// what must be a procedure if the call could be resolved.
        /// </returns>
        Address? ResolveIndirectCall(RtlCall instr);

        /// <summary>
        /// Given a string, attempts to parse it into an <see cref="Address"/>.
        /// </summary>
        /// <param name="sAddress">Textual representation of an address.</param>
        /// <param name="addr">Resulting address.</param>
        /// <returns></returns>
        bool TryParseAddress(string? sAddress, [MaybeNullWhen(false)] out Address addr);

        /// <summary>
        /// Extract user settings from this instance.
        /// </summary>
        /// <returns>A dictionary of user options.
        /// </returns>
        Dictionary<string, object>? SaveUserOptions();

        /// <summary>
        /// Guess signature from the name of the procedure.
        /// </summary>
        /// <param name="fnName"></param>
        /// <returns>null if there is no way to guess a ProcedureSignature from the name.</returns>
        ProcedureBase_v1? SignatureFromName(string fnName);

        /// <summary>
        /// Given an imported symbol name, attempts unmangle the name to a type.
        /// </summary>
        /// <param name="importName">Name to demangle.</param>
        /// <returns>A triple consisting of the "bare" name, the data type of the name,
        /// and the data type of any class containing the name.
        /// </returns>

        (string, SerializedType, SerializedType)? DataTypeFromImportName(string importName);

        /// <summary>
        /// Write one or more metadata files for the loaded program.
        /// </summary>
        /// <param name="program">A <see cref="Program"/> whose file metadata is to be written.</param>
        /// <param name="path">Full path to use (without file extension).</param>
        void WriteMetadata(Program program, string path);
    }

    /// <summary>
    /// Implementation of functionality common to most platforms.
    /// </summary>
    [Designer("Reko.Gui.Design.PlatformDesigner,Reko.Gui")]
    public abstract class Platform : IPlatform
    {
        /// <summary>
        /// Initializes a Platform instance
        /// </summary>
        /// <param name="services">An <see cref="IServiceProvider"/> instance
        /// used to obtain services from the environment.</param>
        /// <param name="arch">Default processor architecture of the environment.</param>
        /// <param name="platformId">Platform ID for the platform.</param>
        protected Platform(IServiceProvider services, IProcessorArchitecture arch, string platformId)
        {
            this.Services = services;
            this.Architecture = arch;
            this.PlatformIdentifier = platformId;
            this.Heuristics = new PlatformHeuristics();
            this.DefaultTextEncoding = Encoding.ASCII;
            this.Metadata = null;
            this.CharacteristicsLibs = Array.Empty<CharacteristicsLibrary>();
            this.Name = platformId;
            this.Description = GetType().Name;
            this.PlatformProcedures = new Dictionary<Address, ExternalProcedure>();
            this.ProcedurePrologs = LoadProcedurePrologs();
            var cfgSvc = services?.GetService<IConfigurationService>();
            var platformDef = cfgSvc?.GetEnvironment(this.PlatformIdentifier);
            this.CallingConventions = CreateEmptyCallingConventionsDictionary(platformDef);
            this.TrashedRegisters = new HashSet<RegisterStorage>(LoadTrashedRegisters(platformDef));
            this.PreservedRegisters = new HashSet<RegisterStorage>(LoadPreservedRegisters(platformDef));
        }

        /// <inheritdoc/>
        public IProcessorArchitecture Architecture { get; }

        /// <inheritdoc/>
        public IServiceProvider Services { get; }

        /// <inheritdoc/>
        public virtual TypeLibrary? Metadata { get; protected set; }

        /// <inheritdoc/>
        public CharacteristicsLibrary[] CharacteristicsLibs { get; protected set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public PlatformHeuristics Heuristics { get; set; }
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public virtual MemoryMap_v1? MemoryMap
        {
            get => this.mmap ?? Architecture.MemoryMap;
            set => this.mmap = value;
        }
        private MemoryMap_v1? mmap;

        /// <inheritdoc/>
        public virtual PrimitiveType FramePointerType { get { return Architecture.FramePointerType; } }
        /// <inheritdoc/>
        public virtual PrimitiveType PointerType { get { return Architecture.PointerType; } }
        /// <inheritdoc/>
        public MaskedPattern[] ProcedurePrologs { get; }

        /// <inheritdoc/>
        public int StructureMemberAlignment { get; protected set; }

        /// <summary>
        /// String identifier used by Reko to locate platform-specfic information from the 
        /// app.config file.
        /// </summary>
        public string PlatformIdentifier { get; private set; }

        /// <summary>
        /// The default encoding for byte-encoded text.
        /// </summary>
        /// <remarks>
        /// We use ASCII as the lowest common denominator here, but some arcane platforms (e.g.
        /// ZX-81) don't use ASCII.
        /// </remarks>
        public virtual Encoding DefaultTextEncoding { get; set; }

        /// <summary>
        /// Thae names of calling conventions, organized by processor architecture.
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlyCollection<string>> CallingConventions { get; }

        /// <inheritdoc/>
        public abstract string DefaultCallingConvention { get; }

        /// <inheritdoc/>
        public virtual IReadOnlySet<RegisterStorage> TrashedRegisters { get; protected set; }

        /// <inheritdoc/>
        public virtual IReadOnlySet<RegisterStorage> PreservedRegisters { get; protected set; }


        /// <summary>
        /// Procedures provided by this platform at well-known addresses.
        /// </summary>
        /// <remarks>
        /// This is typically the case with microcomputer or embedded platforms, where calls to
        /// well-known addresses in ROMs are made.
        /// </remarks>
        public Dictionary<Address, ExternalProcedure> PlatformProcedures { get; set; }

        /// <summary>
        /// Some architectures platforms (I'm looking at you ARM Thumb) will use addresses
        /// that are offset by 1. Most don't.
        /// </summary>
        /// <param name="addr"></param>
        /// <returns>Adjusted address</returns>
        public virtual Address AdjustProcedureAddress(Address addr)
        {
            return addr;
        }

        /// <inheritdoc/>
        public virtual IPlatformEmulator CreateEmulator(SegmentMap segmentMap, Dictionary<Address, ImportReference> importReferences)
        {
            throw new NotImplementedException("Emulation has not been implemented for this platform yet.");
        }

        /// <inheritdoc/>
        public IEnumerable<Address> CreatePointerScanner(
            SegmentMap segmentMap,
            EndianImageReader rdr,
            IEnumerable<Address> address,
            PointerScannerFlags pointerScannerFlags)
        {
            return Architecture.CreatePointerScanner(segmentMap, rdr, address, pointerScannerFlags);
        }

        /// <inheritdoc/>
        public TypeLibrary CreateMetadata()
        {
            var metadata = EnsureTypeLibraries(PlatformIdentifier);
            return metadata.Clone();
        }

        /// <summary>
        /// Obtains an instance of a <see cref="ICallingConvention" /> that understands the calling convention named
        /// <paramref name="ccName"/>.
        /// </summary>
        /// <remarks>
        /// By default, there is no known calling convention.</remarks>
        /// <param name="ccName">Name of the calling convention. Specifying a null
        /// or empty string returns the default calling convention for the platform.
        /// </param>
        public virtual ICallingConvention? GetCallingConvention(string? ccName)
        {
            // The default platform has no idea, so let the architecture decide.
            // Some architectures define a standard calling procedure.
            return this.Architecture.GetCallingConvention(ccName);
        }

        /// <summary>
        /// Creates an empty imagemap based on the absolute memory map. It is 
        /// the caller's responsibility to fill in the MemoryArea properties
        /// of each resulting ImageSegment.
        /// </summary>
        /// <returns></returns>
        public virtual SegmentMap? CreateAbsoluteMemoryMap()
        {
            if (this.MemoryMap is null || this.MemoryMap.Segments is null)
                return null;
            var listener = Services.RequireService<IEventListener>();
            var segs = MemoryMap.Segments.Select(s => MemoryMap_v1.LoadSegment(s, this, listener))
                .Where(s => s is not null)
                .ToSortedList(s => s!.Address, s => s!);
            return new SegmentMap(
                segs.Values.First().Address,
                segs.Values.ToArray());
        }


        /// <inheritdoc/>
        public virtual CParser CreateCParser(TextReader rdr, ParserState? state)
        {
            state ??= new ParserState();
            var lexer = new CLexer(rdr, CLexer.StdKeywords);
            var parser = new CParser(state, lexer);
            return parser;
        }

        /// <inheritdoc/>
        public virtual ICallingConvention? DetermineCallingConvention(FunctionType signature, IProcessorArchitecture? arch)
        {
            return null;
        }

        /// <summary>
        /// Utility function for subclasses that loads all type libraries and
        /// characteristics libraries defined in the Reko configuration file.
        /// </summary>
        /// <param name="envName"></param>
        public virtual TypeLibrary EnsureTypeLibraries(string envName)
        {
            if (Metadata is not null)
                return Metadata;
            PlatformDefinition? envCfg = LoadPlatformDefinition(envName);
            if (envCfg is null)
                throw new ApplicationException(
                    $"Environment '{envName}' doesn't appear in the configuration file. Your installation may be out-of-date.");
            this.Metadata = new TypeLibrary(envCfg.CaseInsensitive);

            var tlSvc = Services.RequireService<ITypeLibraryLoaderService>();

            foreach (var tl in envCfg.TypeLibraries
                .Where(t => t.MatchArchitecture(Architecture.Name))
                .OfType<TypeLibraryDefinition>())
            {
                Metadata = tlSvc.LoadMetadataIntoLibrary(this, tl, Metadata);
            }
            this.CharacteristicsLibs = envCfg.CharacteristicsLibraries
                .Select(cl => tlSvc.LoadCharacteristics(cl.Name!))
                .Where(cl => cl is not null).ToArray();

            ApplyCharacteristicsToServices(CharacteristicsLibs, Metadata);
            return Metadata;
        }


        private static void ApplyCharacteristicsToServices(CharacteristicsLibrary[] characteristicsLibs, TypeLibrary metadata)
        {
            foreach (var ch in characteristicsLibs.SelectMany(cl => cl.Entries))
            {
                foreach (var m in metadata.Modules.Values)
                {
                    if (m.ServicesByName.TryGetValue(ch.Key, out SystemService? svc))
                    {
                        svc.Characteristics = ch.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Creates the skeleton of the per-platform calling conventions. The individual lists
        /// will be filled in on-demand.
        /// </summary>
        private static Dictionary<string, IReadOnlyCollection<string>> CreateEmptyCallingConventionsDictionary(
            PlatformDefinition? platformDef)
        {
            var result = new Dictionary<string, IReadOnlyCollection<string>>();
            if (platformDef is null || platformDef.Architectures is null)
                return result;
            foreach (var arch in platformDef.Architectures)
            {
                var ccsByName = new List<string>();
                foreach (var ccName in arch.CallingConventions)
                {
                    ccsByName.Add(ccName);
                }
                Debug.Assert(arch.Name is not null);
                result.Add(arch.Name, ccsByName);
            }
            return result;
        }

        /// <inheritdoc/>
        public abstract int GetBitSizeFromCBasicType(CBasicType cb);

        /// <inheritdoc/>
        public virtual string? GetPrimitiveTypeName(PrimitiveType pt, string language)
        {
            return null;
        }

        /// <inheritdoc/>
        public virtual Constant? FindGlobalPointerValue(Program program, Address addrStart)
        {
            return null;
        }

        /// <inheritdoc/>
        public virtual ImageSymbol? FindMainProcedure(Program program, Address addrStart)
        {
            // By default, we don't provide this service, but individual platforms 
            // may have the knowledge of how to find the "real" main program.
            return null;
        }

        /// <inheritdoc/>
        public abstract SystemService? FindService(int vector, ProcessorState? state, IMemory? memory);

        /// <inheritdoc/>
        public virtual DispatchProcedure_v1? FindDispatcherProcedureByAddress(Address addr)
        {
            return null;
        }

        /// <inheritdoc/>
        public virtual SystemService? FindService(RtlInstruction rtl, ProcessorState? state, IMemory? memory)
        {
            return null;
        }

        /// <inheritdoc/>
        public virtual SystemService FindService(string name)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// If the instructions located at the address the image reader is reading are a 
        /// trampoline, returns the procedure where the destination is located, otherwise
        /// returns null.
        /// </summary>
        public virtual Trampoline? GetTrampolineDestination(Address addrInstr, List<RtlInstructionCluster> clusters, IRewriterHost host)
        {
            return null;
        }

        /// <inheritdoc/>
        public virtual ProcedureBase? GetTrampolineDestination(Address addrInstr, IEnumerable<RtlInstruction> instrs, IRewriterHost host)
        {
            return null;
        }

        /// <summary>
        /// Determines whether a register is never used
        /// as arguments to a procedure. 
        /// </summary>
        /// <remarks>
        /// Typically, the stack pointer register is such a register.
        /// Some architectures or ABIs define global registers that are preserved 
        /// across calls; these should also be present in this set.
        /// </remarks>
        public virtual bool IsImplicitArgumentRegister(RegisterStorage reg)
        {
            return reg == Architecture.StackRegister;
        }

        /// <inheritdoc/>
        public virtual bool IsPossibleArgumentRegister(RegisterStorage reg)
        {
            return false;
        }

        /// <inheritdoc/>
        protected RegisterStorage[] LoadTrashedRegisters(PlatformDefinition? platformDef)
        {
            var pa = platformDef?.Architectures?.SingleOrDefault(a => a.Name == Architecture.Name);
            if (pa is null)
                return Array.Empty<RegisterStorage>();
            return pa.TrashedRegisters
                .Select(r => Architecture.GetRegister(r))
                .Where(r => r is { })
                .ToArray()!;
        }

        /// <inheritdoc/>
        protected RegisterStorage[] LoadPreservedRegisters(PlatformDefinition? platformDef)
        {
            var pa = platformDef?.Architectures?.SingleOrDefault(a => a.Name == Architecture.Name);
            if (pa is null)
                return Array.Empty<RegisterStorage>();
            return pa.PreservedRegisters
                .Select(r => Architecture.GetRegister(r))
                .Where(r => r is { })
                .ToArray()!;
        }

        /// <inheritdoc/>
        public virtual Address? MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return Architecture.MakeAddressFromConstant(c, codeAlign);
        }

        /// <inheritdoc/>
        public virtual void InjectProcedureEntryStatements(Procedure proc, Address addr, CodeEmitter emitter)
        {
        }

        private PlatformDefinition? LoadPlatformDefinition(string envName)
        {
            var cfgSvc = Services?.GetService<IConfigurationService>();
            return cfgSvc?.GetEnvironment(envName);
        }

        private MaskedPattern[] LoadProcedurePrologs()
        {
            var envDef = LoadPlatformDefinition(this.PlatformIdentifier);
            var arch = envDef?.Architectures?.FirstOrDefault(a => a.Name == this.Architecture.Name);
            if (arch is null)
                return Array.Empty<MaskedPattern>();
            return arch.ProcedurePrologs.ToArray();
        }

        /// <summary>
        /// Given a linear address, converts it to an Address instance. By default,
        /// use the architecture pointer size for the address.
        /// </summary>
        /// <remarks>
        /// The method is virtual to allow a platform to override the pointer size. For instance
        /// although the PowerPC 64 has 64-bit addresses, the Playstation3 implementation 
        /// uses 32-bit addresses.
        /// </remarks>
        /// <param name="uAddr">Address expressed as an unsigned long.</param>
        /// <param name="codeAlign">If true, align the resulting <see cref="Address"/> correctly
        /// for a code pointer.
        /// </param>
        /// <returns>The resulting address.</returns>
        public virtual Address MakeAddressFromLinear(ulong uAddr, bool codeAlign)
        {
            return Address.Create(Architecture.PointerType, uAddr);
        }

        /// <inheritdoc/>
        public virtual bool TryParseAddress(string? sAddress, [MaybeNullWhen(false)] out Address addr)
        {
            return Architecture.TryParseAddress(sAddress, out addr);
        }

        /// <summary>
        /// If the platform can be customized by user, load those customizations here.
        /// </summary>
        /// <returns>The options serialized as strings, or lists of objects, or dictionaries of objects.</returns>
        public virtual void LoadUserOptions(Dictionary<string, object> options) { }

        /// <summary>
        /// If the platform can be customized by user, save those customizations here.
        /// </summary>
        /// <returns>The options serialized as strings, or lists of objects, or dictionaries of objects.</returns>
        public virtual Dictionary<string, object>? SaveUserOptions() { return null; }
        
        /// <summary>
        /// Guess signature from the name of the procedure.
        /// </summary>
        /// <param name="fnName"></param>
        /// <returns>null if there is no way to guess a ProcedureSignature from the name.</returns>
        public virtual ProcedureBase_v1? SignatureFromName(string fnName)
        {
            return null;
        }

        /// <inheritdoc/>
        public virtual (string, SerializedType, SerializedType)? DataTypeFromImportName(string importName)
        {
            return null;
        }

        /// <inheritdoc/>
        public virtual ProcedureBase? LookupProcedureByAddress(Address addr)
        {
            return this.PlatformProcedures.TryGetValue(addr, out var extProc)
                ? extProc
                : null;
        }

        /// <inheritdoc/>
        public abstract ExternalProcedure? LookupProcedureByName(string? moduleName, string procName);

        /// <inheritdoc/>
        public virtual ExternalProcedure? LookupProcedureByOrdinal(string moduleName, int ordinal)
        {
            return null;
        }

        /// <inheritdoc/>
        public virtual Storage? PossibleReturnValue(IEnumerable<Storage> storages)
        {
            return null;
        }

        /// <inheritdoc/>
        public virtual Expression? ResolveImportByName(string? moduleName, string globalName)
        {
            var ep = LookupProcedureByName(moduleName, globalName);
            if (ep is not null)
                return new ProcedureConstant(PointerType, ep);
            else
                return null;
        }

        /// <inheritdoc/>
        public virtual Expression? ResolveImportByOrdinal(string moduleName, int ordinal)
        {
            var ep = LookupProcedureByOrdinal(moduleName, ordinal);
            if (ep is not null)
                return new ProcedureConstant(PointerType, ep);
            else
                return null;
        }


        /// <summary>
        /// Given an indirect call, attempt to resolve it into an address.
        /// </summary>
        /// <param name="instr"></param>
        /// <returns>null if the call couldn't be resolved, or an Address to
        /// what must be a procedure if the call could be resolved.
        /// </returns>
        public virtual Address? ResolveIndirectCall(RtlCall instr)
        {
            return null;
        }

        /// <inheritdoc/>
        public virtual ProcedureCharacteristics? LookupCharacteristicsByName(string procName)
        {
            var metadata = EnsureTypeLibraries(PlatformIdentifier);
            return CharacteristicsLibs.Select(cl => cl.Lookup(procName))
                .Where(c => c is not null)
                .FirstOrDefault();
        }

        /// <inheritdoc/>
        public virtual void WriteMetadata(Program program, string path)
        {
        }
    }

    /// <summary>
    /// The default platform is used when a specific platform cannot be determined.
    /// </summary>
    /// <remarks>
    /// "All the world's a VAX"  -- not Henry Spencer
    /// </remarks>
    public class DefaultPlatform : Platform
    {
        /// <summary>
        /// Creates a new instance of the default platform.
        /// </summary>
        /// <param name="services"><see cref="IServiceProvider"/> instance.</param>
        /// <param name="arch">Default <see cref="IProcessorArchitecture"/>.</param>
        public DefaultPlatform(
            IServiceProvider services,
            IProcessorArchitecture arch)
            : base(services, arch, "default")
        {
            this.TypeLibraries = new List<TypeLibrary>();
            this.Description = "(Unknown operating environment)";
            this.StructureMemberAlignment = 8;
            this.TrashedRegisters = new HashSet<RegisterStorage>();
        }

        /// <summary>
        /// Creates a new instance of the default platform.
        /// </summary>
        /// <param name="services"><see cref="IServiceProvider"/> instance.</param>
        /// <param name="arch">Default <see cref="IProcessorArchitecture"/>.</param>
        /// <param name="description">Platform description.</param>
        public DefaultPlatform(
            IServiceProvider services,
            IProcessorArchitecture arch,
            string description) : base(services, arch, "default")
        {
            this.TypeLibraries = new List<TypeLibrary>();
            this.Description = description;
            this.StructureMemberAlignment = 8;
            this.TrashedRegisters = new HashSet<RegisterStorage>();
        }

        /// <inheritdoc/>
        public List<TypeLibrary> TypeLibraries { get; }

        /// <inheritdoc/>
        public override string DefaultCallingConvention => "";

        /// <inheritdoc/>
        public override IReadOnlySet<RegisterStorage> TrashedRegisters { get; protected set; }

        /// <inheritdoc/>
        public override SystemService? FindService(int vector, ProcessorState? state, IMemory? memory)
        {
            return null;
        }

        /// <inheritdoc/>
        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            switch (cb)
            {
            case CBasicType.Bool: return 8;
            case CBasicType.Char: return 8;
            case CBasicType.WChar_t: return 16;
            case CBasicType.Short: return 16;
            case CBasicType.Int: return 32;      // Assume 32-bit int.
            case CBasicType.Long: return 32;
            case CBasicType.LongLong: return 64;
            case CBasicType.Float: return 32;
            case CBasicType.Double: return 64;
            case CBasicType.LongDouble: return 64;
            case CBasicType.Int64: return 64;
            default: throw new NotImplementedException($"C basic type {cb} not implemented yet.");
            }
        }

        /// <inheritdoc/>
        public override ExternalProcedure? LookupProcedureByName(string? moduleName, string procName)
        {
            //$Identical to Win32, move into base class?
            return TypeLibraries
                .Select(t => t.Lookup(procName))
                .Where(sig => sig is not null)
                .Select(sig => new ExternalProcedure(procName, sig!))
                .FirstOrDefault();
        }
    }
}
