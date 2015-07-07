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
using Decompiler.Gui;
using Decompiler.Gui.Windows;
using Decompiler.Gui.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    public class DisassemblyViewInteractor : IWindowPane, ICommandTarget
    {
        private Address startAddress;
        private Program program;
        private TextBox txtDisassembly;
        private IServiceProvider sp;

        public void DumpAssembler()
        {
            var decSvc = sp.GetService<IDecompilerService>();
            if (decSvc == null)
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
                    var arch = program.Architecture;
                    var dumper = new Dumper(arch);
                    dumper.ShowAddresses = true;
                    dumper.ShowCodeBytes = true;
                    var image = program.Image;
                    var dasm = program.CreateDisassembler(StartAddress).GetEnumerator();
                    while (dasm.MoveNext())
                    {
                        var instr = dasm.Current;
                        if (lines <= 0)
                            break;
                        dumper.DumpAssemblerLine(image, instr, writer);
                        --lines;
                    }
                    txtDisassembly.Text = writer.ToString();
                }
            }
        }

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
                program != null &&
                program.Architecture != null &&
                program.Image != null &&
                StartAddress != null;
        }

        private void GotoAddress()
        {
			var dlgFactory = sp.RequireService<IDialogFactory>();
            using (IAddressPromptDialog dlg = dlgFactory.CreateAddressPromptDialog())
            {
                if (sp.GetService<IDecompilerShellUiService>().ShowModalDialog(dlg) == DialogResult.OK)
                {
                    StartAddress = dlg.Address;
                    DumpAssembler();
                }
            }
        }

        #region IWindowPane Members

        public Control CreateControl()
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
            if (cmdId.Guid == CmdSets.GuidDecompiler)
            {
                switch (cmdId.ID)
                {
                case CmdIds.ViewGoToAddress: status.Status = MenuStatus.Enabled | MenuStatus.Visible; return true;
                case CmdIds.ActionMarkProcedure: status.Status = MenuStatus.Enabled | MenuStatus.Visible; return true;
                }
            }
            return false;
        }

        public bool Execute(CommandID cmdId)
        {
            if (cmdId.Guid == CmdSets.GuidDecompiler)
            {
                switch (cmdId.ID)
                {
                case CmdIds.ViewGoToAddress: GotoAddress(); return true;
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
