#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Core
{
    /// <summary>
    /// Knows how to deserialize Reko type libraries and add types and 
    /// functions into a TypeLibrary.
    /// </summary>
    public class TypeLibraryDeserializer : ISerializedTypeVisitor<DataType>
    {
        private readonly IPlatform platform;
        private readonly IDictionary<string, DataType> types;
        private readonly Dictionary<string, UnionType> unions;
        private readonly Dictionary<string, StructureType> structures;
        private readonly TypeLibrary library;
        private string? defaultConvention;
        private string? moduleName;
        private readonly int bitsPerStorageUnit;

        public TypeLibraryDeserializer(IPlatform platform, bool caseInsensitive, TypeLibrary dstLib)
        {
            this.library = dstLib ?? throw new ArgumentNullException(nameof(dstLib));
            this.platform = platform;
            var cmp = caseInsensitive ? StringComparer.InvariantCultureIgnoreCase : StringComparer.InvariantCulture;
            types = dstLib.Types;
            this.unions = new Dictionary<string, UnionType>(cmp);
            this.structures = new Dictionary<string, StructureType>(cmp);
            this.bitsPerStorageUnit = platform.Architecture.MemoryGranularity;
        }

        public TypeLibrary Load(SerializedLibrary sLib)
        {
            moduleName = sLib.ModuleName;
            ReadDefaults(sLib.Defaults);
            LoadTypes(sLib);
            LoadProcedures(sLib);
            LoadGlobals(sLib);
            return library;
        }

        private ModuleDescriptor EnsureModule(string? moduleName, TypeLibrary dstLib)
        {
            moduleName ??= "";
            if (!dstLib.Modules.TryGetValue(moduleName, out ModuleDescriptor mod))
            {
                mod = new ModuleDescriptor(moduleName);
                dstLib.Modules.Add(moduleName, mod);
            }
            return mod;
        }

        private void LoadGlobals(SerializedLibrary sLib)
        {
            var mod = EnsureModule(this.moduleName, this.library);
            if (sLib.Globals != null)
            {
                foreach (var g in sLib.Globals.Where(gg => !string.IsNullOrEmpty(gg.Name) && gg.Type != null))
                {
                    var globalType = g.Type!;
                    var globalName = g.Name!;
                    var dt = this.LoadType(globalType);
                    var sym = ImageSymbol.Create(SymbolType.Data, platform.Architecture, null!, globalName);
                    mod.GlobalsByName[globalName] = sym;
                    if (g.Ordinal != GlobalVariable_v1.NoOrdinal)
                    {
                        mod.GlobalsByOrdinal[g.Ordinal] = sym;
                    }
                    library.Globals[globalName] = dt;       //$REVIEW: How to cope with colissions MODULE1!foo and MODULE2!foo?
                }
            }
        }

        private void LoadProcedures(SerializedLibrary serializedLibrary)
        {
            if (serializedLibrary.Procedures == null)
                return;

            foreach (object o in serializedLibrary.Procedures)
            {
                if (o is Procedure_v1 sp)
                {
                    LoadProcedure(sp);
                }
                else if (o is SerializedService svc)
                {
                    LoadService(svc);
                }
            }
        }

        public void LoadProcedure(Procedure_v1 sp)
        {
            try
            {
                if (sp.Name is null)
                    return;
                if (sp.Characteristics != null)
                {
                    library.Characteristics[sp.Name] = sp.Characteristics;
                }
                var sser = new ProcedureSerializer(platform, this, this.defaultConvention ?? "");
                var signature = sser.Deserialize(sp.Signature, platform.Architecture.CreateFrame());
                if (signature is null)
                    return;
                library.Signatures[sp.Name] = signature;
                if (platform.TryParseAddress(sp.Address, out var addr))
                {
                    this.library.Procedures[addr] = (sp.Name, signature);
                }
                else
                {
                    var mod = EnsureModule(this.moduleName, this.library);
                    var svc = new SystemService
                    {
                        ModuleName = mod.ModuleName,
                        Name = sp.Name,
                        Signature = signature,
                    };
                    mod.ServicesByName[sp.Name] = svc;    //$BUGBUG: catch dupes?

                    if (sp.Ordinal != Procedure_v1.NoOrdinal)
                    {
                        mod.ServicesByOrdinal[sp.Ordinal] = svc;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print("An error occurred when loading the signature of procedure {0}.", sp.Name);
                throw new ApplicationException(
                    string.Format("An error occurred when loading the signature of procedure {0}.", sp.Name),
                    ex);
            }
        }

        public SystemService LoadService(SerializedService ssvc)
        {
            var svc = ssvc.Build(platform, this.library);
            LoadService(svc.SyscallInfo!.Vector, svc);
            return svc;
        }

        public void LoadService(string entryName, SystemService svc)
        {
            var mod = EnsureModule(svc.ModuleName, this.library);
            mod.ServicesByName[entryName] = svc;
        }

        public void LoadService(int ordinal, SystemService svc)
        {
            var mod = EnsureModule(svc.ModuleName, this.library);
            if (ordinal != Serialization.Procedure_v1.NoOrdinal)
            {
                if (mod.ServicesByOrdinal.ContainsKey(ordinal))
                {
                    Debug.Print("{0}: Duplicated ordinal {1} used for {2}", svc.ModuleName, ordinal, svc.Name);
                }
                else
                {
                    mod.ServicesByOrdinal.Add(ordinal, svc);
                }
            }
            if (svc.SyscallInfo != null)
            {
                if (!mod.ServicesByVector.TryGetValue(svc.SyscallInfo.Vector, out var services))
                {
                    services = new List<SystemService>();
                    mod.ServicesByVector.Add(svc.SyscallInfo.Vector, services);
                }
                services.Add(svc);
            }
        }

        private void LoadTypes(SerializedLibrary serializedLibrary)
        {
            if (serializedLibrary.Types != null)
            {
                foreach (var sType in serializedLibrary.Types)
                {
                    LoadType(sType);
                }
            }
        }

        public DataType LoadType(SerializedType sType)
        {
            return sType.Accept(this);
        }

        public ExternalProcedure? LoadExternalProcedure(
            ProcedureBase_v1 sProc, ProcedureCharacteristics? chr = null)
        {
            if (sProc.Name is null)
                return null;
            var sSig = sProc.Signature;
            var sser = new ProcedureSerializer(platform, this, this.defaultConvention ?? "");
            var sig = sser.Deserialize(sSig, platform.Architecture.CreateFrame());    //$BUGBUG: catch dupes?
            if (sig is null)
                return null;
            return new ExternalProcedure(sProc.Name, sig, chr)
            {
                EnclosingType = sSig?.EnclosingType
            };
        }

        public void ReadDefaults(SerializedLibraryDefaults? defaults)
        {
            if (defaults != null && defaults.Signature != null)
            {
                defaultConvention = defaults.Signature.Convention;
            }
            if (string.IsNullOrEmpty(defaultConvention))
            {
                defaultConvention = platform.DefaultCallingConvention;
            }
        }

        public DataType VisitPrimitive(PrimitiveType_v1 primitive)
        {
            var bitSize = primitive.Domain != Domain.Boolean
                ? bitsPerStorageUnit * primitive.ByteSize
                : 1;

            var pt = PrimitiveType.Create(primitive.Domain, bitSize);
            pt.Qualifier = primitive.Qualifier;
            return pt;
        }

        public DataType VisitPointer(PointerType_v1 pointer)
        {
            DataType dt;
            if (pointer.DataType == null)
                dt = new UnknownType();
            else
            {
                try
                {
                    //$TODO: remove the try-catch when done.
                    dt = pointer.DataType.Accept(this);
#if FUTURE
                    //$HACK: this is a dangerous assumption, not all
                    // pointers to char are pointing to strings -- 
                    // but most (90% or more) do. With const char
                    // this rises to 99%
                    if (dt is PrimitiveType pt && pt.Domain == Domain.Character)
                    {
                        dt = StringType.NullTerminated(dt);
                    }
#endif
                }
                catch
                {
                    Debug.Print("** Dropping exception on floor ***********");
                    dt = new UnknownType(platform.PointerType.Size);
                }
            }
            int bitSize = platform.PointerType.BitSize;
            if (pointer.PointerSize != 0)
            {
                bitSize = pointer.PointerSize * platform.Architecture.MemoryGranularity;
            }
            return new Pointer(dt, bitSize) { Qualifier = pointer.Qualifier };
        }

        public DataType VisitMemberPointer(MemberPointer_v1 memptr)
        {
            DataType baseType;
            if (memptr.DeclaringClass == null)
                baseType = new UnknownType();
            else 
                baseType = memptr.DeclaringClass.Accept(this);
            DataType dt;
            if (memptr.MemberType == null)
                dt = new UnknownType();
            else
                dt = memptr.MemberType.Accept(this);
            return new MemberPointer(baseType, dt, platform.PointerType.BitSize);
        }

        public DataType VisitReference(ReferenceType_v1 reference)
        {
            DataType dt;
            if (reference.Referent == null)
                dt = new UnknownType();
            else
                dt = reference.Referent.Accept(this);
            return new ReferenceTo(dt);
        }

        public DataType VisitArray(ArrayType_v1 array)
        {
            var dt = array.ElementType!.Accept(this);
            return new ArrayType(dt, array.Length);
        }

        public DataType VisitCode(CodeType_v1 code)
        {
            return new CodeType();
        }

        public DataType VisitString(StringType_v2 str)
        {
            var dt = str.CharType!.Accept(this);
            if (str.Termination ==  StringType_v2.ZeroTermination)
                return StringType.NullTerminated(dt);
            if (str.Termination == StringType_v2.MsbTermination)
                return StringType.LengthPrefixedStringType((PrimitiveType)dt, PrimitiveType.Byte);
            throw new NotImplementedException();
        }

        public DataType VisitSignature(SerializedSignature sSig)
        {
            var sser = new ProcedureSerializer(platform, this, this.defaultConvention!);
            return sser.Deserialize(sSig, platform.Architecture.CreateFrame())!;
        }

        public DataType VisitStructure(StructType_v1 structure)
        {
            if (structure.Name == null || !structures.TryGetValue(structure.Name, out var str))
            {
                str = new StructureType(structure.Name, structure.ByteSize, true);
                str.ForceStructure = structure.ForceStructure;
                if (structure.Name != null)
                {
                    structures.Add(structure.Name, str);
                }
                if (structure.Fields != null)
                {
                    var fields = structure.Fields.Select(f => new StructureField(f.Offset, f.Type!.Accept(this), f.Name));
                    str.Fields.AddRange(fields);
                }
                // str.Size = str.GetInferredSize();
                return str;
            }
            else if (str.Fields.Count == 0 && structure.Fields != null)
            {
                // Forward reference resolved.
                var fields = structure.Fields.Select(
                    f => new StructureField(
                        f.Offset,
                        f.Type!.Accept(this),
                        f.Name));
                str.Fields.AddRange(fields);
                str.Size = structure.ByteSize;
                return str;
            }
            else
            { 
                return str;
            }
        }

        public DataType VisitTypedef(SerializedTypedef typedef)
        {
            var dt = typedef.DataType!.Accept(this);
            //$BUGBUG: check for type equality if already exists.
            if (types.TryGetValue(typedef.Name!, out var dtOld) &&
                dtOld is TypeReference tr)
            {
                tr.Referent = dt;
            }
            else
            {
                types[typedef.Name!] = dt;
            }
            return dt;
        }

        public DataType VisitTypeReference(TypeReference_v1 typeReference)
        {
            var typeName = typeReference.TypeName!;
            if (types.TryGetValue(typeName, out DataType type))
                return new TypeReference(typeName, type);
            return new TypeReference(typeName, new UnknownType());
        }

        public DataType VisitUnion(UnionType_v1 sUnion)
        {
            if (sUnion.Name == null || !unions.TryGetValue(sUnion.Name, out var union))
            {
                union = new UnionType (sUnion.Name, null, true);
                if (sUnion.Name != null)
                    unions.Add(sUnion.Name, union);
                if (sUnion.Alternatives != null)
                {
                    var alts = sUnion.Alternatives.Select((a, i) => new UnionAlternative(a.Name, a.Type!.Accept(this), i));
                    union.Alternatives.AddRange(alts);
                }
                return union;
            }
            else if (union.Alternatives.Count == 0)
            {
                // Resolve forward reference.
                var alts = sUnion.Alternatives.Select((a, i) => new UnionAlternative(a.Name, a.Type!.Accept(this), i));
                union.Alternatives.AddRange(alts);
                return union;
            }
            else
            {
                return union;
            }
        }

        public DataType VisitEnum(SerializedEnumType enumType)
        {
            var members = enumType.Values != null
                ? enumType.Values.ToSortedList(k => k.Name!, v => (long)v.Value)
                : new SortedList<string, long>();
            return new EnumType
            {
                Name = enumType.Name!,
                Size = enumType.Size,
                Members = members
            };
        }

        public DataType VisitTemplate(SerializedTemplate sTemplate)
        {
            //$TODO: Reko's type system doesn't encompass templated / generic
            // types yet, so we fake a template instance.
            var dts = sTemplate.TypeArguments.Select(ta => ta.Accept(this));
            return new StructureType
            {
                Name = $"{sTemplate.Name}<{string.Join(",", dts)}>",
            };
        }

        public DataType VisitVoidType(VoidType_v1 voidType)
        {
            return VoidType.Instance;
        }

        public void SetModuleName(string libName)
        {
            this.moduleName = libName;
        }
    }
}
