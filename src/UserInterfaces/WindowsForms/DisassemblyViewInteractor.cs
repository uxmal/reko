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

using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.Core.Output;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Forms;
using Reko.Gui.Services;
using Reko.Gui.ViewModels;
using Reko.Services;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms
{
    public class DisassemblyViewInteractor : IWindowPane, ICommandTarget
    {
        private Address startAddress;
        private Program program;
        private TextBox txtDisassembly;
        private IServiceProvider sp;

        public DisassemblyViewInteractor()
        {

        }

        public IWindowFrame Frame { get; set; }

        public Program Program
        {
            get { return program; }
            set
            {
                program = value;
                DumpAssembler();
            }
        }

        public Address StartAddress
        {
            get { return startAddress; }
            set
            {
                startAddress = value;
                DumpAssembler();
            }
        }

        public void DumpAssembler()
        {
            var decSvc = sp.GetService<IDecompilerService>();
            if (decSvc is null)
                throw new InvalidOperationException("Expected IDecompilerService to be available.");

            if (!IsProgramLoaded(program))
            {
                txtDisassembly.Text = "";
            }
            else
            {
                int lines = CountVisibleLines();
                if (lines < 1)
                    lines = 1;
                using (var writer = new StringWriter())
                {
                    var dumper = new Dumper(program);
                    dumper.ShowAddresses = true;
                    dumper.ShowCodeBytes = true;
                    var options = new MachineInstructionRendererOptions(platform: program.Platform);
                    if (program.SegmentMap.TryFindSegment(StartAddress, out ImageSegment segment))
                    {
                        var formatter = new FormatterInstructionWriter(
                            new TextFormatter(writer),
                            program.Procedures,
                            program.ImageSymbols,
                            true);
                        var dasm = program.CreateDisassembler(program.Architecture, StartAddress).GetEnumerator();
                        while (dasm.MoveNext())
                        {
                            var instr = dasm.Current;
                            if (lines <= 0)
                                break;
                            dumper.DumpAssemblerLine(segment.MemoryArea, program.Architecture, instr, formatter, options);
                            --lines;
                        }
                    }
                    txtDisassembly.Text = writer.ToString();
                }
            }
        }

        private int CountVisibleLines()
        {
            return (txtDisassembly.Height + txtDisassembly.Font.Height - 1) / txtDisassembly.Font.Height;
        }

        internal void ClearText()
        {
            txtDisassembly.Text = "";
        }

        private bool IsProgramLoaded(Program program)
        {
            return
                program is not null &&
                program.Architecture is not null &&
                program.ImageMap is not null;
        }

        private async ValueTask GotoAddress()
        {
			var dlgFactory = sp.RequireService<IDialogFactory>();
            using (IAddressPromptDialog dlg = dlgFactory.CreateAddressPromptDialog())
            {
                if (await sp.GetService<IDecompilerShellUiService>().ShowModalDialog(dlg) == Gui.Services.DialogResult.OK)
                {
                    StartAddress = dlg.Address;
                    DumpAssembler();
                }
            }
        }

        #region IWindowPane Members

        public object CreateControl()
        {
            txtDisassembly = new TextBox
			{
				Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0))),
				Multiline = true,
				Name = "txtDisassembly",
				ReadOnly = true,
				WordWrap = false,
			};
            txtDisassembly.Resize += txtDisassembly_Resize;
            txtDisassembly.KeyDown += txtDisassembly_KeyDown;
            txtDisassembly.KeyPress += txtDisassembly_KeyPress;
            return txtDisassembly;
        }

        void txtDisassembly_KeyDown(object sender, KeyEventArgs e)
        {
            Debug.Print("Down:  c:{0} d:{1} v:{2}", e.KeyCode, e.KeyData, e.KeyValue);
        }

        void txtDisassembly_KeyPress(object sender, KeyPressEventArgs e)
        {
            Debug.Print("Press: ch:{0}", e.KeyChar);
        }

        public void SetSite(IServiceProvider sp)
        {
            this.sp = sp;
        }

        public void Close()
        {
        }

        #endregion

        #region ICommandTarget Members

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch ((CmdIds) cmdId.ID)
                {
                case CmdIds.ViewGoToAddress: status.Status = MenuStatus.Enabled | MenuStatus.Visible; return true;
                case CmdIds.ActionMarkProcedure: status.Status = MenuStatus.Enabled | MenuStatus.Visible; return true;
                }
            }
            return false;
        }

        public async ValueTask<bool> ExecuteAsync(CommandID cmdId)
        {
            if (cmdId.Guid == CmdSets.GuidReko)
            {
                switch ((CmdIds) cmdId.ID)
                {
                case CmdIds.ViewGoToAddress: await GotoAddress(); return true;
                }
            }
            return false;
        }

        #endregion

        void txtDisassembly_Resize(object sender, EventArgs e)
        {
            DumpAssembler();
        }
    }
}
