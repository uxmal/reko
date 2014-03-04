#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
using Decompiler.Core.Configuration;
using Decompiler.Core.Expressions;
using Decompiler.Core.Serialization;
using Decompiler.Core.Services;
using System;
using System.IO;
using System.Linq;

namespace Decompiler.Environments.Win32
{
	public class Win32Platform : Platform
	{
        private IProcessorArchitecture arch;
		private SystemService int3svc;
        private TypeLibrary [] TypeLibs;

		public Win32Platform(IServiceProvider services, IProcessorArchitecture arch) : base(services, arch)
		{
			this.arch = arch;
            int3svc = new SystemService
            {
                SyscallInfo = new SyscallInfo
                {
                    Vector = 3,
                    RegisterValues = new RegValue[0],
                },
                Name = "int3",
                Signature = new ProcedureSignature(null, new Identifier[0]),
                Characteristics = new ProcedureCharacteristics(),
            };
        }

        public override ProcedureSignature LookupProcedure(string procName)
        {
            if (TypeLibs == null)
            {
                var envCfg = Services.RequireService<IDecompilerConfigurationService>().GetEnvironment("win32");
                var tlSvc = Services.RequireService<ITypeLibraryLoaderService>();
                this.TypeLibs = ((System.Collections.IEnumerable) envCfg.TypeLibraries)
                    .OfType<ITypeLibraryElement>()
                    .Select(tl => tlSvc.LoadLibrary(arch, tl.Name)).ToArray();
            }
            return TypeLibs.Select(t => t.Lookup(procName))
                .Where(sig => sig != null)
                .FirstOrDefault();
        }

		public override SystemService FindService(int vector, ProcessorState state)
		{
			if (int3svc.SyscallInfo.Matches(vector, state))
				return int3svc;
			throw new NotImplementedException("INT services are not supported by " + this.GetType().Name);
		}

        public override string DefaultCallingConvention
        {
            get { return "stdapi"; }
        }

        public override ProcedureSignature SignatureFromName(string fnName, IProcessorArchitecture arch)
        {
            return SignatureGuesser.SignatureFromName(fnName, arch);
        }
	}
}
