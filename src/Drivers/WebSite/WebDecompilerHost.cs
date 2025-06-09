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

using Reko.Core;
using Reko.Core.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Web;

namespace Reko.WebSite
{
#if WEB_FORMS
    public class WebDecompilerHost : IDecompiledFileService
	{
		private StringWriter assembler;
		private StringWriter writer; 
		private StringWriter discard;

		public WebDecompilerHost()
		{
			assembler = new StringWriter();
			writer = new StringWriter();
			discard = new StringWriter();
		}

        public TextWriter CreateTextWriter(string path)
        {
            throw new NotImplementedException();
        }

		public string FetchSample(HttpServerUtility server, string file)
		{
			StringWriter sw = new StringWriter();
			string filename = server.MapPath(Path.Combine("SampleFiles", file));
			using (StreamReader rdr = new StreamReader(filename))
			{
				string line = rdr.ReadLine();
				while (line is not null)
				{
					sw.WriteLine(line);
					line = rdr.ReadLine();
				}
			}
			return sw.ToString();
		}

		public void PopulateSampleFiles(HttpServerUtility server, string wildcard, DropDownList ddl)
		{
			string sampleDir = server.MapPath("SampleFiles");
			ddl.Items.Add(new ListItem("Choose sample", ""));
			DirectoryInfo di = new DirectoryInfo(sampleDir);
			foreach (FileInfo f in di.GetFiles(wildcard))
			{
				ddl.Items.Add(new ListItem(Path.GetFileNameWithoutExtension(f.Name), f.Name));
			}
		}

		public TextWriter DecompiledCodeWriter
		{
			get { return writer; }
		}

		public TextWriter DisassemblyWriter
		{
			get { return assembler; }
		}

		public TextWriter IntermediateCodeWriter
		{
			get { return discard; }
		}

		public TextWriter TypesWriter
		{
			get { return writer; }
		}

		public void WriteDiagnostic(Diagnostic d, string format, params object[] args)
		{
			writer.Write("{0}: ", d);
			writer.Write(format, args);
			writer.WriteLine("<br />");
		}

        public void WriteDisassembly(Program program, Action<string, Dictionary<ImageSegment, List<ImageMapItem>>, Formatter> writer)
        {
            throw new NotImplementedException();
        }

        public void WriteIntermediateCode(Program program, Action<string, IEnumerable<IAddressable>, TextWriter> writer)
        {
            throw new NotImplementedException();
        }

        public void WriteTypes(Program program, Action<string, TextWriter> writer)
        {
            throw new NotImplementedException();
        }

        public void WriteDecompiledCode(Program program, Action<string, IEnumerable<IAddressable>, TextWriter> writer)
        {
            throw new NotImplementedException();
        }

        public void WriteGlobals(Program program, Action<string, TextWriter> writer)
        {
            throw new NotImplementedException();
        }
    }
#endif
}
