#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Hll.C;
using Reko.Core.Emulation;
using Reko.Core.Expressions;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Reko.Core.Loading;
using Reko.Core.Code;
using Reko.Core.Machine;

namespace Reko.ImageLoaders.LLVM
{
    public class LLVMPlatform : IPlatform
    {
        private readonly IServiceProvider services;

#nullable disable
        public LLVMPlatform(IServiceProvider services)
        {
            this.services = services;
            this.Name = "LLVM";
            this.CallingConventions = new Dictionary<string, IReadOnlyCollection<string>>();
            this.StructureMemberAlignment = 4;  //$BUG: is arch-specific.
            this.TrashedRegisters = new HashSet<RegisterStorage>();
            this.PreservedRegisters = new HashSet<RegisterStorage>();
        }
#nullable enable

        public IProcessorArchitecture Architecture { get; set; }

        public IReadOnlyDictionary<string, IReadOnlyCollection<string>> CallingConventions { get; }

        public string DefaultCallingConvention { get; set; }

        public Encoding DefaultTextEncoding { get; set; }

        public string Description { get; set; }

        public PrimitiveType FramePointerType { get; set; }

        public PlatformHeuristics Heuristics { get; set; }

        public MemoryMap_v1? MemoryMap { get; set; }

        public string Name { get; private set; }

        public string PlatformIdentifier { get; set; }

        public MaskedPattern[] ProcedurePrologs => Array.Empty<MaskedPattern>();

        public PrimitiveType PointerType { get; set; }

        public int StructureMemberAlignment { get; set; }

        public IReadOnlySet<RegisterStorage> TrashedRegisters { get; }
        public IReadOnlySet<RegisterStorage> PreservedRegisters { get; }


        public Address AdjustProcedureAddress(Address addrCode)
        {
            throw new NotImplementedException();
        }

        public SegmentMap CreateAbsoluteMemoryMap()
        {
            throw new NotImplementedException();
        }

#nullable enable
        public CParser CreateCParser(TextReader rdr, ParserState? state = null)
        {
            state ??= new ParserState();
            var lexer = new CLexer(rdr, CLexer.StdKeywords);
            var parser = new CParser(state, lexer);
            return parser;
        }

        public IPlatformEmulator CreateEmulator(SegmentMap segmentMap, Dictionary<Address, ImportReference> importReferences)
        {
            throw new NotImplementedException();
        }

        public bool IsImplicitArgumentRegister(RegisterStorage reg)
        {
            return false;
        }


        public TypeLibrary CreateMetadata()
        {
            return new TypeLibrary();
        }

        public IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> addr, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public ICallingConvention GetCallingConvention(string? ccName)
        {
            throw new NotImplementedException();
        }

        public (string, SerializedType, SerializedType)? DataTypeFromImportName(string importName)
        {
            throw new NotImplementedException();
        }

        public ICallingConvention? DetermineCallingConvention(FunctionType signature, IProcessorArchitecture? arch)
        {
            return null;
        }

        public DispatchProcedure_v1 FindDispatcherProcedureByAddress(Address addr)
        {
            throw new NotImplementedException();
        }

        public Core.Expressions.Constant? FindGlobalPointerValue(Program program, Address addrStart)
        {
            return null;
        }


        public ImageSymbol FindMainProcedure(Program program, Address addrStart)
        {
            throw new NotImplementedException();
        }

        public SystemService FindService(RtlInstruction call, ProcessorState? state, IMemory? memory)
        {
            throw new NotImplementedException();
        }

        public SystemService FindService(int vector, ProcessorState? state, IMemory? memory)
        {
            throw new NotImplementedException();
        }

        public int GetBitSizeFromCBasicType(CBasicType cb)
        {
            throw new NotImplementedException();
        }

        public string GetPrimitiveTypeName(PrimitiveType t, string language)
        {
            throw new NotImplementedException();
        }

        public Trampoline? GetTrampolineDestination(Address addrInstr, List<RtlInstructionCluster> instrs, IRewriterHost host)
        {
            return null;
        }

        public ProcedureBase? GetTrampolineDestination(Address addrInstr, IEnumerable<RtlInstruction> instrs, IRewriterHost host)
        {
            return null;
        }

        public void InjectProcedureEntryStatements(Procedure proc, Address addr, CodeEmitter emitter)
        {
        }

        public bool IsPossibleArgumentRegister(RegisterStorage reg)
        {
            return false;
        }

        public void LoadUserOptions(Dictionary<string, object> options)
        {
            throw new NotImplementedException();
        }

        public ProcedureCharacteristics LookupCharacteristicsByName(string procName)
        {
            throw new NotImplementedException();
        }

        public ProcedureBase? LookupProcedureByAddress(Address addr)
        {
            return null;
        }

        public ExternalProcedure LookupProcedureByName(string? moduleName, string procName)
        {
            throw new NotImplementedException();
        }

        public ExternalProcedure LookupProcedureByOrdinal(string moduleName, int ordinal)
        {
            throw new NotImplementedException();
        }

        public Address? MakeAddressFromConstant(Core.Expressions.Constant c, bool codeAlign)
        {
            throw new NotImplementedException();
        }

        public Address MakeAddressFromLinear(ulong uAddr, bool codeAlign)
        {
            throw new NotImplementedException();
        }

        public Storage? PossibleReturnValue(IEnumerable<Storage> liveOutStorages)
        {
            return null;
        }

        public SerializedService PreprocessSerializedService(SerializedService service) => service;

        public Expression ResolveImportByName(string? moduleName, string globalName)
        {
            throw new NotImplementedException();
        }

        public Expression ResolveImportByOrdinal(string? moduleName, int ordinal)
        {
            throw new NotImplementedException();
        }

        public Address? ResolveIndirectCall(RtlCall call)
        {
            return null;
        }

        public Dictionary<string, object> SaveUserOptions()
        {
            throw new NotImplementedException();
        }

        public ProcedureBase_v1 SignatureFromName(string importName)
        {
            throw new NotImplementedException();
        }

        public bool TryParseAddress(string? sAddress, out Address addr)
        {
            throw new NotImplementedException();
        }

        public void WriteMetadata(Program program, string path)
        {
        }

        public List<RtlInstruction>? InlineCall(Address addrCallee, Address addrContinuation, EndianImageReader rdr, IStorageBinder binder)
        {
            return null;
        }
    }
}