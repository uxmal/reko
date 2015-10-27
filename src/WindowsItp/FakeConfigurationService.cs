using Reko.Arch.X86;
using Reko.Core.Assemblers;
using Reko.Core.Configuration;
using Reko.Gui;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reko.WindowsItp
{
    public class FakeConfigurationService : IConfigurationService
    {
        public ICollection GetImageLoaders()
        {
            throw new NotImplementedException();
        }

        public System.Collections.ICollection GetArchitectures()
        {
            throw new NotImplementedException();
        }

        public System.Collections.ICollection GetEnvironments()
        {
            throw new NotImplementedException();
        }

        public ICollection GetRawFiles()
        {
            throw new NotImplementedException();
        }

        public OperatingEnvironment GetEnvironment(string envName)
        {
            throw new NotImplementedException();
        }

        public Core.IProcessorArchitecture GetArchitecture(string archLabel)
        {
            return new X86ArchitectureFlat32();
        }

        public System.Collections.ICollection GetSignatureFiles()
        {
            throw new NotImplementedException();
        }

        public ICollection GetAssemblers()
        {
            throw new NotImplementedException();
        }

        public Assembler GetAssembler(string assemblerName)
        {
            throw new NotImplementedException();
        }

        public RawFileElement GetRawFile(string rawFileFormat)
        {
            throw new NotImplementedException();
        }

        public string GetPath(string path)
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
    }

}