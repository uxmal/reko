#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Environments.Windows;
using Reko.Gui.Services;
using Reko.ImageLoaders.MzExe;
using Reko.ImageLoaders.OdbgScript;
using Reko.UserInterfaces.WindowsForms;
using Reko.UserInterfaces.WindowsForms.Controls;
using Reko.UserInterfaces.WindowsForms.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Reko.WindowsItp
{
    public partial class ItpForm : Form
    {
        private ProcedurePropertiesDialog procDlg;
        private Form propOptionsDlg;

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
            using (var dlg = new Reko.UserInterfaces.WindowsForms.Forms.SearchDialog())
            {
                dlg.Services = sc;
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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

            public void Delete(string name)
            {
                throw new NotImplementedException();
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

        private void emulatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sc = new ServiceContainer();
            var fs = new FileStream(@"D:\dev\jkl\dec\halsten\decompiler_paq\upx\demo.exe", FileMode.Open);
            var size = fs.Length;
            var abImage = new byte[size];
            fs.Read(abImage, 0, (int)size);
            var exe = new ExeImageLoader(sc, ImageLocation.FromUri("file:fool.exe"), abImage);
            var lfanew = exe.LoadLfaToNewHeader();
            var peLdr = new PeImageLoader(sc, ImageLocation.FromUri("file:foo.exe"), abImage, lfanew.Value);
            var addr = peLdr.PreferredBaseAddress;
            var program = peLdr.LoadProgram(addr);
            peLdr.Relocate(program, addr);
            var win32 = new Win32Emulator(program.SegmentMap, program.Platform, program.ImportReferences);
            var emu = program.Architecture.CreateEmulator(program.SegmentMap, win32);
            emu.InstructionPointer = program.EntryPoints.Values.First().Address;
            emu.ExceptionRaised += delegate { throw new Exception(); };
            emu.WriteRegister(Registers.esp, (uint)peLdr.PreferredBaseAddress.ToLinear() + 0x0FFC);
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
            var ldr = new OdbgScriptLoader(
                new Reko.Loading.NullImageLoader(
                    sc, ImageLocation.FromUri("foo.exe"), abImage));
            ldr.Argument = @"D:\dev\jkl\dec\halsten\decompiler_paq\upx\upx_ultimate.txt";
            var addr = ldr.PreferredBaseAddress;
            var program = ldr.LoadProgram(addr);
        }

        private void assumeRegistesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new AssumedRegisterValuesDialog();
            dlg.Architecture = new X86ArchitectureFlat64(new ServiceContainer(), "x86-protected-64", new Dictionary<string, object>());
            dlg.ShowDialog(this);
        }

        private void textViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new TextViewDialog();
            dlg.ShowDialog(this);
        }

        private void controlsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void codeViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void byteMapViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[] buf = GenerateImageData();
            var mem = new ByteMemoryArea(Address.Ptr32(0x0040000), buf);
            var dlg = new ByteMapDialog();
            var ctrl = new ByteMapView();
            dlg.Controls.Add(ctrl);
            ctrl.Dock = DockStyle.Fill;
            ctrl.SegmentMap = new SegmentMap(Address.Ptr32(0x0040000),
                new ImageSegment("foo", mem, AccessMode.ReadWriteExecute));
            dlg.ShowDialog();
        }

        private byte[] GenerateImageData()
        {
#if RANDOM
            var rnd = new Random();
            var buf = new byte[0x4000];
            rnd.NextBytes(buf);
            return buf;
#else
            return Enumerable.Range(0, 4000)
                .Select(n => (byte)n)
                .ToArray();
#endif
        }

        private void procedureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.procDlg is null)
            {
                this.procDlg = new ProcedurePropertiesDialog();
                this.procDlg.FormClosed += delegate { this.procDlg = null; };
            }
            this.procDlg.Show();
        }

        private void symbolSourcesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sc = new ServiceContainer();
            var cfgSvc = new FakeConfigurationService();
            var fsSvc = new FileSystemService();
            var uiSvc = new FakeDecompilerShellUiService(this);
            sc.AddService<IConfigurationService>(cfgSvc);
            sc.AddService<IFileSystemService>(fsSvc);
            sc.AddService<IDecompilerShellUiService>(uiSvc);
            var dlg = new SymbolSourceDialog();
            dlg.Services = sc;
            dlg.ShowDialog(this);
        }

        private void propertyOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.propOptionsDlg is null)
            {
                this.propOptionsDlg = new ProcedureOptionsDialog();
                this.propOptionsDlg.FormClosed += delegate { this.procDlg = null; };
            }
            this.propOptionsDlg.Show();
        }

        private void rewriterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new RewriterPerformanceDialog())
            {
                dlg.ShowDialog(this);
            }
        }

        private void suffixArrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new SuffixArrayPerformanceDialog())
            {
                dlg.ShowDialog(this);
            }
        }

        private void decoderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new DecoderPerformanceDialog())
            {
                dlg.ShowDialog(this);
            }
        }

        private void structureFieldsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new StructureFieldPerformanceDialog())
            {
                dlg.ShowDialog(this);
            }
        }
    }
}
