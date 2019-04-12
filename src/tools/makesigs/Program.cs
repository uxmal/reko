#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

using makesigs;
using Reko.Core;
using Reko.Core.Configuration;
using System;
using System.ComponentModel.Design;

namespace RekoMakeSigs
{
    class Program
    {
        private readonly IServiceProvider services;
        private readonly IConfigurationService config;

        static void Main(string[] args)
        {
            string sourceFile;
            string destFile;

            Console.WriteLine("Reko static library signature generator");
            Console.WriteLine("=======================================");

            if (args.Length == 0)
            {
                Console.WriteLine("You are required to specify the library to generate a signature for");
                Console.WriteLine("Example to process mystaticlib.lib");
                Console.WriteLine("makeSigs mystaticlib.lib");
                Console.WriteLine("The produced signiture will be place in the same directory as the source");
                Console.WriteLine("To place the file in a different location you can provide a seconnd parameter");
                Console.WriteLine("Example to process c:\\mystaticlib.lib d:\\MySigs\\X86\\myrenamedstaticlib.lib.sig");
                Console.WriteLine("SigGen mystaticlib.lib d:\\MySigs\\X86\\myrenamedstaticlib.lib.sig");
                Console.WriteLine("Its important to keep the .sig file extension for the output file");
                return;
            }
            if (args.Length == 1)
            {
                sourceFile = args[0];
                destFile = sourceFile + ".sig";
            }
            if (args.Length == 2)
            {
                sourceFile = args[0];
                destFile = args[1];
            }
            else
            {
                Console.WriteLine("Too many parameter specfied");
                return;
            }

            var services = new ServiceContainer();
            var config = RekoConfigurationService.Load();

            services.AddService<IConfigurationService>(config);
           
            SignitureGenerator buf = new SignitureGenerator(services, sourceFile);
            if (buf.GenerateSigniture(destFile))
            {
                Console.WriteLine("Signature generation completed");
            }
            else
            {
                Console.WriteLine("Error generating the signatures");
            }
        }      
    }
}
