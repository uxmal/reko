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

using Reko.Core.Loading;
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

        /// <summary>
        /// Creates an empty type library where string comparisons 
        /// are case sensitive.
        /// </summary>
        public TypeLibrary() : this(
            false,
            new Dictionary<string, DataType>(),
            new Dictionary<string, FunctionType>(Comparer(false)),
            new Dictionary<string, ProcedureCharacteristics>(
                Comparer(false)),
            new Dictionary<string, DataType>())
        {
        }

        /// <summary>
        /// Creates an empty type library.
        /// </summary>
        /// <param name="caseInsensitive">Determines whether 
        /// names in the type library are matched using case sensitive
        /// or insensitive comparisons.
        /// </param>
        public TypeLibrary(bool caseInsensitive) : this(
            caseInsensitive,
            new Dictionary<string, DataType>(),
            new Dictionary<string, FunctionType>(Comparer(caseInsensitive)),
            new Dictionary<string, ProcedureCharacteristics>(
                Comparer(caseInsensitive)),
            new Dictionary<string, DataType>())
        {
        }

        /// <summary>
        /// Constructs a type library.
        /// </summary>
        /// <param name="types">Data types to include in the type library.</param>
        /// <param name="signatures">Functions to include in the type library.</param>
        /// <param name="characteristics"><see cref="ProcedureCharacteristics"/> to include in the type library.</param>
        /// <param name="importedGlobals">Global variables to include in the type library.</param>
        /// <param name="caseInsensitive">Whether name string comparisons should be 
        /// case sensitive or insensitive.</param>
        public TypeLibrary(
            bool caseInsensitive,
            IDictionary<string,DataType> types,
            IDictionary<string, FunctionType> signatures,
            IDictionary<string, ProcedureCharacteristics> characteristics,
            IDictionary<string, DataType> importedGlobals)
        {
            this.isCaseInsensitive = caseInsensitive;
            this.Types = types;
            this.Signatures = signatures;
            this.Characteristics = characteristics;
            this.ImportedGlobals = importedGlobals;
            this.Procedures = new Dictionary<Address, (string, FunctionType)>();
            this.GlobalsByAddress = new Dictionary<Address, UserGlobal>();
            this.Modules = new Dictionary<string, ModuleDescriptor>();
            this.Annotations = new Dictionary<Address, Annotation>();
            this.Segments = new SortedList<Address, ImageSegment>();
        }

        /// <summary>
        /// Maps addresses to procedure names and signatures.
        /// </summary>
        public IDictionary<Address, (string Name, FunctionType Signature)> Procedures { get; private set; }

        /// <summary>
        /// Maps addresses to global variables.
        /// </summary>
        public IDictionary<Address, UserGlobal> GlobalsByAddress { get; private set; }

        /// <summary>
        /// Maps names to data types.
        /// </summary>
        public IDictionary<string, DataType> Types { get; private set; }

        /// <summary>
        /// Maps names to function signatures.
        /// </summary>
        public IDictionary<string, FunctionType> Signatures { get; private set; }
        
        /// <summary>
        /// Maps procedure names to characteristics.
        /// </summary>
        public IDictionary<string, ProcedureCharacteristics> Characteristics { get; private set; }

        /// <summary>
        /// Maps global variable names to datatypes.
        /// </summary>
        public IDictionary<string, DataType> ImportedGlobals { get; private set; }

        /// <summary>
        /// Maps names to module descriptors.
        /// </summary>
        public IDictionary<string, ModuleDescriptor> Modules { get; private set; }

        /// <summary>
        /// Maps addresses to annotations (comments).
        /// </summary>
        public IDictionary<Address, Annotation> Annotations { get; private set; }

        /// <summary>
        /// Maps addresses to segments.
        /// </summary>
        public SortedList<Address, ImageSegment> Segments { get; }

        private static StringComparer Comparer(bool caseSensitive) =>
            caseSensitive
                ? StringComparer.InvariantCultureIgnoreCase
                : StringComparer.InvariantCulture;

        /// <summary>
        /// Creates a copy of this type library.
        /// </summary>
        /// <returns>A copy of this type library.
        /// </returns>
        public TypeLibrary Clone()
        {
            var clone = new TypeLibrary(this.isCaseInsensitive)
            {
                Types = new Dictionary<string, DataType>(this.Types),
                Signatures = new Dictionary<string, FunctionType>(this.Signatures, Comparer(this.isCaseInsensitive)),
                GlobalsByAddress = new Dictionary<Address, UserGlobal>(this.GlobalsByAddress),
                ImportedGlobals = new Dictionary<string, DataType>(this.ImportedGlobals),
                Procedures = new Dictionary<Address, (string, FunctionType)>(this.Procedures),
                Modules = this.Modules.ToDictionary(k => k.Key, v => v.Value.Clone(), StringComparer.InvariantCultureIgnoreCase),
                Annotations = this.Annotations.Values.ToDictionary(
                    k => k.Address, 
                    v => new Annotation(v.Address, v.Text)),
                isCaseInsensitive = this.isCaseInsensitive
            };
            return clone;
        }

        /// <summary>
        /// Writes a text representation of the type library to a <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">Output sink.</param>
		public void Write(TextWriter writer)
		{
            var f = new TextFormatter(writer);
			foreach (var de in Signatures.OrderBy(d => d.Key, StringComparer.InvariantCulture))
			{
				string name = de.Key;
				de.Value.Emit(de.Key, FunctionType.EmitFlags.ArgumentKind, f);
				writer.WriteLine();
			}

            var tf = new TypeReferenceFormatter(f);
            foreach (var de in ImportedGlobals.OrderBy(d => d.Key, StringComparer.InvariantCulture))
            {
                tf.WriteDeclaration(de.Value, de.Key);
            }
		}

        /// <summary>
        /// Lookup a function signature by name.
        /// </summary>
        /// <param name="procedureName">Procedure name.</param>
		public FunctionType? Lookup(string procedureName)
		{
            if (!Signatures.TryGetValue(procedureName, out FunctionType? sig))
                return null;
			return sig;
		}

        /// <summary>
        /// Lookup a type by name.
        /// </summary>
        /// <param name="typedefName">Data type name.</param>

        public DataType? LookupType(string typedefName)
        {
            if (!Types.TryGetValue(typedefName, out DataType? dt))
                return null;
            return dt;
        }
    }
}
