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

using Decompiler.Core.Output;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Decompiler.Core
{
	public class TypeLibrary
	{
		public TypeLibrary() : this(
            new Dictionary<string, DataType>(),
            new Dictionary<string, ProcedureSignature>())
        {
        }

        public TypeLibrary(IDictionary<string,DataType> types, IDictionary<string, ProcedureSignature> procedures)
        {
            this.Types = types;
            this.Signatures = procedures;
        }

        public IDictionary<string, DataType> Types { get; private set; }
        public IDictionary<string, ProcedureSignature> Signatures { get; private set; }

		public void Write(TextWriter writer)
		{
            var sl = new SortedList<string, ProcedureSignature>(
                Signatures,
                StringComparer.InvariantCulture);
            Formatter f = new Formatter(writer);
			foreach (KeyValuePair<string,ProcedureSignature> de in sl)
			{
				string name = (string) de.Key;
				de.Value.Emit(de.Key, ProcedureSignature.EmitFlags.ArgumentKind, f);
				writer.WriteLine();
			}
		}

		public void Load(IProcessorArchitecture arch, string fileName)
		{
			XmlSerializer ser = SerializedLibrary.CreateSerializer();
			SerializedLibrary slib;
			using (FileStream stm = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				slib = (SerializedLibrary) ser.Deserialize(stm);
			}
            Load(arch, slib);
		}

        [Obsolete]
        public void Load(IProcessorArchitecture arch, SerializedLibrary slib)
        {
            var tlldr = new TypeLibraryLoader(arch);
            var tlib = tlldr.Load(slib);
            Signatures = tlib.Signatures;
            Types = tlib.Types;
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
