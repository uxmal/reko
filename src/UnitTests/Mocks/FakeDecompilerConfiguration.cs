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

using Reko.Core;
using Reko.Core.Assemblers;
using Reko.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Mocks
{
    public class FakeDecompilerConfiguration : IConfigurationService
    {
        private List<LoaderDefinition> imageLoaders = new List<LoaderDefinition>();
        private IServiceProvider services;

        public FakeDecompilerConfiguration(IServiceProvider services = null)
        {
            this.services = services;
        }

        public ICollection<LoaderDefinition> GetImageLoaders()
        {
            return imageLoaders;
        }

        public LoaderDefinition GetImageLoader(string loaderName)
        {
            throw new NotImplementedException();
        }


        public ICollection<ArchitectureDefinition> GetArchitectures()
        {
            throw new NotImplementedException();
        }

        public ICollection<AssemblerDefinition> GetAssemblers()
        {
            throw new NotImplementedException();
        }

        public ICollection<RawFileDefinition> GetRawFiles()
        {
            throw new NotImplementedException();

        }
        public ICollection<PlatformDefinition> GetEnvironments()
        {
            throw new NotImplementedException();
        }

        public RawFileDefinition GetRawFile(string rawFileFormat)
        {
            throw new NotImplementedException();
        }

        public ICollection<SignatureFileDefinition> GetSignatureFiles()
        {
            return new SignatureFileDefinition[0];
        }

        public string GetInstallationRelativePath(params string[] pathComponents)
        {
            throw new NotImplementedException();
        }

        public IProcessorArchitecture GetArchitecture(string sArch)
        {
            throw new NotImplementedException();
        }

        public Assembler GetAssembler(string sAsm)
        {
            return new Reko.Assemblers.x86.X86TextAssembler(services, new Reko.Arch.X86.X86ArchitectureReal("x86-real-16"));
        }

        public PlatformDefinition GetEnvironment(string envName)
        {
            return null;
        }

        IEnumerable<UiStyleDefinition> IConfigurationService.GetDefaultPreferences()
        {
            throw new NotImplementedException();
        }

        public ICollection<SymbolSourceDefinition> GetSymbolSources()
        {
            throw new NotImplementedException();
        }
    }
}
