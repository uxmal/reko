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
using Decompiler.Core.Assemblers;
using Decompiler.Core.Configuration;
using System;
using System.Collections;
using System.Text;

namespace Decompiler.UnitTests.Mocks
{
    public class FakeDecompilerConfiguration : IConfigurationService
    {
        private ArrayList imageLoaders = new ArrayList();

        public System.Collections.ICollection GetImageLoaders()
        {
            return imageLoaders;
        }

        public System.Collections.ICollection GetArchitectures()
        {
            throw new NotImplementedException();
        }

        public ICollection GetAssemblers()
        {
            throw new NotImplementedException();
        }

        public ICollection GetRawFiles()
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

        public System.Collections.ICollection GetEnvironments()
        {
            throw new NotImplementedException();
        }

        public OperatingEnvironment GetEnvironment(string envName)
        {
            throw new NotImplementedException();
        }

        public RawFileElement GetRawFile(string rawFileFormat)
        {
            throw new NotImplementedException();
        }

        public DefaultPreferences GetDefaultPreferences()
        {
            throw new NotImplementedException();
        }


        public ICollection GetSignatureFiles()
        {
            return new SignatureFileElement[0];
        }


        public string GetPath(string path)
        {
            throw new NotImplementedException();
        }
    }
}
