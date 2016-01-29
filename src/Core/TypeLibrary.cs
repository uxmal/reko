#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace Reko.Core
{
    /// <summary>
    /// A type library contains metadata about DataTypes and functions.
    /// </summary>
    public class TypeLibrary
	{
		public TypeLibrary() : this(
            new Dictionary<string, DataType>(),
            new Dictionary<string, ProcedureSignature>())
        {
        }

        public TypeLibrary(
            IDictionary<string,DataType> types,
            IDictionary<string, ProcedureSignature> procedures)
        {
            this.Types = types;
            this.Signatures = procedures;
            this.Modules = new Dictionary<string, ModuleDescriptor>();
        }

        public IDictionary<string, DataType> Types { get; private set; }
        public IDictionary<string, ProcedureSignature> Signatures { get; private set; }
        public IDictionary<string, ModuleDescriptor> Modules { get; private set; }

        public TypeLibrary Clone()
        {
            var clone = new TypeLibrary();
            clone.Types = new Dictionary<string, DataType>(this.Types);
            clone.Signatures = new Dictionary<string, ProcedureSignature>(this.Signatures);
            clone.Modules = this.Modules.ToDictionary(k => k.Key, v => v.Value.Clone(), StringComparer.InvariantCultureIgnoreCase);
            return clone;
        }

		public void Write(TextWriter writer)
		{
            var sl = new SortedList<string, ProcedureSignature>(
                Signatures,
                StringComparer.InvariantCulture);
            TextFormatter f = new TextFormatter(writer);
			foreach (KeyValuePair<string,ProcedureSignature> de in sl)
			{
				string name = (string) de.Key;
				de.Value.Emit(de.Key, ProcedureSignature.EmitFlags.ArgumentKind, f);
				writer.WriteLine();
			}
		}

		public ProcedureSignature Lookup(string procedureName)
		{
			ProcedureSignature sig;
            if (!Signatures.TryGetValue(procedureName, out sig))
                return null;
			return sig;
		}

        public DataType LookupType(string typedefName)
        {
            DataType dt;
            if (!Types.TryGetValue(typedefName, out dt))
                return null;
            return dt;
        }
    }
}
