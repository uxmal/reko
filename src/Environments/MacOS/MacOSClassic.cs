#region License
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

using Reko.Arch.M68k;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Lib;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Environments.MacOS
{
    public class MacOSClassic : Platform
    {
        private MacOsRomanEncoding encoding;

        public MacOSClassic(IServiceProvider services, IProcessorArchitecture arch)
            : base(services, arch, "macOs")
        {
            encoding = new MacOsRomanEncoding();
            LoadMacOsServices();
        }

        public override HashSet<RegisterStorage> CreateImplicitArgumentRegisters()
        {
            return new HashSet<RegisterStorage> { Registers.a7 };
        }

        public override ProcedureSerializer CreateProcedureSerializer(ISerializedTypeVisitor<DataType> typeLoader, string defaultConvention)
        {
            throw new NotImplementedException();
        }

        public override SystemService FindService(int vector, ProcessorState state)
        {
            if (TypeLibs.Length == 0)
                return null;
            SystemService svc;
            this.TypeLibs[0].ServicesByVector.TryGetValue(vector&0xFFFF , out svc);
            return svc;
        }

        public override string DefaultCallingConvention
        {
            get { throw new NotImplementedException(); }
        }

        public override Encoding DefaultTextEncoding
        {
            get { return encoding; }
        }

        public override ProcedureBase GetTrampolineDestination(ImageReader imageReader, IRewriterHost host)
        {
            return null;
        }

        public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            throw new NotImplementedException();
        }

        public override ExternalProcedure LookupProcedureByOrdinal(string moduleName, int ordinal)
        {
            return base.LookupProcedureByOrdinal(moduleName, ordinal);
        }

        public void LoadMacOsServices()
        {
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var envCfg = cfgSvc.GetEnvironment("macOs");
            var tlSvc = Services.RequireService<ITypeLibraryLoaderService>();
            this.TypeLibs = ((IEnumerable)envCfg.TypeLibraries)
                .OfType<ITypeLibraryElement>()
                .Select(tl => tlSvc.LoadLibrary(this, cfgSvc.GetInstallationRelativePath(tl.Name)))
                .Where(tl => tl != null).ToArray();
        }
    }
}
