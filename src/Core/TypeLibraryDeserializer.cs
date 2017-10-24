#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Knows how to deserialize Reko type libraries and add types and 
    /// functions into a TypeLibrary.
    /// </summary>
    public class TypeLibraryDeserializer : ISerializedTypeVisitor<DataType>
    {
        private IPlatform platform;
        private IDictionary<string, DataType> types;
        private Dictionary<string, UnionType> unions;
        private Dictionary<string, StructureType> structures;
        private string defaultConvention;
        private string moduleName;
        private TypeLibrary library;

        public TypeLibraryDeserializer(IPlatform platform, bool caseInsensitive, TypeLibrary dstLib)
        {
            if (dstLib == null)
                throw new ArgumentNullException("dstLib");
            this.platform = platform;
            var cmp = caseInsensitive ? StringComparer.InvariantCultureIgnoreCase : StringComparer.InvariantCulture;
            this.library = dstLib;
            types = dstLib.Types;
            this.unions = new Dictionary<string, UnionType>(cmp);
            this.structures = new Dictionary<string, StructureType>(cmp);
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

        private ModuleDescriptor EnsureModule(string moduleName, TypeLibrary dstLib)
        {
            ModuleDescriptor mod;
            moduleName = moduleName ?? "";
            if (!dstLib.Modules.TryGetValue(moduleName, out mod))
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
                    var dt = this.LoadType(g.Type);
                    var sym = new ImageSymbol { Type = SymbolType.Data };
                    mod.GlobalsByName[g.Name] = sym;
                    if (g.Ordinal != GlobalVariable_v1.NoOrdinal)
                    {
                        mod.GlobalsByOrdinal[g.Ordinal] = sym;
                    }
                    library.Globals[g.Name] = dt;       //$REVIEW: How to cope with colissions MODULE1!foo and MODULE2!foo?
                }
            }
        }

        private void LoadProcedures(SerializedLibrary serializedLibrary)
        {
            if (serializedLibrary.Procedures != null)
            {
                foreach (object o in serializedLibrary.Procedures)
                {
                    Procedure_v1 sp = o as Procedure_v1;
                    if (sp != null)
                    {
                        LoadProcedure(sp);
                    }
                    SerializedService svc = o as SerializedService;
                    if (svc != null)
                    {
                        LoadService(svc);
                    }
                }
            }
        }

        public void LoadProcedure(Procedure_v1 sp)
        {
            try
            {
                var sser = new ProcedureSerializer(platform, this, this.defaultConvention);
                var signature = sser.Deserialize(sp.Signature, platform.Architecture.CreateFrame());
                library.Signatures[sp.Name] = signature;
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
                    mod.ServicesByVector[sp.Ordinal] = svc;
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

        public void LoadService(SerializedService ssvc)
        {
            var svc = ssvc.Build(platform, this.library);
            LoadService(svc.SyscallInfo.Vector, svc);
        }

        public void LoadService(string entryName, SystemService svc)
        {
            var mod = EnsureModule(svc.ModuleName, this.library);
            mod.ServicesByName.Add(entryName, svc);
        }

        public void LoadService(int ordinal, SystemService svc)
        {
            var mod = EnsureModule(svc.ModuleName, this.library);
            mod.ServicesByVector.Add(ordinal, svc);
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

        public ExternalProcedure LoadExternalProcedure(ProcedureBase_v1 sProc)
        {
            var sSig = sProc.Signature;
            var sser = new ProcedureSerializer(platform, this, this.defaultConvention);
            var sig = sser.Deserialize(sSig, platform.Architecture.CreateFrame());    //$BUGBUG: catch dupes?
            return new ExternalProcedure(sProc.Name, sig)
            {
                EnclosingType = sSig.EnclosingType
            };
        }

        public void ReadDefaults(SerializedLibraryDefaults defaults)
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
            return PrimitiveType.Create(primitive.Domain, primitive.ByteSize);
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
                }
                catch
                {
                    dt = new UnknownType();
                }
            }
            return new Pointer(dt, platform.PointerType.Size);
        }

        public DataType VisitMemberPointer(MemberPointer_v1 memptr)
        {
            var baseType = memptr.DeclaringClass.Accept(this);
            DataType dt;
            if (memptr.MemberType == null)
                dt = new UnknownType();
            else
                dt = memptr.MemberType.Accept(this);
            return new MemberPointer(baseType, dt, platform.PointerType.Size);
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
            var dt = array.ElementType.Accept(this);
            return new ArrayType(dt, array.Length);
        }

        public DataType VisitCode(CodeType_v1 code)
        {
            return new CodeType();
        }

        public DataType VisitString(StringType_v2 str)
        {
            var dt = str.CharType.Accept(this);
            if (str.Termination ==  StringType_v2.ZeroTermination)
                return StringType.NullTerminated(dt);
            throw new NotImplementedException();
        }

        public DataType VisitSignature(SerializedSignature sSig)
        {
            var sser = new ProcedureSerializer(platform, this, this.defaultConvention);
            return sser.Deserialize(sSig, platform.Architecture.CreateFrame());
        }

        public DataType VisitStructure(StructType_v1 structure)
        {
            StructureType str;
            if (structure.Name == null || !structures.TryGetValue(structure.Name, out str))
            {
                str = new StructureType(structure.Name, structure.ByteSize, true);
                str.ForceStructure = structure.ForceStructure;
                if (structure.Name != null)
                {
                    structures.Add(structure.Name, str);
                }
                if (structure.Fields != null)
                {
                    var fields = structure.Fields.Select(f => new StructureField(f.Offset, f.Type.Accept(this), f.Name));
                    str.Fields.AddRange(fields);
                }
                return str;
            }
            else if (str.Fields.Count == 0 && structure.Fields != null)
            {
                // Forward reference resolved.
                var fields = structure.Fields.Select(
                    f => new StructureField(
                        f.Offset,
                        f.Type.Accept(this),
                        f.Name));
                str.Fields.AddRange(fields);
                return str;
            }
            else
            { 
                return str;
            }
        }

        public DataType VisitTypedef(SerializedTypedef typedef)
        {
            var dt = typedef.DataType.Accept(this);
            types[typedef.Name] = dt;       //$BUGBUG: check for type equality if already exists.
            return null;
        }

        public DataType VisitTypeReference(TypeReference_v1 typeReference)
        {
            DataType type;
            if (types.TryGetValue(typeReference.TypeName, out type))
                return new TypeReference(typeReference.TypeName, type);
            return new TypeReference(typeReference.TypeName, new UnknownType());
        }

        public DataType VisitUnion(UnionType_v1 sUnion)
        {
            UnionType union;
            if (sUnion.Name == null || !unions.TryGetValue(sUnion.Name, out union))
            {
                union = new UnionType (sUnion.Name, null, true);
                if (sUnion.Name != null)
                    unions.Add(sUnion.Name, union);
                if (sUnion.Alternatives != null)
                {
                    var alts = sUnion.Alternatives.Select((a, i) => new UnionAlternative(a.Name, a.Type.Accept(this), i));
                    union.Alternatives.AddRange(alts);
                }
                return union;
            }
            else if (union.Alternatives.Count == 0)
            {
                // Resolve forward reference.
                var alts = sUnion.Alternatives.Select((a, i) => new UnionAlternative(a.Name, a.Type.Accept(this), i));
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
            return PrimitiveType.Word32;
        }

        public DataType VisitTemplate(SerializedTemplate sTemplate)
        {
            throw new NotImplementedException();
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
