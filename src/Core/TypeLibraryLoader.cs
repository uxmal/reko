#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Decompiler.Core
{
    /// <summary>
    /// Knows how to persist and depersist Decompiler type libraries (which are just XML files).
    /// </summary>
    public class TypeLibraryLoader : ISerializedTypeVisitor<DataType>
    {
        private IProcessorArchitecture arch;
        private Dictionary<string, DataType> types;
        private Dictionary<string, UnionType> unions;
        private Dictionary<string, StructureType> structures;
        private Dictionary<string, ProcedureSignature> signaturesByName;
        private string defaultConvention;
        private Dictionary<string, SystemService> servicesByName;
        private Dictionary<int, SystemService> servicesByOrdinal;
        private string moduleName;

        public TypeLibraryLoader(IProcessorArchitecture arch, bool caseInsensitive)
        {
            this.arch = arch;
            var cmp = caseInsensitive ? StringComparer.InvariantCultureIgnoreCase : StringComparer.InvariantCulture;
            this.types = new Dictionary<string, DataType>(cmp)
            {
                { "va_list", arch.FramePointerType } ,  
                { "size_t", arch.WordWidth },
            };
            this.signaturesByName = new Dictionary<string, ProcedureSignature>(cmp);
            this.unions = new Dictionary<string, UnionType>(cmp);
            this.structures = new Dictionary<string, StructureType>(cmp);
            this.servicesByName = new Dictionary<string, SystemService>(cmp);
            this.servicesByOrdinal = new Dictionary<int, SystemService>();
        }

        public TypeLibrary Load(SerializedLibrary sLib)
        {
            moduleName = sLib.ModuleName;
            ReadDefaults(sLib.Defaults);
            LoadTypes(sLib);
            LoadProcedures(sLib);
            return BuildLibrary();
        }

        public TypeLibrary BuildLibrary()
        {
            var lib = new TypeLibrary(types, signaturesByName);
            lib.ModuleName = moduleName;
            foreach (var de in servicesByName)
            {
                lib.ServicesByName.Add(de.Key, de.Value);
            }
            foreach (var de in servicesByOrdinal)
            {
                lib.ServicesByVector.Add(de.Key, de.Value);
            }
            return lib;
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
                var sser = arch.CreateProcedureSerializer(this, this.defaultConvention);
                var signature = sser.Deserialize(sp.Signature, arch.CreateFrame());
                signaturesByName[sp.Name] =  signature;   //$BUGBUG: catch dupes?   
                if (sp.Ordinal != Procedure_v1.NoOrdinal)
                {
                    servicesByOrdinal[sp.Ordinal] = new SystemService { 
                        Name = sp.Name,
                        Signature = signature,
                    };
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
            var svc = ssvc.Build(arch);
            servicesByOrdinal[svc.SyscallInfo.Vector] = svc;
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

        public void LoadType(SerializedType sType)
        {
            sType.Accept(this);
        }
        
        public void ReadDefaults(SerializedLibraryDefaults defaults)
        {
            if (defaults == null)
                return;
            if (defaults.Signature != null)
            {
                defaultConvention = defaults.Signature.Convention;
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
                dt = pointer.DataType.Accept(this);
            return new Pointer(dt, arch.PointerType.Size);
        }

        public DataType VisitMemberPointer(MemberPointer_v1 memptr)
        {
            var baseType = memptr.DeclaringClass.Accept(this);
            DataType dt;
            if (memptr.MemberType == null)
                dt = new UnknownType();
            else
                dt = memptr.MemberType.Accept(this);
            return new MemberPointer(baseType, dt, arch.PointerType.Size);
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

        public DataType VisitSignature(SerializedSignature signature)
        {
            //$BUGBUG: what we want is to unify FunctionType and ProcedureSignature....
            return new Pointer(new CodeType(), arch.PointerType.Size);
        }

        public DataType VisitStructure(SerializedStructType structure)
        {
            StructureType str;
            if (!structures.TryGetValue(structure.Name, out str))
            {
                 str = new StructureType(structure.Name, structure.ByteSize);
                structures.Add(structure.Name, str);
                if (structure.Fields != null)
                {
                    var fields = structure.Fields.Select(f => new StructureField(f.Offset, f.Type.Accept(this), f.Name));
                    str.Fields.AddRange(fields);
                }
                return str;
            }
            else
            {
                return new TypeReference(str);
            }
        }

        public DataType VisitTypedef(SerializedTypedef typedef)
        {
            var dt = typedef.DataType.Accept(this);
            types[typedef.Name] = dt;       //$BUGBUG: check for type equality if already exists.
            return null;
        }

        public DataType VisitTypeReference(SerializedTypeReference typeReference)
        {
            DataType type;
            if (types.TryGetValue(typeReference.TypeName, out type))
                return new TypeReference(typeReference.TypeName, type);
            return new TypeReference(typeReference.TypeName, new UnknownType());
        }

        public DataType VisitUnion(UnionType_v1 union)
        {
            UnionType un;
            if (union.Name == null || !unions.TryGetValue(union.Name, out un))
            {
                un = new UnionType { Name = union.Name };
                if (union.Name != null)
                    unions.Add(union.Name, un);
                var alts = union.Alternatives.Select(a => new UnionAlternative(a.Name, a.Type.Accept(this)));
                un.Alternatives.AddRange(alts);
                return un;
            }
            else 
            {
                return new TypeReference(un);
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

        public void LoadService(string entryName, SystemService svc)
        {
            this.servicesByName[entryName] = svc;
        }

        public void SetModuleName(string libName)
        {
            this.moduleName = libName;
        }

        public void LoadService(int ordinal, SystemService svc)
        {
            this.servicesByOrdinal[ordinal] = svc;
        }
    }
}
