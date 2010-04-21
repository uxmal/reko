/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core.Serialization;
using Decompiler.Core.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Decompiler.Core
{
	public class SignatureLibrary
	{
		private IProcessorArchitecture arch;
		private bool caseInsensitive;
		private Dictionary<string,ProcedureSignature> hash;

		private string defaultConvention;

		public SignatureLibrary(IProcessorArchitecture arch)
		{
			this.arch = arch; 
		}

		public void Write(TextWriter writer)
		{
            SortedList<string, ProcedureSignature> sl = new SortedList<string, ProcedureSignature>(
                hash,
                StringComparer.InvariantCulture);
            Formatter f = new Formatter(writer);
			foreach (KeyValuePair<string,ProcedureSignature> de in sl)
			{
				string name = (string) de.Key;
				de.Value.Emit(de.Key, ProcedureSignature.EmitFlags.ArgumentKind, f);
				writer.WriteLine();
			}
		}

		public void Load(string s)
		{
            hash = new Dictionary<string, ProcedureSignature>();
			XmlSerializer ser = new XmlSerializer(typeof (SerializedLibrary));
			SerializedLibrary slib;
			using (FileStream stm = new FileStream(s, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				slib = (SerializedLibrary) ser.Deserialize(stm);
			}
			caseInsensitive = slib.Case == "insensitive";
			ReadDefaults(slib.Defaults);
			foreach (object o in slib.Procedures)
			{
				SerializedProcedure sp = o as SerializedProcedure;
				if (sp != null)
				{
					string key = caseInsensitive ? sp.Name.ToUpper() : sp.Name;
					ProcedureSerializer sser = new ProcedureSerializer(arch, this.defaultConvention);
                    hash[key] = sser.Deserialize(sp.Signature, arch.CreateFrame());
				}
			}
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

		public ProcedureSignature Lookup(string procedureName)
		{
			if (caseInsensitive)
				procedureName = procedureName.ToUpper();
			ProcedureSignature sig;
            if (!hash.TryGetValue(procedureName, out sig))
                return null;
			return sig;
		}

		public IDictionary<string,ProcedureSignature> Signatures
		{
			get { return hash; }
		}
	}
}
