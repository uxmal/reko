#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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
    /// Knows how to persist and depersist type libraries.
    /// </summary>
    public class TypeLibraryLoader : ISerializedTypeVisitor<DataType>
    {
        private IProcessorArchitecture arch;
        private Dictionary<string, DataType> types;
        private Dictionary<string, UnionType> unions;
        private Dictionary<string, StructureType> structures;
        private Dictionary<string, ProcedureSignature> procedures;
        private bool caseInsensitive;
        private string defaultConvention;

        public TypeLibraryLoader(IProcessorArchitecture arch)
        {
            this.arch = arch;
        }

        public TypeLibrary Load(SerializedLibrary serializedLibrary)
        {
            caseInsensitive = serializedLibrary.Case == "insensitive";
            var cmp = caseInsensitive ? StringComparer.InvariantCultureIgnoreCase : StringComparer.InvariantCulture;
            this.types = new Dictionary<string, DataType>(cmp)
            {
                { "va_list", PrimitiveType.Pointer32 } ,        //$BUGBUG: hardwired
                { "size_t", PrimitiveType.UInt32 },             //$BUGBUG: hardwired
            };
            this.procedures = new Dictionary<string, ProcedureSignature>(cmp);
            this.unions = new Dictionary<string, UnionType>(cmp);
            this.structures = new Dictionary<string, StructureType>(cmp);
            ReadDefaults(serializedLibrary.Defaults);

            if (serializedLibrary.Types != null)
            {
                foreach (var sType in serializedLibrary.Types)
                {
                    sType.Accept(this);
                }
            }
            if (serializedLibrary.Procedures != null)
            {
                foreach (object o in serializedLibrary.Procedures)
                {
                    SerializedProcedure sp = o as SerializedProcedure;
                    if (sp != null)
                    {
                        try
                        {
                            ProcedureSerializer sser = new ProcedureSerializer(arch, this, this.defaultConvention);
                            procedures[sp.Name] = sser.Deserialize(sp.Signature, arch.CreateFrame());    //$BUGBUG: catch dupes?   
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException(
                                string.Format("An error occurred when loading the signature of procedure {0}.", sp.Name),
                                ex);
                        }
                    }
                }
            }
            return new TypeLibrary(types, procedures);
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
        public DataType VisitPrimitive(SerializedPrimitiveType primitive)
        {
            return PrimitiveType.Create(primitive.Domain, primitive.ByteSize);
        }

        public DataType VisitPointer(SerializedPointerType pointer)
        {
            var dt = pointer.DataType.Accept(this);
            return new Pointer(dt, arch.PointerType.Size);
        }

        public DataType VisitArray(SerializedArrayType array)
        {
            var dt = array.ElementType.Accept(this);
            return new ArrayType(dt, array.Length);
        }

        public DataType VisitSignature(SerializedSignature signature)
        {
            //$BUGBUG: what we want is to unify FunctionType and ProcedureSignature....
            return PrimitiveType.PtrCode32;
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
                else
                    Debug.Print("Huh? structure {0} has no fields?", structure.Name);
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
            return new TypeReference(typeReference.TypeName, types[typeReference.TypeName]);
        }

        public DataType VisitUnion(SerializedUnionType union)
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

        public DataType VisitVoidType(SerializedVoidType voidType)
        {
            return VoidType.Instance;
        }
    }
}
