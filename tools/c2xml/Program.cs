#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Decompiler.Tools.C2Xml
{
    /// <summary>
    /// This program is used to convert ANSI C header files into the 
    /// XML format that the Decompiler uses.
    /// </summary>
    class Program
    {
        static int Main(string[] args)
        {
            return new Program().Execute(args);
        }

        public int Execute(string []args)
        {
            TextReader input = Console.In;
            TextWriter output = Console.Out;
            if (args.Length > 2)
            {
                Usage();
                return 1;
            }
            if (args.Length >= 1)
            {
                try
                {
                    input = new StreamReader(args[0]);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("c2xml: Unable to open file {0} for reading. {1}", args[0], ex.Message);
                    return 1;
                }
            }
            if (args.Length == 2)
            {
                try
                {
                    output = new StreamWriter(args[1]);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("c2xml: Unable to open file {0} for writing. {1}", args[1], ex.Message);
                    return 1;
                }
            }
            var xWriter = new XmlTextWriter(output)
            {
                Formatting = Formatting.Indented
            };
            XmlConverter c = new XmlConverter(input, xWriter);
            c.Convert();
            output.Flush();
            return 0;
        }

        static void Usage()
        {
            Console.Error.WriteLine("usage: c2xml [input-filename [output-filename]]");
            Console.Error.WriteLine("   Reads the filename or standard input and converts the stream of tokens to XML");
        }
    }
}
