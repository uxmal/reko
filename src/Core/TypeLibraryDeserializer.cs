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

using Reko.Core.Collections;
using Reko.Core.Loading;
using Reko.Core.Serialization;
using Reko.Core.Services;
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

        /// <summary>
        /// Constructs a type library deserializer.
        /// </summary>
        /// <param name="platform"><see cref="IPlatform"/> instance to use.</param>
        /// <param name="caseInsensitive">True if string comparisons should be case insensitive.</param>
        /// <param name="dstLib">Type library to populate.</param>
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

        /// <summary>
        /// Loads a serialized type library.
        /// </summary>
        /// <param name="sLib">Serialized type library.</param>
        /// <returns>The resulting type library.</returns>
        public TypeLibrary Load(SerializedLibrary sLib)
        {
            moduleName = sLib.ModuleName;
            ReadDefaults(sLib.Defaults);
            LoadTypes(sLib);
            LoadProcedures(sLib);
            LoadGlobals(sLib);
            LoadAnnotations(sLib);
            LoadSegments(sLib);
            return library;
        }

        private ModuleDescriptor EnsureModule(string? moduleName, TypeLibrary dstLib)
        {
            moduleName ??= "";
            if (!dstLib.Modules.TryGetValue(moduleName, out ModuleDescriptor? mod))
            {
                mod = new ModuleDescriptor(moduleName);
                dstLib.Modules.Add(moduleName, mod);
            }
            return mod;
        }

        private void LoadGlobals(SerializedLibrary sLib)
        {
            var mod = EnsureModule(this.moduleName, this.library);
            if (sLib.Globals is not null)
            {
                foreach (var g in sLib.Globals.Where(gg => !string.IsNullOrEmpty(gg.Name) && gg.DataType is not null))
                {
                    var globalType = g.DataType!;
                    var globalName = g.Name!;
                    if (platform.Architecture.TryParseAddress(g.Address, out var addr))
                    {
                        //$HACK: we are extracting user variables from an .inc file.
                        //$BUG: This really should belong to a specific program.
                        // Most users are only looking at a single binary, but this will
                        // fail if 2 or more binaries are loaded.
                        // Implementing it correctly will require changes to project format.
                        library.GlobalsByAddress[addr] = new UserGlobal(addr, globalName, globalType);
                    }
                    else
                    {
                        var dt = this.LoadType(globalType);
                        //$REVIEW: why is a null address being passed to ImageSymbol.Create?
                        var sym = ImageSymbol.Create(SymbolType.Data, platform.Architecture, default, globalName);
                        mod.GlobalsByName[globalName] = sym;
                        if (g.Ordinal != GlobalVariable_v1.NoOrdinal)
                        {
                            mod.GlobalsByOrdinal[g.Ordinal] = sym;
                        }
                        //$REVIEW: How to cope with colissions MODULE1!foo and MODULE2!foo?
                        library.ImportedGlobals[globalName] = dt;
                    }
                }
            }
        }

        private void LoadProcedures(SerializedLibrary serializedLibrary)
        {
            if (serializedLibrary.Procedures is null)
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

        private void LoadAnnotations(SerializedLibrary serializedLibrary)
        {
            if (serializedLibrary.Annotations is null)
                return;
            foreach (var a in serializedLibrary.Annotations)
            {
                if (this.platform.TryParseAddress(a.Address, out var addr))
                {
                    var annotation = new Annotation(addr, a.Text ?? "");
                    library.Annotations[addr] = annotation;
                }
            }
        }

        private void LoadSegments(SerializedLibrary sLib)
        {
            if (sLib.Segments is null)
                return;
            foreach (var s in sLib.Segments)
            {
                var seg = MemoryMap_v1.LoadSegment(s, platform, NullEventListener.Instance);
                if (seg is not null)
                {
                    library.Segments.Add(seg.Address, seg);
                }
            }
        }

        /// <summary>
        /// Loads the procedure from the serialized library.
        /// </summary>
        /// <param name="sProc">Seralized procedure.</param>
        public void LoadProcedure(Procedure_v1 sProc)
        {
            try
            {
                if (sProc.Name is null)
                    return;
                if (sProc.Characteristics is not null)
                {
                    library.Characteristics[sProc.Name] = sProc.Characteristics;
                }
                var sser = new ProcedureSerializer(platform, this, this.defaultConvention ?? "");
                var signature = sser.Deserialize(sProc.Signature, platform.Architecture.CreateFrame());
                if (signature is null)
                    return;
                library.Signatures[sProc.Name] = signature;
                if (platform.TryParseAddress(sProc.Address, out var addr))
                {
                    this.library.Procedures[addr] = (sProc.Name, signature);
                }
                else
                {
                    var mod = EnsureModule(this.moduleName, this.library);
                    var svc = new SystemService
                    {
                        ModuleName = mod.ModuleName,
                        Name = sProc.Name,
                        Signature = signature,
                    };
                    mod.ServicesByName[sProc.Name] = svc;    //$BUGBUG: catch dupes?

                    if (sProc.Ordinal != Procedure_v1.NoOrdinal)
                    {
                        mod.ServicesByOrdinal[sProc.Ordinal] = svc;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print("An error occurred when loading the signature of procedure {0}.", sProc.Name);
                throw new ApplicationException(
                    string.Format("An error occurred when loading the signature of procedure {0}.", sProc.Name),
                    ex);
            }
        }

        /// <summary>
        /// Loads a serialized service into the type library.
        /// </summary>
        /// <param name="ssvc">Serialized service instance.</param>
        /// <returns>Deserialized service.
        /// </returns>
        public SystemService LoadService(SerializedService ssvc)
        {
            var svc = ssvc.Build(platform, this.library);
            LoadService(svc.SyscallInfo!.Vector, svc);
            return svc;
        }

        /// <summary>
        /// Loads a serialized service into the type library.
        /// </summary>
        /// <param name="entryName">Entry name of the service.</param>
        /// <param name="svc">Servie instance.</param>
        public void LoadService(string entryName, SystemService svc)
        {
            var mod = EnsureModule(svc.ModuleName, this.library);
            mod.ServicesByName[entryName] = svc;
        }

        /// <summary>
        /// Loads a serialized service into the type library.
        /// </summary>
        /// <param name="ordinal">Ordinal of the service.</param>
        /// <param name="svc">Servie instance.</param>
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
            if (svc.SyscallInfo is not null)
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
            if (serializedLibrary.Types is not null)
            {
                foreach (var sType in serializedLibrary.Types)
                {
                    LoadType(sType);
                }
            }
        }

        /// <summary>
        /// Loads a serialized type into the type library.
        /// </summary>
        /// <param name="sType">Serialized type.</param>
        /// <returns>The deserialized type.</returns>
        public DataType LoadType(SerializedType sType)
        {
            return sType.Accept(this);
        }

        /// <summary>
        /// Loads an external procedure from the serialized library.
        /// </summary>
        /// <param name="sProc">Serialized library.</param>
        /// <param name="chr">Optional procedure characteristics.
        /// </param>
        /// <returns>
        /// Loaded external procedure, or null if the procedure
        /// is missing a name or signature.
        /// </returns>
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

        /// <summary>
        /// Loads the default calling convention from the serialized library.
        /// </summary>
        /// <param name="defaults">Default settings.
        /// </param>
        public void ReadDefaults(SerializedLibraryDefaults? defaults)
        {
            if (defaults is not null && defaults.Signature is not null)
            {
                defaultConvention = defaults.Signature.Convention;
            }
            if (string.IsNullOrEmpty(defaultConvention))
            {
                defaultConvention = platform.DefaultCallingConvention;
            }
        }

        /// <inheritdoc/>
        public DataType VisitPrimitive(PrimitiveType_v1 primitive)
        {
            var bitSize = primitive.Domain != Domain.Boolean
                ? bitsPerStorageUnit * primitive.ByteSize
                : 1;

            var pt = PrimitiveType.Create(primitive.Domain, bitSize);
            pt.Qualifier = primitive.Qualifier;
            return pt;
        }

        /// <inheritdoc/>
        public DataType VisitPointer(PointerType_v1 pointer)
        {
            DataType dt;
            if (pointer.DataType is null)
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

        /// <inheritdoc/>
        public DataType VisitMemberPointer(MemberPointer_v1 memptr)
        {
            DataType baseType;
            if (memptr.DeclaringClass is null)
                baseType = new UnknownType();
            else 
                baseType = memptr.DeclaringClass.Accept(this);
            DataType dt;
            if (memptr.MemberType is null)
                dt = new UnknownType();
            else
                dt = memptr.MemberType.Accept(this);
            return new MemberPointer(baseType, dt, platform.PointerType.BitSize);
        }

        /// <inheritdoc/>
        public DataType VisitReference(ReferenceType_v1 reference)
        {
            DataType dt;
            if (reference.Referent is null)
                dt = new UnknownType();
            else
                dt = reference.Referent.Accept(this);
            return new ReferenceTo(dt);
        }

        /// <inheritdoc/>
        public DataType VisitArray(ArrayType_v1 array)
        {
            var dt = array.ElementType!.Accept(this);
            return new ArrayType(dt, array.Length);
        }

        /// <inheritdoc/>
        public DataType VisitCode(CodeType_v1 code)
        {
            return new CodeType();
        }

        /// <inheritdoc/>
        public DataType VisitString(StringType_v2 str)
        {
            var dt = str.CharType!.Accept(this);
            if (str.Termination ==  StringType_v2.ZeroTermination)
                return StringType.NullTerminated(dt);
            if (str.Termination == StringType_v2.MsbTermination)
                return StringType.LengthPrefixedStringType((PrimitiveType)dt, PrimitiveType.Byte);
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public DataType VisitSignature(SerializedSignature sSig)
        {
            var sser = new ProcedureSerializer(platform, this, this.defaultConvention!);
            return sser.Deserialize(sSig, platform.Architecture.CreateFrame())!;
        }

        /// <inheritdoc/>
        public DataType VisitStructure(StructType_v1 structure)
        {
            if (structure.Name is null || !structures.TryGetValue(structure.Name, out var str))
            {
                str = new StructureType(structure.Name, structure.ByteSize, true);
                str.ForceStructure = structure.ForceStructure;
                if (structure.Name is not null)
                {
                    structures.Add(structure.Name, str);
                }
                if (structure.Fields is not null)
                {
                    var fields = structure.Fields.Select(f => new StructureField(f.Offset, f.Type!.Accept(this), f.Name));
                    str.Fields.AddRange(fields);
                }
                // str.Size = str.GetInferredSize();
                return str;
            }
            else if (str.Fields.Count == 0 && structure.Fields is not null)
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public DataType VisitTypeReference(TypeReference_v1 typeReference)
        {
            var typeName = typeReference.TypeName!;
            if (types.TryGetValue(typeName, out DataType? type))
                return new TypeReference(typeName, type);
            return new TypeReference(typeName, new UnknownType());
        }

        /// <inheritdoc/>
        public DataType VisitUnion(UnionType_v1 sUnion)
        {
            if (sUnion.Name is null || !unions.TryGetValue(sUnion.Name, out var union))
            {
                union = new UnionType (sUnion.Name, null, true);
                if (sUnion.Name is not null)
                    unions.Add(sUnion.Name, union);
                if (sUnion.Alternatives is not null)
                {
                    var alts = sUnion.Alternatives.Select((a, i) => new UnionAlternative(a.Name, a.Type!.Accept(this), i));
                    union.Alternatives.AddRange(alts);
                }
                return union;
            }
            else if (union.Alternatives.Count == 0)
            {
                // Resolve forward reference.
                var alts = sUnion.Alternatives!.Select((a, i) => new UnionAlternative(a.Name, a.Type!.Accept(this), i));
                union.Alternatives.AddRange(alts);
                return union;
            }
            else
            {
                return union;
            }
        }

        /// <inheritdoc/>
        public DataType VisitEnum(SerializedEnumType enumType)
        {
            var members = enumType.Values is not null
                ? enumType.Values.ToSortedList(k => k.Name!, v => (long)v.Value)
                : new SortedList<string, long>();
            int size = enumType.Size;
            if (size == 0)
            {
                size = platform.Architecture.WordWidth.Size;
            }
            return new EnumType(enumType.Name!, size, members);
        }

        DataType ISerializedTypeVisitor<DataType>.VisitTemplate(SerializedTemplate sTemplate)
        {
            //$TODO: Reko's type system doesn't encompass templated / generic
            // types yet, so we fake a template instance.
            var dts = sTemplate.TypeArguments.Select(ta => ta.Accept(this));
            return new StructureType
            {
                Name = $"{sTemplate.Name}<{string.Join(",", dts)}>",
            };
        }

        /// <inheritdoc/>
        public DataType VisitVoidType(VoidType_v1 voidType)
        {
            return VoidType.Instance;
        }

        /// <summary>
        /// Sets the module name.
        /// </summary>
        /// <param name="libName">New module name.</param>
        public void SetModuleName(string libName)
        {
            this.moduleName = libName;
        }
    }
}
