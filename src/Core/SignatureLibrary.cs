/* 
 * Copyright (C) 1999-2008 John Källén.
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
using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Decompiler.Core
{
	public class SignatureLibrary
	{
		private IProcessorArchitecture arch;
		private bool caseInsensitive;
		private Hashtable hash;

		private string defaultConvention;

		public SignatureLibrary(IProcessorArchitecture arch)
		{
			this.arch = arch; 
		}

		public void Write(TextWriter tw)
		{
			SortedList sl = new SortedList(hash, new Comparer(System.Globalization.CultureInfo.InvariantCulture));
			foreach (DictionaryEntry de in sl)
			{
				string name = (string) de.Key;
				ProcedureSignature sig = (ProcedureSignature) de.Value;
				sig.Emit(name, ProcedureSignature.EmitFlags.ArgumentKind, tw);
				tw.WriteLine();
			}
		}

		public void Load(string s)
		{
			hash = new Hashtable();
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
					SignatureSerializer sser = new SignatureSerializer(arch, this.defaultConvention);
					hash[key] = sser.Deserialize(sp.Signature, new Frame(null));
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
			ProcedureSignature sig = (ProcedureSignature) hash[procedureName];
			if (sig == null)
				throw new ArgumentException(string.Format("The imported function '{0}' was not found", procedureName));
			return sig;
		}

		public IDictionary Signatures
		{
			get { return hash; }
		}
	}
}
