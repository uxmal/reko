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

using Reko.Core.Assemblers;
using Reko.Core.Configuration;
using Reko.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using Reko.Core;

namespace Reko.WindowsItp
{
    public class FakeConfigurationService : IConfigurationService
    {
        public ICollection<Architecture> GetArchitectures()
        {
            throw new NotImplementedException();
        }

        public ICollection<AssemblerElement> GetAssemblers()
        {
            throw new NotImplementedException();
        }

        public ICollection<OperatingEnvironment> GetEnvironments()
        {
            throw new NotImplementedException();
        }

        public ICollection<LoaderConfiguration> GetImageLoaders()
        {
            throw new NotImplementedException();
        }

        public ICollection<RawFileElement> GetRawFiles()
        {
            throw new NotImplementedException();
        }

        public ICollection<SignatureFile> GetSignatureFiles()
        {
            throw new NotImplementedException();
        }

        public IProcessorArchitecture GetArchitecture(string archLabel)
        {
            throw new NotImplementedException();
        }

        public Assembler GetAssembler(string assemblerName)
        {
            throw new NotImplementedException();
        }

        public OperatingEnvironment GetEnvironment(string envName)
        {
            throw new NotImplementedException();
        }

        public LoaderConfiguration GetImageLoader(string loaderName)
        {
            throw new NotImplementedException();
        }

        public string GetInstallationRelativePath(params string [] pathComponents)
        {
            throw new NotImplementedException();
        }

        public RawFileElement GetRawFile(string rawFileFormat)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<Core.Configuration.UiStyle> GetDefaultPreferences()
        {
            return new Core.Configuration.UiStyle[] {
                    new UiStyleElement { Name = UiStyles.MemoryWindow, FontName="Lucida Console, 9pt"},
                    new UiStyleElement { Name = UiStyles.MemoryCode, ForeColor = "#000000", BackColor="#FFC0C0", },
                    new UiStyleElement { Name = UiStyles.MemoryHeuristic, ForeColor = "#000000", BackColor="#FFE0E0"},
                    new UiStyleElement { Name = UiStyles.MemoryData, ForeColor="#000000", BackColor="#C0C0FF" },

                    new UiStyleElement { Name = UiStyles.Disassembler,  FontName="Lucida Console, 9pt" },
                    new UiStyleElement { Name = UiStyles.DisassemblerOpcode, ForeColor = "#801010" },

                    new UiStyleElement { Name = UiStyles.CodeWindow, FontName = "Lucida Console, 9pt"},
                    new UiStyleElement { Name = UiStyles.CodeKeyword, ForeColor="#00C0C0" },
                    new UiStyleElement { Name = UiStyles.CodeComment, ForeColor="#00C000" },
                };
        }

        public ICollection<SymbolSource> GetSymbolSources()
        {
            return new List<SymbolSource>
            {
                new SymbolSourceDefinition { Name = "Bobsym", Description="BOB symbol loader", TypeName="BobSymSource,Bob" },
                new SymbolSourceDefinition { Name = "PDB", Description="PDB", TypeName="PDBSymSource,PDBLoader" }
            };
        }
    }
}