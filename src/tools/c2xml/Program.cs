﻿#region License
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

using DocoptNet;
using Reko.Core;
using Reko.Core.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Reko.Tools.C2Xml
{
    /// <summary>
    /// This program is used to convert ANSI C header files into the 
    /// XML format that the Decompiler uses.
    /// </summary>
    class Program
    {
        private const string usage = @"c2xml - Convert ANSI C to Reko XML

Usage: 
    c2xml -a <arch> 
          -e <env>
          <inputfile> [<outputfile>]

Options:
  -a (x86|z80)     Processor architecture
  -e (win32|sysV)  Operating environment
";

        static int Main(string[] args)
        {
            return new Program().Execute(args);
        }

        public int Execute(string [] args)
        {
            TextReader input = Console.In;
            TextWriter output = Console.Out;
            var sc = new ServiceContainer();
            var rekoCfg = new DecompilerConfiguration();

            var docopt = new Docopt();
            IDictionary<string, ValueObject> options;
            try {
                options = docopt.Apply(usage, args);
            } catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return 1;
            }
            var arch = rekoCfg.GetArchitecture(options["-a"].ToString());
            if (arch == null)
            {
                Console.WriteLine(
                    "c2xml: unknown architecture '{0}'. Check the c2xml config file for supported architectures.",
                    options["-a"]);
                return -1;
            }
            var envElem = rekoCfg.GetEnvironment(options["-e"].ToString());
            if (envElem == null)
            {
                Console.WriteLine(
                   "c2xml: unknown environment '{0}'. Check the c2xml config file for supported architectures.",
                   options["-e"]);
                return -1;
            }

            var platform = envElem.Load(sc, arch);
            try
            {
                input = new StreamReader(options["<inputfile>"].ToString());
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("c2xml: unable to open file {0} for reading. {1}", options["<inputfile>"], ex.Message);
                return 1;
            }

            if (options.ContainsKey("<outputfile>") &&  
                options["<outputfile>"] != null)
            {
                try
                {
                    output = new StreamWriter(options["<outputfile>"].ToString());
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("c2xml: unable to open file {0} for writing. {1}", options["<outputfile>"], ex.Message);
                    return 1;
                }
            }
            var xWriter = new XmlTextWriter(output)
            {
                Formatting = Formatting.Indented
            };

            XmlConverter c = new XmlConverter(input, xWriter, platform);
            c.Convert();
            output.Flush();
            return 0;
        }

        static void Usage()
        {
            Console.Error.WriteLine("usage: c2xml <platform> [<input-filename> [<output-filename>]]");
            Console.Error.WriteLine("   <platform>        name of platform the file is for");
            Console.Error.WriteLine("   <input-filename>  preprocessed c file  - standard input if omitted");
            Console.Error.WriteLine("   <output-filename> destination xml file - standard output if omitted");
            Console.Error.WriteLine("   ----------------------------------------------------");
            Console.Error.WriteLine("   Note: input files have to be preprocessed beforehand");
            Console.Error.WriteLine("     to preprocess a file with GCC:");
            Console.Error.WriteLine("       gcc -E FILE.h | sed '/^\\#/d' >RESULT.h");
            Console.Error.WriteLine("     to preprocess a file with the Microsoft C/C++ compiler:");
            Console.Error.WriteLine("       CL.EXE /EP FILE.h >RESULT.h");
        }
    }
}
