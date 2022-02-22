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

using Reko.Core.Output;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reko.Core
{
    /// <summary>
    /// A type library contains metadata about DataTypes and functions.
    /// </summary>
    public class TypeLibrary
	{
        private bool isCaseInsensitive;

		public TypeLibrary(bool caseInsensitive = false) : this(
            caseInsensitive,
            new Dictionary<string, DataType>(),
            new Dictionary<string, FunctionType>(Comparer(caseInsensitive)),
            new Dictionary<string, ProcedureCharacteristics>(
                Comparer(caseInsensitive)),
            new Dictionary<string, DataType>())
        {
        }


        public TypeLibrary(
            bool caseInsensitive,
            IDictionary<string,DataType> types,
            IDictionary<string, FunctionType> procedures,
            IDictionary<string, ProcedureCharacteristics> characteristics,
            IDictionary<string, DataType> globals)
        {
            this.isCaseInsensitive = caseInsensitive;
            this.Types = types;
            this.Signatures = procedures;
            this.Characteristics = characteristics;
            this.Globals = globals;
            this.Procedures = new Dictionary<Address, (string, FunctionType)>();
            this.Modules = new Dictionary<string, ModuleDescriptor>();
        }

        public IDictionary<Address, (string Name, FunctionType Signature)> Procedures { get; private set; }
        public IDictionary<string, DataType> Types { get; private set; }
        public IDictionary<string, FunctionType> Signatures { get; private set; }
        public IDictionary<string, ProcedureCharacteristics> Characteristics { get; private set; }
        public IDictionary<string, DataType> Globals { get; private set; }
        public IDictionary<string, ModuleDescriptor> Modules { get; private set; }

        private static StringComparer Comparer(bool caseSensitive) =>
            caseSensitive
                ? StringComparer.InvariantCultureIgnoreCase
                : StringComparer.InvariantCulture;

        public TypeLibrary Clone()
        {
            var clone = new TypeLibrary(false)
            {
                Types = new Dictionary<string, DataType>(this.Types),
                Signatures = new Dictionary<string, FunctionType>(this.Signatures, Comparer(this.isCaseInsensitive)),
                Globals = new Dictionary<string, DataType>(this.Globals),
                Procedures = new Dictionary<Address, (string, FunctionType)>(this.Procedures),
                Modules = this.Modules.ToDictionary(k => k.Key, v => v.Value.Clone(), StringComparer.InvariantCultureIgnoreCase),
                isCaseInsensitive = this.isCaseInsensitive
            };
            return clone;
        }

		public void Write(TextWriter writer)
		{
            TextFormatter f = new TextFormatter(writer);
			foreach (var de in Signatures.OrderBy(d => d.Key, StringComparer.InvariantCulture))
			{
				string name = de.Key;
				de.Value.Emit(de.Key, FunctionType.EmitFlags.ArgumentKind, f);
				writer.WriteLine();
			}

            var tf = new TypeReferenceFormatter(f);
            foreach (var de in Globals.OrderBy(d => d.Key, StringComparer.InvariantCulture))
            {
                tf.WriteDeclaration(de.Value, de.Key);
            }
		}

		public FunctionType? Lookup(string procedureName)
		{
            if (!Signatures.TryGetValue(procedureName, out FunctionType? sig))
                return null;
			return sig;
		}

        public DataType? LookupType(string typedefName)
        {
            if (!Types.TryGetValue(typedefName, out DataType? dt))
                return null;
            return dt;
        }
    }
}
