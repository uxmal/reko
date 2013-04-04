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
            this.types = new Dictionary<string, DataType>(cmp);
            this.procedures = new Dictionary<string, ProcedureSignature>(cmp);
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
                        string key = sp.Name;
                        ProcedureSerializer sser = new ProcedureSerializer(arch, this.defaultConvention);
                        procedures.Add(key, sser.Deserialize(sp.Signature, arch.CreateFrame()));
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
            throw new NotImplementedException();
        }

        public DataType VisitSignature(SerializedSignature signature)
        {
            throw new NotImplementedException();
        }

        public DataType VisitStructure(SerializedStructType structure)
        {
            throw new NotImplementedException();
        }

        public DataType VisitTypedef(SerializedTypedef typedef)
        {
            var dt = typedef.DataType.Accept(this);
            types.Add(typedef.Name, dt);
            return null;
        }

        public DataType VisitTypeReference(SerializedTypeReference typeReference)
        {
            throw new NotImplementedException();
        }

        public DataType VisitUnion(SerializedUnionType union)
        {
            throw new NotImplementedException();
        }

        public DataType VisitEnum(SerializedEnumType serializedEnumType)
        {
            throw new NotImplementedException();
        }
    }
}
