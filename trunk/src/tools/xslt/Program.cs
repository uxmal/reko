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

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;

namespace Decompiler.Tools.Xslt
{
	public class Program
	{
		static int Main(string[] args)
		{
			if (args.Length != 3)
				Usage();
			if (!File.Exists(args[0]))
			{
				Console.Out.WriteLine("Transform file {0} is missing.", args[0]);
				return 1;
			}
			if (!File.Exists(args[1]))
			{
				Console.Out.WriteLine("Source file {0} is missing.", args[1]);
				return 1;
			}
			try
			{
				StringWriter sw = new StringWriter();
				using (Stream stmSheet = new FileStream(args[0], FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					XmlTextReader rdrSheet = new XmlTextReader(stmSheet);
					FilteringXmlWriter wrtOut = new FilteringXmlWriter(sw);
#if VS2003 || MONO

					XslTransform xslt = new XslTransform();
					xslt.Load(rdrSheet, null, null);
					using (FileStream stm = new FileStream(args[1], FileMode.Open, FileAccess.Read))
					{
						XPathDocument doc = new XPathDocument(stm);
						xslt.Transform(doc, null, wrtOut);
					}
				
#else
					XslCompiledTransform xslt = new XslCompiledTransform();
					xslt.Load(rdrSheet);
					xslt.Transform(args[1], wrtOut);
#endif
				}

				StringReader rdr = new StringReader(sw.ToString().Replace("&amp;", "&")); 
				using (Stream stm = new FileStream(args[2], FileMode.Create, FileAccess.Write, FileShare.Read))
				{
					StreamWriter sw2 = new StreamWriter(stm);
					int ch = rdr.Read();
					while (ch >= 0)
					{
						sw2.Write((char)ch);
						ch = rdr.Read();
					}
					sw2.Close();
				}
			}
			catch (Exception e)
			{
				Console.Out.WriteLine("An error occurred when transforming. " + e.Message);
				return 1;
			}
			return 0;
		}

		private static void Usage()
		{
			Console.Out.WriteLine("usage: xslt [transform] [inputfile] [outputfile]");
		}
	}
}
