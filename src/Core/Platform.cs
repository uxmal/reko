#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Core.CLanguage;
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        string Name { get; }
        IProcessorArchitecture Architecture { get; }
        string DefaultCallingConvention { get; }
        Encoding DefaultTextEncoding { get; set; }
        string Description { get; set; }
        PrimitiveType FramePointerType { get; }
        PlatformHeuristics Heuristics { get; }
        /// <summary>
        /// Some platforms, especially microcomputers, have well-known
        /// procedures and global variables at absolute addresses. These are
        /// available in the MemoryMap.
        /// </summary>
        MemoryMap_v1 MemoryMap { get; set; }
        string PlatformIdentifier { get; }
        PrimitiveType PointerType { get; }

        Address AdjustProcedureAddress(Address addrCode);
        HashSet<RegisterStorage> CreateImplicitArgumentRegisters();
        HashSet<RegisterStorage> CreateTrashedRegisters();

        IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> addr, PointerScannerFlags flags);
        CallingConvention GetCallingConvention(string ccName);
        TypeLibrary CreateMetadata();

        /// <summary>
        /// Creates a platform emulator for this platform.
        /// </summary>
        /// <param name="segmentMap">Loaded program image.</param>
        /// <param name="importReferences">Imported procedures.</param>
        /// <returns>The created platform emulators.
        /// </returns>
        IPlatformEmulator CreateEmulator(SegmentMap segmentMap, Dictionary<Address, ImportReference> importReferences);

        /// <summary>
        /// Creates an empty SegmentMap based on the absolute memory map. It is 
        /// the caller's responsibility to fill in the MemoryArea properties
        /// of each resulting ImageSegment.
        /// </summary>
        /// <returns></returns>
        SegmentMap CreateAbsoluteMemoryMap();

        /// <summary>
        /// Given a procedure signature, determines whether it conforms to any
        /// of the platform's defined calling conventions.
        /// </summary>
        /// <remarks>
        /// Some platforms, like Win32, will have several well known 
        /// calling conventions. Others, like many ELF implementations,
        /// will have one and only one calling convention. On such platforms
        /// we will assume that the calling convention is represented by
        /// the empty string "". //$REVIEW: this probably highlights the 
        /// need for a CallingConvention abstraction.
        /// </remarks>
        /// <param name="signature"></param>
        /// <returns>The name of the calling convention, or null
        /// if no calling convention could be determined.</returns>
        string DetermineCallingConvention(FunctionType signature);

        /// <summary>
        /// Given a C basic type, returns the number of bytes that type is
        /// represented with on this platform.
        /// </summary>
        /// <param name="cb">A C Basic type, like int, float etc.</param>
        /// <returns>Number of bytes used by this platform.
        /// </returns>
        int GetByteSizeFromCBasicType(CBasicType cb);

        /// <summary>
        /// Given a primitive type <paramref name="t"/> returns the
        /// rendering of that primitive in the programming language
        /// <paramref name="language"/>
        /// </summary>
        /// <param name="t">Primitive type</param>
        /// <param name="language">Programming language to use</param>
        /// <returns></returns>
        string GetPrimitiveTypeName(PrimitiveType t, string language);

        ProcedureBase GetTrampolineDestination(IEnumerable<RtlInstructionCluster> instrs, IRewriterHost host);

        /// <summary>
        /// Given an executable entry point, find the location of the "main" program,
        /// bypassing any runtime startup code.
        /// </summary>
        /// <param name="imageMap">Program image in which to search</param>
        /// <param name="addrStart">The entrypoint according to the image.</param>
        /// <returns>null if no known runtime code was found, otherwise the 
        /// an ImageSymbol corresponding to the "real" user main procedure.</returns>
        ImageSymbol FindMainProcedure(Program program, Address addrStart);

        /// <summary>
        /// Given a vector and the current processor state, finds a system
        /// service.
        /// </summary>
        /// <remarks>
        /// This method is used to resolve system calls or traps where 
        /// the actual service are selected by registers or stack values.
        /// </remarks>
        /// <param name="vector"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        SystemService FindService(int vector, ProcessorState state);
        SystemService FindService(RtlInstruction call, ProcessorState state);
        DispatchProcedure_v1 FindDispatcherProcedureByAddress(Address addr);

        string FormatProcedureName(Program program, Procedure proc);

        /// <summary>
        /// Injects any platform specific instructions to the beginning 
        /// of a procedure.
        /// </summary>
        /// <param name="emitter"></param>
        void InjectProcedureEntryStatements(Procedure proc, Address addr, CodeEmitter emitter);

        void LoadUserOptions(Dictionary<string, object> options);
        ExternalProcedure LookupProcedureByName(string moduleName, string procName);
        ExternalProcedure LookupProcedureByOrdinal(string moduleName, int ordinal);
        Expression ResolveImportByName(string moduleName, string globalName);
        Expression ResolveImportByOrdinal(string moduleName, int ordinal);
        ProcedureCharacteristics LookupCharacteristicsByName(string procName);
        Address MakeAddressFromConstant(Constant c, bool codeAlign);
        Address MakeAddressFromLinear(ulong uAddr, bool codeAlign);

        /// <summary>
        /// Given an indirect call, attempt to resolve it into an address.
        /// </summary>
        /// <param name="instr"></param>
        /// <returns>null if the call couldn't be resolved, or an Address to
        /// what must be a procedure if the call could be resolved.
        /// </returns>
        Address ResolveIndirectCall(RtlCall instr);

        bool TryParseAddress(string sAddress, out Address addr);
        Dictionary<string, object> SaveUserOptions();
        ProcedureBase_v1 SignatureFromName(string importName);
        Tuple<string, SerializedType, SerializedType> DataTypeFromImportName(string importName);
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
        /// <param name="arch"></param>
        protected Platform(IServiceProvider services, IProcessorArchitecture arch, string platformId)
        {
            this.Services = services;
            this.Architecture = arch;
            this.PlatformIdentifier = platformId;
            this.Heuristics = new PlatformHeuristics();
            this.DefaultTextEncoding = Encoding.ASCII;
        }

        public IProcessorArchitecture Architecture { get; private set; }
        public IServiceProvider Services { get; private set; }
        public virtual TypeLibrary Metadata { get; protected set; }
        public CharacteristicsLibrary[] CharacteristicsLibs { get; protected set; }
        public string Description { get; set; }
        public PlatformHeuristics Heuristics { get; set; }
        public string Name { get; set; }
        public virtual MemoryMap_v1 MemoryMap { get; set; }
        public virtual PrimitiveType FramePointerType { get { return Architecture.FramePointerType; } }
        public virtual PrimitiveType PointerType { get { return Architecture.PointerType; } }

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

        public abstract string DefaultCallingConvention { get; }

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

        public abstract IPlatformEmulator CreateEmulator(SegmentMap segmentMap, Dictionary<Address, ImportReference> importReferences);

        /// <summary>
        /// Creates a set that represents those registers that are never used
        /// as arguments to a procedure. 
        /// </summary>
        /// <remarks>
        /// Typically, the stack pointer register is one of these registers.
        /// Some architectures define global registers that are preserved 
        /// across calls; these should also be present in this set.
        /// </remarks>
        public virtual HashSet<RegisterStorage> CreateImplicitArgumentRegisters()
        {
            return new HashSet<RegisterStorage>()
            {
                Architecture.StackRegister,
            };
        }

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
        public abstract HashSet<RegisterStorage> CreateTrashedRegisters();

        public IEnumerable<Address> CreatePointerScanner(
            SegmentMap segmentMap,
            EndianImageReader rdr,
            IEnumerable<Address> address,
            PointerScannerFlags pointerScannerFlags)
        {
            return Architecture.CreatePointerScanner(segmentMap, rdr, address, pointerScannerFlags);
        }

        public TypeLibrary CreateMetadata()
        {
            EnsureTypeLibraries(PlatformIdentifier);
            return Metadata.Clone();
        }

        /// <summary>
        /// Creates a CallingConvention that understands the calling convention named
        /// <paramref name="ccName"/>.
        /// </summary>
        /// <param name="ccName">Name of the calling convention.</param>
        public abstract CallingConvention GetCallingConvention(string ccName);
        
        /// <summary>
        /// Creates an empty imagemap based on the absolute memory map. It is 
        /// the caller's responsibility to fill in the MemoryArea properties
        /// of each resulting ImageSegment.
        /// </summary>
        /// <returns></returns>
        public virtual SegmentMap CreateAbsoluteMemoryMap()
        {
            if (this.MemoryMap == null || this.MemoryMap.Segments == null)
                return null;
            var diagSvc = Services.RequireService<IDiagnosticsService>();
            var segs = MemoryMap.Segments.Select(s => MemoryMap_v1.LoadSegment(s, this, diagSvc))
                .Where(s => s != null)
                .ToSortedList(s => s.Address);
            return new SegmentMap(
                segs.Values.First().Address,
                segs.Values.ToArray());
        }

        public virtual string DetermineCallingConvention(FunctionType signature)
        {
            return null;
        }

        /// <summary>
        /// Utility function for subclasses that loads all type libraries and
        /// characteristics libraries defined in the Reko configuration file.
        /// </summary>
        /// <param name="envName"></param>
        public virtual void EnsureTypeLibraries(string envName)
        {
            if (Metadata == null)
            {
                var cfgSvc = Services.RequireService<IConfigurationService>();
                var envCfg = cfgSvc.GetEnvironment(envName);
                if (envCfg == null)
                    throw new ApplicationException(string.Format(
                        "Environment '{0}' doesn't appear in the configuration file. Your installation may be out-of-date.",
                        envName));
                this.Metadata = new TypeLibrary();

                var tlSvc = Services.RequireService<ITypeLibraryLoaderService>();

                foreach (var tl in envCfg.TypeLibraries
                    .Where(t => t.Architecture == null ||
                                t.Architecture.Contains(Architecture.Name))
                    .OfType<TypeLibraryDefinition>())
                {
                    Metadata = tlSvc.LoadMetadataIntoLibrary(this, tl, Metadata); 
                }
                this.CharacteristicsLibs = envCfg.CharacteristicsLibraries
                    .Select(cl => tlSvc.LoadCharacteristics(cl.Name))
                    .Where(cl => cl != null).ToArray();

                ApplyCharacteristicsToServices(CharacteristicsLibs, Metadata);
            }
        }

        private void ApplyCharacteristicsToServices(CharacteristicsLibrary[] characteristicsLibs, TypeLibrary metadata)
        {
            foreach (var ch in characteristicsLibs.SelectMany(cl => cl.Entries))
            {
                foreach (var m in metadata.Modules.Values)
                {
                    if (m.ServicesByName.TryGetValue(ch.Key, out SystemService svc))
                    {
                        svc.Characteristics = ch.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Formats a program/module and a procedure name together.
        /// </summary>
        /// <remarks>
        /// This is done in the Windows way {module}!{procname}. Other platforms
        /// may have other conventions. Please override this in the other platforms
        /// to give the correct output.</remarks>
        /// <param name="program"></param>
        /// <param name="proc"></param>
        /// <returns></returns>
        public virtual string FormatProcedureName(Program program, Procedure proc)
        {
            return string.Format("{0}!{1}", program.Name, proc.Name);
        }

        public abstract int GetByteSizeFromCBasicType(CBasicType cb);

        public virtual string GetPrimitiveTypeName(PrimitiveType pt, string language)
        {
            return null;
        }

        public virtual ImageSymbol FindMainProcedure(Program program, Address addrStart)
        {
            // By default, we don't provide this service, but individual platforms 
            // may have the knowledge of how to find the "real" main program.
            throw new NotSupportedException();
        }

        public abstract SystemService FindService(int vector, ProcessorState state);

        public virtual DispatchProcedure_v1 FindDispatcherProcedureByAddress(Address addr)
        {
            return null;
        }

        public virtual SystemService FindService(RtlInstruction rtl, ProcessorState state)
        {
            return null;
        }

        public virtual SystemService FindService(string name)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// If the instructions located at the address the image reader is reading are a 
        /// trampoline, returns the procedure where the destination is located, otherwise
        /// returns null.
        /// </summary>
        /// <param name="imageReader"></param>
        /// <returns></returns>
        public virtual ProcedureBase GetTrampolineDestination(IEnumerable<RtlInstructionCluster> instrs, IRewriterHost host)
        {
            return null;
        }

        public virtual Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return Architecture.MakeAddressFromConstant(c, codeAlign);
        }

        public virtual void InjectProcedureEntryStatements(Procedure proc, Address addr, CodeEmitter emitter)
        {
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
        /// <param name="uAddr"></param>
        /// <returns></returns>
        public virtual Address MakeAddressFromLinear(ulong uAddr, bool codeAlign)
        {
            return Address.Create(Architecture.PointerType, uAddr);
        }

        public virtual bool TryParseAddress(string sAddress, out Address addr)
        {
            return Architecture.TryParseAddress(sAddress, out addr);
        }

        public abstract ExternalProcedure LookupProcedureByName(string moduleName, string procName);

        /// <summary>
        /// If the platform can be customized by user, load those customizations here.
        /// </summary>
        /// <returns>The options serialized as strings, or lists of objects, or dictionaries of objects.</returns>
        public virtual void LoadUserOptions(Dictionary<string, object> options) { }

        /// <summary>
        /// If the platform can be customized by user, save those customizations here.
        /// </summary>
        /// <returns>The options serialized as strings, or lists of objects, or dictionaries of objects.</returns>
        public virtual Dictionary<string, object> SaveUserOptions() { return null; }
        
        /// <summary>
        /// Guess signature from the name of the procedure.
        /// </summary>
        /// <param name="fnName"></param>
        /// <returns>null if there is no way to guess a ProcedureSignature from the name.</returns>
        public virtual ProcedureBase_v1 SignatureFromName(string fnName)
        {
            return null;
        }

        public virtual Tuple<string, SerializedType, SerializedType> DataTypeFromImportName(string importName)
        {
            return null;
        }

        public virtual ExternalProcedure LookupProcedureByOrdinal(string moduleName, int ordinal)
        {
            return null;
        }

        public virtual Expression ResolveImportByName(string moduleName, string globalName)
        {
            var ep = LookupProcedureByName(moduleName, globalName);
            if (ep != null)
                return new ProcedureConstant(PointerType, ep);
            else
                return null;
        }

        public virtual Expression ResolveImportByOrdinal(string moduleName, int ordinal)
        {
            var ep = LookupProcedureByOrdinal(moduleName, ordinal);
            if (ep != null)
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
        public virtual Address ResolveIndirectCall(RtlCall instr)
        {
            return null;
        }

        public virtual ProcedureCharacteristics LookupCharacteristicsByName(string procName)
        {
            EnsureTypeLibraries(PlatformIdentifier);
            return CharacteristicsLibs.Select(cl => cl.Lookup(procName))
                .Where(c => c != null)
                .FirstOrDefault();
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
        public DefaultPlatform(
            IServiceProvider services,
            IProcessorArchitecture arch)
            : base(services, arch, "default")
        {
            this.TypeLibraries = new List<TypeLibrary>();
            this.Description = "(Unknown operating environment)";
        }

        public DefaultPlatform(
            IServiceProvider services,
            IProcessorArchitecture arch,
            string description) : base(services, arch, "default")
        {
            this.TypeLibraries = new List<TypeLibrary>();
            this.Description = description;
        }

        public List<TypeLibrary> TypeLibraries { get; private set; }

        public override string DefaultCallingConvention
        {
            get { return ""; }
        }

        public override IPlatformEmulator CreateEmulator(SegmentMap segmentMap, Dictionary<Address, ImportReference> importReferences)
        {
            throw new NotSupportedException();
        }

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            return new HashSet<RegisterStorage>();
        }

        public override CallingConvention GetCallingConvention(string ccName)
        {
            // The default platform has no idea, so let the architecture decide.
            // Some architectures define a standard calling procedure.
            return this.Architecture.GetCallingConvention(ccName);
        }

        public override SystemService FindService(int vector, ProcessorState state)
        {
            return null;
        }

        public override int GetByteSizeFromCBasicType(CBasicType cb)
        {
            switch (cb)
            {
            case CBasicType.Bool: return 1;
            case CBasicType.Char: return 1;
            case CBasicType.WChar_t: return 2;
            case CBasicType.Short: return 2;
            case CBasicType.Int: return 4;      // Assume 32-bit int.
            case CBasicType.Long: return 4;
            case CBasicType.LongLong: return 8;
            case CBasicType.Float: return 4;
            case CBasicType.Double: return 8;
            case CBasicType.LongDouble: return 8;
            case CBasicType.Int64: return 8;
            default: throw new NotImplementedException(string.Format("C basic type {0} not supported.", cb));
            }
        }

        public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            //$Identical to Win32, move into base class?
            return TypeLibraries
                .Select(t => t.Lookup(procName))
                .Where(sig => sig != null)
                .Select(sig => new ExternalProcedure(procName, sig))
                .FirstOrDefault();
        }
    }
}
