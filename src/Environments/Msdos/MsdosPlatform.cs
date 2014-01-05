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
using Decompiler.Core.Serialization;
using System;
using System.IO;
using System.Reflection;

namespace Decompiler.Environments.Msdos
{
	public class MsdosPlatform : Platform
	{
		private SystemService [] realModeServices; 

		public MsdosPlatform(IServiceProvider services, IProcessorArchitecture arch) : base(services, arch)
		{
			LoadRealmodeServices(arch);
		}

		public override SystemService FindService(int vector, ProcessorState state)
		{
			foreach (SystemService svc in realModeServices)
			{
				if (svc.SyscallInfo.Matches(vector, state))
					return svc;
			}
			return null;
		}

        public override ProcedureSignature LookupProcedure(string procName)
        {
            throw new NotImplementedException();
        }

		public void LoadRealmodeServices(IProcessorArchitecture arch)
		{
			string prefix = Environment.GetEnvironmentVariable("DECOMPILERROOTDIR") ?? ".";
			// TODO: extract runtime files ( like "realmodeintservices.xml") to their own directory ?
			string libPath = Path.Combine(prefix,"src","Environments","Msdos","realmodeintservices.xml");
			if (!File.Exists (libPath))
			{
				prefix = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);
				libPath = Path.Combine(prefix,"realmodeintservices.xml");
				if (!File.Exists(libPath))
				{
					libPath = Path.Combine(Directory.GetCurrentDirectory() , "realmodeintservices.xml");
				}
			}

            SerializedLibrary lib;
            using (FileStream stm = new FileStream(libPath, FileMode.Open))
			{
                lib = SerializedLibrary.LoadFromStream(stm);
			}

            int i = 0;
			realModeServices = new SystemService[lib.Procedures.Count];
			foreach (SerializedService ssvc in lib.Procedures)
			{
				realModeServices[i++] = ssvc.Build(arch);
			}
		}

        public override string DefaultCallingConvention
        {
            get { return "cdecl"; }
        }
	}
}
