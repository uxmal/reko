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
using Reko.Core.Assemblers;
using Reko.Core.Configuration;
using Reko.Environments.Win32;
using Reko.Gui;
using Reko.Gui.Windows.Forms;
using Reko.ImageLoaders.MzExe;
using Reko.ImageLoaders.OdbgScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;

namespace Reko.WindowsItp
{
    public partial class ItpForm : Form
    {
        public ItpForm()
        {
            InitializeComponent();
        }

        private void memoryControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new MemoryControlDialog())
            {
                dlg.ShowDialog(this);
            }
        }

        private void rtfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new RtfDialog())
            {
                dlg.ShowDialog(this);
            }
        }

        private void webControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new WebDialog())
            {
                dlg.ShowDialog(this);
            }
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sc = new System.ComponentModel.Design.ServiceContainer();
            sc.AddService(typeof(ISettingsService), new DummySettingsService());
            using (var dlg = new Reko.Gui.Windows.Forms.SearchDialog())
            {
                dlg.Services = sc;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    //string.foo();
                }
            }
        }

        private class DummySettingsService : ISettingsService
        {
            public object Get(string settingName, object defaultValue)
            {
                return null;
            }

            public string[] GetList(string settingName)
            {
                return null;
            }

            public void SetList(string name, IEnumerable<string> values)
            {
            }

            public void Set(string name, object value)
            {
            }


            public void Load()
            {
            }

            public void Save()
            {
            }
        }

        private void treeViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new TreeViewDialog())
            {
                dlg.ShowDialog(this);
            }
        }

        private void projectBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new ProjectBrowserDialog())
            {
                dlg.ShowDialog(this);
            }
        }

        private void activationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new ActiveControlForm())
            {
                dlg.ShowDialog(this);
            }
        }

        private void disassemblyControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new DisassemblyControlForm())
            {
                dlg.ShowDialog(this);
            }
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new UserPreferencesDialog())
            {
                var sc = new ServiceContainer();
                var cfgSvc = new FakeConfigurationService();
                sc.AddService(typeof(IConfigurationService), cfgSvc);
                var uipSvc = new UiPreferencesService(cfgSvc, new FakeSettingsService());
                uipSvc.Load();
                sc.AddService(typeof(IUiPreferencesService), uipSvc);
                dlg.Services = sc;
                dlg.ShowDialog(this);
            }
        }

        private class FakeSettingsService : ISettingsService
        {
            public object Get(string settingName, object defaultValue)
            {
                return defaultValue;
            }

            public string[] GetList(string settingName)
            {
                throw new NotImplementedException();
            }

            public void SetList(string name, IEnumerable<string> values)
            {
                throw new NotImplementedException();
            }

            public void Set(string name, object value)
            {
                throw new NotImplementedException();
            }

            public void Load()
            {
                throw new NotImplementedException();
            }

            public void Save()
            {
                throw new NotImplementedException();
            }
        }

        private class FakeUiPreferencesService : IUiPreferencesService
        {
            public event EventHandler UiPreferencesChanged;

            public FakeUiPreferencesService()
            {
                this.Styles = new Dictionary<string, Gui.UiStyle>();

            }
            public IDictionary<string,Gui.UiStyle> Styles { get; private set; }

            public Gui.UiStyle MemoryWindowStyle
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public System.Drawing.Font DisassemblerFont
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public Gui.UiStyle DisassemblerWindowStyle
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public Gui.UiStyle CodeWindowStyle
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public System.Drawing.Font SourceCodeFont
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public System.Drawing.Color SourceCodeForegroundColor
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public System.Drawing.Color SourceCodeBackgroundColor
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public System.Drawing.Size WindowSize
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public FormWindowState WindowState
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public void Load()
            {
                throw new NotImplementedException();
            }

            public void Save()
            {
                throw new NotImplementedException();
            }
        }

        private class FakeConfigurationService : IConfigurationService
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
                    new UiStyleElement { Name = UiStyles.MemoryData, ForeColor="#000000", BackColor="#8080FF" },
                               
                    new UiStyleElement { Name = UiStyles.Disassembler,  FontName="Lucida Console, 9pt" },
                    new UiStyleElement { Name = UiStyles.DisassemblerOpcode, ForeColor = "#801010" },
                               
                    new UiStyleElement { Name = UiStyles.CodeWindow, FontName = "Lucida Console, 9pt"},
                    new UiStyleElement { Name = UiStyles.CodeKeyword, ForeColor="#00C0C0" },
                    new UiStyleElement { Name = UiStyles.CodeComment, ForeColor="#00C000" },
                };
            }
        }

        private void emulatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sc = new ServiceContainer();
            var fs = new FileStream(@"D:\dev\jkl\dec\halsten\decompiler_paq\upx\demo.exe", FileMode.Open);
            var size = fs.Length;
            var abImage = new byte[size];
            fs.Read(abImage, 0, (int) size);
            var exe = new ExeImageLoader(sc, "foolexe", abImage);
            var peLdr = new PeImageLoader(sc, "foo.exe" ,abImage, exe.e_lfanew); 
            var addr = peLdr.PreferredBaseAddress;
            var program = peLdr.Load(addr);
            var rr = peLdr.Relocate(addr);
            var win32 = new Win32Emulator(program.Image, program.Platform, program.ImportReferences);
            var emu = new X86Emulator((IntelArchitecture) program.Architecture, program.Image, win32);
            emu.InstructionPointer = rr.EntryPoints[0].Address;
            emu.ExceptionRaised += delegate { throw new Exception(); };
            emu.WriteRegister(Registers.esp, (uint) peLdr.PreferredBaseAddress.ToLinear() + 0x0FFC);
            emu.Start();
        }

        private void ollyScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sc = new ServiceContainer();
            var fs = new FileStream(@"D:\dev\jkl\dec\halsten\decompiler_paq\upx\w32HelloWorld.exe", FileMode.Open);
            //var fs = new FileStream(@"D:\dev\jkl\dec\halsten\decompiler_paq\upx\demo.exe", FileMode.Open);
            var size = fs.Length;
            var abImage = new byte[size];
            fs.Read(abImage, 0, abImage.Length);
            var ldr = new OdbgScriptLoader(sc, "foo.exe", abImage);
            ldr.Argument = @"D:\dev\jkl\dec\halsten\decompiler_paq\upx\upx_ultimate.txt";
            var addr = ldr.PreferredBaseAddress;
            var program = ldr.Load(addr);
            var rr = ldr.Relocate(addr);
        }
    }
}
