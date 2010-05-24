/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core;
using Decompiler.Gui;
using Decompiler.Gui.Windows;
using Decompiler.Gui.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    public class DisassemblyViewInteractor : IWindowPane, ICommandTarget
    {
        private Address startAddress;
        private TextBox txtDisassembly;
        private IServiceProvider sp;

        public void DumpAssembler()
        {
            var decSvc = sp.GetService<IDecompilerService>();
            if (decSvc == null)
                throw new InvalidOperationException("Expected IDecompilerService to be available.");
            var Decompiler = decSvc.Decompiler;


            if (!IsProgramLoaded(Decompiler))
            {
                txtDisassembly.Text = "";
            }
            else
            {
                int lines = CountVisibleLines();
                if (lines < 1)
                    lines = 1;
                StringWriter writer = new StringWriter();
                var arch = Decompiler.Program.Architecture;
                Dumper dumper = arch.CreateDumper();
                Disassembler dasm = arch.CreateDisassembler(Decompiler.Program.Image.CreateReader(StartAddress));
                while (lines > 0)
                {
                    dumper.DumpAssemblerLine(Decompiler.Program.Image, dasm, true, true, writer);
                    --lines;
                }
                txtDisassembly.Text = writer.ToString();
            }
        }

        public Address StartAddress { get { return startAddress; }
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

        private bool IsProgramLoaded(IDecompiler decompiler)
        {
            return
                decompiler != null &&
                decompiler.Program.Architecture != null &&
                decompiler.Program.Image != null &&
                StartAddress != null;
        }

        private void GotoAddress()
        {
            using (IAddressPromptDialog dlg = CreateAddressPromptDialog())
            {
                if (sp.GetService<IDecompilerShellUiService>().ShowModalDialog(dlg) == DialogResult.OK)
                {
                    StartAddress = dlg.Address;
                    DumpAssembler();
                }
            }
        }

        public virtual IAddressPromptDialog CreateAddressPromptDialog()
        {
            return new AddressPromptDialog();
        }


        #region IWindowPane Members

        public Control CreateControl()
        {
            txtDisassembly = new TextBox();
            this.txtDisassembly.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.txtDisassembly.Multiline = true;
            this.txtDisassembly.Name = "txtDisassembly";
            this.txtDisassembly.ReadOnly = true;
            this.txtDisassembly.WordWrap = false;
            txtDisassembly.Resize += txtDisassembly_Resize;
            txtDisassembly.KeyDown += txtDisassembly_KeyDown;
            txtDisassembly.KeyPress += txtDisassembly_KeyPress;
            return txtDisassembly;
        }

        void txtDisassembly_KeyDown(object sender, KeyEventArgs e)
        {
            Debug.WriteLine(string.Format("Down:  c:{0} d:{1} v:{2}", e.KeyCode, e.KeyData, e.KeyValue));
        }

        void txtDisassembly_KeyPress(object sender, KeyPressEventArgs e)
        {
            Debug.WriteLine(string.Format("Press: ch:{0}", e.KeyChar));
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

        public bool QueryStatus(ref Guid cmdSet, int cmdId, CommandStatus status, CommandText text)
        {
            if (cmdSet == CmdSets.GuidDecompiler)
            {
                switch (cmdId)
                {
                case CmdIds.ViewGoToAddress: status.Status = MenuStatus.Enabled | MenuStatus.Visible; return true;
                }
            }
            return false;
        }

        public bool Execute(ref Guid cmdSet, int cmdId)
        {
            if (cmdSet == CmdSets.GuidDecompiler)
            {
                switch (cmdId)
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
