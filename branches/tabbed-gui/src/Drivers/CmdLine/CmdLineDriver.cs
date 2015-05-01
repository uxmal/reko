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

using Decompiler.Core.Configuration;
using Decompiler.Core.Services;
using Decompiler.Loading;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decompiler.CmdLine
{
    public class CmdLineDriver
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Usage();
                Environment.Exit(0);
            }
            var services = new ServiceContainer();
            var host = new CmdLineHost();
            var listener = new CmdLineListener();
            services.AddService(typeof(DecompilerEventListener), listener);
            services.AddService(typeof(IDecompilerConfigurationService), new DecompilerConfiguration());
            services.AddService(typeof(ITypeLibraryLoaderService), new TypeLibraryLoaderServiceImpl());
            var ldr = new Loader(services);
            var dec = new DecompilerDriver(ldr, host, services);
            dec.Decompile(args[0]);
        }

        private static void Usage()
        {
            Console.Out.WriteLine("usage: decompile <filename>");
            Console.Out.WriteLine("    <filename> can be either an executable file or a project file.");
        }
    }
}
