#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
        private List<LoaderConfiguration> imageLoaders = new List<LoaderConfiguration>();

        public ICollection<LoaderConfiguration> GetImageLoaders()
        {
            return imageLoaders;
        }

        public LoaderConfiguration GetImageLoader(string loaderName)
        {
            throw new NotImplementedException();
        }


        public ICollection<Architecture> GetArchitectures()
        {
            throw new NotImplementedException();
        }

        public ICollection<AssemblerElement> GetAssemblers()
        {
            throw new NotImplementedException();
        }

        public ICollection<RawFileElement> GetRawFiles()
        {
            throw new NotImplementedException();

        }
        public ICollection<OperatingEnvironment> GetEnvironments()
        {
            throw new NotImplementedException();
        }

        public RawFileElement GetRawFile(string rawFileFormat)
        {
            throw new NotImplementedException();
        }

        public ICollection<SignatureFile> GetSignatureFiles()
        {
            return new SignatureFileElement[0];
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
            throw new NotImplementedException();
        }

        public OperatingEnvironment GetEnvironment(string envName)
        {
            return null;
        }

        IEnumerable<UiStyle> IConfigurationService.GetDefaultPreferences()
        {
            throw new NotImplementedException();
        }

        public ICollection<SymbolSource> GetSymbolSources()
        {
            throw new NotImplementedException();
        }
    }
}
