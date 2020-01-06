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

using System;
using System.Collections.Generic;
using System.Text;
using Reko.Core;
using Reko.Core.CLanguage;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;

namespace Reko.ImageLoaders.LLVM
{
    public class LLVMPlatform : IPlatform
    {
        private IServiceProvider services;

        public LLVMPlatform(IServiceProvider services)
        {
            this.services = services;
            this.Name = "LLVM";
        }

        public IProcessorArchitecture Architecture { get; set; }

        public string DefaultCallingConvention { get; set; }

        public Encoding DefaultTextEncoding { get; set; }

        public string Description { get; set; }

        public PrimitiveType FramePointerType { get; set; }

        public PlatformHeuristics Heuristics { get; set; }

        public MemoryMap_v1 MemoryMap { get; set; }

        public string Name { get; private set; }

        public string PlatformIdentifier { get; set; }

        public PrimitiveType PointerType { get; set; }

        public Address AdjustProcedureAddress(Address addrCode)
        {
            throw new NotImplementedException();
        }

        public SegmentMap CreateAbsoluteMemoryMap()
        {
            throw new NotImplementedException();
        }

        public IPlatformEmulator CreateEmulator(SegmentMap segmentMap, Dictionary<Address, ImportReference> importReferences)
        {
            throw new NotImplementedException();
        }

        public HashSet<RegisterStorage> CreateImplicitArgumentRegisters()
        {
            throw new NotImplementedException();
        }

        public TypeLibrary CreateMetadata()
        {
            return new TypeLibrary();
        }

        public IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> addr, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public CallingConvention GetCallingConvention(string ccName)
        {
            throw new NotImplementedException();
        }

        public HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            throw new NotImplementedException();
        }

        public Tuple<string, SerializedType, SerializedType> DataTypeFromImportName(string importName)
        {
            throw new NotImplementedException();
        }

        public string DetermineCallingConvention(FunctionType signature)
        {
            throw new NotImplementedException();
        }

        public DispatchProcedure_v1 FindDispatcherProcedureByAddress(Address addr)
        {
            throw new NotImplementedException();
        }

        public ImageSymbol FindMainProcedure(Program program, Address addrStart)
        {
            throw new NotImplementedException();
        }

        public SystemService FindService(RtlInstruction call, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public SystemService FindService(int vector, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public string FormatProcedureName(Program program, Procedure proc)
        {
            throw new NotImplementedException();
        }

        public int GetByteSizeFromCBasicType(CBasicType cb)
        {
            throw new NotImplementedException();
        }

        public string GetPrimitiveTypeName(PrimitiveType t, string language)
        {
            throw new NotImplementedException();
        }

        public ProcedureBase GetTrampolineDestination(IEnumerable<RtlInstructionCluster> instrs, IRewriterHost host)
        {
            throw new NotImplementedException();
        }

        public void InjectProcedureEntryStatements(Procedure proc, Address addr, CodeEmitter emitter)
        {
            throw new NotImplementedException();
        }

        public void LoadUserOptions(Dictionary<string, object> options)
        {
            throw new NotImplementedException();
        }

        public ProcedureCharacteristics LookupCharacteristicsByName(string procName)
        {
            throw new NotImplementedException();
        }

        public ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            throw new NotImplementedException();
        }

        public ExternalProcedure LookupProcedureByOrdinal(string moduleName, int ordinal)
        {
            throw new NotImplementedException();
        }

        public Address MakeAddressFromConstant(Core.Expressions.Constant c, bool codeAlign)
        {
            throw new NotImplementedException();
        }

        public Address MakeAddressFromLinear(ulong uAddr, bool codeAlign)
        {
            throw new NotImplementedException();
        }

        public Expression ResolveImportByName(string moduleName, string globalName)
        {
            throw new NotImplementedException();
        }

        public Expression ResolveImportByOrdinal(string moduleName, int ordinal)
        {
            throw new NotImplementedException();
        }

        public Address ResolveIndirectCall(RtlCall call)
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

        public bool TryParseAddress(string sAddress, out Address addr)
        {
            throw new NotImplementedException();
        }
    }
}