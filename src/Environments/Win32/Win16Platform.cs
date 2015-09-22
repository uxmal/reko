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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Lib;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.IO;
using System.Linq;

namespace Reko.Environments.Win32
{
    public class Win16Platform : Platform
    {
        private TypeLibrary[] typelibs;

        public Win16Platform(IServiceProvider services, IProcessorArchitecture arch) 
            : base(services, arch)
        {
        }

        public override string DefaultCallingConvention
        {
            get { return "pascal";  }
        }

        public override BitSet CreateImplicitArgumentRegisters()
        {
            return new BitSet(0);
        }

        public override ProcedureSerializer CreateProcedureSerializer(ISerializedTypeVisitor<DataType> typeLoader, string defaultConvention)
        {
            return new X86ProcedureSerializer((IntelArchitecture)Architecture, typeLoader, defaultConvention);
        }

        public override SystemService FindService(int vector, ProcessorState state)
        {
            EnsureTypeLibraries();
            return null;
        }

        public override ProcedureBase GetTrampolineDestination(ImageReader imageReader, IRewriterHost host)
        {
            return null;
        }

        public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            EnsureTypeLibraries();
            return null;
        }

        public void EnsureTypeLibraries()
        {
            if (typelibs == null)
            {
                var cfgSvc = Services.RequireService<IConfigurationService>();
                var envCfg = cfgSvc.GetEnvironment("elf-neutral");
                var tlSvc = Services.RequireService<ITypeLibraryLoaderService>();
                this.typelibs = ((System.Collections.IEnumerable)envCfg.TypeLibraries)
                    .OfType<ITypeLibraryElement>()
                    .Select(tl => new WineSpecFileLoader(Services, tl.Name, File.ReadAllBytes(tl.Name))
                                    .Load(this))
                    .Where(tl => tl != null).ToArray();
            }
        }
    }
}
