#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;

namespace Reko.Tools.Xslt
{
	public class Program
	{
		static int Main(string[] args)
		{
            TextReader input = Console.In;
            TextWriter output = Console.Out;

            if (args.Length > 3 || args.Length < 1)
            {
                Usage();
                return 1;
            }
			if (!File.Exists(args[0]))
			{
				Console.Out.WriteLine("Transform file {0} is missing.", args[0]);
				return 1;
			}
            if (args.Length >= 2)
            {
                try
                {
                    input = new StreamReader(args[1]);
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine("Cannot open source file {0} for reading. {1}", args[1], ex.Message);
                    return 1;
                }
			}
            if (args.Length == 3)
            {
                try
                {
                    output = new StreamWriter(args[2]);
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine("Cannot open destination file {0} for writing. {1}", args[2], ex.Message);
                }
            }
			try
			{
                XslCompiledTransform xslt = new XslCompiledTransform();
                using (Stream stmSheet = new FileStream(args[0], FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					XmlTextReader rdrSheet = new XmlTextReader(stmSheet);
					xslt.Load(rdrSheet);
				}

                xslt.Transform(new XmlTextReader(input), new FilteringXmlWriter(output));
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
