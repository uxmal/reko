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

using Decompiler.Arch.X86;
using Decompiler.Core.Configuration;
using Decompiler.Environments.Win32;
using Decompiler.Gui;
using Decompiler.Gui.Windows.Forms;
using Decompiler.ImageLoaders.MzExe;
using Decompiler.ImageLoaders.OdbgScript;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;

namespace Decompiler.WindowsItp
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
            using (var dlg = new Decompiler.Gui.Windows.Forms.SearchDialog())
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
                sc.AddService(typeof(IDecompilerConfigurationService), cfgSvc);
                dlg.Services = sc;
                dlg.ShowDialog(this);
            }
        }

        private class FakeConfigurationService : IDecompilerConfigurationService
        {
            public System.Collections.ICollection GetImageLoaders()
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

            public OperatingEnvironment GetEnvironment(string envName)
            {
                throw new NotImplementedException();
            }

            public Core.IProcessorArchitecture GetArchitecture(string archLabel)
            {
                return new X86ArchitectureFlat32();
            }

            public DefaultPreferences GetDefaultPreferences()
            {
                throw new NotImplementedException();
            }


            public System.Collections.ICollection GetSignatureFiles()
            {
                throw new NotImplementedException();
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
            var peLdr = new PeImageLoader(sc, "foo.exe" ,abImage, exe.e_lfanew); // new Address(0x00100000), new List<EntryPoint>());
            var addr = peLdr.PreferredBaseAddress;
            var lr = peLdr.Load(addr);
            var rr = peLdr.Relocate(addr);
            var win32 = new Win32Emulator(lr.Image, lr.Platform, lr.ImportReferences);
            var emu = new X86Emulator((IntelArchitecture) lr.Architecture, lr.Image, win32);
            emu.InstructionPointer = rr.EntryPoints[0].Address;
            emu.ExceptionRaised += delegate { throw new Exception(); };
            emu.WriteRegister(Registers.esp, peLdr.PreferredBaseAddress.Linear + 0x0FFC);
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
            var lr = ldr.Load(addr);
            var rr = ldr.Relocate(addr);
        }
    }
}
